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

namespace GadrocsWorkshop.Helios.Gauges.F16C.Instruments
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;

    [HeliosControl("Helios.F16C.FuelGaugeBlk50", "Fuel Gauge (Blk50)", "F-16", typeof(GaugeRenderer),HeliosControlFlags.NotShownInUI)]
    public class FuelGaugeBlk50 : BaseGauge
    {
        private HeliosValue _fuelQuantityFRNumeric;
        private HeliosValue _fuelQuantityFRPounds;
        private GaugeNeedle _needleFR;
        private HeliosValue _fuelQuantityALNumeric;
        private HeliosValue _fuelQuantityALPounds;
        private GaugeNeedle _needleAL;
        private HeliosValue _fuelQuantityTotalNumeric;
        private HeliosValue _fuelQuantityTotalPounds;
        private CalibrationPointCollectionDouble _needleCalibration;
        private GaugeDrumCounter _drum;
        private GaugeDrumCounter _drumHundreds;
        private double _valueMultiplier = 1d;
        public FuelGaugeBlk50()
            : base("Fuel Gauge Block 50", new Size(370,370))
        {
            Components.Add(new GaugeImage("{F-16C}/Gauges/Instruments/Viper-Fuel-Faceplate.xaml", new Rect(0d, 0d, 370d, 370d)));
            _drum = new GaugeDrumCounter("{F-16C}/Gauges/Instruments/drum_tape.xaml", new Point(124d, 258d), "##", new Size(10d, 15d), new Size(27.2d, 35d));
            _drum.Value = 0;
            _drum.Clip = new RectangleGeometry(new Rect(124d, 258d, 54.4d, 35d));
            Components.Add(_drum);
            _drumHundreds = new GaugeDrumCounter("{F-16C}/Gauges/Instruments/drum_tape_hundreds.xaml", new Point(178.4d, 258d), "%", new Size(30d, 15d), new Size(81.6d, 35d));
            _drumHundreds.Value = 0;
            _drumHundreds.Clip = new RectangleGeometry(new Rect(178.4d, 258d, 81.6, 35d));
            Components.Add(_drumHundreds);

            _needleCalibration = new CalibrationPointCollectionDouble(0d, 0d, 5000d, 252d)
                {
                new CalibrationPointDouble(4200d, 252d)
                };

            _needleAL = new GaugeNeedle("{F-16C}/Gauges/Instruments/Viper-Fuel-Needle-AL.xaml", new Point(184.500d, 185.000d), new Size(74.667d, 196.535d), new Point(37.668d, 159.573d), 360d - 127d);
            Components.Add(_needleAL);
            _needleFR = new GaugeNeedle("{F-16C}/Gauges/Instruments/Viper-Fuel-Needle-FR.xaml", new Point(184.500d, 185.000d), new Size(101.494d, 196.535d), new Point(64.495d, 159.573d), 360d - 127d);
            Components.Add(_needleFR);

            _fuelQuantityFRNumeric = new HeliosValue(this, new BindingValue(0d), "", "Fuel Quantity Fore (Numeric)", "Numeric representation of Fuel Quantity.", "Number between 0 &  1, where 1 represents 5000", BindingValueUnits.Numeric);
            _fuelQuantityFRNumeric.Execute += new HeliosActionHandler(FuelQuantityFR_Execute);
            Actions.Add(_fuelQuantityFRNumeric);
            _fuelQuantityFRPounds = new HeliosValue(this, new BindingValue(0d), "", "Fuel Quantity Fore (Pounds)", "Fuel Quantity in pounds.", "0 - 5000", BindingValueUnits.Pounds);
            _fuelQuantityFRPounds.Execute += new HeliosActionHandler(FuelQuantityFR_Execute);
            Actions.Add(_fuelQuantityFRPounds);
            _fuelQuantityALNumeric = new HeliosValue(this, new BindingValue(0d), "", "Fuel Quantity Aft (Numeric)", "Numeric representation of Fuel Quantity.", "Number between 0 &  1, where 1 represents 5000", BindingValueUnits.Numeric);
            _fuelQuantityALNumeric.Execute += new HeliosActionHandler(FuelQuantityAL_Execute);
            Actions.Add(_fuelQuantityALNumeric);
            _fuelQuantityALPounds = new HeliosValue(this, new BindingValue(0d), "", "Fuel Quantity Aft (Pounds)", "Fuel Quantity in pounds.", "0 - 5000", BindingValueUnits.Pounds);
            _fuelQuantityALPounds.Execute += new HeliosActionHandler(FuelQuantityAL_Execute);
            Actions.Add(_fuelQuantityALPounds);
            _fuelQuantityTotalNumeric = new HeliosValue(this, new BindingValue(0d), "", "Fuel Quantity Total (Numeric)", "Numeric representation of Fuel Quantity.", "Number between 0 &  1, where 1 represents 100,000", BindingValueUnits.Numeric);
            _fuelQuantityTotalNumeric.Execute += new HeliosActionHandler(FuelQuantityTotal_Execute);
            Actions.Add(_fuelQuantityTotalNumeric);
            _fuelQuantityTotalPounds = new HeliosValue(this, new BindingValue(0d), "", "Fuel Quantity Total (Pounds)", "Fuel Quantity in pounds.", "0 - 99999", BindingValueUnits.Pounds);
            _fuelQuantityTotalPounds.Execute += new HeliosActionHandler(FuelQuantityTotal_Execute);
            Actions.Add(_fuelQuantityTotalPounds);
        }
        #region Properties
        internal double ValueMultiplier
        {
            get => _valueMultiplier;
            set => _valueMultiplier = value;
        }
        #endregion
        void FuelQuantityFR_Execute(object action, HeliosActionEventArgs e)
        {
            double fuelQuantityLbs = 0d;
            HeliosValue hValue = action as HeliosValue;
            switch (hValue.Unit.LongName)
            {
                case "Numeric":
                    fuelQuantityLbs = e.Value.DoubleValue * ValueMultiplier;
                    break;
                case "Pounds":
                    fuelQuantityLbs = e.Value.DoubleValue;
                    break;
                default:
                    break;
            }
            _needleFR.Rotation = _needleCalibration.Interpolate(fuelQuantityLbs);
        }
        void FuelQuantityAL_Execute(object action, HeliosActionEventArgs e)
        {
            double fuelQuantityLbs = 0d;
            HeliosValue hValue = action as HeliosValue;
            switch (hValue.Unit.LongName)
            {
                case "Numeric":
                    fuelQuantityLbs = e.Value.DoubleValue * ValueMultiplier;
                    break;
                case "Pounds":
                    fuelQuantityLbs = e.Value.DoubleValue;
                    break;
                default:
                    break;
            }
            _needleAL.Rotation = _needleCalibration.Interpolate(fuelQuantityLbs);
        }
        void FuelQuantityTotal_Execute(object action, HeliosActionEventArgs e)
        {
            double fuelQuantityLbs = 0d;
            HeliosValue hValue = action as HeliosValue;
            switch (hValue.Unit.LongName)
            {
                case "Numeric":
                    fuelQuantityLbs = e.Value.DoubleValue * ValueMultiplier * 20;
                    break;
                case "Pounds":
                    fuelQuantityLbs = e.Value.DoubleValue;
                    break;
                default:
                    break;
            }
            _drum.Value = fuelQuantityLbs / 1000;
            _drumHundreds.Value = fuelQuantityLbs % 1000 / 100;
        }
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            if (_valueMultiplier != 1d)
            {
                writer.WriteElementString("ValueMultiplier", _valueMultiplier.ToString(CultureInfo.InvariantCulture));
            }
        }

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            ValueMultiplier = reader.Name.Equals("ValueMultiplier") ? double.Parse(reader.ReadElementString("ValueMultiplier"), CultureInfo.InvariantCulture) : 1d;
        }
    }
}

