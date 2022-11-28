// Copyright 2014 Craig Courtney
// Copyright 2020 Helios Contributors
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

using System.Windows;

namespace GadrocsWorkshop.Helios.Controls
{
    /// <summary>
    /// base class for analog rotary knobs rather than rotary switches
    /// </summary>
    public abstract class RotaryKnob : Rotary, Capabilities.IConfigurableImageLocation
    {
        private string _knobImage;
        private double _rotation;

        protected RotaryKnob(string name, Size defaultSize) : base(name, defaultSize)
        {
            // no code
        }


        public string KnobImage
        {
            get { return _knobImage; }
            set
            {
                if ((_knobImage == null && value != null)
                    || (_knobImage != null && !_knobImage.Equals(value)))
                {
                    string oldValue = _knobImage;
                    _knobImage = value;
                    OnPropertyChanged("KnobImage", oldValue, value, true);
                    Refresh();
                }
            }
        }


        /// <summary>
        /// Performs a replace of text in this controls image names
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        public void ReplaceImageNames(string oldName, string newName)
        {
            KnobImage = !string.IsNullOrEmpty(KnobImage) ? KnobImage : string.IsNullOrEmpty(oldName) ? newName + KnobImage : KnobImage.Replace(oldName, newName);
        }

        public double KnobRotation
        {
            get { return this._rotation; }
            protected set
            {
                if (!this._rotation.Equals(value))
                {
                    double oldValue = this._rotation;
                    this._rotation = value % 360d;
                    OnPropertyChanged("KnobRotation", oldValue, value, false);
                    OnDisplayUpdate();
                }
            }
        }
    }
}