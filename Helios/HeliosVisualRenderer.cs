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

        /// <summary>
        /// Adds an effect to the DrawingVisual if needed and then uses this as a 
        /// brush for rendering a Rectangle
        /// </summary>
        /// <param name="drawingContext"></param>
        /// <param name="image"></param>
        /// <param name="imageRectangle"></param>
        private void RenderEffect(DrawingContext drawingContext, ImageSource image, Rect imageRectangle)
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
            }
            else
            {
                _effect = ConfigManager.ProfileManager.CurrentEffect as Effects.ColorAdjustEffect;
            }

            System.Windows.Controls.Image imageControl = new System.Windows.Controls.Image
            {
                Source = image,
                Width = image != null ? image.Width : 0,
                Height = image != null ? image.Width : 0,

            };
            if (!Visual.EffectsExclusion)
            {
                imageControl.Effect = _effect;
            }
            VisualBrush visualBrush = new VisualBrush(imageControl);
            drawingContext.DrawRectangle(visualBrush, null, imageRectangle);
        }

        /// <summary>
        /// Renders the DrawingVisual to a bitmap (which is an ImageSource)
        /// </summary>
        /// <param name="drawingContext"></param>
        /// <param name="visual"></param>
        /// <param name="rectangle"></param>
        protected void RenderVisual(DrawingContext drawingContext, DrawingVisual visual, Rect rectangle)
        {
            if(visual.ContentBounds.IsEmpty || rectangle.Width == 0 || rectangle.Height == 0) { return; }
            RenderTargetBitmap rtb = new RenderTargetBitmap(Convert.ToInt32(rectangle.Width), Convert.ToInt32(rectangle.Height), 96, 96, PixelFormats.Pbgra32);
            rtb.Render(visual);
            drawingContext.DrawImage(rtb, rectangle);
            RenderEffect(drawingContext, rtb, rectangle);
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
        protected void DrawImage(DrawingContext drawingContext, ImageSource image, Rect rectangle)
        {
            if (!NeedsEffect)
            {
                drawingContext.DrawImage(image, rectangle);
            }
            else
            {
                DrawingVisual visual = new DrawingVisual();
                DrawingContext tempDrawingContext = visual.RenderOpen();
                tempDrawingContext.DrawImage(image, rectangle);
                tempDrawingContext.Close();
                RenderVisual(drawingContext, visual, rectangle);
            }

        }
        /// <summary>
        /// Proxy for the DrawingContext DrawGeometry method.
        /// Determines whether a shader effect is to be used, and if so,
        /// then it will render the Geometry into a DrawingVisual which can then 
        /// be manipulated and have a shader effect added to it.
        /// </summary>
        /// <param name="drawingContext"></param>
        /// <param name="brush"></param>
        /// <param name="pen"></param>
        /// <param name="path"></param>
        /// <param name="rectangle"></param>
        protected void DrawGeometry(DrawingContext drawingContext, Brush brush, Pen pen, PathGeometry path, Rect rectangle)
        {
            if (!NeedsEffect)
            {
                drawingContext.DrawGeometry(brush, pen, path);
            }
            else
            {
                DrawingVisual visual = new DrawingVisual();
                DrawingContext tempDrawingContext = visual.RenderOpen();
                tempDrawingContext.DrawGeometry(brush, pen, path);
                tempDrawingContext.Close();
                RenderVisual(drawingContext, visual, rectangle);
            }
        }
        /// <summary>
        /// Proxy for the DrawingContext DrawRectangle method.
        /// Determines whether a shader effect is to be used, and if so,
        /// then it will render the Rectangle into a DrawingVisual which can then 
        /// be manipulated and have a shader effect added to it.        
        /// </summary>
        /// <param name="drawingContext"></param>
        /// <param name="brush"></param>
        /// <param name="pen"></param>
        /// <param name="rectangle"></param>
        protected void DrawRectangle(DrawingContext drawingContext, Brush brush, Pen pen, Rect rectangle)
        {
            if (!NeedsEffect)
            {
                drawingContext.DrawRectangle(brush, pen, rectangle);
            }
            else
            {
                DrawingVisual visual = new DrawingVisual();
                DrawingContext tempDrawingContext = visual.RenderOpen();
                tempDrawingContext.DrawRectangle(brush, pen, rectangle);
                tempDrawingContext.Close();
                RenderVisual(drawingContext, visual, rectangle);
            }
        }
        /// <summary>
        /// Proxy for the DrawingContext DrawRoundedRectangle method.
        /// Determines whether a shader effect is to be used, and if so,
        /// then it will render the RoundedRectangle into a DrawingVisual which can then 
        /// be manipulated and have a shader effect added to it.        
        /// </summary>
        /// <param name="drawingContext"></param>
        /// <param name="brush"></param>
        /// <param name="pen"></param>
        /// <param name="rectangle"></param>
        protected void DrawRoundedRectangle(DrawingContext drawingContext, Brush brush, Pen pen, Rect rectangle, double radiusX, double radiusY)
        {
            if (!NeedsEffect)
            {
                drawingContext.DrawRoundedRectangle(brush, pen, rectangle, radiusX, radiusY);
            }
            else
            {
                DrawingVisual visual = new DrawingVisual();
                DrawingContext tempDrawingContext = visual.RenderOpen();
                tempDrawingContext.DrawRoundedRectangle(brush, pen, rectangle, radiusX, radiusY);
                tempDrawingContext.Close();
                RenderVisual(drawingContext, visual, rectangle);
            }
        }
        /// <summary>
        /// Proxy for the DrawingContext DrawText method.
        /// Determines whether a shader effect is to be used, and if so,
        /// then it will render the Text into the appropraite DrawingContext.
        /// If Effects are used then this is a temporary Context for conversion 
        /// into a DrawingVisual which can then 
        /// be manipulated and have a shader effect added to it.        
        /// </summary>
        /// <param name="drawingContext"></param>
        /// <param name="textVisual"></param>
        /// <param name="brush"></param>
        /// <param name="text"></param>
        /// <param name="rectangle"></param>
        protected void DrawText(DrawingContext drawingContext, ITextControl textVisual, Brush brush, string text, Rect rectangle)
        {
            if (!NeedsEffect)
            {
                textVisual.TextFormat.RenderText(drawingContext, brush, text, rectangle);
            }
            else
            {
                DrawingVisual visual = new DrawingVisual();
                DrawingContext tempDrawingContext = visual.RenderOpen();
                textVisual.TextFormat.RenderText(tempDrawingContext, brush, text, rectangle);
                tempDrawingContext.Close();
                RenderVisual(drawingContext, visual, rectangle);
            }
        }
#endregion Draw Proxies
    }
}
