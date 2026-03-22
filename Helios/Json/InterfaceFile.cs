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

using GadrocsWorkshop.Helios.Interfaces.Capabilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using GadrocsWorkshop.Helios.Util;

namespace GadrocsWorkshop.Helios.Json
{
    public class InterfaceHeader
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        public static string FindInterfaceFile(string filePattern)
        {
            string userInterfaceSpecsFolder = UserInterfaceSpecsFolder;
            string jsonPath = null;
            if (!Directory.Exists(userInterfaceSpecsFolder))
            {
                Logger.Debug("no user Interfaces folder at {Path}; not loading interfaces from documents", Anonymizer.Anonymize(userInterfaceSpecsFolder));
            }
            else
            {
                jsonPath = Directory.EnumerateFiles(userInterfaceSpecsFolder, filePattern, SearchOption.AllDirectories).FirstOrDefault();
            }

            // if not found in user files, try system files
            if (jsonPath == null)
            {
                string systemInterfaceSpecsFolder = SystemInterfaceSpecsFolder;
                if (Directory.Exists(systemInterfaceSpecsFolder))
                {
                    jsonPath = Directory.EnumerateFiles(systemInterfaceSpecsFolder, filePattern, SearchOption.AllDirectories).FirstOrDefault();
                }
                else
                {
                    Logger.Info("no system Interfaces folder at {Path}; not loading interfaces from documents", Anonymizer.Anonymize(systemInterfaceSpecsFolder));
                }
            }

            // if not a file, report it does not exist
            if (jsonPath != null && !File.Exists(jsonPath))
            {
                return null;
            }

            return jsonPath;
        }

        public static string FindInterfaceFileByInterfaceName(string interfaceName, InterfaceType interfaceType)
        {
            // NOTE: we only instantiate one of these per profile, and we always want latest information,
            // so there is no reason to cache or index anything and we just linear search all the headers
            // XXX measure how long this takes and see if maybe we have to do something like index caching based on file checksums
            Logger.Info("Searching for interface definition for {Name}", interfaceName);
            foreach (string specFilePath in EnumerateInterfaceFilePaths())
            {
                InterfaceHeader header = LoadHeader(specFilePath);
                if (header.Type == interfaceType &&
                    header.Name == interfaceName)
                {
                    Logger.Info("Found interface definition at {Path}", Anonymizer.Anonymize(specFilePath));
                    return specFilePath;
                }
            }
            return null;
        }

        public static IEnumerable<string> EnumerateInterfaceFilePaths()
        {
            IEnumerable<string> userPaths;
            string userInterfaceSpecsFolder = UserInterfaceSpecsFolder;
            if (Directory.Exists(userInterfaceSpecsFolder))
            {
                userPaths = Directory.EnumerateFiles(userInterfaceSpecsFolder, FilePattern, SearchOption.AllDirectories);
            }
            else
            {
                Logger.Debug("no user Interfaces folder at {Path}; not enumerating interfaces from documents", Anonymizer.Anonymize(userInterfaceSpecsFolder));
                userPaths = Array.Empty<string>();
            }

            IEnumerable<string> systemPaths;
            string systemInterfaceSpecsFolder = SystemInterfaceSpecsFolder;
            if (Directory.Exists(systemInterfaceSpecsFolder))
            {
                systemPaths = Directory.EnumerateFiles(systemInterfaceSpecsFolder, FilePattern, SearchOption.AllDirectories);
            }
            else
            {
                Logger.Info("no system Interfaces folder at {Path}; not loading interfaces from documents", Anonymizer.Anonymize(systemInterfaceSpecsFolder));
                systemPaths = Array.Empty<string>();
            }

            return userPaths.Concat(systemPaths);
        }

        public static InterfaceHeader LoadHeader(string jsonPath)
        {
            using (StreamReader file = File.OpenText(jsonPath))
            {
                // XXX check schema first and see if we can get decent readable error messages from that
                JsonSerializer serializer = new JsonSerializer();

                InterfaceHeader loaded = (InterfaceHeader) serializer.Deserialize(file, typeof(InterfaceHeader));
                if (null == loaded)
                {
                    throw new Exception("failed to load any header from interface JSON");
                }

                return loaded;
            }
        }

        /// <summary>
        /// types of interfaces that can be declared in JSON
        /// </summary>
        public enum InterfaceType
        {
            [Description(
                "the default, contains only parameters and functions for an existing interface type and does not create a new type")]
            Existing,

            [Description("DCS interface")] DCS
        }

        #region Properties

        [JsonProperty("source", Order = -16)] public string Source { get; set; } = "Helios";

        [JsonProperty("version", Order = -15)] public string VersionString { get; set; }

        [JsonProperty("commit", Order = -14)] public string Commit { get; set; } = "";

        [JsonProperty("type", Order = -13)]
        [JsonConverter(typeof(StringEnumConverter))]
        public InterfaceType Type { get; set; } = InterfaceType.Existing;

        /// <summary>
        /// display name of the interface
        /// </summary>
        [JsonProperty("name", Order = -12)]
        public string Name { get; set; }

        [JsonProperty("module", Order = -11)] public string Module { get; set; }
        [JsonProperty("vehicles", Order = -2)] public IList<string> Vehicles { get; set; }

        #endregion

        public static string FilePattern => "*.hif.json";

        public static string UserInterfaceSpecsFolder => Path.Combine(ConfigManager.DocumentPath, "Interfaces");

        private static string SystemInterfaceSpecsFolder => Path.Combine(ConfigManager.ApplicationPath, "Interfaces");
    }

    public class InterfaceFile<TFunction> : InterfaceHeader where TFunction : class, IBuildableFunction
    {
        /// <summary>
        /// load function definitions into an existing interface
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="functionHost"></param>
        /// <param name="jsonPath"></param>
        /// <returns></returns>
        public static InterfaceFile<TFunction> Load<TInterface>(TInterface functionHost, string jsonPath)
            where TInterface : class, ISoftInterface
        {
            using (StreamReader file = File.OpenText(jsonPath))
            {
                // XXX check schema first and see if we can get decent readable error messages from that
                JsonSerializer serializer = new JsonSerializer();
                serializer.Converters.Add(new Function.ConverterForJsonNet<TFunction, TInterface>());
                serializer.Converters.Add(new BindingValueUnit.ConverterForJsonNet());

                // don't append to arrays
                serializer.ObjectCreationHandling = ObjectCreationHandling.Replace;

                // this isn't a cross process capable context, so we are zeroing all the bits of the valid states
                serializer.Context = new StreamingContext(0, functionHost);
                InterfaceFile<TFunction> loaded = (InterfaceFile<TFunction>) serializer.Deserialize(file,
                    typeof(InterfaceFile<TFunction>));
                if (null == loaded)
                {
                    throw new Exception("failed to load any functions from interface JSON");
                }

                return loaded;
            }
        }

        #region Properties

        [JsonProperty("functions", Order = -1)]
        public IEnumerable<TFunction> Functions { get; set; }

        #endregion
    }
}