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

using System.Collections.Generic;
using System.Windows;
using Newtonsoft.Json;

namespace GadrocsWorkshop.Helios.Patching.DCS
{
    /// <summary>
    /// A collection of viewports that is stored to a file for each previously
    /// configured Profile that uses MonitorSetup.  Merged these files is used to
    /// collect viewports from many Profiles into one Combined Monitor Setup.
    /// </summary>
    public class ViewportSetupFile : NotificationObject
    {
        /// <summary>
        /// true if the viewport setup file exists, even if it is out of date
        /// </summary>
        [JsonIgnore]
        public bool Exists { get; internal set; }

        /// <summary>
        /// This string uniquely identifies the monitor layout that was active when these
        /// viewports were generated.  Using these viewports with a different monitor layout
        /// may result in invalid configurations.
        /// </summary>
        [JsonProperty("monitorLayoutKey")]
        public string MonitorLayoutKey { get; internal set; } = "";

        [JsonProperty("dcsmonitorsetupadditionallua")]
        public string DCSMonitorSetupAdditionalLua { get; internal set; } = "";

        [JsonProperty("dcsrestricttovehicle")]
        public string DCSRestrictToVehicle { get; internal set; } = "";

        [JsonProperty("viewports")] public Dictionary<string, Rect> Viewports { get; } = new Dictionary<string, Rect>();
        internal IEnumerable<StatusReportItem> Merge(string name, ViewportSetupFile from)
        {
            if (MonitorLayoutKey != from.MonitorLayoutKey)
            {
                yield return new StatusReportItem
                {
                    Status =
                        $"The stored viewport data from profile '{name}' does not match the current monitor layout",
                    Recommendation =
                        $"Configure DCS Monitor Setup for profile '{name}' to update the merged viewport data",
                    Severity = StatusReportItem.SeverityCode.Warning,
                    Link = StatusReportItem.ProfileEditor,
                    Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
                };
            }

            foreach (KeyValuePair<string, Rect> viewport in from.Viewports)
            {
                string vehicleKey = $"{(string.IsNullOrWhiteSpace(from.DCSRestrictToVehicle) ? "" : "_")}{from.DCSRestrictToVehicle}{(string.IsNullOrWhiteSpace(from.DCSRestrictToVehicle) ? "" : ".")}{viewport.Key}";
                if (!Viewports.TryGetValue(vehicleKey, out Rect existingRect))
                {
                    // just copy it
                    Viewports.Add(vehicleKey, viewport.Value);
                    continue;
                }

                if (existingRect.Equals(viewport.Value))
                {
                    // no problem
                    continue;
                }

                // overwrite and warn
                Viewports[vehicleKey] = viewport.Value;
                yield return new StatusReportItem
                {
                    Status = $"profile '{name}' defines the viewport '{vehicleKey}' at a different screen location",
                    Recommendation =
                        $"Resolve viewport conflicts or do not include profile '{name}' in the combined monitor setup",
                    Severity = StatusReportItem.SeverityCode.Warning,
                    Link = StatusReportItem.ProfileEditor,
                    Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
                };
            }
        }

        internal void Clear()
        {
            Viewports.Clear();
        }
    }
}