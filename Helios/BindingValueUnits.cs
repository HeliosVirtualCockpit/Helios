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

namespace GadrocsWorkshop.Helios
{
    using GadrocsWorkshop.Helios.Units;
    using System.Collections.Generic;

    /// <summary>
    /// Helper class with all default unit constructors.
    /// </summary>
    public static class BindingValueUnits
    {        
        // Generic Units
        public static readonly NoValueUnit NoValue = new NoValueUnit();
        public static readonly NumericUnit Numeric = new NumericUnit();
        public static readonly TextUnit Text = new TextUnit();
        public static readonly BooleanUnit Boolean = new BooleanUnit();

        // Angle Units
        public static readonly RadiansUnit Radians = new RadiansUnit();
        public static readonly DegreesUnit Degrees = new DegreesUnit();

        // Temperature Units
        public static readonly CelsiusUnit Celsius = new CelsiusUnit();

        // Distance Units
        public static readonly MetersUnit Meters = new MetersUnit();
        public static readonly KilometersUnit Kilometers = new KilometersUnit();
        public static readonly FeetUnit Feet = new FeetUnit();
        public static readonly MilesUnit Miles = new MilesUnit();
        public static readonly NauticalMilesUnit NauticalMiles = new NauticalMilesUnit();

        // Revolutions Units
        public static readonly RPMPercentUnit RPMPercent = new RPMPercentUnit();
        public static RPMUnit RPM(double maxValue)
        {
            return new RPMUnit(maxValue);
        }

        // Time Units
        public static readonly SecondsUnit Seconds = new SecondsUnit();
        public static readonly MinuteUnit Minutes = new MinuteUnit();
        public static readonly HoursUnit Hours = new HoursUnit();

        // Mass Units
        public static readonly PoundsUnit Pounds = new PoundsUnit();
        public static readonly KilogramsUnit Kilograms = new KilogramsUnit();

        // Area Units
        public static readonly SquareInchUnit SquareInch = new SquareInchUnit();
        public static readonly SquareFootUnit SquareFoot = new SquareFootUnit();
        public static readonly SquareCentimeterUnit SquareCentimeter = new SquareCentimeterUnit();

        // Speed Units
        public static readonly BindingValueUnit MetersPerSecond = new SpeedUnit(Meters, Seconds, "m/s", "Meters per second");
        public static readonly BindingValueUnit FeetPerSecond = new SpeedUnit(Feet, Seconds, "fps", "Feet per second");
        public static readonly BindingValueUnit FeetPerMinute = new SpeedUnit(Feet, Minutes, "fpm", "Feet per minute");
        public static readonly BindingValueUnit MilesPerHour = new SpeedUnit(Miles, Hours, "mph", "Miles per hour");
        public static readonly BindingValueUnit Knots = new SpeedUnit(NauticalMiles, Hours, "kts", "Knots");
        public static readonly BindingValueUnit KilometersPerHour = new SpeedUnit(Kilometers, Hours, "km/h", "Kilometers per hour");

        // Mass Flow Units
        public static readonly BindingValueUnit PoundsPerHour = new MassFlowUnit(Pounds, Hours, "PPH", "Pound per hour");

        // Pressure Units
        public static readonly BindingValueUnit PoundsPerSquareInch = new PressureUnit(Pounds, SquareInch, "PSI", "Pounds per square inch");
        public static readonly BindingValueUnit PoundsPerSquareFoot = new PressureUnit(Pounds, SquareFoot, "lb/ft2", "Pounds per square foot");
        public static readonly BindingValueUnit InchesOfMercury = new InchofMercuryUnit();
        public static readonly BindingValueUnit MilimetersOfMercury = new MilimetersOfMercury();
        public static readonly BindingValueUnit Millibar = new MillibarUnit();
        public static readonly BindingValueUnit Bar = new BarUnit();
        public static readonly BindingValueUnit KilgramsForcePerSquareCentimenter = new PressureUnit(Kilograms, SquareCentimeter, "kgf/cm2", "Kilograms force per square centimeter");

        // Volume Units
        public static readonly BindingValueUnit Liters = new Liters();

        //Electrical Units
        public static readonly BindingValueUnit Volts = new NumericUnit();

        // cache all the fields by name
        private static Dictionary<string, BindingValueUnit> _unitByName;
        private static Dictionary<string, string> _nameByLongName;

        static BindingValueUnits()
        {
            _unitByName = new Dictionary<string, BindingValueUnit>();
            _nameByLongName = new Dictionary<string, string>();
            System.Type unitsClass = typeof(BindingValueUnits);
            foreach (System.Reflection.FieldInfo field in unitsClass.GetFields())
            {
                if (field.Name == "Volts")
                {
                    // not its own unit
                    continue;
                }
                if (field.GetValue(null) is BindingValueUnit unit)
                {
                    _unitByName.Add(field.Name, unit);
                    _nameByLongName.Add(unit.LongName, field.Name);
                }
            }
        }

        // fetch item by name via reflection, used for deserialization
        public static BindingValueUnit FetchUnitByName(string name)
        {
            if (_unitByName.TryGetValue(name, out BindingValueUnit unit))
            {
                return unit;
            }
            ConfigManager.LogManager.LogError($"implementation error: '{name}' is not a valid binding value unit; please update BindingValueUnits class");
            // survive, because this is from external input
            return null;
        }

        // fetch name via reflection, used for serialization
        public static string FetchUnitName(BindingValueUnit unit)
        {
            if (_nameByLongName.TryGetValue(unit.LongName, out string name))
            {
                return name;
            }
            // this does not require input and will happen while still developing
            throw new System.Exception($"implementation error: binding unit with long name '{unit.LongName}' not found; please update BindingValueUnits class");
        }

        // all valid unit names
        public static IEnumerable<string> UnitNames => _unitByName.Keys;
    }
}
