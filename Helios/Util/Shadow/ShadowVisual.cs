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

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using GadrocsWorkshop.Helios.Controls.Capabilities;

namespace GadrocsWorkshop.Helios.Util.Shadow
{
    /// <summary>
    /// This class represents a Helios visual being observed.
    /// It is the model class.
    /// </summary>
    public class ShadowVisual : NotificationObject
    {
        public event EventHandler<RawViewportEventArgs> ViewportChanged;

        public event EventHandler<RawMonitorEventArgs> MonitorChanged;

        /// <summary>
        /// callbacks to our parent
        /// </summary>
        protected IShadowVisualParent _parent;

        /// <summary>
        /// true if this item is a viewport extent, in which case Viewport property is an interface for it
        /// </summary>
        public bool IsViewport => Viewport != null;

        /// <summary>
        /// true if viewport location is directly on monitor, however if the Viewport is on a child, then the location
        /// is not tracked and automatic updating is not performed.
        /// </summary>
        public bool IsViewportLocationReliable => Visual.Parent.Equals(Monitor);

        /// <summary>
        /// map from visual child to its shadow representation
        /// </summary>
        public Dictionary<HeliosVisual, ShadowVisual> Children { get; protected set; } = new Dictionary<HeliosVisual, ShadowVisual>();

        /// <summary>
        /// the visual we are shadowing
        /// </summary>
        public HeliosVisual Visual { get; protected set; }

        /// <summary>
        /// the monitor on which this visual is placed
        /// </summary>
        public Monitor Monitor { get; protected set; }

        /// <summary>
        /// viewport extent or null if this item is not a viewport
        /// </summary>
        public IViewportExtent Viewport { get; protected set; }

        internal ShadowVisual(IShadowVisualParent parent, Monitor monitor, HeliosVisual visual, bool recurse = true)
        {
            // create a shadow object
            _parent = parent;
            Monitor = monitor;
            Visual = visual;
            Viewport = visual as IViewportExtent;
            if (IsViewport)
            {
                _parent.AddViewport(this);
                visual.PropertyChanged += Viewport_PropertyChanged;
            }

            // observe changes
            monitor.Moved += Monitor_Modified;
            monitor.Resized += Monitor_Modified;
            visual.Moved += Visual_Modified;
            visual.Resized += Visual_Modified;
            visual.Children.CollectionChanged += Visual_Children_CollectionChanged;

            // NOTE: changes in monitor children tracked in ShadowMonitor

            if (recurse)
            {
                Instrument(monitor, visual);
            }
        }

        private void Viewport_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // scan for changes to viewport extents, which our customers might care about
            switch (e.PropertyName)
            {
                case nameof(IViewportExtent.ViewportName):
                case nameof(IViewportExtent.RequiresPatches):
                {
                    ViewportChanged?.Invoke(this, new RawViewportEventArgs(Visual));
                    break;
                }
            }
        }

        private void Monitor_Modified(object sender, EventArgs e)
        {
            MonitorChanged?.Invoke(this, new RawMonitorEventArgs(Monitor));
        }

        private void Visual_Modified(object sender, EventArgs e)
        {
            OnVisualModified();
        }

        /// <summary>
        /// recusively add all descendants for tracking
        /// </summary>
        /// <param name="monitor"></param>
        /// <param name="visual"></param>
        protected void Instrument(Monitor monitor, HeliosVisual visual)
        {
            foreach (HeliosVisual child in visual.Children)
            {
                Children[child] = new ShadowVisual(_parent, monitor, child);
            }
        }

        private void Visual_Children_CollectionChanged(object sender,
            NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (HeliosVisual newChild in e.NewItems)
                {
                    OnAdded(newChild);
                }
            }

            if (e.OldItems != null)
            {
                foreach (HeliosVisual oldChild in e.OldItems)
                {
                    OnRemoved(oldChild);
                }
            }
        }

        protected virtual void OnAdded(HeliosVisual newChild)
        {
            Children[newChild] = new ShadowVisual(_parent, Monitor, newChild);
        }

        protected virtual void OnRemoved(HeliosVisual oldChild)
        {
            if (!Children.ContainsKey(oldChild))
            {
                // this has happened before and it is unclear why
                ConfigManager.LogManager.LogInfo(
                    $"the Visual object {oldChild.Name} of type {oldChild.TypeIdentifier} was not found in the data used for tracking viewports and monitors; probable program error");
                return;
            }

            ShadowVisual shadow = Children[oldChild];
            shadow.Dispose();
            Children.Remove(oldChild);
        }

        protected virtual void OnVisualModified()
        {
            if (!IsViewport)
            {
                return;
            }

            // we actually care about this change
            ViewportChanged?.Invoke(this, new RawViewportEventArgs(Visual));
        }

        internal virtual void Dispose()
        {
            if (IsViewport)
            {
                _parent.RemoveViewport(this);
                Visual.PropertyChanged -= Viewport_PropertyChanged;
            }

            Visual.Moved -= Visual_Modified;
            Visual.Resized -= Visual_Modified;
            Visual.Children.CollectionChanged -= Visual_Children_CollectionChanged;
            foreach (ShadowVisual child in Children.Values)
            {
                child.Dispose();
            }
        }
    }
}