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
using System.Collections.Generic;
using System.Windows;
using GadrocsWorkshop.Helios.ComponentModel;

namespace GadrocsWorkshop.Helios.Interfaces.Falcon.Gauges.RWR
{
	[HeliosControl("Helios.Falcon.RWR", "Block 50/52 RWR", "Falcon Simulator", typeof(RWRRenderer))] 
	public class RWR : HeliosVisual
	{
		private bool _on;
		private Dictionary<string, HeliosValue> _values = new Dictionary<string, HeliosValue>();
		private FalconInterface _falconInterface;
		private RadarContact[] _contacts;
		private bool _flash4Hz;

		private string _bezelImage = "{HeliosFalcon}/Images/RWR/rwr_bezel.png";
		private string[] _rwrInfo;

		public RWR()
			: base("RWR", new Size(400, 387))
		{
			AddValue("Threat Warning Prime", "handoff indicator", "Threat warning prime handoff dot indicator.", "True if lit.", BindingValueUnits.Boolean);
			AddValue("Threat Warning Prime", "launch indicator", "Threat warning prime launch indicator.", "True if lit.", BindingValueUnits.Boolean);
			AddValue("Threat Warning Prime", "prioirty mode indicator", "Threat warning prime priority mode indicator.", "True if lit.", BindingValueUnits.Boolean);
			AddValue("Threat Warning Prime", "open mode indicator", "Threat warning prime open mode indicator.", "True if lit.", BindingValueUnits.Boolean);
			AddValue("Threat Warning Prime", "naval indicator", "Threat warning prime naval indicator.", "True if lit.", BindingValueUnits.Boolean);
			AddValue("Threat Warning Prime", "unknown indicator", "Threat warning prime unkown indicator.", "True if lit.", BindingValueUnits.Boolean);
			AddValue("Threat Warning Prime", "unknown mode indicator", "Threat warning prime unkown mode indicator.", "True if lit.", BindingValueUnits.Boolean);
			AddValue("Threat Warning Prime", "target step indicator", "Threat warning prime target step indicator.", "True if lit.", BindingValueUnits.Boolean);
			AddValue("Aux Threat Warning", "search indicator", "Aux threat warning search indicator.", "True if lit.", BindingValueUnits.Boolean);
			AddValue("Aux Threat Warning", "activity indicator", "Aux threat warning activity indicator.", "True if lit.", BindingValueUnits.Boolean);
			AddValue("Aux Threat Warning", "low altitude indicator", "Aux threat warning low altitude indicator.", "True if lit.", BindingValueUnits.Boolean);
			AddValue("Aux Threat Warning", "power indicator", "Aux threat warning system power indicator.", "True if lit.", BindingValueUnits.Boolean);
		}

		#region Properties

		/// <summary>
		/// Returns true if the RWR is powered on.
		/// </summary>
		public bool IsOn
		{
			get
			{
				return _on;
			}
			private set
			{
				if (!_on.Equals(value))
				{
					bool oldValue = _on;
					_on = value;
					OnPropertyChanged("IsOn", oldValue, value, false);
				}
			}
		}


