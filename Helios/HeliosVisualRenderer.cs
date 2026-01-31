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
    using GadrocsWorkshop.Helios.Controls;
    using GadrocsWorkshop.Helios.Controls.Capabilities;
    using GadrocsWorkshop.Helios.Effects;
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Base class for visual renderers.
    /// </summary>
    public abstract class HeliosVisualRenderer : NotificationObject
    {
        private WeakReference _visual = new WeakReference(null);
        private bool _needsRefresh = true;
        private TransformGroup _transform;
        private Effects.ColorAdjustEffect _effect;
        private DrawingVisual _dVisual = new DrawingVisual();
        private DrawingContext _tempDrawingContext;
        private VisualBrush _brush = new VisualBrush();

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
        protected bool NeedsEffect
        {
            get => ConfigManager.ProfileManager.CurrentEffect != null && (ConfigManager.ProfileManager.CurrentEffect as ColorAdjustEffect).Enabled && Visual.IsVisible && !Visual.EffectsExclusion;
        }
        protected ColorAdjustEffect Effect
        {
            get
            {
                if (!NeedsEffect) return null;

                if(_effect == null)
                {
                    _effect = GetEffect;
                }
                return _effect;
            }
        }
        private ColorAdjustEffect GetEffect
        {
            get
            {
                ColorAdjustEffect effect = null;
                if (!_designTime)
                {
                    // Attempt to cache the ShaderEffect if we're in Control Center
                    if (effect == null && ConfigManager.ProfileManager.CurrentEffect != null)
                    {
                        effect = ConfigManager.ProfileManager.CurrentEffect as ColorAdjustEffect;
                    }
                }
                else
                {
                    effect = ConfigManager.ProfileManager.CurrentEffect as ColorAdjustEffect;
                }
                return effect;
            }
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
        public void Render(DrawingContext drawingContext, System.Windows.Size size)
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
            if (scaleX != 1d || scaleY != 1d)
            {
                drawingContext.PushTransform(new ScaleTransform(scaleX, scaleY));
            }
            OnRender(drawingContext);
            if (scaleX != 1d || scaleY != 1d)
            {
                drawingContext.Pop();
            }
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

        /// <summary>
        /// Adds an effect to the DrawingVisual if needed and then uses this as a 
        /// brush for rendering a Rectangle
        /// </summary>
        /// <param name="drawingContext"></param>
        /// <param name="image"></param>
        /// <param name="imageRectangle"></param>
        private void RenderEffect(DrawingContext drawingContext, DrawingVisual dVisual, Rect imageRectangle)
        {
            // ShaderEffect can be deleted in Profile Editor so we always need to get it from ProfileManager
            if (!_designTimeChecked)
            {
                _designTime = ConfigManager.Application.ShowDesignTimeControls;
                _designTimeChecked = true;

            }
            _effect = GetEffect;

            dVisual.Effect = !Visual.EffectsExclusion ? _effect : null;
            _brush.Visual = dVisual;
            drawingContext.DrawRectangle(_brush, null, imageRectangle);
        }

        /// <summary>
        /// Renders the DrawingVisual to a bitmap (which is an ImageSource)
        /// </summary>
        /// <param name="drawingContext"></param>
        /// <param name="visual"></param>
        /// <param name="rectangle"></param>
        protected void RenderVisual(DrawingContext drawingContext, DrawingVisual visual, Rect rectangle)
        {
            if(visual.ContentBounds.IsEmpty || rectangle.Width == 0 || rectangle.Height == 0) 
            { 
                return; 
            }

            RenderEffect(drawingContext, visual, rectangle);
        }

        #region Draw Proxies
        /// <summary>
        /// Proxy for the DrawingContext DrawImage method.
        /// Determines whether a shader effect is to be used, and if so,
        /// then it will render the image into a DrawingVisual which can then 
        /// be manipulated and have a shader effect added to it.
        /// </summary>
        /// <param name="drawingContext">Default DrawingContext</param>
        /// <param name="image">ImageSource to be drawn</param>
        /// <param name="rectangle"></param>
        protected void DrawImage(DrawingContext drawingContext, ImageSource imageSource, Rect rectangle)
        {
            if (!NeedsEffect)
            {
                drawingContext.DrawImage(imageSource, rectangle);
            }
            else
            {
                _tempDrawingContext = _dVisual.RenderOpen();
                _tempDrawingContext.DrawImage(imageSource, rectangle);
                _tempDrawingContext.Close();
                RenderVisual(drawingContext, _dVisual, !_dVisual.ContentBounds.IsEmpty ? _dVisual.ContentBounds : rectangle);
            }
        }
        protected void DrawGroup(DrawingContext drawingContext, DrawingGroup group)
        {
            if (!NeedsEffect)
            {
                drawingContext.DrawDrawing(group);
            }
            else
            {
                _tempDrawingContext = _dVisual.RenderOpen();
                _tempDrawingContext.DrawDrawing(group);
                _tempDrawingContext.Close();
                RenderVisual(drawingContext, _dVisual, !_dVisual.ContentBounds.IsEmpty ? _dVisual.ContentBounds : new Rect(0,0,Visual.Width, Visual.Height));
            }
        }
#endregion Draw Proxies
    }
}
