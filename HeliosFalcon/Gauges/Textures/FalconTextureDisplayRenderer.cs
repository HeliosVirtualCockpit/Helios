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

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GadrocsWorkshop.Helios.ComponentModel;

namespace GadrocsWorkshop.Helios.Interfaces.Falcon.Gauges.Textures
{
    class FalconTextureDisplayRenderer : HeliosVisualRenderer
    {
        private FalconTextureDisplay _display;
        private ImageBrush _defaultImage;
        private ImageBrush _runningImage;
        private Rect _displayRect = new Rect(0, 0, 0, 0);
        

        protected override void OnPropertyChanged(PropertyNotificationEventArgs args)
        {
            if (args.PropertyName.Equals("Visual"))
            {
                _display = args.NewValue as FalconTextureDisplay;
                OnRefresh();
            }
            base.OnPropertyChanged(args);
        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            if (_display != null)
            {
                if (_display.TextureMemory != null && _display.TextureMemory.IsDataAvailable)
                {
                    drawingContext.DrawRectangle(CreateImageBrush(), null, _displayRect);
                }
                else if (_display.IsRunning && _display.TransparencyEnabled)
                {
                    drawingContext.DrawRectangle(_runningImage, null, _displayRect);
                }
                else if (_display.IsRunning && !_display.TransparencyEnabled)
                {
                    drawingContext.DrawRectangle(Brushes.Black, null, _displayRect);
                }
                else if (_defaultImage != null)
                {
                    drawingContext.DrawRectangle(_defaultImage, null, _displayRect);
                }
            }
        }

        private ImageBrush CreateImageBrush()
        {
            ImageBrush brush = null;
            if (_display.TextureMemory.IsDataAvailable)
            {
                NativeMethods.DDSURFACEDESC2 surfaceDesc = (NativeMethods.DDSURFACEDESC2)_display.TextureMemory.MarshalTo(typeof(NativeMethods.DDSURFACEDESC2), 4);
                PixelFormat format = PixelFormats.Bgr32;  //.Format32bppRgb;
                switch (surfaceDesc.ddpfPixelFormat.dwRGBBitCount)
                {
                    case 16:
                        format = PixelFormats.Bgr555; // PixelFormat.Format16bppRgb555;
                        break;
                    case 24:
                        format = PixelFormats.Bgr24; // PixelFormat.Format24bppRgb;
                        break;
                    case 32:
                        format = PixelFormats.Bgr32; // PixelFormat.Format32bppRgb;
                        break;
                }

                int offset = (surfaceDesc.lPitch * (int)_display.TextureRect.Y) + ((int)_display.TextureRect.X * (surfaceDesc.ddpfPixelFormat.dwRGBBitCount / 8));

                BitmapSource image = BitmapSource.Create((int)_display.TextureRect.Width,
                                                         (int)_display.TextureRect.Height,
                                                         96,
                                                         96,
                                                         format,
                                                         null,
                                                         _display.TextureMemory.GetPointer(offset + 4 + surfaceDesc.dwSize),
                                                         surfaceDesc.lPitch * (int)_display.TextureRect.Height,
                                                         surfaceDesc.lPitch);

                if(_display.TransparencyEnabled)
                {
                    image = ImageTransparency(image);
                }

                brush = new ImageBrush(image);

                _defaultImage = new ImageBrush(image)
                {
                    Stretch = Stretch.Fill,
                    TileMode = TileMode.None,
                    Viewport = new Rect(0d, 0d, 1d, 1d),
                    ViewportUnits = BrushMappingMode.RelativeToBoundingBox
                };
            }

            return brush;
        }

