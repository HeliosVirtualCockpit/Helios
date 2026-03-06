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

using GadrocsWorkshop.Helios.ComponentModel;
using GadrocsWorkshop.Helios.Interfaces.Capabilities;
using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
using GadrocsWorkshop.Helios.Json;
using GadrocsWorkshop.Helios.Util;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GadrocsWorkshop.Helios.Interfaces.DCS.Soft
{
    /// <summary>
    /// DCS interface that is entirely defined in an Interface File and can become an interface for any aircraft
    /// </summary>
    [HeliosInterface("Helios.DCSInterfaceFile", "DCS Interface File", typeof(SoftInterfaceEditor),
        typeof(SoftInterfaceFactory), UniquenessKey = "Helios.DCSInterface")]
    public class SoftInterface : DCSInterface, ISoftInterface
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private IList<string> _vehicles;

        /// <summary>
        /// constructor used during Add Interface discovery
        /// </summary>
        /// <param name="name"></param>
        /// <param name="exportDeviceName"></param>
        /// <param name="specFilePath"></param>
        public SoftInterface(string name, string exportDeviceName, string specFilePath) : base(name, exportDeviceName,
            null)
        {
            SpecFilePath = specFilePath;
        }

        /// <summary>
        /// constructor used when deserializing from profile XML
        /// </summary>
        /// <param name="name"></param>
        public SoftInterface(string name) : base(name, null, null)
        {
            // leave everything null until Attach so we can get our XML read first
        }

        private void LoadInterfaceFile(string specFilePath)
        {
            // we are being instantiated for use, not just for the interface picker, so now we really load
            InterfaceFile<UDPInterface.NetworkFunction> loaded =
                InterfaceFile<UDPInterface.NetworkFunction>.Load(this, specFilePath);

            // find the Lua code next to the interface spec file
            string exportFunctionsPath = Path.ChangeExtension(specFilePath, "lua");
            if (File.Exists(exportFunctionsPath))
            {
                ExportFunctionsPath = exportFunctionsPath;
            }
            else
            {
                // not all interfaces have custom Lua code, that is ok
                Logger.Info(
                    "no custom Lua export functions found at {Path}; there will be no custom code in the associated interface",
                    Anonymizer.Anonymize(exportFunctionsPath));
            }

            // the first listed vehicle is all we support right now
            VehicleName = loaded.Vehicles.FirstOrDefault();
            _vehicles = loaded.Vehicles as IList<string>;
            // XXX enable this once implemented
            // ModuleName = loaded.Module

            // display warning if not done
            Incomplete = loaded.Functions.Any(interfaceFunction => interfaceFunction.Unimplemented);

            // add all the functions we read
            InstallFunctions(loaded);
        }

        private IList<StatusReportItem> CreateMissingFileStatus() =>
            new StatusReportItem
            {
                Severity = StatusReportItem.SeverityCode.Error,
                Status = $"The interface definition file for this interface ({Name}) was not found",
                Recommendation =
                    $"Please install the matching interface definition file ({InterfaceHeader.FilePattern}) to {InterfaceHeader.UserInterfaceSpecsFolder}",
                Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate,
                Link = StatusReportItem.ProfileEditor
            }.AsReport();

        #region Overrides

        protected override void AttachToProfileOnMainThread()
        {
            // if we are being deserialized from a profile, SpecFilePath will be null and we need to find the current spec file for this interface
            SpecFilePath = SpecFilePath ?? InterfaceHeader.FindInterfaceFileByInterfaceName(Name, InterfaceHeader.InterfaceType.DCS);

            if (HasInterfaceFile)
            {
                LoadInterfaceFile(SpecFilePath);
            }
            else
            {
                // graceful failure
                Logger.Error("Interface definition file for {Name} not installed; DCS interface will not function",
                    Name);

                // this will suppress an error and also cause this profile not to auto load
                VehicleName = "MissingFile";

                // XXX enable this once implemented
                // ModuleName = "MissingFile";
            }

            // now we are completely defined; start up
            base.AttachToProfileOnMainThread();
        }

        public override IEnumerable<StatusReportItem> PerformReadyCheck()
        {
            if (!HasInterfaceFile)
            {
                // missing interface file, so our status can't consider the configuration
                return CreateMissingFileStatus();
            }

            return base.PerformReadyCheck();
        }

        public override void InvalidateStatusReport()
        {
            if (!HasInterfaceFile)
            {
                // missing interface file, so our status can't consider the configuration
                PublishStatusReport(CreateMissingFileStatus());
                return;
            }

            base.InvalidateStatusReport();
        }

        #endregion

        #region Properties

        public string SpecFilePath { get; private set; }

        public bool HasInterfaceFile => SpecFilePath != null;

        public bool Incomplete { get; private set; }

        public override IList<string> ImpersonatedVehicles
        {
            get => _vehicles;
            // set { _vehicles = value; }
        }

        #endregion
    }
}