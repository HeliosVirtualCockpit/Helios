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

using GadrocsWorkshop.Helios.Gauges;

namespace GadrocsWorkshop.Helios.ProfileEditor.ViewModel
{
    public abstract class ToolboxItem : NotificationObject
    {
        private HeliosVisual _iconInstance;
        private bool _isDragging;

        #region Properties

        public HeliosVisual ControlIconInstance
        {
            get
            {
                if (_iconInstance == null)
                {
                    _iconInstance = CreateControl();
                    ConfigureIcon(_iconInstance);
                }
                _iconInstance.EffectsExclusion = true;
                if(_iconInstance is CompositeBaseGauge cbg)
                {       
                    foreach(var g in cbg.Components)
                    {
                        g.EffectsExclusion = true;
                    }
                }
                return _iconInstance;
            }
        }

        public abstract string Name { get; }

        public abstract string Category { get; }

        public abstract bool IsRemoveable { get; }

        public bool IsBeingDragged
        {
            get
            {
                return _isDragging;
            }
            set
            {
                if (!_isDragging.Equals(value))
                {
                    bool oldValue = _isDragging;
                    _isDragging = value;
                    OnPropertyChanged("IsBeingDragged", oldValue, value, false);
                }
            }
        }

        #endregion

        public abstract void ConfigureIcon(HeliosVisual control);

        public abstract HeliosVisual CreateControl();
    }
}