        BitmapSource ImageTransparency(BitmapSource sourceImage)
        {
            if (sourceImage.Format != PixelFormats.Bgra32)
            {
                sourceImage = new FormatConvertedBitmap(sourceImage, PixelFormats.Bgra32, null, 0.0);
            }

            int sourceStride = sourceImage.PixelWidth * sourceImage.Format.BitsPerPixel / 8;
            byte[] sourcePixels = new byte[sourceImage.PixelHeight * sourceStride];
            sourceImage.CopyPixels(sourcePixels, sourceStride, 0);

            ImageSource maskImageSource = ConfigManager.ImageManager.LoadImage("{HeliosFalcon}/Images/Textures/hud_mask.png", (int)_display.TextureRect.Width, (int)_display.TextureRect.Height);
            BitmapSource maskImage = (BitmapSource)maskImageSource;

            int maskStride = maskImage.PixelWidth * maskImage.Format.BitsPerPixel / 8;
            byte[] maskPixels = new byte[maskImage.PixelHeight * maskStride];
            maskImage.CopyPixels(maskPixels, maskStride, 0);

            byte targetThreshold = 20;

            int numPixels = Math.Min(sourceImage.PixelHeight * sourceStride, maskImage.PixelHeight * maskStride);

            for (int i = 0; i < numPixels; i += sourceImage.Format.BitsPerPixel / 8)
            {
                if (maskPixels[i] == 0)
                {
                    sourcePixels[i + 3] = 0;
                }
                else
                {
                    bool sourcePixelValid = false;
                    byte pixelMaxValue = Math.Max(Math.Max(sourcePixels[i], sourcePixels[i + 1]), sourcePixels[i + 2]);

                    if (sourcePixels[i] > targetThreshold)
                    {
                        sourcePixels[i] = 255;
                        sourcePixelValid = true;
                    }

                    if (sourcePixels[i + 1] > targetThreshold)
                    {
                        sourcePixels[i + 1] = 255;
                        sourcePixelValid = true;
                    }

                    if (sourcePixels[i + 2] > targetThreshold)
                    {
                        sourcePixels[i + 2] = 255;
                        sourcePixelValid = true;
                    }

                    if (sourcePixelValid)
                    {
                        sourcePixels[i + 3] = pixelMaxValue;
                    }
                    else
                    {
                        sourcePixels[i] = 200;
                        sourcePixels[i + 1] = 255;
                        sourcePixels[i + 2] = 200;
                        sourcePixels[i + 3] = 25;
                    }
                }
            }

            BitmapSource newImage = BitmapSource.Create(sourceImage.PixelWidth, sourceImage.PixelHeight,sourceImage.DpiX, sourceImage.DpiY, PixelFormats.Bgra32, sourceImage.Palette, sourcePixels, sourceStride);
            return newImage;
        }

        protected override void OnRefresh()
        {
            if (_display != null)
            {
                ImageSource defaultImage = ConfigManager.ImageManager.LoadImage(_display.DefaultImage);
                if (defaultImage == null)
                {
                    _defaultImage = null;
                }
                else
                {
                    _defaultImage = new ImageBrush(defaultImage)
                    {
                        Stretch = Stretch.Fill,
                        TileMode = TileMode.None,
                        Viewport = new Rect(0d, 0d, 1d, 1d),
                        ViewportUnits = BrushMappingMode.RelativeToBoundingBox
                    };
                }

                ImageSource runningImage = ConfigManager.ImageManager.LoadImage("{HeliosFalcon}/Images/Textures/hud_mask.png");
                if (runningImage == null)
                {
                    _runningImage = null;
                }
                else
                {
                    _runningImage = new ImageBrush(runningImage)
                    {
                        Opacity = 0.1,
                        Stretch = Stretch.Fill,
                        TileMode = TileMode.None,
                        Viewport = new Rect(0d, 0d, 1d, 1d),
                        ViewportUnits = BrushMappingMode.RelativeToBoundingBox
                    };
                }

                _displayRect.Width = _display.Width;
                _displayRect.Height = _display.Height;
            }
            else
            {
                _defaultImage = null;
                _runningImage = null;
            }
        }
    }
}
