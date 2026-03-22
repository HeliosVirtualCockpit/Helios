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

using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
using GadrocsWorkshop.Helios.Interfaces.DCS.FC2;
using GadrocsWorkshop.Helios.Json.BindingValue;
using GadrocsWorkshop.Helios.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GadrocsWorkshop.Helios.Json
{
    public class InterfaceExport
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// find all the network functions; slow
        /// </summary>
        private IEnumerable<System.Type> FindNetworkFunctions() => FindAssemblies()
            .SelectMany(assembly => assembly.ExportedTypes)
            .Where(type => !type.IsAbstract)
            .Where(type => typeof(UDPInterface.NetworkFunction).IsAssignableFrom(type));

        /// <summary>
        /// enumerate all assemblies that may contains network functions; slow
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Assembly> FindAssemblies()
        {
            // core assembly, presumably Helios.dll
            yield return Assembly.GetAssembly(typeof(UDPInterface.NetworkFunction));

            // REVISIT: if we end up refactoring DCS interfaces out to other plugin assemblies, we will need a mechanism to find these
        }

        /// <summary>
        /// configures all our generator options except the special handling for NetworkFunction, so we can disable it
        /// </summary>
        /// <param name="generator"></param>
        public static void ConfigureGenerator(JSchemaGenerator generator)
        {
            generator.SchemaReferenceHandling = SchemaReferenceHandling.Objects;
            // generator.SchemaIdGenerationHandling = SchemaIdGenerationHandling.TypeName;
            generator.DefaultRequired = Required.DisallowNull;
            generator.GenerationProviders.Add(new BindingValueUnit.SchemaGenerationProviderForJsonNet());
            generator.GenerationProviders.Add(new StringEnumGenerationProvider());
        }

        public void GenerateInterfaceJson()
        {
            UDPInterface.BaseUDPInterface.IsWritingFunctionsToJson = true;
            foreach (HeliosInterfaceDescriptor descriptor in ConfigManager.ModuleManager.InterfaceDescriptors)
            {
                if (descriptor.ParentTypeIdentifier != null)
                {
                    // don't serialize child interfaces
                    continue;
                }

                HeliosInterface newInterface = descriptor.CreateInstance();
                if (!(newInterface is UDPInterface.BaseUDPInterface udpInterface))
                {
                    // we currently only export these types of interfaces
                    continue;
                }

                string name = udpInterface.TypeIdentifier;
                if ((name == null) || (name.Length < 1))
                {
                    name = udpInterface.Name;
                }

                string moduleName = name;
                IList<string> vehicles = null;
                if (udpInterface is DCSInterface dcs)
                {
                    moduleName = dcs.ModuleName;
                    vehicles = descriptor.ImpersonatedVehicleNames?? dcs.Vehicles;
                }

                IComparer<UDPInterface.NetworkFunction> functionComparer = new CanonicalFunctionOrder();
                InterfaceFile<UDPInterface.NetworkFunction> jsonObject =
                    new InterfaceFile<UDPInterface.NetworkFunction>
                    {
                        VersionString =
                            VersionChecker.VersionToString(Assembly.GetEntryAssembly()?.GetName().Version ?? new System.Version(0, 0, 0, 0)),
                        Module = moduleName,
                        Name = udpInterface.Name,
                        Functions = udpInterface.Functions
                            .Where(func => func.Serializable)
                            .OrderBy(func => func, functionComparer),
                        Vehicles = vehicles
                    };
                string jsonPath = Path.Combine(ConfigManager.DocumentPath, "Interfaces", "HeliosInterfaces", $"{name}.hif.json");
                Directory.CreateDirectory(Path.GetDirectoryName(jsonPath) ?? throw new System.Exception("document path must have a directory component"));
                File.WriteAllText(
                    jsonPath,
                    JsonConvert.SerializeObject(
                        jsonObject,
                        new JsonSerializerSettings
                        {
                            
                            Formatting = Formatting.Indented,
                            Converters = new List<JsonConverter>
                            {
                                new BindingValueUnit.ConverterForJsonNet(),
                                new ConverterForJsonNet()
                            }
                        })
                );
                if (udpInterface is DCSInterface dcsInterface && dcsInterface.ExportFunctionsPath != null)
                {
                    string luaPath = Path.ChangeExtension(jsonPath, "lua");
                    File.WriteAllText(luaPath, GetExportFunctions(dcsInterface));
                }
            }

            UDPInterface.BaseUDPInterface.IsWritingFunctionsToJson = false;
        }

        private string GetExportFunctions(DCSInterface dcs)
        {
            try
            {
                Uri exportFunctionsUri = new Uri(dcs.ExportFunctionsPath);
                if (exportFunctionsUri.IsFile)
                {
                    if (!File.Exists(dcs.ExportFunctionsPath))
                    {
                        // this should have been detected by parent
                        Logger.Error("custom Lua export functions not found at {Path}; there will be no custom code in the associated interface", Anonymizer.Anonymize(dcs.ExportFunctionsPath));
                        return "";
                    }

                    // this is allowed only for export functions, other resources must be read from compiled packs, which is
                    // why ReadResourceFile does not allow this case
                    return FileUtility.ReadFile(dcs.ExportFunctionsPath);
                }
                return Resources.ReadResourceFile(dcs.ExportFunctionsPath);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"failed to load interface-specific functions for {dcs.VehicleName} from '{Anonymizer.Anonymize(dcs.ExportFunctionsPath)}'");
                throw;
            }


        }
        public void GenerateInterfaceSchema()
        {
            JsonLicenses.LoadLicenses();
            JSchemaGenerator generator = new JSchemaGenerator();
            ConfigureGenerator(generator);
            generator.GenerationProviders.Add(new NetworkFunction.SchemaGenerationProviderForJsonNet());

            JSchema functions = new JSchema();
            foreach (System.Type networkFunctionType in FindNetworkFunctions())
            {
                functions.OneOf.Add(generator.Generate(networkFunctionType));
            }

            JSchema schema = generator.Generate(typeof(InterfaceFile<UDPInterface.NetworkFunction>));
            schema.Properties["functions"].Items.Clear();
            schema.Properties["functions"].Items.Add(functions);

            string schemaPath = Path.Combine(ConfigManager.DocumentPath, "Interfaces", "HeliosInterfaces", "hif.schema.json");
            Directory.CreateDirectory(Path.GetDirectoryName(schemaPath) ?? throw new System.Exception("document path for schema generation must have a directory component"));
            using (StreamWriter file = File.CreateText(schemaPath))
            using (JsonTextWriter writer = new JsonTextWriter(file)
            {
                Formatting = Formatting.Indented
            })
            {
                schema.WriteTo(writer, new JSchemaWriterSettings());
            }
        }
    }
}