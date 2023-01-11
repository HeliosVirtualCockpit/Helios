// Copyright 2023 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Interfaces.Vendor.Functions
{
    /// <summary>
    /// Helios interfaces that implement this interface can be asked to create extra triggers and values
    /// for controlling indicators on USB devices.
    /// </summary>
    public interface IHotasFunctions
    {
        void Reset();
        void CreateActionsAndValues();
    }
}

