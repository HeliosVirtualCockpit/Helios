// Copyright 2021 Ammo Goettsch
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

namespace GadrocsWorkshop.Helios.Interfaces.Capabilities
{
    /// <summary>
    /// a HeliosInterface that can be deserialized
    /// </summary>
    public interface ISoftInterface
    {
        #region Hooks

        Type ResolveFunctionType(string typeName);
        IList<string> ImpersonatedVehicles { get; }
        #endregion
    }
}