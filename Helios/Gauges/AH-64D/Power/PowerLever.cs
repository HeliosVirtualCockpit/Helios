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

namespace GadrocsWorkshop.Helios.Gauges.AH64D.Altimeter
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.AH64D.Power", "Power Levers", "AH-64D", typeof(GaugeRenderer))]
    public class Lever : BaseGauge
    {
        private int _lastHiddenLeftImageIndex;
        private HeliosValue _leverageLeft;
        private int _lastHiddenRightImageIndex;
        private int _rightComponentOffset;
        private HeliosValue _leverageRight;
        //private CalibrationPointCollectionDouble _needleCalibration;


        public Lever()
            : base("Power Levers", new Size(288d, 540d))
        {
            for(_rightComponentOffset=0; _rightComponentOffset < 11; _rightComponentOffset++)
            {
                Components.Add(new GaugeImage($"{{Helios}}/Images/AH-64D/Power/Left_Power_Lever_{_rightComponentOffset}.png", new Rect(0d, 0d, 140d, 540d)));
                Components[_rightComponentOffset].IsHidden = true;
            }
            Components[0].IsHidden = false;
            _lastHiddenLeftImageIndex = 0;
            _leverageLeft = new HeliosValue(this, new BindingValue(0d), "", "Left Power Lever", "Current position", "", BindingValueUnits.Numeric);
            _leverageLeft.SetValue(new BindingValue(0), true);
            _leverageLeft.Execute += new HeliosActionHandler(MoveLeftLever_Execute);
            Actions.Add(_leverageLeft);

            for (int i = 0; i < 11; i++)
            {
                Components.Add(new GaugeImage($"{{Helios}}/Images/AH-64D/Power/Right_Power_Lever_{i}.png", new Rect(148d, 0d, 140d, 540d)));
                Components[i + _rightComponentOffset].IsHidden = true;
            }
            Components[_rightComponentOffset].IsHidden = false;
            _lastHiddenRightImageIndex = _rightComponentOffset;
            _leverageRight = new HeliosValue(this, new BindingValue(0d), "", "Right Power Lever", "Current position", "", BindingValueUnits.Numeric);
            _leverageRight.SetValue(new BindingValue(0), true);
            _leverageRight.Execute += new HeliosActionHandler(MoveRightLever_Execute);
            Actions.Add(_leverageRight);
        }

        void MoveLeftLever_Execute(object action, HeliosActionEventArgs e)
        {
            _leverageLeft.SetValue(new BindingValue(Clamp(Math.Round(e.Value.DoubleValue * 10d), 0, 10)), true);
            Components[_lastHiddenLeftImageIndex].IsHidden = true;
            _lastHiddenLeftImageIndex = Convert.ToInt32(_leverageLeft.Value.DoubleValue);
            Components[_lastHiddenLeftImageIndex].IsHidden = false;
        }
        void MoveRightLever_Execute(object action, HeliosActionEventArgs e)
        {
            _leverageRight.SetValue(new BindingValue(Clamp(Math.Round(e.Value.DoubleValue * 10d), 0, 10)), true);
            Components[_lastHiddenRightImageIndex].IsHidden = true;
            _lastHiddenRightImageIndex = _rightComponentOffset + Convert.ToInt32(_leverageRight.Value.DoubleValue);
            Components[_lastHiddenRightImageIndex].IsHidden = false;
        }
        private double Clamp(double value, double min, double max)
        {
            if (value < min)
            {
                return min;
            }
            if (value > max)
            {
                return max;
            }
            return value;
        }
    }
}
