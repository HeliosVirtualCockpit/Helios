// Copyright 2020 Helios Contributors
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

using System.Collections.Generic;
using System.Linq;
using GadrocsWorkshop.Helios.Interfaces.Capabilities;
using Newtonsoft.Json;

namespace GadrocsWorkshop.Helios.Interfaces.Common
{
    public class InterfaceStatus : NotificationObject, IStatusReportObserver
    {
        private InterfaceStatus(HeliosInterface heliosInterface)
        {
            Interface = heliosInterface;
            ReadyCheck = heliosInterface as IReadyCheck;
            Name = $"{heliosInterface.Name}";
            if (heliosInterface is IExtendedDescription extendedInfo)
            {
                Description = extendedInfo.Description;
            }
            else
            {
                Description = Name;
            }

            HeliosInterfaceDescriptor descriptor =
                ConfigManager.ModuleManager.InterfaceDescriptors[heliosInterface.TypeIdentifier];
            HasEditor = descriptor.InterfaceEditorType != null;
            Subscription = heliosInterface as IStatusReportNotify;
            Subscription?.Subscribe(this);
            ///Todo: work out how to get the deprecated message into the status report
            Deprecated = descriptor.Deprecated;
            Description += Deprecated ? " * * * Interface is marked \"deprecated\" * * *":"";
        }


        /// <summary>
        /// backing field for property Name, contains
        /// the status reporting name for this status item
        /// </summary>
        private string _private;

        public void Dispose()
        {
            Subscription?.Unsubscribe(this);
        }

        public static bool TryManage(HeliosInterface heliosInterface, out InterfaceStatus managed)
        {
            managed = new InterfaceStatus(heliosInterface);
            return true;
        }

        public void ReceiveStatusReport(string statusName, string description, IList<StatusReportItem> statusReport)
        {
            Name = statusName;
            Description = description;
            Report = statusReport;
        }

        /// <summary>
        /// the ready check interface of Interface
        /// </summary>
        [JsonIgnore]
        public IReadyCheck ReadyCheck { get; }

        /// <summary>
        /// the Helios interface object being observed and queried for status
        /// </summary>
        [JsonIgnore]
        public HeliosInterface Interface { get; }

        /// <summary>
        /// the status reporting name for this status item
        /// </summary>
        [JsonProperty("name")]
        public string Name
        {
            get => _private;
            set
            {
                if (_private != null && _private == value)
                {
                    return;
                }

                string oldValue = _private;
                _private = value;
                OnPropertyChanged("Name", oldValue, value, true);
            }
        }

        /// <summary>
        /// backing field for property Description, contains
        /// additional description as a sentence fragment or null
        /// </summary>
        private string _description;

        /// <summary>
        /// additional description as a sentence fragment or null
        /// </summary>
        [JsonIgnore]
        public string Description
        {
            get => _description;
            set
            {
                if (_description != null && _description == value)
                {
                    return;
                }

                string oldValue = _description;
                _description = value;
                OnPropertyChanged("Description", oldValue, value, true);
            }
        }

        [JsonIgnore] public bool HasEditor { get; }

        [JsonIgnore] public bool Deprecated { get; set; }

        /// <summary>
        /// backing field for property Report, contains
        /// the most recently received status report from the managed interface
        /// </summary>
        private IList<StatusReportItem> _report = new List<StatusReportItem>();

        /// <summary>
        /// the most recently received status report from the managed interface
        /// </summary>
        [JsonProperty("report")]
        public IList<StatusReportItem> Report
        {
            get => _report;
            set
            {
                if (_report != null && _report == value)
                {
                    return;
                }

                IList<StatusReportItem> oldValue = _report;
                _report = value;
                OnPropertyChanged("Report", oldValue, value, true);
            }
        }

        /// <summary>
        /// the managed interface's status report subscription interface or null
        /// </summary>
        [JsonIgnore]
        public IStatusReportNotify Subscription { get; }

        internal void PerformCheck()
        {
            // prefer the status report notification
            if (Subscription != null)
            {
                Subscription.InvalidateStatusReport();
                return;
            }

            // as a last resort, get a static report from ready check
            if (ReadyCheck != null)
            {
                Report = ReadyCheck.PerformReadyCheck().ToList();
            }
        }
    }
}