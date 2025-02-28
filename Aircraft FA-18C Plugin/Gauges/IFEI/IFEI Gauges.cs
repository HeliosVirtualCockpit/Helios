//  Copyright 2018 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Gauges.FA18C
{
    using GadrocsWorkshop.Helios.Gauges;
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System;
    using System.Windows.Media;
    using System.Windows;

    [HeliosControl("Helios.FA18C.IFEIGauges", "IFEI Needles & Flags", "F/A-18C Gauges", typeof(GaugeRenderer), HeliosControlFlags.NotShownInUI)]
    public class IFEI_Gauges : AltImageGauge
    {
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 1, 1);
        private Rect _scaledScreenRect = SCREEN_RECT;
        private double _glassReflectionOpacity;
        public const double GLASS_REFLECTION_OPACITY_DEFAULT = 0.3;

        private string _imageLocation = "{FA-18C}/Gauges/IFEI/";
        private GaugeNeedle _gnleftnoz;
        private HeliosValue _leftNozzle;
        private HeliosValue _leftNozzleNeedle;
        private CalibrationPointCollectionDouble _needleLeftCalibration;
        private GaugeNeedle _gnrightnoz;
        private HeliosValue _rightNozzle;
        private HeliosValue _rightNozzleNeedle;
        private CalibrationPointCollectionDouble _needleRightCalibration;
        private GaugeImage _giNeedleBackground;
        private HeliosValue _nozzleNeedleBackground;
        private GaugeImage _gireflection;
        private GaugeImage _giZulu;
        private HeliosValue _indicatorZulu;
        private GaugeImage _giBingo;
        private HeliosValue _indicatorBingo;
        private GaugeImage _giFF;
        private HeliosValue _indicatorFuelFlow;
        private GaugeImage _giTemp;
        private HeliosValue _indicatorTemp;
        private GaugeImage _giRPM;
        private HeliosValue _indicatorRPM;
        private GaugeImage _giOil;
        private HeliosValue _indicatorOil;
        private GaugeImage _giNoz;
        private HeliosValue _indicator_Noz;
        private GaugeImage _giGaugeMarksL;
        private HeliosValue _indicatorMarksLeft;
        private GaugeImage _giGaugeMarksR;
        private HeliosValue _indicatorMarksRight;
        private GaugeImage _giGaugeMarksL000;
        private HeliosValue _indicatorMarksLeft000;
        private GaugeImage _giGaugeMarksR000;
        private HeliosValue _indicatorMarksRight000;
        private GaugeImage _giGaugeMarksL050;
        private HeliosValue _indicatorMarksLeft050;
        private GaugeImage _giGaugeMarksR050;
        private HeliosValue _indicatorMarksRight050;
        private GaugeImage _giGaugeMarksL100;
        private HeliosValue _indicatorMarksLeft100;
        private GaugeImage _giGaugeMarksR100;
        private HeliosValue _indicatorMarksRight100;
        private GaugeImage _giClockDots1;
        private HeliosValue _indicatorClockDots1;
        private GaugeImage _giClockDots2;
        private HeliosValue _indicatorClockDots2;
        private GaugeImage _giTimerDots1;
        private HeliosValue _indicatorTimerDots1;
        private GaugeImage _giTimerDots2;
        private HeliosValue _indicatorTimerDots2;
        private HeliosValue _indicatorLFuel;
        private GaugeImage _giLeftFuel;
        private HeliosValue _indicatorRFuel;
        private GaugeImage _giRightFuel;

        public IFEI_Gauges()
            : base("IFEI_Gauges", new Size(779, 702), "Alt", false)
        {
            // These points are an approximation because DCS does not expose the nozzle position so we infer it from the Fuel Flow
            _needleLeftCalibration = new CalibrationPointCollectionDouble(0d, 0d, 400d, 90d) {
                new CalibrationPointDouble(6d, 72d),
                new CalibrationPointDouble(13d, 54d),
                new CalibrationPointDouble(14d, 36d),
                new CalibrationPointDouble(15d, 27d),
                new CalibrationPointDouble(18d, 18d),
                new CalibrationPointDouble(19d, 9d),
                new CalibrationPointDouble(24d, 0d),
                new CalibrationPointDouble(26d, 9d),
                new CalibrationPointDouble(90d, 9d),
                new CalibrationPointDouble(100d, 18d),
                new CalibrationPointDouble(130d, 27d),
                new CalibrationPointDouble(140d, 36d),
                new CalibrationPointDouble(150d, 45d),
                new CalibrationPointDouble(170d, 63d),
                new CalibrationPointDouble(200d, 72d),
                new CalibrationPointDouble(210d, 81d),
                new CalibrationPointDouble(230d, 90d)
            };
            _needleRightCalibration = new CalibrationPointCollectionDouble(0d, 0d, 400d, -90d) {
                new CalibrationPointDouble(6d, -72d),
                new CalibrationPointDouble(13d, -54d),
                new CalibrationPointDouble(14d, -36d),
                new CalibrationPointDouble(15d, -27d),
                new CalibrationPointDouble(18d, -18d),
                new CalibrationPointDouble(19d, -9d),
                new CalibrationPointDouble(24d, -0d),
                new CalibrationPointDouble(26d, -9d),
                new CalibrationPointDouble(90d, -9d),
                new CalibrationPointDouble(100d, -18d),
                new CalibrationPointDouble(130d, -27d),
                new CalibrationPointDouble(140d, -36d),
                new CalibrationPointDouble(150d, -45d),
                new CalibrationPointDouble(170d, -63d),
                new CalibrationPointDouble(200d, -72d),
                new CalibrationPointDouble(210d, -81d),
                new CalibrationPointDouble(230d, -90d)
            };

            _giGaugeMarksL = new GaugeImage(_imageLocation + "IFEI Left Nozzle Gauge Marks.xaml", new Rect(80d, 270d, 277d, 137d));
            Components.Add(_giGaugeMarksL);
            _giGaugeMarksR = new GaugeImage(_imageLocation + "IFEI Right Nozzle Gauge Marks.xaml", new Rect(80d, 270d, 277d, 137d));
            Components.Add(_giGaugeMarksR);
            _giGaugeMarksL000 = new GaugeImage(_imageLocation + "IFEI Left 0 Nozzle Gauge Marks.xaml", new Rect(80d, 270d, 277d, 137d));
            Components.Add(_giGaugeMarksL000);
            _giGaugeMarksR000 = new GaugeImage(_imageLocation + "IFEI Right 0 Nozzle Gauge Marks.xaml", new Rect(80d, 270d, 277d, 137d));
            Components.Add(_giGaugeMarksR000);
            _giGaugeMarksL050 = new GaugeImage(_imageLocation + "IFEI Left 50 Nozzle Gauge Marks.xaml", new Rect(80d, 270d, 277d, 137d));
            Components.Add(_giGaugeMarksL050);
            _giGaugeMarksR050 = new GaugeImage(_imageLocation + "IFEI Right 50 Nozzle Gauge Marks.xaml", new Rect(80d, 270d, 277d, 137d));
            Components.Add(_giGaugeMarksR050);
            _giGaugeMarksL100 = new GaugeImage(_imageLocation + "IFEI Left 100 Nozzle Gauge Marks.xaml", new Rect(80d, 270d, 277d, 137d));
            Components.Add(_giGaugeMarksL100);
            _giGaugeMarksR100 = new GaugeImage(_imageLocation + "IFEI Right 100 Nozzle Gauge Marks.xaml", new Rect(80d, 270d, 277d, 137d));
            Components.Add(_giGaugeMarksR100);
            _giNeedleBackground = new GaugeImage(_imageLocation + "IFEI Needle Background.xaml", new Rect(80d, 270d, 277d, 93.987d));
            Components.Add(_giNeedleBackground);

            _gnleftnoz = new GaugeNeedle(_imageLocation + "IFEI Left Needle.xaml", new Point(83d, 273d), new Size(92d, 6d), new Point(3d, 3d));
            Components.Add(_gnleftnoz);
            _gnleftnoz.IsHidden = true;
            _leftNozzle = new HeliosValue(this, BindingValue.Empty, "", "Left Nozzle Position", "Left Nozzle Position in %.", "", BindingValueUnits.Numeric);
            _leftNozzle.Execute += new HeliosActionHandler(LeftNozzlePosition_Execute);
            Actions.Add(_leftNozzle);

            _gnrightnoz = new GaugeNeedle(_imageLocation + "IFEI Right Needle.xaml", new Point(354d, 273d), new Size(92d, 6d), new Point(89d, 3d));
            Components.Add(_gnrightnoz);
            _gnrightnoz.IsHidden = true;
            _rightNozzle = new HeliosValue(this, BindingValue.Empty, "", "Right Nozzle Position", "Right Nozzle Position in %.", "", BindingValueUnits.Numeric);
            _rightNozzle.Execute += new HeliosActionHandler(RightNozzlePosition_Execute);
            Actions.Add(_rightNozzle);

            _giNoz = new GaugeImage(_imageLocation + "IFEI Legends Noz.xaml", new Rect(202d, 336d, 28d, 12d));
            Components.Add(_giNoz);
            _giRPM = new GaugeImage(_imageLocation + "IFEI Legends RPM.xaml", new Rect(202d, 102d, 28d, 12d));
            Components.Add(_giRPM);
            _giFF = new GaugeImage(_imageLocation + "IFEI Legends FF.xaml", new Rect(202d, 214d, 30d, 24d));
            Components.Add(_giFF);
            _giTemp = new GaugeImage(_imageLocation + "IFEI Legends Temp.xaml", new Rect(198d, 157d, 36d, 12d));
            Components.Add(_giTemp);
            _giBingo = new GaugeImage(_imageLocation + "IFEI Legends Bingo.xaml", new Rect(596d, 236d, 40d, 12d));
            Components.Add(_giBingo);
            _giLeftFuel = new GaugeImage(_imageLocation + "IFEI Legends L.xaml", new Rect(689d, 114d, 8d, 12d));
            Components.Add(_giLeftFuel);
            _giRightFuel = new GaugeImage(_imageLocation + "IFEI Legends R.xaml", new Rect(689d, 180d, 8d, 12d));
            Components.Add(_giRightFuel);
            _giZulu = new GaugeImage(_imageLocation + "IFEI Legends Z.xaml", new Rect(688d, 376d, 8d, 12d));
            Components.Add(_giZulu);
            _giOil = new GaugeImage(_imageLocation + "IFEI Legends Oil.xaml", new Rect(206d, 450d, 22d, 12d));
            Components.Add(_giOil);
            _giClockDots1 = new GaugeImage(_imageLocation + "IFEI Clock Separator.xaml", new Rect(577d, 366d, 7.055d, 19.273d));
            Components.Add(_giClockDots1);
            _giClockDots2 = new GaugeImage(_imageLocation + "IFEI Clock Separator.xaml", new Rect(630d, 366d, 7.055d, 19.273d));
            Components.Add(_giClockDots2);
            _giTimerDots1 = new GaugeImage(_imageLocation + "IFEI Clock Separator.xaml", new Rect(577d, 423d, 7.055d, 19.273d));
            Components.Add(_giTimerDots1);
            _giTimerDots2 = new GaugeImage(_imageLocation + "IFEI Clock Separator.xaml", new Rect(630d, 423d, 7.055d, 19.273d));
            Components.Add(_giTimerDots2);
            _gireflection = new GaugeImage(_imageLocation + "IFEI Reflections.png", new Rect(0d, 0d, 779d, 702d));
            Components.Add(_gireflection);

            _gireflection.IsHidden = false;
            _giOil.IsHidden = true;
            _giZulu.IsHidden = true;
            _giLeftFuel.IsHidden = true;
            _giRightFuel.IsHidden = true;
            _giBingo.IsHidden = true;
            _giTemp.IsHidden = true;
            _giFF.IsHidden = true;
            _giRPM.IsHidden = true;
            _giNoz.IsHidden = true;
            _giGaugeMarksL.IsHidden = true;
            _giGaugeMarksR.IsHidden = true;
            _giGaugeMarksL000.IsHidden = true;
            _giGaugeMarksR000.IsHidden = true;
            _giGaugeMarksL050.IsHidden = true;
            _giGaugeMarksR050.IsHidden = true;
            _giGaugeMarksL100.IsHidden = true;
            _giGaugeMarksR100.IsHidden = true;
            _giClockDots1.IsHidden = true;
            _giClockDots2.IsHidden = true;
            _giTimerDots1.IsHidden = true;
            _giTimerDots2.IsHidden = true;
            _giNeedleBackground.IsHidden = true;

            _indicatorZulu = new HeliosValue(this, new BindingValue(0d), "", "Zulu Time Flag", "Z flag indicating Zulu time on IFEI", "", BindingValueUnits.Boolean);
            _indicatorZulu.Execute += new HeliosActionHandler(Indicator_Execute);
            Actions.Add(_indicatorZulu);
            _indicatorLFuel = new HeliosValue(this, new BindingValue(0d), "", "Left Fuel Flag", "L flag indicating Left fuel quantity on IFEI", "", BindingValueUnits.Boolean);
            _indicatorLFuel.Execute += new HeliosActionHandler(Indicator_Execute);
            Actions.Add(_indicatorLFuel);
            _indicatorRFuel = new HeliosValue(this, new BindingValue(0d), "", "Right Fuel Flag", "R flag indicating Right fuel quantity on IFEI", "", BindingValueUnits.Boolean);
            _indicatorRFuel.Execute += new HeliosActionHandler(Indicator_Execute);
            Actions.Add(_indicatorRFuel);
            _indicatorBingo = new HeliosValue(this, new BindingValue(0d), "", "Bingo Flag", "Show Bingo on IFEI", "", BindingValueUnits.Boolean);
            _indicatorBingo.Execute += new HeliosActionHandler(Indicator_Execute);
            Actions.Add(_indicatorBingo);
            _indicatorFuelFlow = new HeliosValue(this, new BindingValue(0d), "", "FF Flag", "Show FF on IFEI", "", BindingValueUnits.Boolean);
            _indicatorFuelFlow.Execute += new HeliosActionHandler(Indicator_Execute);
            Actions.Add(_indicatorFuelFlow);
            _indicatorTemp = new HeliosValue(this, new BindingValue(0d), "", "Temp Flag", "Show Temp on IFEI", "", BindingValueUnits.Boolean);
            _indicatorTemp.Execute += new HeliosActionHandler(Indicator_Execute);
            Actions.Add(_indicatorTemp);
            _indicatorRPM = new HeliosValue(this, new BindingValue(0d), "", "RPM Flag", "Show RPM on IFEI", "", BindingValueUnits.Boolean);
            _indicatorRPM.Execute += new HeliosActionHandler(Indicator_Execute);
            Actions.Add(_indicatorRPM);
            _indicatorOil = new HeliosValue(this, new BindingValue(0d), "", "Oil Flag", "Show Oil on IFEI", "", BindingValueUnits.Boolean);
            _indicatorOil.Execute += new HeliosActionHandler(Indicator_Execute);
            Actions.Add(_indicatorOil);
            _indicator_Noz = new HeliosValue(this, new BindingValue(0d), "", "Noz Flag", "Show Noz on IFEI", "", BindingValueUnits.Boolean);
            _indicator_Noz.Execute += new HeliosActionHandler(Indicator_Execute);
            Actions.Add(_indicator_Noz);
            _indicatorMarksLeft = new HeliosValue(this, new BindingValue(0d), "", "Left Scale Flag", "Show Left Scale on IFEI", "", BindingValueUnits.Boolean);
            _indicatorMarksLeft.Execute += new HeliosActionHandler(Indicator_Execute);
            Actions.Add(_indicatorMarksLeft);
            _indicatorMarksRight = new HeliosValue(this, new BindingValue(0d), "", "Right Scale Flag", "Show Right Scale on IFEI", "", BindingValueUnits.Boolean);
            _indicatorMarksRight.Execute += new HeliosActionHandler(Indicator_Execute);
            Actions.Add(_indicatorMarksRight);
            _indicatorMarksLeft000 = new HeliosValue(this, new BindingValue(0d), "", "Left Scale 0 Flag", "Show Left Scale 0 value on IFEI", "", BindingValueUnits.Boolean);
            _indicatorMarksLeft000.Execute += new HeliosActionHandler(Indicator_Execute);
            Actions.Add(_indicatorMarksLeft000);
            _indicatorMarksRight000 = new HeliosValue(this, new BindingValue(0d), "", "Right Scale 0 Flag", "Show Right Scale 0 value on IFEI", "", BindingValueUnits.Boolean);
            _indicatorMarksRight000.Execute += new HeliosActionHandler(Indicator_Execute);
            Actions.Add(_indicatorMarksRight000);
            _indicatorMarksLeft050 = new HeliosValue(this, new BindingValue(0d), "", "Left Scale 50 Flag", "Show Left Scale 50 value on IFEI", "", BindingValueUnits.Boolean);
            _indicatorMarksLeft050.Execute += new HeliosActionHandler(Indicator_Execute);
            Actions.Add(_indicatorMarksLeft050);
            _indicatorMarksRight050 = new HeliosValue(this, new BindingValue(0d), "", "Right Scale 50 Flag", "Show Right Scale 50 value on IFEI", "", BindingValueUnits.Boolean);
            _indicatorMarksRight050.Execute += new HeliosActionHandler(Indicator_Execute);
            Actions.Add(_indicatorMarksRight050);
            _indicatorMarksLeft100 = new HeliosValue(this, new BindingValue(0d), "", "Left Scale 100 Flag", "Show Left Scale 10 value on IFEI", "", BindingValueUnits.Boolean);
            _indicatorMarksLeft100.Execute += new HeliosActionHandler(Indicator_Execute);
            Actions.Add(_indicatorMarksLeft100);
            _indicatorMarksRight100 = new HeliosValue(this, new BindingValue(0d), "", "Right Scale 100 Flag", "Show Right Scale 10 value on IFEI", "", BindingValueUnits.Boolean);
            _indicatorMarksRight100.Execute += new HeliosActionHandler(Indicator_Execute);
            Actions.Add(_indicatorMarksRight100);
            _indicatorClockDots1 = new HeliosValue(this, new BindingValue(0d), "", "Clock HH MM separator", "Separator character between hours & mins on the IFEI clock", "", BindingValueUnits.Boolean);
            _indicatorClockDots1.Execute += new HeliosActionHandler(Indicator_Execute);
            Actions.Add(_indicatorClockDots1);
            _indicatorClockDots2 = new HeliosValue(this, new BindingValue(0d), "", "Clock MM SS separator", "Separator character between mins & secs on the IFEI clock", "", BindingValueUnits.Boolean);
            _indicatorClockDots2.Execute += new HeliosActionHandler(Indicator_Execute);
            Actions.Add(_indicatorClockDots2);
            _indicatorTimerDots1 = new HeliosValue(this, new BindingValue(0d), "", "Timer H MM separator", "Separator character between hours & mins on the IFEI timer", "", BindingValueUnits.Boolean);
            _indicatorTimerDots1.Execute += new HeliosActionHandler(Indicator_Execute);
            Actions.Add(_indicatorTimerDots1);
            _indicatorTimerDots2 = new HeliosValue(this, new BindingValue(0d), "", "Timer MM SS separator", "Separator character between mins & secs on the IFEI timer", "", BindingValueUnits.Boolean);
            _indicatorTimerDots2.Execute += new HeliosActionHandler(Indicator_Execute);
            Actions.Add(_indicatorTimerDots2);
            _leftNozzleNeedle = new HeliosValue(this, new BindingValue(0d), "", "Left Nozzle Needle Flag", "Left nozzle needle appearance on IFEI", "", BindingValueUnits.Boolean);
            _leftNozzleNeedle.Execute += new HeliosActionHandler(Indicator_Execute);
            Actions.Add(_leftNozzleNeedle);
            _rightNozzleNeedle = new HeliosValue(this, new BindingValue(0d), "", "Right Nozzle Needle Flag", "Right nozzle needle appearance on IFEI", "", BindingValueUnits.Boolean);
            _rightNozzleNeedle.Execute += new HeliosActionHandler(Indicator_Execute);
            Actions.Add(_rightNozzleNeedle);
            _nozzleNeedleBackground = new HeliosValue(this, new BindingValue(0d), "", "Nozzle Needle Background Flag", "nozzle needle background appearance on IFEI", "", BindingValueUnits.Boolean);
            _nozzleNeedleBackground.Execute += new HeliosActionHandler(Indicator_Execute);
            Actions.Add(_nozzleNeedleBackground);
            // initialize opacity value and related visual
            GlassReflectionOpacity = GLASS_REFLECTION_OPACITY_DEFAULT;


        }

        #region Properties
        public override bool EnableAlternateImageSet
        {
            get => base.EnableAlternateImageSet;
            set
            {
                bool newValue = value;
                bool oldValue = base.EnableAlternateImageSet; 
                
                if (newValue != oldValue)
                {
                    base.EnableAlternateImageSet = newValue;
                    _gireflection.IsHidden = newValue;
                }
            }
        }
        public double GlassReflectionOpacity
        {
            get
            {
                return _glassReflectionOpacity;
            }
            set
            {
                // clamp to max opacity
                double newValue = Math.Min(value, 1.0);

                double oldValue = _glassReflectionOpacity;
                if (newValue != oldValue)
                {
                    _glassReflectionOpacity = newValue;

                    // don't render at all if fully transparent
                    _gireflection.IsHidden = (newValue == 0.0);

                    // render at this opacity, if applicable
                    _gireflection.Opacity = newValue;

                    // notify change after change is made
                    OnPropertyChanged("GlassReflectionOpacity", oldValue, newValue, true);
                }
            }
        }
        #endregion

        protected override void OnProfileChanged(HeliosProfile oldProfile) {
            base.OnProfileChanged(oldProfile);
        }

        void LeftNozzlePosition_Execute(object action, HeliosActionEventArgs e)
        {          
            _gnleftnoz.Rotation = _needleLeftCalibration.Interpolate(e.Value.DoubleValue);
        }

        void RightNozzlePosition_Execute(object action, HeliosActionEventArgs e)
        {
            _gnrightnoz.Rotation = _needleRightCalibration.Interpolate(e.Value.DoubleValue);
        }

        void Indicator_Execute(object action,HeliosActionEventArgs e)
        {
            HeliosValue _haction = (HeliosValue) action;
            String _hactionVal = e.Value.StringValue;
            switch (_haction.Name)
            {
                case "Zulu Time Flag":
                    _giZulu.IsHidden = (_hactionVal == "1") ? false : true;
                    break;
                case "Bingo Flag":
                    _giBingo.IsHidden = (_hactionVal == "1") ? false : true;
                    break;
                case "FF Flag":
                    _giFF.IsHidden = (_hactionVal == "1") ? false : true;
                    break;
                case "Temp Flag":
                    _giTemp.IsHidden = (_hactionVal == "1") ? false : true;
                    break;
                case "RPM Flag":
                    _giRPM.IsHidden = (_hactionVal == "1") ? false : true;
                    break;
                case "Oil Flag":
                    _giOil.IsHidden = (_hactionVal == "1") ? false : true;
                    break;
                case "Noz Flag":
                    _giNoz.IsHidden = (_hactionVal == "1") ? false : true;
                    break;
                case "Left Scale Flag":
                    _giGaugeMarksL.IsHidden = (_hactionVal == "1") ? false : true;
                    _giNeedleBackground.IsHidden = (_hactionVal == "1") ? false : true;
                    break;
                case "Right Scale Flag":
                    _giGaugeMarksR.IsHidden = (_hactionVal == "1") ? false : true;
                    break;
                case "Left Scale 0 Flag":
                    _giGaugeMarksL000.IsHidden = (_hactionVal == "1") ? false : true;
                    break;
                case "Right Scale 0 Flag":
                    _giGaugeMarksR000.IsHidden = (_hactionVal == "1") ? false : true;
                    break;
                case "Left Scale 50 Flag":
                    _giGaugeMarksL050.IsHidden = (_hactionVal == "1") ? false : true;
                    break;
                case "Right Scale 50 Flag":
                    _giGaugeMarksR050.IsHidden = (_hactionVal == "1") ? false : true;
                    break;
                case "Left Scale 100 Flag":
                    _giGaugeMarksL100.IsHidden = (_hactionVal == "1") ? false : true;
                    break;
                case "Right Scale 100 Flag":
                    _giGaugeMarksR100.IsHidden = (_hactionVal == "1") ? false : true;
                    break;
                case "Clock HH MM separator":
                    _giClockDots1.IsHidden = (_hactionVal == "1") ? false : true;
                    break;
                case "Clock MM SS separator":
                    _giClockDots2.IsHidden = (_hactionVal == "1") ? false : true;
                    break;
                case "Timer H MM separator":
                    _giTimerDots1.IsHidden = (_hactionVal == "1") ? false : true;
                    break;
                case "Timer MM SS separator":
                    _giTimerDots2.IsHidden = (_hactionVal == "1") ? false : true;
                    break;
                case "Nozzle Needle Background Flag":
                    _giNeedleBackground.IsHidden = (_hactionVal == "1") ? false : true;
                    break;
                case "Left Nozzle Needle Flag":
                    _gnleftnoz.IsHidden = (_hactionVal == "1") ? false : true;
                    break;
                case "Right Nozzle Needle Flag":
                    _gnrightnoz.IsHidden = (_hactionVal == "1") ? false : true;
                    break;
                case "Left Fuel Flag":
                    _giLeftFuel.IsHidden = (_hactionVal == "1") ? false : true;
                    break;
                case "Right Fuel Flag":
                    _giRightFuel.IsHidden = (_hactionVal == "1") ? false : true;
                    break;                   
                default:
                    break;
            }
        }
        public override bool HitTest(Point location)
        {
            if (_scaledScreenRect.Contains(location))
            {
                return false;
            }

            return true;
        }

        public override void MouseDown(Point location)
        {
            // No-Op
        }

        public override void MouseDrag(Point location)
        {
            // No-Op
        }

        public override void MouseUp(Point location)
        {
            // No-Op
        }

    }
}
