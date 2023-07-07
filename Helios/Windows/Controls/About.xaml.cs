//  Copyright 2014 Craig Courtney
//  Copyright 2020 Helios Contributors
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
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GadrocsWorkshop.Helios.Util;

namespace GadrocsWorkshop.Helios.Windows.Controls
{
    /// <summary>
    /// Splash screen for Helios, currently also used as about box without any buttons
    /// </summary>
    public partial class About : Window
    {
        // primary authors of the code
        public static readonly string[] Authors = { "Gadroc", "derammo", "BlueFinBima" };

        // these are listed alphabetically
        public static string[] Contributors =>
            ContributorsArray.OrderBy(name => name, StringComparer.InvariantCultureIgnoreCase).ToArray();

        // raw contributor list in no particular order, sorted alphabetically when displayed
        private static readonly string[] ContributorsArray = 
        {
            "BeamRider",
            "CaptZeen",         // for making Helios more than a toolbox
            "Cylution",         // MiG-21 Interface
            "damien022",        // Composite Visual
            "Jabbers",          // for expert counsel and a little bit of code
            "KiwiLostInMelb",   // Key Sender
            "Phar71",
            "Rachmaninoff",
            "wheelchock",       // for reviving BMS support and General Development work
            "Will Hartsell",    // for IRIS
            "WillianG83",
            "yzfanimal",
            "ZoeESummers",      // for UX work
            "ertiyu",           // for testing
            "Polaris",          // for testing
            "Sliceback",        // for testing
            "norsetto",         // for M-2000C work
            "Biluf",            // for M-2000C viewports
            "linknet",          // for BMS & General Development work,
            "talbotmcinnis",    // for code contribution
            "MadKreator37"      // Testing, F-15E work and community support
        };

        public About()
        {
            InitializeComponent();
        }

        protected override void OnActivated(EventArgs e)
        {
            Version runningVersion = ConfigManager.VersionChecker.RunningVersion;
            VersionBlock.Text = runningVersion.Major.ToString() + "." + runningVersion.Minor.ToString() + "." + runningVersion.Build.ToString() + "." + runningVersion.Revision.ToString("0000");
            AuthorsBlock.Text = string.Join("; ", Authors);
            ContributionBlock.Text = string.Join("; ", Contributors);
            StatusBlock.Text = RunningVersion.IsDevelopmentPrototype ? "Development Prototype" : "Released";
            ForkBlock.Text = $"Fork: {KnownLinks.GitRepoUrl()}";
            base.OnActivated(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            Close();
            base.OnKeyDown(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            Close();
            base.OnMouseDown(e);
        }
    }
}
