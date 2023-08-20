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
// 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using GadrocsWorkshop.Helios.Controls.Capabilities;
using GadrocsWorkshop.Helios.Controls.Special;
using GadrocsWorkshop.Helios.Interfaces.Capabilities;
using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
using GadrocsWorkshop.Helios.Util;
using GadrocsWorkshop.Helios.Util.DCS;
using GadrocsWorkshop.Helios.Util.Shadow;

namespace GadrocsWorkshop.Helios.Patching.DCS
{
    internal class MonitorSetupGenerator : DCSConfiguration
    {
        #region Nested

        private delegate StatusReportItem ProcessMonitorSetupFile(InstallationLocation location, string name,
            string directoryPath, string filePath, string correctContents);

        #endregion

        #region Private

        private readonly MonitorSetup _parent;

        /// <summary>
        /// the entire text of the combined monitor setup lua file
        /// </summary>
        private string _combinedMonitorSetup;

        /// <summary>
        /// the entire text of the monitor setup lua file
        /// </summary>
        private string _monitorSetup;

        /// <summary>
        /// the viewport setup file for the current profile or null, written to disk only if we finish configuration
        /// </summary>
        private ViewportSetupFile _localViewports;

        /// <summary>
        /// the viewport setup file containing all viewports for the current monitor setup
        /// </summary>
        private ViewportSetupFile _allViewports;

        #endregion

        public MonitorSetupGenerator(MonitorSetup parent)
        {
            _parent = parent;
            SubscribeToLocationChanges();
        }

        protected override void Update()
        {
            _parent.InvalidateStatusReport();
        }

        private const StatusReportItem.StatusFlags INFORMATIONAL =
            StatusReportItem.StatusFlags.Verbose | StatusReportItem.StatusFlags.ConfigurationUpToDate;

        /// <summary>
        /// generate the monitor setup file but do not write it out yet 
        /// </summary>
        /// <returns></returns>
        private IEnumerable<StatusReportItem> UpdateMonitorSetup(MonitorSetupTemplate template, bool verbose)
        {
            List<FormattableString> lines = CreateHeader(template);

            foreach (StatusReportItem item in GatherViewports(template))
            {
                yield return item;
            }

            // emit in sorted canonical order so we can compare files later

            foreach (KeyValuePair<string, Rect> viewport in _allViewports.Viewports.OrderBy(p => p.Key))
            {
                if (TryCreateViewport(lines, viewport, out FormattableString code))
                {
                    yield return new StatusReportItem
                    {
                        Status = $"{template.MonitorSetupFileBaseName}: {code}",
                        Flags = INFORMATIONAL
                    };
                }
                else
                {
                    yield return new StatusReportItem
                    {
                        Status =
                            $"viewport '{viewport.Key}' was not included in monitor setup because it is not entirely contained in rendered resolution",
                        Severity = StatusReportItem.SeverityCode.Warning,
                        Recommendation = "adjust the viewport location or the monitor layout",
                        Link = StatusReportItem.ProfileEditor,
                        Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
                    };
                }
            }

            // find main and ui view extents
            Rect mainView = Rect.Empty;
            Rect uiView = Rect.Empty;
            foreach (DCSMonitor monitor in _parent.Monitors)
            {
                string monitorDescription =
                    $"Monitor at Windows coordinates ({monitor.Monitor.Left},{monitor.Monitor.Top}) of size {monitor.Monitor.Width}x{monitor.Monitor.Height}";
                if (!monitor.Included)
                {
                    if (verbose) yield return new StatusReportItem
                    {
                        Status = $"{monitorDescription} is not included in monitor setup",
                        Flags = INFORMATIONAL
                    };
                    continue;
                }

                Rect rect = monitor.Monitor.CalculateWindowsDesktopRect();
                if (monitor.Main)
                {
                    if (_parent.MonitorLayoutMode == MonitorLayoutMode.TopLeftQuarter)
                    {
                        // special
                        Rect scaled = rect;
                        scaled.Scale(0.5d, 0.5d);
                        mainView.Union(scaled);
                    }
                    else
                    {
                        mainView.Union(rect);
                    }
                    if (verbose) yield return new StatusReportItem
                    {
                        Status = $"{monitorDescription} is used for main view",
                        Flags = INFORMATIONAL
                    };
                }

                if (monitor.UserInterface)
                {
                    uiView.Union(rect);
                    if (verbose) yield return new StatusReportItem
                    {
                        Status = $"{monitorDescription} is used to display the DCS user interface",
                        Flags = INFORMATIONAL
                    };
                }

                if (monitor.ViewportCount > 0)
                {
                    string plural = monitor.ViewportCount > 1 ? "s" : "";
                    if (verbose) yield return new StatusReportItem
                    {
                        Status = $"{monitorDescription} has {monitor.ViewportCount} viewport{plural} from this profile",
                        Flags = INFORMATIONAL
                    };
                }
            }

            // trim to rendered area in case of partial screens
            mainView.Intersect(_parent.Rendered);
            uiView.Intersect(_parent.Rendered);

            // change to DCS coordinates (0,0 at top left of rect, physical pixels)
            ConvertToDCS(ref mainView);
            ConvertToDCS(ref uiView);

            CreateMainView(lines, mainView);
            if (verbose) yield return new StatusReportItem
            {
                Status =
                    $"MAIN = {{ x = {mainView.Left}, y = {mainView.Top}, width = {mainView.Width}, height = {mainView.Height} }}",
                Flags = INFORMATIONAL
            };

            // check for separate UI view
            StatusReportItem uiStatus = CreateUserInterfaceViewIfRequired(lines, mainView, uiView, out string uiViewName);
            if (verbose) yield return uiStatus;

            // set up required names for viewports (well-known to DCS)
            lines.Add($"UIMainView = {uiViewName}");
            lines.Add($"GU_MAIN_VIEWPORT = Viewports.Center");

            foreach (FormattableString line in lines)
            {
                ConfigManager.LogManager.LogDebug(FormattableString.Invariant(line));
            }

            if (template.Combined)
            {
                if (!_allViewports.DCSMonitorSetupAdditionalLua.Equals(""))
                {
                    lines.Add($"{_allViewports.DCSMonitorSetupAdditionalLua}");
                }
                _combinedMonitorSetup = Render(lines);
            }
            else
            {
                if (!_localViewports.DCSMonitorSetupAdditionalLua.Equals(""))
                {
                    lines.Add($"{_localViewports.DCSMonitorSetupAdditionalLua}");
                }
                _monitorSetup = Render(lines);
            }
        }

