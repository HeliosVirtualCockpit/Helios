//  Copyright 2014 Craig Courtney
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

using System.Collections.Generic;

namespace GadrocsWorkshop.Helios
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Windows.Media.Effects;
    using System.Windows.Threading;
    using System.Xml;

    internal class ProfileManager : IProfileManager2
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static ShaderEffect _currentEffect;

        public bool SaveProfile(HeliosProfile profile)
        {            
            try
            {                
                string tempPath = Path.ChangeExtension(profile.Path, "tmp");
                string backupPath = Path.ChangeExtension(profile.Path, "bak");

                // Delete tmp file if exists
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }

                TextWriter writer = new StreamWriter(tempPath, false);
                TypeConverter boolConverter = TypeDescriptor.GetConverter(typeof(bool));

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;

                XmlWriter xmlWriter = XmlWriter.Create(writer, settings);
                HeliosBindingCollection bindings = new HeliosBindingCollection();

                HeliosSerializer serializer = new HeliosSerializer(null);
                serializer.SerializeProfile(profile, xmlWriter);

                profile.IsDirty = false;
                xmlWriter.Close();
                writer.Close();

                // Delete existing backup
                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                }

                // backup existing file
                if (File.Exists(profile.Path))
                {
                    File.Move(profile.Path, backupPath);
                }

                // Rename .tmp to actual
                File.Move(tempPath, profile.Path);

                profile.LoadTime = Directory.GetLastWriteTime(profile.Path);

                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error saving profile");
                return false;
            }
        }

        public HeliosProfile LoadProfile(string path)
        {
            try
            {
                HeliosProfile profile = LoadProfile(path, null, out IEnumerable<string> loadingWork);
                foreach (string progress in loadingWork)
                {
                    Logger.Debug(progress);
                }
                return profile;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error loading profile " + path);
                return null;
            }
        }

        public HeliosProfile LoadProfile(string path, Dispatcher dispatcher)
        {
            // this signature is illegal now but we keep it since it is in an interface
            throw new InvalidOperationException("LoadProfile with a dispatcher is no longer supported.");
        }

        public HeliosProfile LoadProfile(string path, out IEnumerable<string> loadingWork)
        {
            return LoadProfile(path, null, out loadingWork);
        }

        private HeliosProfile LoadProfile(string path, Dispatcher dispatcher, out IEnumerable<string> loadingWork)
        {
            if (ConfigManager.ImageManager is IImageManager2 imageManager2)
            {
                imageManager2.ClearFailureTracking();
            }

            if (!File.Exists(path))
            {
                loadingWork = new string[0];
                return null;
            }

            HeliosProfile profile = new HeliosProfile(false)
            {
                Path = path,
                Name = Path.GetFileNameWithoutExtension(path),
                IsDirty = false,
                // ReSharper disable once AssignNullToNotNullAttribute file exists asserted above
                LoadTime = Directory.GetLastWriteTime(path)
            };
            loadingWork = LoadingWork(profile, dispatcher);
            _currentEffect = profile.Effect;
            return profile;
        }

        private IEnumerable<string> LoadingWork(HeliosProfile profile, Dispatcher dispatcher)
        {
            XmlReaderSettings settings = new XmlReaderSettings
            {
                IgnoreComments = true,
                IgnoreWhitespace = true
            };

            using (TextReader reader = new StreamReader(profile.Path))
            {
                using (XmlReader xmlReader = XmlReader.Create(reader, settings))
                {
                    HeliosSerializer deserializer = new HeliosSerializer(dispatcher);
                    int profileVersion = deserializer.GetProfileVersion(xmlReader);

                    if (profileVersion != 3)
                    {
                        profile.IsInvalidVersion = true;
                    }
                    else
                    {
                        foreach (string progress in deserializer.DeserializeProfile(profile, xmlReader))
                        {
                            yield return progress;
                        }
                    }
                    xmlReader.Close();
                }
                reader.Close();
            }
        }
        public ShaderEffect CurrentEffect {
            get => _currentEffect;
            set { 
                if (!value.Equals(_currentEffect))
                {
                    _currentEffect = value;
                }
            } 
        }
    }

}
