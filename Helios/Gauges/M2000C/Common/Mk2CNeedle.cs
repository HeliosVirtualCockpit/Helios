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

namespace GadrocsWorkshop.Helios.Gauges.M2000C.Mk2CNeedle
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System.Windows;
    using System.Windows.Media;

    [HeliosControl("Helios.M2000C.Mk2CNeedle", "Mk2C Needle", "M2000C Gauges", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class Mk2CNeedle : BaseGauge
    {
        private HeliosValue _value;
        private GaugeNeedle _needle;
        private CalibrationPointCollectionDouble _needleCalibration;

        public Mk2CNeedle()
            : this("Mk2C Needle", "{ Helios}/Gauges/M2000C/Common/needleB.xaml", "", "", 
                  new Point(0,0), new Size(10d, 15d), new Point(12d, 19d), BindingValueUnits.Numeric, new double[] { 0d, 0d, 1d, 360d }, null)
        {
        }

        public Mk2CNeedle(string name, string needleWay, string actionIdentifier, string valueDescription, 
            Point posn, Size size, Point centerPoint, BindingValueUnit typeValue, double[] initialCalibration, double[,] calibrationPoints)
            : base(name, size)
        {
            _needleCalibration = new CalibrationPointCollectionDouble(initialCalibration[0], initialCalibration[1], initialCalibration[2], initialCalibration[3]);
            if (calibrationPoints != null)
            {
                for (int c = 0; c < calibrationPoints.Length/2; c++)
                {
                    _needleCalibration.Add(new CalibrationPointDouble(calibrationPoints[c, 0], calibrationPoints[c, 1]));
                }
            }
            _needle = new GaugeNeedle(needleWay, posn, size, centerPoint);
            Components.Add(_needle);

            _value = new HeliosValue(this, new BindingValue(0d), "", actionIdentifier, name + " - " + actionIdentifier, valueDescription, typeValue);
            _value.Execute += new HeliosActionHandler(NeedleValue_Execute);
            Actions.Add(_value);
        }

        void NeedleValue_Execute(object action, HeliosActionEventArgs e)
        {
            _needle.Rotation = _needleCalibration.Interpolate(e.Value.DoubleValue);
        }
    }
}