        private static string Render(IEnumerable<FormattableString> lines) => string.Join(Environment.NewLine, lines.Select(FormattableString.Invariant));

        internal void ConvertToDCS(ref Rect windowsRect)
        {
            windowsRect.Offset(-_parent.Rendered.TopLeft.X, -_parent.Rendered.TopLeft.Y);
            windowsRect.Scale(ConfigManager.DisplayManager.PixelsPerDip, ConfigManager.DisplayManager.PixelsPerDip);
        }

        private bool TryCreateViewport(ICollection<FormattableString> lines, KeyValuePair<string, Rect> viewport, out FormattableString code)
        {
            Rect viewportRect = viewport.Value;
            viewportRect.Intersect(_parent.Rendered);
            if (viewportRect.Width < viewport.Value.Width || viewportRect.Height < viewport.Value.Height)
            {
                // viewports that aren't entire rendered do not work
                string message = $"viewport '{viewport.Key}' not included in monitor setup because it is not entirely contained in rendered resolution";
                ConfigManager.LogManager.LogInfo(message);
                lines.Add($"--- {message}");
                code = null;
                return false;
            }
            foreach(ShadowVisual shadowVisual in _parent.Viewports)
            {
                if(shadowVisual.Viewport.ViewportName == viewport.Key)
                {
                    if (!shadowVisual.IsViewportDirectlyOnMonitor)
                    {
                        string viewportInfo = $"Changes to viewport '{viewport.Key}' are not tracked automatically.  If changes have been made to this viewport\'s size or location, ensure a \"Reload Status\" is performed before monitor configuration is attempted.";
                        ConfigManager.LogManager.LogInfo(viewportInfo);
                    }
                    break;
                }
            }

            ConvertToDCS(ref viewportRect);
            code = $"{viewport.Key} = {{ x = {viewportRect.Left}, y = {viewportRect.Top}, width = {viewportRect.Width}, height = {viewportRect.Height} }}";
            lines.Add(code);
            return true;
        }

