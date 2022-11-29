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

using GadrocsWorkshop.Helios.ComponentModel;
using GadrocsWorkshop.Helios.Interfaces.Capabilities;
using System;
using System.Xml;

namespace GadrocsWorkshop.Helios.Interfaces.HeliosInformation
{
    [HeliosInterface("Helios.Base.HeliosInformation", "Helios", typeof(HeliosInformationInterfaceEditor),
        typeof(UniqueHeliosInterfaceFactory), AutoAdd = true)]
    public class HeliosInformationInterface : HeliosInterface, IExtendedDescription
    {
        private HeliosValue _heliosVersion;
        
        public HeliosInformationInterface()
            : base("Helios")
        {
            _heliosVersion = new HeliosValue(this, BindingValue.Empty, "Version", "Helios Version", "The Helios Version number.", "Example: 1.6.1000.0000", BindingValueUnits.Text);
            Values.Add(_heliosVersion);
            Triggers.Add(_heliosVersion);
        }

		private void Profile_ProfileTick(object sender, EventArgs e)
        {
            _heliosVersion.SetValue(new BindingValue(ConfigManager.HeliosVersion), false);
        }

        #region Overrides of HeliosInterface

        protected override void OnProfileChanged(HeliosProfile oldProfile)
        {
            base.OnProfileChanged(oldProfile);

            if (oldProfile != null)
            {
                oldProfile.ProfileTick -= Profile_ProfileTick;
            }

            if (Profile != null)
            {
                Profile.ProfileTick += Profile_ProfileTick;
            }
        }

		protected override void AttachToProfileOnMainThread()
		{
			base.AttachToProfileOnMainThread();
		}

		protected override void DetachFromProfileOnMainThread(HeliosProfile oldProfile)
		{
			base.DetachFromProfileOnMainThread(oldProfile);
		}

		public override void ReadXml(XmlReader reader)
		{
            // No-Op
        }

        public override void WriteXml(XmlWriter writer)
		{
            // No-Op
		}

		#endregion

		#region IExtendedDescription

		public string Description => "Interface to send key strokes or react to key presses";

        public string RemovalNarrative =>
            "Delete this interface and remove all of its bindings from the Profile, making it impossible to simulate or receive keyboard input.";

        #endregion
    }
}