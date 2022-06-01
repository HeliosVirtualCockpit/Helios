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

namespace GadrocsWorkshop.Helios.Gauges.AH64D.CMWS
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.AH64D.CMWS_THREATDISPLAY", "CMWS Threat Display", "AH-64D", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class CMWSThreatDisplay : BaseGauge
    {
        private HeliosValue _readyFlag;
        private HeliosValue _dispensingFlag;
        private HeliosValue _threatDirectionFrontLeft;
        private HeliosValue _threatDirectionFrontRight;
        private HeliosValue _threatDirectionAftLeft;
        private HeliosValue _threatDirectionAftRight;
        private HeliosValue _threatArrowFront;
        private HeliosValue _threatArrowRight;
        private HeliosValue _threatArrowLeft;
        private HeliosValue _threatArrowAft;

        private GaugeImage _arrowBackgroundImage;
        private GaugeImage _readyFlagImage;
        private GaugeImage _dispensingFlagImage;
        private GaugeImage _threatDirectionFrontLeftImage;
        private GaugeImage _threatDirectionFrontRightImage;
        private GaugeImage _threatDirectionAftLeftImage;
        private GaugeImage _threatDirectionAftRightImage;
        private GaugeImage _threatArrowFrontImage;
        private GaugeImage _threatArrowRightImage;
        private GaugeImage _threatArrowLeftImage;
        private GaugeImage _threatArrowAftImage;
        public CMWSThreatDisplay()
            : base("CMWS Threat Display", new Size(350, 350))
        {
            double arrowDisplayX = 0; double arrowDisplayY = 0;
            _arrowBackgroundImage = new GaugeImage("{Helios}/Images/AH-64D/CMWS/cmws_threat_background.xaml", new Rect(arrowDisplayX, arrowDisplayY,NativeSize.Width, NativeSize.Height));
            _arrowBackgroundImage.IsHidden = false;
            Components.Add(_arrowBackgroundImage);

            _readyFlagImage = new GaugeImage("{Helios}/Images/AH-64D/CMWS/cmws_r.xaml", new Rect(arrowDisplayX, arrowDisplayY,NativeSize.Width, NativeSize.Height));
            _readyFlagImage.IsHidden = true;
            Components.Add(_readyFlagImage);

            _readyFlag = new HeliosValue(this, new BindingValue(false), "CMWS Display_Threat Display", "Ready Flag", "CMWS is Ready.", "True if displayed.", BindingValueUnits.Boolean);
            _readyFlag.Execute += new HeliosActionHandler(Flag_Execute);
            Actions.Add(_readyFlag);

            _dispensingFlagImage = new GaugeImage("{Helios}/Images/AH-64D/CMWS/cmws_d.xaml", new Rect(arrowDisplayX, arrowDisplayY,NativeSize.Width, NativeSize.Height));
            _dispensingFlagImage.IsHidden = true;
            Components.Add(_dispensingFlagImage);

            _dispensingFlag = new HeliosValue(this, new BindingValue(false), "CMWS Display_Threat Display", "Dispensing Flag", "CMWS is dispensing Taking action.", "True if displayed.", BindingValueUnits.Boolean);
            _dispensingFlag.Execute += new HeliosActionHandler(Flag_Execute);
            Actions.Add(_dispensingFlag);

            _threatDirectionFrontLeftImage = new GaugeImage("{Helios}/Images/AH-64D/CMWS/cmws_topleft.xaml", new Rect(arrowDisplayX, arrowDisplayY,NativeSize.Width, NativeSize.Height));
            _threatDirectionFrontLeftImage.IsHidden = true;
            Components.Add(_threatDirectionFrontLeftImage);

            _threatDirectionFrontLeft = new HeliosValue(this, new BindingValue(false), "CMWS Display_Threat Display", "Threat Direction Front Left", "Displayed when there is a threat on front / left corner.", "True if displayed.", BindingValueUnits.Boolean);
            _threatDirectionFrontLeft.Execute += new HeliosActionHandler(Flag_Execute);
            Actions.Add(_threatDirectionFrontLeft);

            _threatDirectionFrontRightImage = new GaugeImage("{Helios}/Images/AH-64D/CMWS/cmws_topright.xaml", new Rect(arrowDisplayX, arrowDisplayY,NativeSize.Width, NativeSize.Height));
            _threatDirectionFrontRightImage.IsHidden = true;
            Components.Add(_threatDirectionFrontRightImage);

            _threatDirectionFrontRight = new HeliosValue(this, new BindingValue(false), "CMWS Display_Threat Display", "Threat Direction Front Right", "Displayed when there is a threat on front / right corner.", "True if displayed.", BindingValueUnits.Boolean);
            _threatDirectionFrontRight.Execute += new HeliosActionHandler(Flag_Execute);
            Actions.Add(_threatDirectionFrontRight);

            _threatDirectionAftLeftImage = new GaugeImage("{Helios}/Images/AH-64D/CMWS/cmws_bottomleft.xaml", new Rect(arrowDisplayX, arrowDisplayY,NativeSize.Width, NativeSize.Height));
            _threatDirectionAftLeftImage.IsHidden = true;
            Components.Add(_threatDirectionAftLeftImage);

            _threatDirectionAftLeft = new HeliosValue(this, new BindingValue(false), "CMWS Display_Threat Display", "Threat Direction Aft Left", "Displayed when there is a threat on rear / left corner.", "True if displayed.", BindingValueUnits.Boolean);
            _threatDirectionAftLeft.Execute += new HeliosActionHandler(Flag_Execute);
            Actions.Add(_threatDirectionAftLeft);

            _threatDirectionAftRightImage = new GaugeImage("{Helios}/Images/AH-64D/CMWS/cmws_bottomright.xaml", new Rect(arrowDisplayX, arrowDisplayY,NativeSize.Width, NativeSize.Height));
            _threatDirectionAftRightImage.IsHidden = true;
            Components.Add(_threatDirectionAftRightImage);

            _threatDirectionAftRight = new HeliosValue(this, new BindingValue(false), "CMWS Display_Threat Display", "Threat Direction Aft Right", "Displayed when there is a threat on rear / right corner.", "True if displayed.", BindingValueUnits.Boolean);
            _threatDirectionAftRight.Execute += new HeliosActionHandler(Flag_Execute);
            Actions.Add(_threatDirectionAftRight);

            _threatArrowFrontImage = new GaugeImage("{Helios}/Images/AH-64D/CMWS/cmws_front.xaml", new Rect(arrowDisplayX, arrowDisplayY,NativeSize.Width, NativeSize.Height));
            _threatArrowFrontImage.IsHidden = true;
            Components.Add(_threatArrowFrontImage);

            _threatArrowFront = new HeliosValue(this, new BindingValue(false), "CMWS Display_Threat Display", "Threat Zone Front", "Displayed when both front detectors are active.", "True if displayed.", BindingValueUnits.Boolean);
            _threatArrowFront.Execute += new HeliosActionHandler(Flag_Execute);
            Actions.Add(_threatArrowFront);

            _threatArrowLeftImage = new GaugeImage("{Helios}/Images/AH-64D/CMWS/cmws_left.xaml", new Rect(arrowDisplayX, arrowDisplayY,NativeSize.Width, NativeSize.Height));
            _threatArrowLeftImage.IsHidden = true;
            Components.Add(_threatArrowLeftImage);

            _threatArrowLeft = new HeliosValue(this, new BindingValue(false), "CMWS Display_Threat Display", "Threat Zone Left", "Displayed when both left detectors are active.", "True if displayed.", BindingValueUnits.Boolean);
            _threatArrowLeft.Execute += new HeliosActionHandler(Flag_Execute);
            Actions.Add(_threatArrowLeft);

            _threatArrowAftImage = new GaugeImage("{Helios}/Images/AH-64D/CMWS/cmws_rear.xaml", new Rect(arrowDisplayX, arrowDisplayY,NativeSize.Width, NativeSize.Height));
            _threatArrowAftImage.IsHidden = true;
            Components.Add(_threatArrowAftImage);

            _threatArrowAft = new HeliosValue(this, new BindingValue(false), "CMWS Display_Threat Display", "Threat Zone Aft", "Displayed when both rear detectors are active.", "True if displayed.", BindingValueUnits.Boolean);
            _threatArrowAft.Execute += new HeliosActionHandler(Flag_Execute);
            Actions.Add(_threatArrowAft);

            _threatArrowRightImage = new GaugeImage("{Helios}/Images/AH-64D/CMWS/cmws_right.xaml", new Rect(arrowDisplayX, arrowDisplayY,NativeSize.Width, NativeSize.Height));
            _threatArrowRightImage.IsHidden = true;
            Components.Add(_threatArrowRightImage);

            _threatArrowRight = new HeliosValue(this, new BindingValue(false), "CMWS Display_Threat Display", "Threat Zone Right", "Displayed when both right detectors are active.", "True if displayed.", BindingValueUnits.Boolean);
            _threatArrowRight.Execute += new HeliosActionHandler(Flag_Execute);
            Actions.Add(_threatArrowRight);

            void Flag_Execute(object action, HeliosActionEventArgs e)
            {
                HeliosValue hAction = (HeliosValue)action;
                Boolean hActionVal = !(e.Value.DoubleValue > 0d ? true : false);
                switch (hAction.Name)
                {
                    case "Ready Flag":
                        _readyFlag.SetValue(e.Value, e.BypassCascadingTriggers);
                        _readyFlagImage.IsHidden = hActionVal;
                        break;
                    case "Dispensing Flag":
                        _dispensingFlag.SetValue(e.Value, e.BypassCascadingTriggers);
                        _dispensingFlagImage.IsHidden = hActionVal;
                        break;
                    case "Threat Direction Front Left":
                        _threatDirectionFrontLeft.SetValue(e.Value, e.BypassCascadingTriggers);
                        _threatDirectionFrontLeftImage.IsHidden = hActionVal;
                        break;
                    case "Threat Direction Front Right":
                        _threatDirectionFrontRight.SetValue(e.Value, e.BypassCascadingTriggers);
                        _threatDirectionFrontRightImage.IsHidden = hActionVal;
                        break;
                    case "Threat Direction Aft Left":
                        _threatDirectionAftLeft.SetValue(e.Value, e.BypassCascadingTriggers);
                        _threatDirectionAftLeftImage.IsHidden = hActionVal;
                        break;
                    case "Threat Direction Aft Right":
                        _threatDirectionAftRight.SetValue(e.Value, e.BypassCascadingTriggers);
                        _threatDirectionAftRightImage.IsHidden = hActionVal;
                        break;
                    case "Threat Zone Front":
                        _threatArrowFront.SetValue(e.Value, e.BypassCascadingTriggers);
                        _threatArrowFrontImage.IsHidden = hActionVal;
                        break;
                    case "Threat Zone Right":
                        _threatArrowRight.SetValue(e.Value, e.BypassCascadingTriggers);
                        _threatArrowRightImage.IsHidden = hActionVal;
                        break;
                    case "Threat Zone Left":
                        _threatArrowLeft.SetValue(e.Value, e.BypassCascadingTriggers);
                        _threatArrowLeftImage.IsHidden = hActionVal;
                        break;
                    case "Threat Zone Aft":
                        _threatArrowAft.SetValue(e.Value, e.BypassCascadingTriggers);
                        _threatArrowAftImage.IsHidden = hActionVal;
                        break;
                    default:
                        break;
                }

            }
        }
    }
}
