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

using Newtonsoft.Json;

namespace ToolsCommon
{
    /// <summary>
    /// This is a loader for Jabbers' JSON format for viewport configuration information.  It must be kept compatible with
    /// https://github.com/jeffboulanger/DCS-Alternative-Launcher/blob/master/DCS.Alternative.Launcher/DomainObjects/Viewport.cs
    /// </summary>
    public class Viewport
    {
        [JsonProperty("description")] public string Description { get; set; }

        [JsonProperty("viewportName")] public string ViewportName { get; set; }

        /*
        [JsonProperty("monitorId")]
        public string MonitorId { get; set; }

        [JsonProperty("seatIndex")]
        public int SeatIndex { get; set; }
        */

        [JsonProperty("relativeInitFilePath")] public string RelativeInitFilePath { get; set; }

        [JsonProperty("originalDisplayWidth")] public int OriginalDisplayWidth { get; set; }

        [JsonProperty("originalDisplayHeight")]
        public int OriginalDisplayHeight { get; set; }

        [JsonProperty("x")] public int X { get; set; }

        [JsonProperty("y")] public int Y { get; set; }

        [JsonProperty("width")] public int Width { get; set; }

        [JsonProperty("height")] public int Height { get; set; }

        [JsonProperty("SuppressViewportNamePrefix")] public bool SuppressViewportNamePrefix { get; set; } = false;

        [JsonProperty("requiresPatch")] public bool RequiresPatch { get; set; } = true;
        [JsonProperty("viewportDisplayName")] public string ViewportDisplayName { get; set; }

        [JsonIgnore] public bool IsValid => RelativeInitFilePath != null && RelativeInitFilePath.Length > 0;
    }
}