        private static List<FormattableString> CreateHeader(MonitorSetupTemplate template)
        {
            List<FormattableString> lines = new List<FormattableString>
            {
                // NOTE: why do we need to run this string through a local function?  does this create a ref somehow or prevent string interning?
                $"_  = function(p) return p end",
                $"name = _('{template.MonitorSetupName}')",
                $"description = 'Generated from {template.SourcesList}'"
            };
            return lines;
        }

        private static void CreateMainView(List<FormattableString> lines, Rect mainView)
        {
            // calling the MAIN viewport "Center" to match DCS' built-in monitor setups, even though it doesn't matter
            lines.Add($"Viewports = {{");
            lines.Add($"  Center = {{");
            lines.Add($"    x = {mainView.Left},");
            lines.Add($"    y = {mainView.Top},");
            lines.Add($"    width = {mainView.Width},");
            lines.Add($"    height = {mainView.Height},");
            lines.Add($"    aspect = {mainView.Width / mainView.Height},");
            lines.Add($"    dx = 0,");
            lines.Add($"    dy = 0");
            lines.Add($"  }}");
            lines.Add($"}}");
        }

        private IEnumerable<StatusReportItem> UpdateLocalViewports(MonitorSetupTemplate template)
        {
            _localViewports = new ViewportSetupFile
            {
                MonitorLayoutKey = _parent.MonitorLayoutKey
            };
            foreach (ShadowVisual shadow in _parent.Viewports)
            {
                string name = shadow.Viewport.ViewportName;
                if (!(shadow.Viewport is DCSMonitorScriptAppender))
                {
                    if (_localViewports.Viewports.ContainsKey(name))
                    {
                        yield return new StatusReportItem
                        {
                            Status =
                                $"The viewport '{name}' exists more than once in this profile.  Each viewport must have a unique name.",
                            Recommendation = $"Rename one of the duplicated viewports with name '{name}' in this profile",
                            Severity = StatusReportItem.SeverityCode.Warning,
                            Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
                        };
                        continue;
                    }
                    // calculate effective screen coordinates by tracing ancestry
                    _localViewports.Viewports.Add(name, shadow.Visual.CalculateWindowsDesktopRect());
                    if (!shadow.IsViewportDirectlyOnMonitor)
                    {
                        yield return new StatusReportItem
                        {
                            Status =
                                $"The viewport '{name}' is not tracked automatically because it is contained within a gauge, control or panel.",
                            Recommendation = $"If changes have been made to viewport {name}\'s size or location, ensure a \"Reload Status\" is performed before monitor configuration is attempted.",
                            Severity = StatusReportItem.SeverityCode.Info,
                            Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
                        };
                        continue;
                    }
                } else
                {
                    // We move the lua code from the actual viewports and combine them into a single property on the ViewportSetupFile
                    DCSMonitorScriptAppender appender = (DCSMonitorScriptAppender)shadow.Viewport;
                    _localViewports.DCSMonitorSetupAdditionalLua += appender.DCSMonitorSetupAdditionalLua + Environment.NewLine;
                }
            }

            // now check against our saved state, which we also have to update
            ViewportSetupFile saved = _parent.Combined.Load(template.ProfileName);
            if (null == saved)
            {
                yield return new StatusReportItem
                {
                    Status = "The viewport data for this profile does not exist",
                    Severity = StatusReportItem.SeverityCode.Error,
                    Recommendation = $"Configure {_parent.Name}"
                };
            }
            else if (saved.MonitorLayoutKey != _localViewports.MonitorLayoutKey
                     || !saved.Viewports.OrderBy(e => e.Key)
                         .SequenceEqual(_localViewports.Viewports.OrderBy(e => e.Key)))
            {
                // monitor layout key and the viewport rectangles in order must be equal
                yield return new StatusReportItem
                {
                    Status = "The viewport data for this profile is out of date",
                    Severity = StatusReportItem.SeverityCode.Error,
                    Recommendation = $"Configure {_parent.Name}"
                };
            }
        }

