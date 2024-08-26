using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;

namespace GadrocsWorkshop.Helios
{
    /// <summary>
    /// private fonts for use in Helios
    /// </summary>
    public class FontManager
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly Dictionary<string, FontFamily> _byName = new Dictionary<string, FontFamily>();

        internal FontManager()
        {
            // AddFont("SFDigitalReadout-Medium.ttf");
        }

        public Dictionary<string, FontFamily> PrivateFontFamilys { get => _byName; }
        public FontFamily GetFontFamilyByName(string name)
        {
            if (_byName.TryGetValue(name, out FontFamily family))
            {
                return family;
            }
            return new FontFamily(name);
        }

        internal void LoadInstalledPrivateFonts()
        {
            if (!Directory.Exists(ConfigManager.ApplicationPath))
            {
                return;
            }

            // NOTE: only true type fully supported (?)
            // https://stackoverflow.com/questions/36986131/systemdrawingtextprivatefontcollection-not-working-when-the-font-is-not-in
            string fontsPath = Path.Combine(ConfigManager.ApplicationPath, "Fonts");
            foreach (string fontFilePath in Directory.EnumerateFiles(fontsPath, "*.ttf", SearchOption.AllDirectories))
            {
                // load the TTF, just to get font family names and face names
                GlyphTypeface glyphTypeface = new GlyphTypeface(new Uri(fontFilePath));

                // load every combination of names in every included language
                foreach (string fontFamilyName in glyphTypeface.FamilyNames.Values)
                {
                    foreach (string fontFaceName in glyphTypeface.FaceNames.Values)
                    {
                        // construct the full name
                        string fullName = $"{fontFamilyName} {fontFaceName}";

                        // now load the exact font we want
                        string fullFontPath = Path.Combine(fontsPath, $"#{fullName}");
                        FontFamily loaded = new FontFamily(fullFontPath);

                        // index
                        _byName[fullName] = loaded;

                        // log all the loaded font names
                        Logger.Info("Loaded private font {Font} for use in Helios", fullName);
                    }
                }
            }
        }
    }
}
