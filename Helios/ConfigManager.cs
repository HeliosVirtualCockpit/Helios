//  Copyright 2014 Craig Courtney
//  Copyright 2022 Helios Contributors
//
//  Helios is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  Helios is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Windows;
using System.Collections.Generic;
using Microsoft.Win32;

namespace GadrocsWorkshop.Helios
{
    public class ConfigManager
    {
        private static string _documentPath;

        /// <summary>
        /// Private constructor to prevent instances.  This class is a Singleton which should be accessed
        /// </summary>
        static ConfigManager()
        {
            string assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            ApplicationPath = Path.GetDirectoryName(assemblyLocation);
            BMSFalconPath = GetBMSFalconPath();
        }

        #region Properties

        public static string DocumentPath
        {
            get => _documentPath;
            set
            {
                _documentPath = value;
                ImagePath = Path.Combine(_documentPath, "Images");
                ProfilePath = Path.Combine(_documentPath, "Profiles");
                TemplatePath = Path.Combine(_documentPath, "Templates");

                List<string> paths = new List<string> { _documentPath, ImagePath, ProfilePath, TemplatePath };
                bool wroteSomething = false;
                foreach (string path in paths)
                {
                    if (!Directory.Exists(path))
                    {
                        try
                        {
                            Directory.CreateDirectory(path);
                            wroteSomething = true;
                        }
                        catch (Exception)
                        {
                            // we probably cannot log, so don't try and make it worse
                            ReportFatalWriteFailureAndExit(path);
                            return;
                        }
                    }
                }

                if (wroteSomething)
                {
                    // already successfully created a folder
                    return;
                }

                try
                {
                    // if we survived to here without needing to create a folder, make sure we test
                    // for write access here before we crash later in weird ways
                    string tempFileName = System.IO.Path.Combine(_documentPath, System.IO.Path.GetRandomFileName());
                    using (FileStream fs = File.Create(tempFileName, 1, FileOptions.DeleteOnClose))
                    {
                        _ = fs;
                    }
                }
                catch (Exception)
                {
                    ReportFatalWriteFailureAndExit(_documentPath);
                }
            }
        }

        private static void ReportFatalWriteFailureAndExit(string path)
        {
            MessageBox.Show($"Helios could not write to its own data folder at '{path}'.  " 
                + "Most likely you have an anti virus or 'application firewall' that is trying to protect your 'Documents' folder.\n\n"
                + "These types of features are sometimes called 'Ransomware Protection' or 'Controlled Folder Access' (in Windows).\n\n"
                + "You will need to give permission to 'Profile Editor.exe', 'HeliosPatching.exe' and 'Control Center.exe' so that\n"
                + "Helios can use its own folders.\n\n"
                + "Helios will now exit.",
                "Write Access to Helios Folder Denied");

            // we cannot gracefully exit
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        /// <summary>
        /// full path to directory where Helios.dll is installed
        /// </summary>
        public static string ApplicationPath { get; }

        public static string ProfilePath { get; private set; }
        public static string ImagePath { get; private set; }
        public static string TemplatePath { get; private set; }
        public static string BMSFalconPath { get; private set; }

        public static IImageManager ImageManager { get; internal set; }

        public static ITemplateManager TemplateManager { get; internal set; }

        public static IModuleManager ModuleManager { get; internal set; }

        public static IProfileManager2 ProfileManager { get; internal set; }

        public static DisplayManager DisplayManager { get; internal set; }

        public static UndoManager UndoManager { get; internal set; }

        public static LogManager LogManager { get; internal set; }

        public static ISettingsManager SettingsManager { get; internal set; }

        public static HeliosApplication Application { get; internal set; }
        public static VersionChecker VersionChecker { get; internal set; }

        /// <summary>
        /// private fonts for use in Helios
        /// </summary>
        public static FontManager FontManager { get; internal set; }

        public static string HeliosVersion
		{
            get
			{
                Version ver = RunningVersion.FromHeliosAssembly();
                return ver.Major.ToString() + "." + ver.Minor.ToString() + "." + ver.Build.ToString("0000") + "." + ver.Revision.ToString("0000");
            }
		}

        #endregion

        private static string GetBMSFalconPath()
        {
            RegistryKey pathKey;
            string[] subkeys;
            string falconRootKey = @"SOFTWARE\WOW6432Node\Benchmark Sims\";
            string falconVersion = "";
            string pathValue = "";

            if (Registry.LocalMachine.OpenSubKey(falconRootKey) != null)
            {
                subkeys = Registry.LocalMachine.OpenSubKey(falconRootKey).GetSubKeyNames();
                if (subkeys.Length > 0)
                {
                    Array.Reverse(subkeys);
                    falconVersion = subkeys[0];
                }
            }

            pathKey = Registry.LocalMachine.OpenSubKey(falconRootKey + falconVersion);

            if (pathKey != null)
            {
                pathValue = (string)pathKey.GetValue("baseDir") ?? "";
            }
            return pathValue;
        }
    }
}
