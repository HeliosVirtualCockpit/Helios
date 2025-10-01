//  Copyright 2014 Craig Courtney
//  Copyright 2022 Helios Contributors
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


namespace GadrocsWorkshop.Helios.Controls
{
    using System;
    using System.Windows;
    using System.Globalization;
    using System.Collections.Generic;
    using System.Windows.Media;
    using GadrocsWorkshop.Helios.Controls.Capabilities;

    public class KineographRenderer : HeliosVisualRenderer
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private Rect _animationFrameRect;
        private string _namePattern;

        #region Properties

        #endregion
        protected override void OnRender ( System.Windows.Media.DrawingContext drawingContext )
        {
            IKineographControl animation = Visual as IKineographControl;
            if (animation != null )
            {
                if(animation.AnimationFrameCount > 0 && animation.AnimationFrameNumber <= animation.AnimationFrameCount-1)
                {
                    DrawImage(drawingContext, animation.AnimationFrames[(int)animation.AnimationFrameNumber], _animationFrameRect);
                }
            }
        }

        protected override void OnRefresh ( )
        {
            IKineographControl animation = Visual as IKineographControl;
            if ( animation != null )
            {
                if (_namePattern != animation.AnimationFrameImageNamePattern)
                {
                    animation.AnimationFrames.Clear();
                    _namePattern = animation.AnimationFrameImageNamePattern;
                }
                _animationFrameRect.Width = animation.Width;
                _animationFrameRect.Height = animation.Height;
                char substitutionChar = animation.AnimationFrameImageNamePattern.Contains("*.") ? '*' : animation.AnimationFrameImageNamePattern.Contains("0.") ? '0' : ' ';
                if (substitutionChar !=  ' ')
                {
                    if (animation.AnimationFrames.Count == 0)
                    {
                        bool imageLoaded = true;
                        if (ConfigManager.ImageManager is IImageManager3 animationCapable)
                        {
                            for (int i = 0; i < 20 && imageLoaded; i++)
                            {
                                try
                                {
                                    animation.AnimationFrames.Add(animationCapable.LoadImage(ImageFileName(animation.AnimationFrameImageNamePattern, i, substitutionChar), LoadImageOptions.SuppressMissingImageMessages));
                                    if (animation.AnimationFrames[animation.AnimationFrames.Count - 1] == null)
                                    {
                                        imageLoaded = false;
                                        animation.AnimationFrames.RemoveAt(animation.AnimationFrames.Count - 1);
                                    }
                                }
                                catch
                                {
                                    if (i > 0)
                                    {
                                        imageLoaded = false; // tolerate image sequences starting at 0 or 1
                                        Logger.Debug($"Unable to load image name {ImageFileName(animation.AnimationFrameImageNamePattern, i, substitutionChar)}");
                                    }
                                }
                            }
                            animation.AnimationIsPng = animation.AnimationFrameImageNamePattern.IndexOf(".png", StringComparison.InvariantCultureIgnoreCase) >= 0;
                            animation.AnimationFrameCount = animation.AnimationFrames.Count;
                            animation.AnimationFrameNumber = 0;
                            Logger.Debug($"Loaded animation frame set {animation.AnimationFrameImageNamePattern} with {animation.AnimationFrames.Count} frames");
                        } else
                        {
                            animation.AnimationFrames.Clear();
                        }
                    }
                }
                else
                {
                    animation.AnimationFrames.Clear();
                }
            }
            else
            {
                animation.AnimationFrames.Clear();
            }
        }
        private string ImageFileName(string baseImageName,int index, char substitutionChar)
        {
            return baseImageName.Replace($"{substitutionChar}.", $"{index}.");
        }
    }
}
