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
    using System.Windows.Media;
    using System.Windows.Controls;
    using System.Windows;

    public abstract class GaugeComponent
    {
        private Geometry _clip;
        private Geometry _renderClip;
        private bool _hidden = false;
        private bool _imageRefresh = false;
        private Effects.ColorAdjustEffect _effect;
        public event EventHandler DisplayUpdate;
        private bool _effectsExclusion = false;


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
            if (_effect == null && ConfigManager.ProfileManager.CurrentEffect != null)
            {
                _effect = ConfigManager.ProfileManager.CurrentEffect as Effects.ColorAdjustEffect;
            }

            Image imageControl = new Image
            {
                Source = image,
                Width = image != null ? image.Width : 0,
                Height = image != null ? image.Height : 0,

            };
            if (_effect != null && !EffectsExclusion)
            {
                imageControl.Effect = _effect;
            }
            VisualBrush visualBrush = new VisualBrush(imageControl);
            drawingContext.DrawRectangle(visualBrush, null, imageRectangle);
        }
    }

}
