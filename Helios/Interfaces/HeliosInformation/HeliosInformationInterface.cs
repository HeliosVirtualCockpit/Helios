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

namespace GadrocsWorkshop.Helios.Interfaces.HeliosInformation
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Interfaces.Capabilities;
    using System;
    using System.Xml;

    [HeliosInterface("Helios.Base.HeliosInformation", "Helios Information", typeof(HeliosInformationInterfaceEditor),
        typeof(UniqueHeliosInterfaceFactory))]
    public class HeliosInformationInterface : HeliosInterface, IExtendedDescription
    {
        private HeliosValue _heliosVersion;
        private HeliosValue _heliosProfileName;

        public HeliosInformationInterface()
            : base("Helios Information")
        {
            _heliosVersion = new HeliosValue(this, BindingValue.Empty, "Helios Version", "Helios Version", "The Helios Version number.", "Example: 1.6.1000.0000", BindingValueUnits.Text);
            Values.Add(_heliosVersion);
            Triggers.Add(_heliosVersion);

            _heliosProfileName = new HeliosValue(this, BindingValue.Empty, "", "Active Profile Name", "Name of the Active Profile.", "Text value without filetype suffix", BindingValueUnits.Text);
            Values.Add(_heliosProfileName);
            Triggers.Add(_heliosProfileName);
        }

        private void Profile_ProfileTick(object sender, EventArgs e)
        {
            _heliosVersion.SetValue(new BindingValue(ConfigManager.HeliosVersion), false);
            _heliosProfileName.SetValue(new BindingValue(ConfigManager.ProfileName), false);
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

		public string Description => "Interface to provide details about the Helios Version you are running.";

        public string RemovalNarrative =>
            "Delete this interface and remove all of its bindings from the Profile.";

        #endregion
    }
}