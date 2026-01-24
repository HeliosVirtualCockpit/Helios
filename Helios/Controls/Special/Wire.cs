// Copyright 2020 Ammo Goettsch
// 
// Helios is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Helios is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using GadrocsWorkshop.Helios.ComponentModel;

namespace GadrocsWorkshop.Helios.Controls.Special
{
    /// <summary>
    /// a piece of wire, simply transmits any value from its input side to its output side
    /// </summary>
    [HeliosControl("Helios.Base.Wire", "Wire", "Special Controls", typeof(ImageDecorationRenderer), HeliosControlFlags.NoEffects | HeliosControlFlags.NotRotatable | HeliosControlFlags.None)]
    public class Wire : ImageDecorationBase
    {
        public Wire(): base("Wire")
        {
            DesignTimeOnly = true;
            Image = "{Helios}/Images/General/wire.png";
            Alignment = ImageAlignment.Stretched;
            Width = 128;
            Height = 128;

            CreateSignal("Numeric Signal", BindingValueUnits.Numeric);
            CreateSignal("Boolean Signal", BindingValueUnits.Boolean);
            CreateSignal("Text Signal", BindingValueUnits.Text);
        }

        private void CreateSignal(string name, BindingValueUnit unit)
        {
            HeliosValue numericSignal = new HeliosValue(this, BindingValue.Empty, "", name, "Current signal on this wire.",
                "Value copied from input to output.", unit);
            numericSignal.Execute += (action, args) => numericSignal.SetValue(args.Value, false);
            Actions.Add(numericSignal);
            Triggers.Add(numericSignal);
            Values.Add(numericSignal);
        }
        public override bool EffectsExclusion
        {
            get => true;
            set { }
        }
    }
}
