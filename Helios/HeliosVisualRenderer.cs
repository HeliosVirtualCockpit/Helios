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
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Controls;

    /// <summary>
    /// Base class for visual renderers.
    /// </summary>
    public abstract class HeliosVisualRenderer : NotificationObject
    {
        private WeakReference _visual = new WeakReference(null);
        private bool _needsRefresh = true;
        private TransformGroup _transform;
        private Effects.ColorAdjustEffect _effect;
        private bool _designTime = false, _designTimeChecked = false;

        #region Properties

        /// <summary>
        /// Visual which this renderer will render.
        /// </summary>
        public HeliosVisual Visual
        {
            get
            {
                return _visual.Target as HeliosVisual;
            }
            set
            {
                if ((_visual == null && value != null)
                    || (_visual != null && !_visual.Equals(value)))
                {
                    HeliosVisual oldControl = _visual.Target as HeliosVisual;
                    _visual = new WeakReference(value);
                    OnPropertyChanged("Visual", oldControl, value, false);
                }
            }
        }

        public Transform Transform
        {
            get { return _transform; }
        }

        #endregion

        /// <summary>
        /// Renders the visual without pushing a scale transform.
        /// </summary>
        /// <param name="drawingContext">Context on which to draw this control.</param>
        public void Render(DrawingContext drawingContext)
        {
            CheckRefresh();
            OnRender(drawingContext);
        }

        /// <summary>
        /// Render the visual at a specified size.
        /// </summary>
        /// <param name="drawingContext">Context on which to draw this control.</param>
        /// <param name="size"></param>
        public void Render(DrawingContext drawingContext, Size size)
        {
            CheckRefresh();
            OnRender(drawingContext, size.Width / Visual.Width, size.Height / Visual.Height);
        }

        private void CheckRefresh()
        {
            if (_needsRefresh)
            {
               OnRefresh();
               UpdateTransform();
                _needsRefresh = false;
            }
        }

        /// <summary>
        /// Renders this control in the given drawing context.
        ///
        /// NOTE: must not do any expensive such as image loads or brush creation, since it is called on every frame
        /// </summary>
        /// <param name="drawingContext">Context on which to draw this control.</param>
        protected abstract void OnRender(DrawingContext drawingContext);

        /// <summary>
        /// Renders this control using scale in the given drawing context.
        ///
        /// NOTE: may be overridden if the client code has a better way of rendering to scale
        /// </summary>
        /// <param name="drawingContext">Context on which to draw this control.</param>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>

        protected virtual void OnRender(DrawingContext drawingContext, double scaleX, double scaleY)
        {
            drawingContext.PushTransform(new ScaleTransform(scaleX, scaleY));
            OnRender(drawingContext);
            drawingContext.Pop();
        }


        /// <summary>
        /// Refreshes and reloads all resources needed to display this visual.
        /// </summary>
        public void Refresh()
        {
            _needsRefresh = true;
            CheckRefresh();
        }

        /// <summary>
        /// Recreate all cached images and text based on current control state
        /// </summary>
        protected abstract void OnRefresh();

        private void UpdateTransform()
        {
            _transform = Visual?.CreateTransform();
        }
        protected virtual void RenderEffect(DrawingContext drawingContext, ImageSource image, Rect imageRectangle)
        {
            // ShaderEffect can be deleted in Profile Editor so we always need to get it from ProfileManager
            if (!_designTimeChecked)
            {
                _designTime = ConfigManager.Application.ShowDesignTimeControls;
                _designTimeChecked = true;

            }
            if (!_designTime)
            {
                // Attempt to cache the ShaderEffect if we're in Control Center
                if (_effect == null && ConfigManager.ProfileManager.CurrentEffect != null)
                {
                    _effect = ConfigManager.ProfileManager.CurrentEffect as Effects.ColorAdjustEffect;
                }
            } else
            {
                _effect = ConfigManager.ProfileManager.CurrentEffect as Effects.ColorAdjustEffect;
            }

            Image imageControl = new Image
                {
                    Source = image,
                    Width = image != null ? image.Width : 0,
                    Height = image != null ? image.Width : 0,

                };
            if (!Visual.EffectsExclusion && Visual.IsVisible && _effect != null && _effect.Enabled)
            {
                imageControl.Effect = _effect;
            }
            VisualBrush visualBrush = new VisualBrush(imageControl);
            drawingContext.DrawRectangle(visualBrush, null, imageRectangle);
        }

    }
}
