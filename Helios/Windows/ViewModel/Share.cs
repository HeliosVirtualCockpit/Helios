// Copyright 2020 Ammo Goettsch
// 
// Helios is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Helios is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using GadrocsWorkshop.Helios.Util;
using Newtonsoft.Json;

namespace GadrocsWorkshop.Helios.Windows.ViewModel
{
    /// <summary>
    /// a view model for sharing an IList of InterfaceStatus
    /// </summary>
    public class Share<T> : DependencyObject
    {
        /// <summary>
        /// backing field for property CopyCommand, contains
        /// handler for Share | Copy
        /// </summary>
        private ICommand _copyCommand;

        /// <summary>
        /// backing field for property FileBugCommand, contains
        /// handler for Share | FileBug
        /// </summary>
        private ICommand _fileBugCommand;

        public string Text { get; }

        internal class EmptyStatus
        {
            public string Name { get; }
            public IList<StatusReportItem> Report { get; }

            internal EmptyStatus()
            {
                Name = "Share";
                Report = new List<StatusReportItem>
                {
                    new StatusReportItem
                    {
                        Status = "No installed interfaces support status reporting.",
                        Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
                    }
                };
            }
        }

        internal class ReportWrapper<TStatusItem>
        {
            [JsonProperty("product")] public string Product { get; } = Assembly.GetExecutingAssembly().GetName().Name;

            [JsonProperty("version")] public string Version { get; } = ConfigManager.VersionChecker.RunningVersion.ToString();

            [JsonProperty("items")] public IList<TStatusItem> Items { get; internal set; }

            public ReportWrapper(IList<TStatusItem> items)
            {
                Items = items;
            }
        }

        private static readonly IList<EmptyStatus> EmptyReport = new List<EmptyStatus>
        {
            new EmptyStatus()
        };

        /// <summary>
        /// string processor to remove user name from report and shorten some paths
        /// </summary>
        private class PrivacyConverter : JsonConverter<string>
        {
            public override string ReadJson(JsonReader reader, Type objectType, string existingValue,
                bool hasExistingValue, JsonSerializer serializer) => reader.ReadAsString();

            public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer)
            {
                writer.WriteValue(Anonymizer.Anonymize(value));
            }
        }

        public Share(IList<T> report)
        {
            // XXX enum converter
            // XXX flags converter
            if (report.Count == 0)
            {
                Text = JsonConvert.SerializeObject(new ReportWrapper<EmptyStatus>(EmptyReport), Formatting.Indented);
            }
            else
            {
                Text = JsonConvert.SerializeObject(new ReportWrapper<T>(report), new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    Converters = new List<JsonConverter>
                    {
                        new PrivacyConverter()
                    }
                });
            }
        }

        /// <summary>
        /// handler for Share | Copy
        /// </summary>
        public ICommand CopyCommand
        {
            get
            {
                _copyCommand = _copyCommand ?? new RelayCommand(parameter =>
                {
                    Clipboard.SetText(Text);
                    if (parameter is IInputElement inputElement)
                    {
                        // fire a close command up the tree from the source of our command so it finds the right window
                        SystemCommands.CloseWindowCommand.Execute(null, inputElement);
                    }
                });
                return _copyCommand;
            }
        }

        /// <summary>
        /// handler for Share | FileBug
        /// </summary>
        public ICommand FileBugCommand
        {
            get
            {
                _fileBugCommand = _fileBugCommand ?? new RelayCommand(FileBug);
                return _fileBugCommand;
            }
        }

        private void FileBug(object parameter)
        {
            string repo = KnownLinks.GitRepoUrl();
            if (repo != null)
            {
                // NOTE: github claims to support paste for attachments, but it doesn't work so we have to drag and drop a file
                string fileName = PrepareStatusReportFile(out string tempPath);
                System.Diagnostics.Process.Start(tempPath);
                string encoded = Uri.EscapeDataString(Properties.Resources.helios_bug_report.Replace("&&HELIOSREPORTTEXT&&", $"## *[Helios has written the report to the file* `{fileName}` *and opened its containing folder in Explorer.  Please drag and drop it from Explorer to here and replace this message with a description of the problem you are reporting.]*\n\n"));
                // launch the default browser
                string url = $"{repo}issues/new?title=CHANGE%20THIS%20TITLE%20TO%20SOMETHING%20MEANINGFUL&labels=New%20Issue&body={encoded}";
                System.Diagnostics.Process.Start(url);
            }

            // close dialog
            if (parameter is IInputElement inputElement)
            {
                // fire a close command up the tree from the source of our command so it finds the right window
                SystemCommands.CloseWindowCommand.Execute(null, inputElement);
            }
        }

        private string PrepareStatusReportFile(out string tempPath)
        {
            long timeStamp = DateTime.Now.ToFileTime();
            string fileName = $"Helios_status_report_{timeStamp}.txt";
            tempPath = Path.Combine(Path.GetTempPath(), $"Helios_status_report_{timeStamp}");
            Directory.CreateDirectory(tempPath);
            string path = Path.Combine(tempPath, fileName);
            File.WriteAllText(path, Text);
            return fileName;
        }

        public string Title
        {
            get => (string) GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(Share<T>),
                new PropertyMetadata("Share anonymized Status Report"));
    }
}