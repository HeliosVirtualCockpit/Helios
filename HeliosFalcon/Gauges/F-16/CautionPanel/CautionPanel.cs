// Copyright 2014 Craig Courtney
//  Copyright 2025 Helios Contributors
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



namespace GadrocsWorkshop.Helios.Gauges.Falcon.CautionPanel
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using GadrocsWorkshop.Helios.Interfaces.Falcon;
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;

    [HeliosControl("Helios.Falcon.CautionPanel", "Falcon BMS Caution Panel", "Falcon BMS F-16", typeof(GaugeRenderer))]
    class CautionPanel : BaseGauge
    {
        private FalconInterface _falconInterface;
        private List<GaugeImage> _cautionIndicators = new List<GaugeImage>();
        private static readonly string[] _cautionImages = new string[32];
        private static readonly Rect[] _cautionRects = new Rect[32];
        private static Rect _rectBase = new Rect(0d, 0d, 200, 156);
        private bool _inFlightLastValue;
        private const string _backplateImage = "{HeliosFalcon}/Gauges/F-16/CautionPanel/caution_panel.png";

        static CautionPanel()
        {
            // Initialize caution image paths and rectangles
            for (int i = 0; i < 32; i++)
            {
                _cautionImages[i] = $"{{HeliosFalcon}}/Gauges/F-16/CautionPanel/caution{i + 1:D2}.png";
            }

            // Initialize rectangles dynamically
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    int index = row * 4 + col;
                    _cautionRects[index] = new Rect(8d + col * 46d, 6d + row * 18d, 46d, 18d);
                }
            }
        }

        public CautionPanel()
            : base("CautionPanel", new Size(_rectBase.Width, _rectBase.Height))
        {
            AddComponents();
        }

        private void AddComponents()
        {
            Components.Add(new GaugeImage(_backplateImage, _rectBase));

            for (int i = 0; i < 32; i++)
            {
                var caution = new GaugeImage(_cautionImages[i], _cautionRects[i])
                {
                    IsHidden = true
                };
                _cautionIndicators.Add(caution);
                Components.Add(caution);
            }
        }

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

        private void Profile_ProfileStarted(object sender, EventArgs e)
        {
            if (Parent.Profile.Interfaces.ContainsKey("Falcon"))
            {
                _falconInterface = Parent.Profile.Interfaces["Falcon"] as FalconInterface;
            }
        }

        private void Profile_ProfileTick(object sender, EventArgs e)
        {
            if (_falconInterface != null)
            {
                BindingValue runtimeFlying = GetValue("Runtime", "Flying");
                bool inFlight = runtimeFlying.BoolValue;

                if (inFlight)
                {
                    ProcessBindingValues();
                    _inFlightLastValue = true;
                }
                else
                {
                    if (_inFlightLastValue)
                    {
                        ResetCautionPanel();
                        _inFlightLastValue = false;
                    }
                }
            }
        }

        private void Profile_ProfileStopped(object sender, EventArgs e)
        {
            _falconInterface = null;
        }


        public override void Reset()
        {
            ResetCautionPanel();
        }

        private void ResetCautionPanel()
        {
            ProcessBindingValues();


        }

        private void ProcessBindingValues()
        {
            for (int i = 0; i < _cautionIndicators.Count; i++)
            {
                string cautionName = GetCautionName(i + 1);
                _cautionIndicators[i].IsHidden = GetValue("Caution", cautionName).BoolValue == true || GetValue("Caution", "MAL/IND pressed").BoolValue == true ? false : true;
            }
        }

        private string GetCautionName(int index)
        {
            // Map index to caution names
            string[] cautionNames = {
            "flight control system indicator", "engine fault indicator", "avionics indicator", "seat arm indicator",
            "electric bus fail indicator", "second engine compressor indicator", "equip hot indicator", "nws fail indicator",
            "probe heat indicator", "fuel oil hot indicator", "radar altimeter indicator", "anti skid indicator",
            "cadc indicator", "Inlet Icing indicator", "iff indicator", "hook indicator",
            "stores config indicator", "overheat indicator", "oxygen low indicator", "atf not engaged",
            "ecm indicator", "cabin pressure indicator", "forward fuel low indicator", "backup fuel control indicator",
            "aft fuel low indicator", "MAL/IND pressed", "MAL/IND pressed", "MAL/IND pressed",
            "MAL/IND pressed", "MAL/IND pressed", "MAL/IND pressed", "MAL/IND pressed"
        };
            return cautionNames[index - 1];
        }

        private BindingValue GetValue(string device, string name)
        {
            return _falconInterface?.GetValue(device, name) ?? BindingValue.Empty;
        }
    }
}
