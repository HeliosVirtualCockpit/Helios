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

namespace GadrocsWorkshop.Helios.Gauges
{
    using System;
    using System.Windows;

    public class CompositeBaseGauge : CompositeVisual
    {
        private GaugeComponentCollection _components = new GaugeComponentCollection();

        protected CompositeBaseGauge(string name, Size nativeSize)
            : base(name, nativeSize)
        {
            _components.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Components_CollectionChanged);
        }

        #region Properties

        public GaugeComponentCollection Components
        {
            get
            {
                return _components;
            }
        }

        #endregion

        void Components_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (GaugeComponent component in e.OldItems)
                {
                    component.DisplayUpdate -= Component_DisplayUpdate;
                }
            }

            if (e.NewItems != null)
            {
                foreach (GaugeComponent component in e.NewItems)
                {
                    component.DisplayUpdate += Component_DisplayUpdate;
                }
            }
            Refresh();
        }

        void Component_DisplayUpdate(object sender, EventArgs e)
        {
            OnDisplayUpdate();
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
