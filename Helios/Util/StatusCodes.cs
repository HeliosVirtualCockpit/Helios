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

namespace GadrocsWorkshop.Helios.Util
{
    /// <summary>
    /// status codes that are common across a number of view models
    /// </summary>
    public enum StatusCodes
    {
        Unknown,
        UpToDate,
        OutOfDate,
        Incompatible,
        NoLocations,
        ResetMonitorsRequired,
        ProfileSaveRequired,
        ResetRequired,
        NotApplicable,
        NoActionsNeeded
    }
}