        private IEnumerable<StatusReportItem> GatherViewports(MonitorSetupTemplate template)
        {
            foreach (StatusReportItem item in UpdateLocalViewports(template))
            {
                yield return item;
            }

            if (!template.Combined)
            {
                _allViewports = _localViewports;
            }
            else
            {
                _allViewports = new ViewportSetupFile
                {
                    MonitorLayoutKey = _parent.MonitorLayoutKey
                };
                foreach (string name in _parent.Combined.CalculateCombinedSetupNames())
                {
                    if (name == template.ProfileName)
                    {
                        // this is the current profile so we take the data from where we generate it instead
                        // of from a file
                        foreach (StatusReportItem item in _allViewports.Merge(name, _localViewports))
                        {
                            yield return item;
                        }
                        if (!_localViewports.DCSMonitorSetupAdditionalLua.Equals(""))
                        {
                            _allViewports.DCSMonitorSetupAdditionalLua += _localViewports.DCSMonitorSetupAdditionalLua + Environment.NewLine;
                        }

                        continue;
                    }

                    ViewportSetupFile generated = _parent.Combined.Load(name);
                    if (null == generated)
                    {
                        yield return new StatusReportItem
                        {
                            Status =
                                $"Could not include the viewports for Helios profile '{name}' because no generated viewport data was found",
                            Recommendation =
                                $"Configure DCS Monitor Setup for Helios profile '{name}', then configure DCS Monitor Setup for current Helios profile",
                            Severity = StatusReportItem.SeverityCode.Warning,
                            Link = StatusReportItem.ProfileEditor
                        };
                        continue;
                    } else
                    {
                        if (!generated.DCSMonitorSetupAdditionalLua.Equals(""))
                        {
                            _allViewports.DCSMonitorSetupAdditionalLua += generated.DCSMonitorSetupAdditionalLua + Environment.NewLine;
                        }
                    }
                    
                    foreach (StatusReportItem item in _allViewports.Merge(name, generated))
                    {
                        yield return item;
                    }
                }
            }
        }

        private string GenerateSavedGamesPath(string savedGamesName) =>
            Path.Combine(KnownFolders.SavedGames, savedGamesName, "Config", "MonitorSetup");

        public ViewportSetupFile LocalViewports => _localViewports;

        private IEnumerable<StatusReportItem> EnumerateMonitorSetupFiles(ProcessMonitorSetupFile handler,
            MonitorSetupTemplate template)
        {
            string fileName = template.FileName;
            HashSet<string> done = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
            foreach (InstallationLocation location in InstallationLocations.Singleton.Active)
            {
                if (done.Contains(location.SavedGamesName))
                {
                    // could have defaulted or configured the same variant twice
                    continue;
                }

                string monitorSetupsDirectory = GenerateSavedGamesPath(location.SavedGamesName);
                string monitorSetupPath = Path.Combine(monitorSetupsDirectory, fileName);
                string correctContents = template.Combined ? _combinedMonitorSetup : _monitorSetup;
                yield return handler(location, fileName, monitorSetupsDirectory, monitorSetupPath, correctContents);
            }
        }

        public StatusReportItem InstallFile(InstallationLocation location, string name, string directoryPath,
            string filePath, string correctContents)
        {
            Directory.CreateDirectory(directoryPath);
            File.WriteAllText(filePath, correctContents);
            return new StatusReportItem
            {
                Status = $"generated monitor setup file '{name}' in {location.DescribeMonitorSetupPath}",
                Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
            };
        }

        public StatusReportItem CheckSetupFile(InstallationLocation location, string name,
            string _, string filePath, string correctContents)
        {
            if (!File.Exists(filePath))
            {
                return new StatusReportItem
                {
                    Status =
                        $"{location.DescribeMonitorSetupPath} does not contain the monitor setup file '{name}'",
                    Recommendation = $"Configure {_parent.Name}",
                    Link = StatusReportItem.ProfileEditor,
                    Severity = StatusReportItem.SeverityCode.Warning
                };
            }

            string contents = File.ReadAllText(filePath);
            if (contents != correctContents)
            {
                return new StatusReportItem
                {
                    Status =
                        $"monitor setup file '{name}' in {location.DescribeMonitorSetupPath} does not match configuration",
                    Recommendation = $"Configure {_parent.Name}",
                    Link = StatusReportItem.ProfileEditor,
                    Severity = StatusReportItem.SeverityCode.Warning
                };
            }

            return new StatusReportItem
            {
                Status =
                    $"monitor setup file '{name}' in {location.DescribeMonitorSetupPath} is up to date",
                Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
            };
        }

