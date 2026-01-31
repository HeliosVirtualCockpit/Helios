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
    using GadrocsWorkshop.Helios.Effects;
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public abstract class GaugeComponent
    {
        private Geometry _clip;
        private Geometry _renderClip;
        private bool _hidden = false;
        private bool _imageRefresh = false;
        private Effects.ColorAdjustEffect _effect;
        public event EventHandler DisplayUpdate;
        private bool _effectsExclusion = false;
        private bool _designTime = false, _designTimeChecked = false;
        private RenderTargetBitmap _rtb;
        private DrawingVisual _visual = new DrawingVisual();
        private VisualBrush _visualBrush = new VisualBrush();
        private Image _imageControl = new Image();
        private DrawingContext _tempDrawingContext;

        #region Properties

        public Geometry Clip
        {
            get
            {
                return _clip;
            }
            set
            {
                _clip = value;
                _clip.Freeze();
            }
        }


        public bool IsHidden
        {
            get
            {
                return _hidden;
            }
            set
            {
                if (!_hidden.Equals(value))
                {
                    _hidden = value;
                    OnDisplayUpdate();
                }
            }
        }

        /// <summary>
        /// Indicates that the Image or images are to be reloaded from disk
        /// </summary>
        public bool ImageRefresh
        {
            get => _imageRefresh;
            set => _imageRefresh = value;
        }
        /// <summary>
        /// Whether this control will have effects applied to is on rendering.
        /// </summary>
        public virtual bool EffectsExclusion
        {
            get => _effectsExclusion;
            set
            {
                if (!_effectsExclusion.Equals(value))
                {
                    _effectsExclusion = value;
                }
            }
        }
        protected bool NeedsEffect
        {
            get => ConfigManager.ProfileManager.CurrentEffect != null && (ConfigManager.ProfileManager.CurrentEffect as ColorAdjustEffect).Enabled && !_hidden && !_effectsExclusion;
        }

        #endregion

        protected void OnDisplayUpdate()
        {
            EventHandler handler = DisplayUpdate;
            if (handler != null)
            {
                handler.Invoke(this, EventArgs.Empty);
            }
        }

        public void Render(DrawingContext drawingContext)
        {
            if (!_hidden)
            {
                if (_renderClip != null)
                {
                    drawingContext.PushClip(_renderClip);
                }
                OnRender(drawingContext);
                if (_renderClip != null)
                {
                    drawingContext.Pop();
                }
            }
        }

        protected abstract void OnRender(DrawingContext drawingContext);

        public void Refresh(double xScale, double yScale)
        {
            if (Clip != null)
            {
                _renderClip = Clip.CloneCurrentValue();
                _renderClip.Transform = new ScaleTransform(xScale, yScale);
            }
            else
            {
                _renderClip = null;
            }
            OnRefresh(xScale, yScale);
        }
        protected abstract void OnRefresh(double xScale, double yScale);

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
            }
            else
            {
                _effect = ConfigManager.ProfileManager.CurrentEffect as Effects.ColorAdjustEffect;
            }

            _imageControl.Source = image;
            _imageControl.Width = image != null ? image.Width : 0;
            _imageControl.Height = image != null ? image.Height : 0;
            _imageControl.Effect = null;

            if (_effect != null && _effect.Enabled && !EffectsExclusion)
            {
                _imageControl.Effect = _effect;
            } else
            {
                _imageControl.Effect = null;
            }
            _visualBrush.Visual = _imageControl;
            drawingContext.DrawRectangle(_visualBrush, null, imageRectangle);
        }
        protected void DrawImage(DrawingContext drawingContext, ImageSource image, Rect rectangle)
        {
            if (!NeedsEffect)
            {
                drawingContext.DrawImage(image, rectangle);
            }
            else
            {
                _tempDrawingContext = _visual.RenderOpen();
                _tempDrawingContext.DrawImage(image, rectangle);
                _tempDrawingContext.Close();
                RenderVisual(drawingContext, _visual, rectangle);
            }
        }
        protected virtual void RenderVisual(DrawingContext drawingContext, DrawingVisual visual, Rect rectangle)
        {
            if (visual.ContentBounds.IsEmpty) { return; }
            rectangle.Width += rectangle.X;
            rectangle.Height += rectangle.Y;
            rectangle.X = rectangle.Y = 0;
            if (_rtb == null || _rtb.PixelWidth != Convert.ToInt32(rectangle.Width) || _rtb.PixelHeight != Convert.ToInt32(rectangle.Height))
            {
                // Address MILERR_WIN32ERROR (Exception from HRESULT: 0x88980003 in PresentationCore
                (_rtb?.GetType().GetField("_renderTargetBitmap", BindingFlags.Instance | BindingFlags.NonPublic)?
                .GetValue(_rtb) as IDisposable)?.Dispose();  // from https://github.com/dotnet/wpf/issues/3067

                _rtb = new RenderTargetBitmap(Convert.ToInt32(rectangle.Width), Convert.ToInt32(rectangle.Height), 96, 96, PixelFormats.Pbgra32);
            } else
            {
                _rtb.Clear();
            }
            _rtb.Render(visual);
            drawingContext.DrawImage(_rtb, rectangle);
            RenderEffect(drawingContext, _rtb, rectangle);
        }
    }
}
