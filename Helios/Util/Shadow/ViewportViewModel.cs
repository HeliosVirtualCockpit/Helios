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
// 

using System.Windows;
using GadrocsWorkshop.Helios.Windows;

namespace GadrocsWorkshop.Helios.Util.Shadow
{
    public class ViewportViewModel : HeliosViewModel<ShadowVisual>
    {
        #region Private

        private Vector _globalOffset;
        private Rect _monitor = new Rect(0, 0, 1, 1);
        private double _scale;
        private Rect _viewport = new Rect(0, 0, 1, 1);

        #endregion

        public ViewportViewModel(ShadowVisual data, Vector globalOffset, double scale) : base(data)
        {
            _globalOffset = globalOffset;
            _scale = scale;
            Update(data.Monitor);
            Update(data.Visual);

            Data.ViewportChanged += Data_ViewportChanged;
            Data.MonitorChanged += Data_MonitorChanged;
        }

        private void Data_MonitorChanged(object sender, RawMonitorEventArgs e)
        {
            // some geometry of our containing monitor may have happened
            Update(e.Raw);
        }

        // not automatically called because we are strongly referenced by event
        public void Dispose()
        {
            Data.ViewportChanged -= Data_ViewportChanged;
            Data.MonitorChanged -= Data_MonitorChanged;
        }

        private void Data_ViewportChanged(object sender, RawViewportEventArgs e)
        {
            // some geometry change may have happened to our viewport specifically
            Update(e.Raw);
        }

        public void Update(Monitor monitor)
        {
            _monitor.X = monitor.Left;
            _monitor.Y = monitor.Top;
            _monitor.Width = monitor.Width;
            _monitor.Height = monitor.Height;
            Update();
        }

        public void Update(Vector globalOffset)
        {
            _globalOffset = globalOffset;
            Update();
        }

        public void Update(HeliosVisual viewport)
        {
            _viewport.X = viewport.Left;
            _viewport.Y = viewport.Top;
            _viewport.Width = viewport.Width;
            _viewport.Height = viewport.Height;

            ///TODO: Re-visit the viewport position determination. There is certainly a proper way to get
            ///position of the viewport on the monitor
            Point monitorViewPointOffset = new Point(0, 0);
            HeliosVisual visual1 = viewport;
            while (visual1.Parent != null && !visual1.GetType().Equals("GadrocsWorkshop.Helios.Monitor"))
            {
                monitorViewPointOffset.X += visual1.Left;
                monitorViewPointOffset.Y += visual1.Top;
                visual1 = visual1.Parent;
            }
            _viewport.Offset(monitorViewPointOffset.X, monitorViewPointOffset.Y);

            Update();
        }

        public void Update(double scale)
        {
            _scale = scale;
            Update();
        }

        private void Update()
        {
            Rect absolute = _viewport;
            absolute.Offset(_monitor.X, _monitor.Y);
            absolute.Offset(_globalOffset);
            absolute.Scale(_scale, _scale);
            Rect = absolute;
        }

        public static readonly DependencyProperty RectProperty =
            DependencyProperty.Register("Rect", typeof(Rect), typeof(ViewportViewModel), new PropertyMetadata(null));

        public Rect Rect
        {
            get => (Rect) GetValue(RectProperty);
            set => SetValue(RectProperty, value);
        }
    }
}