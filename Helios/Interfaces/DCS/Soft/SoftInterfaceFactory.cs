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
using GadrocsWorkshop.Helios.Json;
using GadrocsWorkshop.Helios.Util;

namespace GadrocsWorkshop.Helios.Interfaces.DCS.Soft
{
    /// <summary>
    /// custom factory creates DCS interface instances from interface files
    /// </summary>
    public class SoftInterfaceFactory : UniqueHeliosInterfaceFactory
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private IEnumerable<HeliosInterface> LoadSoftInterfaces()
        {
            HashSet<string> loadedNames = new HashSet<string>();
            foreach (string specFilePath in InterfaceHeader.EnumerateInterfaceFilePaths())
            {
                InterfaceHeader header = InterfaceHeader.LoadHeader(specFilePath);
                if (header.Type != InterfaceHeader.InterfaceType.DCS)
                {
                    // only load fully DCS interfaces fully defined by files
                    continue;
                }

                if (loadedNames.Contains(header.Name))
                {
                    // already loaded, presumably because user file overrode a system file
                    Logger.Debug(
                        "Ignoring DCS interface {Name} at {Path} because an interface of that name was already loaded",
                        header.Name, Anonymizer.Anonymize(specFilePath));
                    continue;
                }

                Logger.Info("Found DCS interface {Name} at {Path}", header.Name,
                    Anonymizer.Anonymize(specFilePath));
                loadedNames.Add(header.Name);

                // create candidate instance, without loading it fully
                yield return new SoftInterface(header.Name, header.Module, specFilePath, header.Vehicles);
            }
        }

        #region Overrides

        public override List<HeliosInterface> GetInterfaceInstances(HeliosInterfaceDescriptor descriptor,
            HeliosProfile profile)
        {
            List<HeliosInterface> interfaces = new List<HeliosInterface>();

            if (!(ConfigManager.Application.AllowSoftInterfaces && GlobalOptions.HasAllowSoftInterfaces))
            {
                Logger.Debug("Soft interfaces disabled on this installation");
                return interfaces;
            }

            if (descriptor == null)
            {
                throw new ArgumentNullException(nameof(descriptor),
                    @"cannot load interfaces from null descriptor; broken implementation");
            }

            if (!IsUnique(descriptor, profile))
            {
                Logger.Debug("SoftInterface cannot coexist with interface already in profile");
                return interfaces;
            }

            // load installed interfaces
            interfaces.AddRange(LoadSoftInterfaces());

            return interfaces;
        }

        // no automatically added interfaces (empty list)
        public override List<HeliosInterface> GetAutoAddInterfaces(HeliosInterfaceDescriptor descriptor,
            HeliosProfile profile) => new List<HeliosInterface>();

        #endregion
    }
}