		/// <summary>
		/// Returns true if there are unknown radar warning contacts which are not visible on RWR scope
		/// </summary>
		private bool HasHiddenUnknownContacts
		{
			get
			{
				if (_contacts != null)
				{
					for (int i = 0; i < _contacts.Length; i++)
					{
						int symbol = (int)_contacts[i].Symbol;
						if (symbol < 0 ||
							symbol == 1 ||
							symbol == 27 ||
							symbol == 28 ||
							symbol == 29)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		/// <summary>
		/// Returns true if there are more than 5 radar warning contacts.
		/// </summary>
		private bool HasHiddenPriorityContacts
		{
			get
			{
				if (_contacts != null)
				{
					int visibleCount = 0;
					for (int i = 0; i < _contacts.Length; i++)
					{
						if (_contacts[i].Lethality > 0 && _contacts[i].Visible)
						{
							visibleCount++;
						}
					}

					return (visibleCount > 5);
				}
				return false;
			}
		}

		/// <summary>
		/// Returns true if there are search mode radar contacts detected.
		/// </summary>
		private bool HasSearchModeContacts
		{
			get
			{
				if (_contacts != null)
				{
					for (int i = 0; i < _contacts.Length; i++)
					{
						int symbol = (int)_contacts[i].Symbol;
						if ((symbol >= 5 && symbol <= 17) ||
							(symbol >= 19 && symbol <= 26) ||
							(symbol == 30) ||
							(symbol >= 54 && symbol <= 56))
						{
							if (_contacts[i].Lethality > 0 && !_contacts[i].Visible)
							{
								return true;
							}
						}
					}
				}
				return false;
			}
		}

		public string BezelImage
		{
			get
			{
				return _bezelImage;
			}
			set
			{
				if ((_bezelImage == null && value != null)
					|| (_bezelImage != null && !_bezelImage.Equals(value)))
				{
					string oldValue = _bezelImage;
					_bezelImage = value;
					OnPropertyChanged("BezelImage", oldValue, value, true);
				}
			}
		}

		internal bool Flash4Hz
		{
			get { return _flash4Hz; }
		}

		internal RadarContact[] Contacts
		{
			get { return _contacts; }
		}

		internal string[] RwrInfo => _rwrInfo;

		#endregion

		protected override void OnProfileChanged(HeliosProfile oldProfile)
		{
			base.OnProfileChanged(oldProfile);
			if (oldProfile != null)
			{
				oldProfile.ProfileStarted -= new EventHandler(Profile_ProfileStarted);
				oldProfile.ProfileTick -= new EventHandler(Profile_ProfileTick);
				oldProfile.ProfileStopped -= new EventHandler(Profile_ProfileStopped);
			}

			if (Profile != null)
			{
				Profile.ProfileStarted += new EventHandler(Profile_ProfileStarted);
				Profile.ProfileTick += new EventHandler(Profile_ProfileTick);
				Profile.ProfileStopped += new EventHandler(Profile_ProfileStopped);
			}
		}


		private void Profile_ProfileStopped(object sender, EventArgs e)
		{
			_falconInterface = null;
		}

		private void Profile_ProfileTick(object sender, EventArgs e)
		{
			if (_falconInterface != null)
			{
				_flash4Hz = DateTime.Now.Millisecond % 250 < 125;
				_contacts = _falconInterface.RadarContacts;
				_rwrInfo = _falconInterface.RwrInfo;
				ProcessIndicators();
				OnDisplayUpdate();
			}
		}

		private void Profile_ProfileStarted(object sender, EventArgs e)
		{
			if (Parent.Profile.Interfaces.ContainsKey("Falcon"))
			{
				_falconInterface = Parent.Profile.Interfaces["Falcon"] as FalconInterface;
			}
		}

		private void ProcessIndicators()
		{
			BindingValue rwrPower = _falconInterface.GetValue("Aux Threat Warning", "power indicator");
			IsOn = rwrPower.BoolValue;

			SetValue("Threat Warning Prime", "handoff indicator", _falconInterface.GetValue("Threat Warning Prime", "handoff indicator"));
			SetValue("Threat Warning Prime", "launch indicator", new BindingValue(_falconInterface.GetValue("Threat Warning Prime", "launch indicator").BoolValue ? _flash4Hz : false));

			BindingValue twpPriority = _falconInterface.GetValue("Threat Warning Prime", "prioirty mode indicator");
			if (twpPriority.BoolValue && HasHiddenPriorityContacts)
			{
				SetValue("Threat Warning Prime", "prioirty mode indicator", new BindingValue(_flash4Hz));
			}
			else
			{
				SetValue("Threat Warning Prime", "prioirty mode indicator", twpPriority);
			}
			SetValue("Threat Warning Prime", "open mode indicator", new BindingValue(rwrPower.BoolValue && !twpPriority.BoolValue));
			SetValue("Threat Warning Prime", "naval indicator", _falconInterface.GetValue("Threat Warning Prime", "naval indicator"));

			BindingValue twpUnknown = _falconInterface.GetValue("Threat Warning Prime", "unknown indicator");
			if (rwrPower.BoolValue && !twpUnknown.BoolValue && HasHiddenUnknownContacts)
			{
				SetValue("Threat Warning Prime", "unknown indicator", new BindingValue(_flash4Hz));
			}
			else
			{
				SetValue("Threat Warning Prime", "unknown indicator", twpUnknown);
			}
			SetValue("Threat Warning Prime", "unknown mode indicator", twpUnknown);

			SetValue("Threat Warning Prime", "target step indicator", _falconInterface.GetValue("Threat Warning Prime", "target step indicator"));

			BindingValue twaSearch = _falconInterface.GetValue("Aux Threat Warning", "search indicator");
			if (!twaSearch.BoolValue && HasSearchModeContacts)
			{
				SetValue("Aux Threat Warning", "search indicator", new BindingValue(_flash4Hz));
			}
			else
			{
				SetValue("Aux Threat Warning", "search indicator", twaSearch);
			}

			SetValue("Aux Threat Warning", "activity indicator", _falconInterface.GetValue("Aux Threat Warning", "activity indicator"));
			SetValue("Aux Threat Warning", "low altitude indicator", _falconInterface.GetValue("Aux Threat Warning", "low altitude indicator"));
			SetValue("Aux Threat Warning", "power indicator", _falconInterface.GetValue("Aux Threat Warning", "power indicator"));
		}

		private HeliosValue AddValue(string device, string name, string description, string valueDescription, BindingValueUnit unit)
		{
			HeliosValue value = new HeliosValue(this, BindingValue.Empty, device, name, description, valueDescription, unit);
			Triggers.Add(value);
			Values.Add(value);
			_values.Add(device + "." + name, value);
			return value;
		}

		private void SetValue(string device, string name, BindingValue value)
		{
			string key = device + "." + name;
			if (_values.ContainsKey(key))
			{
				_values[key].SetValue(value, false);
			}
		}

		public override void MouseDown(Point location)
		{
			// No-Op
		}

		public override void MouseDrag(Point location)
		{
			// No-Op;
		}

		public override void MouseUp(Point location)
		{
			// No-Op;
		}
	}
}