        /// <summary>
        /// create separate UI view if not identical to the main view
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="mainView"></param>
        /// <param name="uiView"></param>
        /// <param name="uiViewName"></param>
        /// <returns></returns>
        public StatusReportItem CreateUserInterfaceViewIfRequired(List<FormattableString> lines, Rect mainView, Rect uiView,
            out string uiViewName)
        {
            FormattableString comment;
            if (uiView != mainView)
            {
                uiViewName = "UI";
                FormattableString code =
                    $"{uiViewName} = {{ x = {uiView.Left}, y = {uiView.Top}, width = {uiView.Width}, height = {uiView.Height} }}";
                comment = code;
                lines.Add(code);
            }
            else
            {
                uiViewName = "Viewports.Center";
                comment = $"UI = MAIN";
            }

            return new StatusReportItem
            {
                Status = FormattableString.Invariant(comment),
                Flags = StatusReportItem.StatusFlags.Verbose | StatusReportItem.StatusFlags.ConfigurationUpToDate
            };
        }

        /// <summary>
        /// attempt to write the monitor setup file to all configured Saved Games folders
        /// and also persist our viewports information
        /// </summary>
        /// <param name="callbacks"></param>
        /// <returns></returns>
        public override InstallationResult Install(IInstallationCallbacks callbacks)
        {
            try
            {
                if (!_parent.CheckMonitorsValid)
                {
                    throw new Exception(
                        "UI should have disabled monitor setup writing without up to date monitors; implementation error");
                }

                if (string.IsNullOrWhiteSpace(_parent.Profile.Path))
                {
                    throw new Exception(
                        "UI should have disabled monitor setup without a profile name; implementation error");
                }

                // create template for combined monitor setup
                MonitorSetupTemplate combinedTemplate = CreateCombinedTemplate();

                // gather all the results into a list to enumerate the yield returns
                List<StatusReportItem> results =
                    new List<StatusReportItem>(EnumerateMonitorSetupFiles(InstallFile, combinedTemplate));
                if (!_parent.GenerateCombined)
                {
                    // add the same tests for separate monitor setup
                    results.AddRange(EnumerateMonitorSetupFiles(InstallFile, CreateSeparateTemplate()));
                }

                // now scan results
                foreach (StatusReportItem item in results)
                {
                    if (item.Severity >= StatusReportItem.SeverityCode.Error)
                    {
                        callbacks.Failure(
                            "Monitor setup file generation has failed",
                            "Some files may have been generated before the failure and these files were not removed.",
                            results);
                        return InstallationResult.Fatal;
                    }

                    // REVISIT we should have simulated first to gather warnings and errors so we can show a danger prompt
                }

                _parent.Combined.Save(combinedTemplate.ProfileName, _localViewports);

                callbacks.Success(
                    "Monitor setup generation successful",
                    "Monitor setup files have been installed into DCS.",
                    results);
                _parent.InvalidateStatusReport();
                return InstallationResult.Success;
            }
            catch (Exception ex)
            {
                ConfigManager.LogManager.LogError("failed to install monitor setup", ex);
                callbacks.Failure("Failed to install monitor setup", ex.Message, new List<StatusReportItem>());
                _parent.InvalidateStatusReport();
                return InstallationResult.Fatal;
            }
        }

