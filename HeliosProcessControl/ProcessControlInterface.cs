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
// 

using GadrocsWorkshop.Helios.Interfaces.Capabilities;
using GadrocsWorkshop.Helios.Interfaces.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace GadrocsWorkshop.Helios.HeliosProcessControl
{
    [ComponentModel.HeliosInterface("HeliosProcessControl.ProcessControlInterface", "Process Control", typeof(ProcessControlInterfaceEditor), typeof(UniqueHeliosInterfaceFactory))]
    public class ProcessControlInterface : HeliosInterfaceWithXml<ProcessControl>, IReadyCheck, IExtendedDescription, IStatusReportNotify
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static readonly Regex _rxDoubleQuotes = new Regex("(?:\\\")([^\"]*)(?:\\\")", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
        private static readonly string executableExtensions = Environment.GetEnvironmentVariable("PATHEXT") ?? ".COM;.EXE;.BAT;.CMD;.VBS;.VBE;.JS;.JSE;.WSF;.WSH;.MSC;";
        private static readonly Regex _rxExe = new Regex("(" + executableExtensions.Replace(";", "|").Replace(".", "\\.") + ")", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

        private readonly HashSet<IStatusReportObserver> _observers = new HashSet<IStatusReportObserver>();

        public ProcessControlInterface() : base("Process Control")
        {
            HeliosAction launchApplication = new HeliosAction(this, "", "", "launch application", "Launches an external application", "Full path to appliation or document you want to launch or URL to a web page.", BindingValueUnits.Text);
            launchApplication.Execute += LaunchApplication_Execute;
            Actions.Add(launchApplication);

            HeliosAction killApplication = new HeliosAction(this, "", "", "kill application", "Kills an external process", "Process Image name of the process to be killed.", BindingValueUnits.Text);
            killApplication.Execute += KillApplication_Execute;
            Actions.Add(killApplication);

            // Add environmental variables for this process only.
            SetEnvironmentVariable("HeliosPath", ConfigManager.DocumentPath);
            SetEnvironmentVariable("BMSFalconPath", ConfigManager.BMSFalconPath);
        }

        protected override void AttachToProfileOnMainThread()
        {
            base.AttachToProfileOnMainThread();
            InputBindings.CollectionChanged += InputBindings_CollectionChanged;
        }

        protected override void DetachFromProfileOnMainThread(HeliosProfile oldProfile)
        {
            InputBindings.CollectionChanged -= InputBindings_CollectionChanged;
            base.DetachFromProfileOnMainThread(oldProfile);
        }

        private void InputBindings_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            InvalidateStatusReport();
        }

        void LaunchApplication_Execute(object action, HeliosActionEventArgs e)
        {
            if (!Model.AllowLaunch)
            {
                Logger.Error("Profile attempted to start a process {Path} but this action is not permitted on this machine",
                    e.Value.StringValue);
                return;
            }

            // Arguments need to be split off from the incoming string, and both the
            // application full path and the arguments could contain spaces.  
            // The first un-double-quoted space is assumed to be the delimiter between 
            // executable and argument.  
            // 
            // Once arguments have been split off, both the executable and arguments are 
            // processed to expand environment variables which might introduce spaces.
            //
            // This process might break backward compatibility because existing launch actions
            // could contain blanks and not be enclosed in double quotes
            //
            MatchCollection matches;
            // Find out if we're launching a common executable for this machine
            matches = _rxExe.Matches(e.Value.StringValue);
            bool exe = matches.Count > 0 ? true : false;

            string expandedPath = e.Value.StringValue;
            string expandedArgs = "";
            if (exe)
            {
                // For executables launches, we resolve environment variables
                Logger.Debug("Launch type determined to be an executable based on %PATHEXT% and value {ActionValue}", e.Value.StringValue);

                matches = _rxDoubleQuotes.Matches(e.Value.StringValue);  // Extract anything which is enclosed in escaped double quotes
                int blank = e.Value.StringValue.IndexOf(" ");
                try
                {
                    if (matches.Count == 0)
                    {
                        //  There is nothing enclosed in double-quotes, so we assume the executable is before the first space and any arguments follow the first space

                        if (blank > 0)
                        {
                            expandedPath = Environment.ExpandEnvironmentVariables(e.Value.StringValue.Substring(0, blank));
                            expandedArgs = Environment.ExpandEnvironmentVariables(e.Value.StringValue.Substring(blank + 1));
                        }
                        else
                        {
                            expandedPath = Environment.ExpandEnvironmentVariables(e.Value.StringValue);
                            expandedArgs = "";
                        }
                    }
                    else
                    {
                        int matchCursor = 0;
                        foreach (Match matchItem in matches)
                        {
                            if (matchItem.Equals(matches[0]))
                            {
                                if (matchItem.Index == 0)
                                {
                                    expandedPath = Environment.ExpandEnvironmentVariables(matchItem.Groups[1].ToString());
                                    matchCursor = matchItem.Length;
                                }
                                else
                                {
                                    expandedPath = Environment.ExpandEnvironmentVariables(e.Value.StringValue.Substring(0, blank));
                                    expandedArgs = Environment.ExpandEnvironmentVariables(e.Value.StringValue.Substring(blank, matchItem.Index - blank));
                                    expandedArgs += " " + Environment.ExpandEnvironmentVariables(matchItem.ToString());
                                    matchCursor = matchItem.Index + matchItem.Length;
                                }
                            }
                            else
                            {
                                if (matchItem.Index == matchCursor)
                                {
                                    expandedArgs += " " + Environment.ExpandEnvironmentVariables(matchItem.ToString());
                                }
                                else
                                {
                                    expandedArgs += Environment.ExpandEnvironmentVariables(e.Value.StringValue.Substring(matchCursor, matchItem.Index - matchCursor));
                                    matchCursor = matchItem.Index + matchItem.Length;
                                    expandedArgs += " " + Environment.ExpandEnvironmentVariables(matchItem.ToString());
                                }
                            }
                            Logger.Debug("Double Quoted item found {ItemText}", matchItem);
                        }
                    }
                }
                catch (ArgumentNullException ex)
                {
                    Logger.Error(ex, "Error caught preparing to identify launch program or arguments for an external application (path={ExpandedPath})", expandedPath);
                }
            }
            try
            {
                if (exe)
                {
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = Path.GetFileName(expandedPath);
                    psi.WorkingDirectory = Path.GetDirectoryName(expandedPath);
                    psi.Arguments = expandedArgs.Trim();
                    psi.UseShellExecute = true;
                    psi.RedirectStandardOutput = false;
                    Process.Start(psi);
                }
                else
                {
                    Logger.Debug("Launch type determined to be non-executable based on %PATHEXT% {ProcessName}", e.Value.StringValue);
                    Process.Start(e.Value.StringValue);
                }
            }
            catch (Exception ex)
            {
                // this is written as info because it is very likely to be a user error and we do not want to see problem reports from this.
                Logger.Info(ex, "Error caught launching external application (path={ExpandedPath}).  This is very likely to be due to a missing program, missing file, or incorrect permissions on what has been specified on the lauinch action.", expandedPath);
            }
        }

        void KillApplication_Execute(object action, HeliosActionEventArgs e)
        {
            try
            {
                if (!Model.AllowKill)
                {
                    Logger.Error("Profile attempted to kill a process {Name} but this action is not permitted on this machine", 
                        e.Value.StringValue);
                    return;
                }
                Process[] localProcessesByName = Process.GetProcessesByName(e.Value.StringValue);
                foreach (Process proc in localProcessesByName)
                {
                    Logger.Info("Killing process image name {ProcessImageName}", e.Value.StringValue);
                    proc.Kill();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error caught during kill processing for process image name {ProcessImageName}", e.Value.StringValue);
            }
        }


        /// <summary>
        /// check configuration related to launching executables
        /// </summary>
        /// <param name="newReport"></param>
        private void CheckExecutablePaths(IList<StatusReportItem> newReport)
        {
            foreach (HeliosBinding binding in InputBindings)
            {
                switch (binding.Action.ActionID)
                {
                    case "launch application":
                        if (binding.ValueSource == BindingValueSources.StaticValue && ValidateExecPath(binding.Value))
                        {
                            newReport.Add(new StatusReportItem
                            {
                                Status = $"Item or application found (or cannot be checked) and is probably viable: '{binding.OutputDescription}'",
                                Recommendation = "No action necessary.",
                                Severity = StatusReportItem.SeverityCode.Info,
                                Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate | StatusReportItem.StatusFlags.Verbose
                            });
                        }
                        else
                        {
                            newReport.Add(new StatusReportItem
                            {
                                Status = $"Argument Error - Item or application not found on this machine or contains incorrect symbols and is not launchable from '{binding.OutputDescription}'",
                                Recommendation = "Install missing application, local file then reload profile.",
                                Severity = StatusReportItem.SeverityCode.Error,
                                Flags = StatusReportItem.StatusFlags.Verbose
                            });
                        }
                        break;
                    case "kill application":
                        newReport.Add(new StatusReportItem
                        {
                            Status = $"A kill action is contained in this profile for process image name '{binding.Value}'",
                            Recommendation = "Ensure that the process being killed is expected and acceptable.",
                            Severity = StatusReportItem.SeverityCode.Info,
                            Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate | StatusReportItem.StatusFlags.Verbose
                        });
                        break;
                    default:
                        break;
                }
            }
        }

        private bool ValidateExecPath(string proposedPath)
        {
            //
            // This attempts to validate the text which will eventually form part of a launch
            // operation.
            //
            // The launch is shell based so in addition to an executable, the input could 
            // also be a data file, arguments. It could also
            // contain environment variables or be a UNC, rooted or relative path.
            // We could also have received a URI just to complicate matters.
            //
            // The pre-flight does not attempt to validate arguments, but it takes a stab at
            // validating the first file or URI
            //

            // Resolve environment variables
            proposedPath = Environment.ExpandEnvironmentVariables(proposedPath);

            MatchCollection matches;

            matches = _rxDoubleQuotes.Matches(proposedPath);                            // Extract anything which is enclosed in escaped double quotes
            int blankPosition = proposedPath.IndexOf(" ");                              // For uncomplicated launches, if there is a blank, it will denote the start of the argument(s) 
            if (matches.Count == 0 & blankPosition <= 0)
            {
                // nothing enclosed in escaped double quotes
                return ExtractPathCheck(proposedPath);
            }
            else if (matches.Count == 0)
            {
                //  There is nothing enclosed in double-quotes, so we assume the file to check is before the first blank
                return ExtractPathCheck(proposedPath.Substring(0, blankPosition));
            }
            else
            {
                // There is text enclosed in escaped double quotes and there are blank characters
                return ExtractPathCheck(matches[0].Groups[1].ToString());
            }
        }

        private static bool ExtractURICheck(string proposedPath)
        {
            try
            {
                Uri uriCheck = new Uri(proposedPath);
                switch (uriCheck.Scheme)
                {
                    case "file":
                        if (uriCheck.IsFile) return File.Exists(proposedPath);
                        break;
                    case "http":
                    case "https":
                        return true;
                    default:
                        return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex, "URI Parsing for {ProposedPath} threw exception", proposedPath);
                return false;
            }
            return true;
        }

        private static bool ExtractPathCheck(string proposedPath)
        {
            // parse assuming not a URI
            try
            {
                proposedPath = Path.GetFullPath(proposedPath);
            }
            catch (ArgumentException ex)
            {
                Logger.Debug(ex, "GetFullPath Parsing for {ProposedPath} threw exception", proposedPath);
                return false;
            }
            catch (NotSupportedException ex)
            {
                // If we have a URI and attempt to get the full path, then this is probably what caused us to be here
                // this is not necessarily a problem so we do not fail and carry on to do URI checks
                Logger.Debug(ex, "GetFullPath Parsing for {ProposedPath} threw exception", proposedPath);
            }
            // Possibly a URI
            return ExtractURICheck(proposedPath);
        }

        private void SetEnvironmentVariable(string envVar, string envVarPath)
		{
            if (Environment.GetEnvironmentVariable(envVar) == null)
			{
                Environment.SetEnvironmentVariable(envVar, envVarPath);
            }
		}

        public IEnumerable<StatusReportItem> PerformReadyCheck()
        {
            List<StatusReportItem> items = new List<StatusReportItem>();
            CheckExecutablePaths(items);
            return items;
        }

        public string Description => "Interface that allows Helios to Start and Stop external programs";

        public string RemovalNarrative =>
            "Removing this interface will prevent this Helios profile from starting or stopping any external programs.";

        public void Subscribe(IStatusReportObserver observer)
        {
            _observers.Add(observer);
        }

        public void Unsubscribe(IStatusReportObserver observer)
        {
            _observers.Remove(observer);
        }

        public void PublishStatusReport(IList<StatusReportItem> statusReport)
        {
            foreach (IStatusReportObserver statusReportObserver in _observers)
            {
                statusReportObserver.ReceiveStatusReport(Name, Description, statusReport);
            }
        }

        public void InvalidateStatusReport()
        {
            PublishStatusReport(PerformReadyCheck().ToList());
        }
    }
}
