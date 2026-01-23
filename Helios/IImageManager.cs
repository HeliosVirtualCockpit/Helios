//  Copyright 2014 Craig Courtney
//  Copyright 2020 Helios Contributors
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
using System.ComponentModel;

namespace GadrocsWorkshop.Helios
{
    using System.Windows.Media;
    using System.Collections.Generic;

    public interface IImageManager
    {
        /// <summary>
        /// Loads an image file from the specified URI.
        /// </summary>
        /// <param name="uri">Can be a file system path, a pack URI, or a special uri of form {assembly}/relativePath</param>
        /// <returns></returns>
        ImageSource LoadImage(string uri);

        /// <summary>
        /// Loads an image file from the specified URI, decoding the image to a specified size.
        /// </summary>
        /// <param name="uri">Can be a file system path, a pack URI, or a special uri of form {assembly}/relativePath</param>
        /// <param name="width">the pixel width to which the image should be scaled during decoding</param>
        /// <param name="height">the pixel height to which the image should be scaled during decoding</param>
        /// <returns></returns>
        ImageSource LoadImage(string uri, int width, int height);

        string MakeImagePathRelative(string filename);
        string MakeImagePathAbsolute(string fileName);

        System.Collections.ArrayList ChangedImages { get; set; }
    }

    public class ImageLoadEventArgs : EventArgs
    {
        public ImageLoadEventArgs(string path)
        {
            Path = path;
        }

        public string Path { get; }
    }

    /// <summary>
    /// Version 2 of IImageManager interface
    /// </summary>
    public interface IImageManager2 : IImageManager
    {
        event EventHandler<ImageLoadEventArgs> ImageLoadSuccess;
        event EventHandler<ImageLoadEventArgs> ImageLoadFailure;

        /// <summary>
        /// discard any previous image load failures; to be called when switching to a new profile
        /// or other context where previous failures would not be relevant to ReplayCurrentFailures
        /// </summary>
        void ClearFailureTracking();

        /// <summary>
        /// replays all failed image loads since the last call to ClearFailureTracking
        /// </summary>
        /// <param name="imageLoadFailureHandler"></param>
        void ReplayCurrentFailures(Action<object, ImageLoadEventArgs> imageLoadFailureHandler);
    }

    [Flags]
    public enum LoadImageOptions
    {
        [Description("Default behavior of LoadImage")]
        None = 0,

        [Description(
            "If the image is loaded from a local file URL and changed on disk, the new version will be used.  In-memory caching of images is disabled.")]
        ReloadIfChangedExternally = 1,

        [Description(
            "If the image cannot be loaded, suppress warning / error messages.")]
        SuppressMissingImageMessages = 2

    }

    /// <summary>
    /// version 3 of public cross-assembly interface IImageManager
    /// </summary>
    public interface IImageManager3 : IImageManager2
    {
        // NOTE: this could technically be a variant of IImageManager instead of IImageManager2, but let's not confuse people

        /// <summary>
        /// Loads an image file from the specified URI.
        /// </summary>
        /// <param name="uri">Can be a file system path, a pack URI, or a special uri of form {assembly}/relativePath</param>
        /// <param name="options">Flags set will change the behavior of LoadImage.  LoadImageOptions.None results in the same behavior as LoadImage from the IImageManager interface.</param>
        /// <returns></returns>
        ImageSource LoadImage(string uri, LoadImageOptions options);

        /// <summary>
        /// Loads an image file from the specified URI, decoding the image to a specified size.
        /// </summary>
        /// <param name="uri">Can be a file system path, a pack URI, or a special uri of form {assembly}/relativePath</param>
        /// <param name="width">the pixel width to which the image should be scaled during decoding</param>
        /// <param name="height">the pixel height to which the image should be scaled during decoding</param>
        /// <param name="options">Flags set will change the behavior of LoadImage.  LoadImageOptions.None results in the same behavior as LoadImage from the IImageManager interface.</param>
        /// <returns></returns>
        ImageSource LoadImage(string uri, int width, int height, LoadImageOptions options);
    }

    /// <summary>
    /// version 4 of public cross-assembly interface IImageManager
    /// </summary>
    public interface IImageManager4 : IImageManager3
    {
        bool CacheObjects { get; set; }

        /// <summary>
        /// when called, clears out any cached objects, so that all image sources and similar objects are
        /// recreated from scratch when they are next loaded
        /// </summary>
        void DropObjectCache();
    }
    /// <summary>
    /// version 4 of public cross-assembly interface IImageManager
    /// </summary>
    public interface IImageManager5 : IImageManager4
    {
        /// <summary>
        /// when called with a filename, it returns a list containing all
        /// files with the same pattern
        /// </summary>
        IEnumerable<ImageSource> LoadAnimationFrames(string animationName);
    }

}