        public override IEnumerable<StatusReportItem> PerformReadyCheck()
        {
            // while we do not have the ability to measure the DPI setting for each screen, we just report the system dpi and an advisory message
            // we always include this advisory because it is affecting basically all our widescreen users right now
            int dpi = ConfigManager.DisplayManager.DPI;
            yield return new StatusReportItem
            {
                Status = $"Windows reports a scaling value of {Math.Round(dpi / 0.96d)}% ({dpi} dpi)",
                Recommendation = "This version of Helios does not support using different display scaling (DPI) on different monitors.  Make sure you use the same scaling value for all displays.",
                Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate | StatusReportItem.StatusFlags.Verbose
            };

            if (!_parent.CheckMonitorsValid)
            {
                yield return new StatusReportItem
                {
                    Status = "Monitor configuration in profile does not match this computer",
                    Recommendation = "Perform 'Reset Monitors' function from the 'Profile' menu",
                    Link = StatusReportItem.ProfileEditor,
                    Severity = StatusReportItem.SeverityCode.Error
                };
                yield break;
            }

            if (string.IsNullOrWhiteSpace(_parent.Profile.Path))
            {
                yield return new StatusReportItem
                {
                    Status =
                        "You must save the profile before you can use Monitor Setup, because it needs to know the profile name",
                    Recommendation = "Save the profile at least once before configuring Monitor Setup",
                    Link = StatusReportItem.ProfileEditor,
                    Severity = StatusReportItem.SeverityCode.Error
                };
                yield break;
            }

            // check if DCS install folders are configured
            InstallationLocations locations = InstallationLocations.Singleton;
            if (!locations.Active.Any())
            {
                yield return new StatusReportItem
                {
                    Status = "No DCS installation locations are configured for monitor setup",
                    Recommendation = "Configure any DCS installation location you use",
                    Link = StatusReportItem.ProfileEditor,
                    Severity = StatusReportItem.SeverityCode.Error
                };
            }

            // gather viewport providers from this profile
            List<IViewportProvider> viewportProviders =
                _parent.Profile.Interfaces.OfType<IViewportProvider>().ToList();
            if (!_parent.UsingViewportProvider)
            {
                foreach (HeliosInterface viewportProvider in viewportProviders.OfType<HeliosInterface>())
                {
                    yield return new StatusReportItem
                    {
                        Status = $"interface '{viewportProvider.Name}' is providing additional viewport patches but this profile uses a third-party solution for those",
                        Severity = StatusReportItem.SeverityCode.Warning,
                        Recommendation = $"Remove the '{viewportProvider.Name}' interface since this profile is configured to expect a third-party solution to provide viewport modifications"
                    };
                }
            }

            // check if any referenced viewports require patches to work
            foreach (IViewportExtent viewport in _parent.Viewports
                .Select(shadow => shadow.Viewport)
                .Where(v => v.RequiresPatches))
            {
                if (!_parent.UsingViewportProvider)
                {
                    yield return new StatusReportItem
                    {
                        Status =
                            $"viewport '{viewport.ViewportName}' must be provided by third-party solution for additional viewports",
                        Recommendation =
                            "Verify that your third-party viewport modifications match the viewport names for this profile",
                        Flags = StatusReportItem.StatusFlags.Verbose |
                                StatusReportItem.StatusFlags.ConfigurationUpToDate
                    };
                    continue;
                }
                bool found = false;
                foreach (IViewportProvider provider in viewportProviders)
                {
                    if (provider.IsViewportAvailable(viewport.ViewportName))
                    {
                        yield return new StatusReportItem
                        {
                            Status =
                                $"viewport '{viewport.ViewportName}' is provided by '{((HeliosInterface) provider).Name}'",
                            Flags = StatusReportItem.StatusFlags.Verbose |
                                    StatusReportItem.StatusFlags.ConfigurationUpToDate
                        };
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    yield return new StatusReportItem
                    {
                        Status = $"viewport '{viewport.ViewportName}' requires patches to be installed",
                        Recommendation =
                            "Add an Additional Viewports interface or configure the viewport extent not to require patches",
                        Link = StatusReportItem.ProfileEditor,
                        Severity = StatusReportItem.SeverityCode.Error,
                        Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
                    };
                }
            }
            // check if any referenced viewports are a Monitor Setup script modifier
            foreach (IViewportExtent viewport in _parent.Viewports
                .Select(shadow => shadow.Viewport).OfType<DCSMonitorScriptAppender>())
            {
                DCSMonitorScriptAppender appender = (DCSMonitorScriptAppender)viewport;
                yield return new StatusReportItem
                {
                    Status =
                        $"Monitor Setup script will be suffixed with command(s) '{appender.DCSMonitorSetupAdditionalLua}' from control '{appender.ViewportName}'.",
                    Recommendation =
                        $"If the Monitor Setup script suffix '{appender.DCSMonitorSetupAdditionalLua}' is not required, delete control '{appender.ViewportName}' from your profile.",
                    Flags = StatusReportItem.StatusFlags.Verbose |
                        StatusReportItem.StatusFlags.ConfigurationUpToDate
                };
                continue;
            }
            bool updated = true;
            bool hasFile = false;

            // calculate shared monitor config
            // and see if it is up to date in all locations
            // NOTE: we have to do this even if this profile uses a separate setup, in case we just removed ourselves,
            // invalidating the combined setup
            MonitorSetupTemplate combinedTemplate = CreateCombinedTemplate();
            string monitorSetupName = combinedTemplate.MonitorSetupName;
            foreach (StatusReportItem item in UpdateMonitorSetup(combinedTemplate, true))
            {
                yield return item;
            }

            foreach (StatusReportItem item in EnumerateMonitorSetupFiles(CheckSetupFile, combinedTemplate))
            {
                if (!item.Flags.HasFlag(StatusReportItem.StatusFlags.ConfigurationUpToDate))
                {
                    updated = false;
                }

                hasFile = true;
                yield return item;
            }

            if (_parent.GenerateCombined)
            {
                // check to make sure our viewports are actually included in the combined config, because
                // settings may not match the profile (https://github.com/HeliosVirtualCockpit/Helios/issues/344)
                if (!_parent.Combined.IsCombined(combinedTemplate.ProfileName))
                {
                    yield return new StatusReportItem
                    {
                        Status = $"current profile specifies using combined monitor setup, but its own viewports are not included",
                        Recommendation =
                            "Configure DCS Monitor Setup and save the Profile to repair corrupted configuration",
                        Link = StatusReportItem.ProfileEditor,
                        Severity = StatusReportItem.SeverityCode.Error
                    };
                }
            }
            else
            {
                // also calculate separate config, if applicable
                // and see if it is up to date in all locations
                MonitorSetupTemplate separateTemplate = CreateSeparateTemplate();
                monitorSetupName = separateTemplate.MonitorSetupName;
                foreach (StatusReportItem item in UpdateMonitorSetup(separateTemplate, false))
                {
                    yield return item;
                }

                foreach (StatusReportItem item in EnumerateMonitorSetupFiles(CheckSetupFile, separateTemplate))
                {
                    if (!item.Flags.HasFlag(StatusReportItem.StatusFlags.ConfigurationUpToDate))
                    {
                        updated = false;
                    }

                    hasFile = true;
                    yield return item;
                }
            }

            foreach (InstallationLocation location in locations.Active)
            {
                if (DCSOptions.TryReadOptions(location, out DCSOptions options))
                {
                    // check if correct monitor resolution selected in DCS
                    yield return ReportResolutionSelected(location, options);

                    // don't tell the user to do this yet if the file isn't done
                    if (hasFile && updated)
                    {
                        // check if monitor setup selected in DCS
                        yield return ReportMonitorSetupSelected(location, options, monitorSetupName);
                    }

                    // check on full screen
                    yield return ReportFullScreen(location, options);
                }
                else
                {
                    // report that the user has to check this themselves (we may not have access and that's ok)
                    string qualifier = _parent.MonitorLayoutMode == MonitorLayoutMode.FromTopLeftCorner ? "at least " : "";
                    yield return new StatusReportItem
                    {
                        Status = $"Helios was unable to check the DCS settings stored in {location.DescribeOptionsPath}",
                        Recommendation =
                            $"Using DCS, please make sure the 'Resolution' in the 'System' options is set to {qualifier}{_parent.Rendered.Width}x{_parent.Rendered.Height}",
                        Severity = StatusReportItem.SeverityCode.Info,
                        Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
                    };

                    // don't tell the user to do this yet if the file isn't done
                    if (hasFile && updated)
                    {
                        yield return new StatusReportItem
                        {
                            Status = $"Helios was unable to check the DCS monitor setup selection stored in {location.DescribeOptionsPath}",
                            Recommendation =
                                $"Using DCS, please make sure 'Monitors' in the 'System' options is set to '{monitorSetupName}'",
                            Severity = StatusReportItem.SeverityCode.Info,
                            Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
                        };
                    }
                }
            }

            if (_parent.GenerateCombined)
            {
                yield break;
            }

            yield return new StatusReportItem
            {
                Status = "This profile requires a specific monitor setup file",
                Recommendation =
                    "You will need to switch 'Monitors' in DCS when you switch Helios Profile",
                Severity = StatusReportItem.SeverityCode.Info,
                Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
            };
        }

        private StatusReportItem ReportResolutionSelected(InstallationLocation location, DCSOptions options)
        {
            string status = $"{location.DescribeOptionsPath} has 'Resolution' set to {options.Graphics.Width}x{options.Graphics.Height}";

            if ((long)Math.Round(_parent.Rendered.Width) == options.Graphics.Width &&
                (long)Math.Round(_parent.Rendered.Height) == options.Graphics.Height)
            {
                // exact match
                return new StatusReportItem
                {
                    Status = status,
                    Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
                };
            }

            if (_parent.MonitorLayoutMode != MonitorLayoutMode.FromTopLeftCorner)
            {
                // inexact match is not permitted in any of these modes
                return new StatusReportItem
                {
                    Status = status,
                    Recommendation =
                        $"Using DCS, please make sure the 'Resolution' in the 'System' options is set to {_parent.Rendered.Width}x{_parent.Rendered.Height}",
                    // not a warning, because the user may simply not have configured DCS yet.  once we do that automatically,
                    // this can be an out of date warning
                    Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
                };
            }

            if (_parent.Rendered.Width > options.Graphics.Width ||
                _parent.Rendered.Height > options.Graphics.Height)
            {
                return new StatusReportItem
                {
                    Status = status,
                    Recommendation =
                        $"Using DCS, choose a 'Resolution' in the 'System' options that is at least {_parent.Rendered.Width} pixels wide and at least {_parent.Rendered.Height} pixels high",
                    // not a warning, because the user may simply not have configured DCS yet.  once we do that automatically,
                    // this can be an out of date warning
                    Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
                };
            }

            return new StatusReportItem
            {
                Status = status,
                Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
            };
        }

        private StatusReportItem ReportFullScreen(InstallationLocation location, DCSOptions options)
        {
            string status = $"{location.DescribeOptionsPath} has 'Fullscreen' set to {options.Graphics.FullScreen.ToString(CultureInfo.InvariantCulture)}";

            if ((!options.Graphics.FullScreen) ||
                _parent.MonitorLayoutMode == MonitorLayoutMode.PrimaryOnly)
            {
                // no problem
                return new StatusReportItem
                {
                    Status = status,
                    Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
                };
            }

            // check if we are trying to use Fullscreen with multiple displays
            Monitor primary = _parent.Profile.CheckedDisplays?.FirstOrDefault(m => m.IsPrimaryDisplay);
            if (null != primary &&
                (primary.Width < options.Graphics.Width ||
                 primary.Height < options.Graphics.Height))
            {
                return new StatusReportItem
                {
                    Status = status,
                    Recommendation =
                        $"Using DCS, uncheck 'Full Screen' in the 'System' options, or DCS may position your views incorrectly or even collapse all your content to the main monitor",
                    // not a warning, because the user may simply not have configured DCS yet.  once we do that automatically,
                    // this can be an out of date warning
                    Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
                };
            }

            return new StatusReportItem
            {
                Status = status,
                Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
            };
        }

        private StatusReportItem ReportMonitorSetupSelected(InstallationLocation location, DCSOptions options, string monitorSetupName)
        {
            string status = $"{location.DescribeOptionsPath} has 'Monitors' set to '{options.Graphics.MultiMonitorSetup}'";

            if (options.Graphics.MultiMonitorSetup.Equals(monitorSetupName, StringComparison.InvariantCultureIgnoreCase))
            {
                // selection is correct for current profile
                return new StatusReportItem
                {
                    Status = status,
                    Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
                };
            }

            return new StatusReportItem
            {
                Status = status,
                Recommendation =
                    $"Using DCS, please make sure 'Monitors' in the 'System' options is set to '{monitorSetupName}'",
                // not a warning, because the user may simply not have configured DCS yet.  once we do that automatically,
                // this can be an out of date warning
                Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
            };
        }

        private MonitorSetupTemplate CreateCombinedTemplate() =>
            new MonitorSetupTemplate(
                _parent.CombinedMonitorSetupName,
                _parent.CurrentProfileName,
                true);

        private MonitorSetupTemplate CreateSeparateTemplate() =>
            new MonitorSetupTemplate(
                _parent.CombinedMonitorSetupName,
                _parent.CurrentProfileName,
                false);
    }
}