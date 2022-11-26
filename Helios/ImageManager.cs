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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using GadrocsWorkshop.Helios.Util;
using NLog;

namespace GadrocsWorkshop.Helios
{
    /// <summary>
    /// ImageManager gives access to loading and resizing images from the appropriate locations.
    /// </summary>
    public class ImageManager : IImageManager4
    {
        private readonly string _documentImagePath;
        private static readonly Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly Uri _documentImageUri;
        private readonly HashSet<string> _failedImagePaths = new HashSet<string>();

        public event EventHandler<ImageLoadEventArgs> ImageLoadSuccess;
        public event EventHandler<ImageLoadEventArgs> ImageLoadFailure;

        private readonly XamlFirewall _xamlFirewall;

        /// <summary>
        /// backing field for property CacheObjects, contains
        /// true if object caching is enabled
        /// </summary>
        private bool _cacheObjects;

        // cache and share image source objects, to avoid recreating and loading files
        // (actual bitmap contents may be evicted due to system caching)
        private readonly Dictionary<string, ImageSource> _objectCache = new Dictionary<string, ImageSource>();

        private static bool _suppressMissingImageMessages;

        /// <summary>
        /// this class represents all the characteristics of an image load that must be identical for
        /// the resulting image to be the same
        /// </summary>
        private class ImageLoadRequest
        {
            public ImageLoadRequest(string uri, int? width, int? height, LoadImageOptions options)
            {
                Uri = uri;
                Width = width;
                Height = height;
                Options = options;

                // can calculate a fixed key, since all properties are read only
                Key = $"{uri}&{width ?? 0}&{height ?? 0}&{Convert.ToInt32(options)}";
            }

            public string Uri { get; }
            public int? Width { get; }
            public int? Height{ get; }
            public LoadImageOptions Options { get; }

            public string Key { get; }
        }

        internal ImageManager(string userImagePath)
        {
            Logger.Debug($"Helios will load user images from {Anonymizer.Anonymize(userImagePath)}");
            _documentImagePath = userImagePath;
            _documentImageUri = new Uri(userImagePath);
            _xamlFirewall = new XamlFirewall();
        }

        private ImageSource DoLoadImage(ImageLoadRequest request)
        {
            if (null == request.Uri)
            {
                return null;
            }

            if (_objectCache.TryGetValue(request.Key, out ImageSource imageSource))
            {
                // NOTE: this caching makes a huge difference (many seconds on a large profile) because there are usually
                // many hundreds of buttons and switches using the same images, and it also makes restarting a profile faster
                return imageSource;
            }

            // parse as URI and check for existence of file, return Uri for resource that exists
            // or null (note: resource is not necesarily allowed to be read)
            Uri imageUri = GetImageUri(request.Uri);
            if (imageUri == null)
            {
                // cache denial
                return CacheImageSource(request, null);
            }

            // based on protocol/scheme, check to make sure source location is permitted
            if (!CheckImageLocationSecurity(imageUri))
            {
                // cache denial
                return CacheImageSource(request, null);
            }

            if (request.Uri.EndsWith(".xaml", StringComparison.InvariantCulture))
            {
                imageSource = imageUri.Scheme == "pack" ? LoadXamlResource(imageUri, request.Width, request.Height) : LoadXamlFile(imageUri, request.Width, request.Height);
                return CacheImageSource(request, imageSource);
            }

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("image being loaded from {URI}", Anonymizer.Anonymize(imageUri));
            }
            
            BitmapImage image = new BitmapImage();
            image.BeginInit();

            // Don't keep disk files open, because we may want to change them.  But it is ok to lazy load from embedded resources that can't change.
            image.CacheOption = imageUri.Scheme == "pack" ? BitmapCacheOption.OnDemand : BitmapCacheOption.OnLoad;
            
            // Override BitMapCreateOptions but only for certain controls that require a reload of images 
            image.CreateOptions = request.Options.HasFlag(LoadImageOptions.ReloadIfChangedExternally) ? BitmapCreateOptions.IgnoreImageCache : BitmapCreateOptions.DelayCreation;
            image.UriSource = imageUri;

            _suppressMissingImageMessages = request.Options.HasFlag(LoadImageOptions.SuppressMissingImageMessages);
            // REVISIT: not clear if it is legal to set decoding in just one axis but we will find out if anyone ever uses this function with only one scaling factor
            if (request.Width.HasValue)
            {
                image.DecodePixelWidth = Math.Max(1, request.Width.Value);
            }
            if (request.Height.HasValue)
            {
                image.DecodePixelHeight = Math.Max(1, request.Height.Value);
            }

            image.EndInit();
            return CacheImageSource(request, image);
        }

        private ImageSource CacheImageSource(ImageLoadRequest request, ImageSource source)
        {
            if (_cacheObjects && !request.Options.HasFlag(LoadImageOptions.ReloadIfChangedExternally))
            {
                _objectCache.Add(request.Key, source);
            }
            return source;
        }

        private bool CheckImageLocationSecurity(Uri imageUri)
        {
            switch (imageUri.Scheme)
            {
                case "pack":
                    Logger.Debug("attempt to load image from application resource");

                    if (imageUri.Authority != "application:,,,")
                    {
                        Logger.Warn("siteoforigin or other pack authority denied for security reasons: {URI}",
                            Anonymizer.Anonymize(imageUri));
                        break;
                    }

                    if (string.IsNullOrEmpty(imageUri.AbsolutePath))
                    {
                        Logger.Warn("empty path cannot be resolved: {URI}", Anonymizer.Anonymize(imageUri));
                        break;
                    }

                    string[] components = imageUri.AbsolutePath.Split('/');
                    if (components.Length < 3 ||
                        components[0].Length > 0 ||
                        !components[1].Contains(";component"))
                    {
                        Logger.Warn("pack reference into file system disallowed for security reasons: {URI}",
                            Anonymizer.Anonymize(imageUri));
                        break;
                    }

                    switch (components[2])
                    {
                        case "Images":
                        case "Gauges":
                            return true;
                        case "Interfaces":
                            // it is possible that this was only ever used in a single BMS-centric template
                            if (imageUri.AbsolutePath.EndsWith(".png"))
                            {
                                return true;
                            }
                            break;
                    }
                    Logger.Warn(
                        "pack reference into assembly disallowed because it does not target 'Images' or 'Gauges' folder or a PNG file in 'Interfaces': {URI}",
                        Anonymizer.Anonymize(imageUri));
                    break;

                case "file":
                    Logger.Debug("attempt to load image from local file");

                    if (imageUri.AbsolutePath.Contains("..") ||
                        !imageUri.AbsolutePath.StartsWith(_documentImageUri.AbsolutePath))
                    {
                        Logger.Warn(
                            "images references must be contained in user image files location {Required}; {URI} disallowed",
                            Anonymizer.Anonymize(_documentImagePath),
                            Anonymizer.Anonymize(imageUri));
                        break;
                    }

                    return true;

                default:
                    // this must be disabled since XAML can do whatever it wants so a profile could download any arbitrary code
                    Logger.Warn("XAML network request denied for security reasons: {URI}",
                        Anonymizer.Anonymize(imageUri));
                    break;
            }

            return false;
        }

        private static ImageSource LoadXamlResource(Uri imageUri, int? width, int? height)
        {
            Logger.Debug("XAML being loaded as vector drawing from trusted resource {URI}", Anonymizer.Anonymize(imageUri));
            StreamResourceInfo streamResourceInfo = Application.GetResourceStream(imageUri);
            if (streamResourceInfo == null)
            {
                // these are supposed to be in our assembly, so log as error
                Logger.Error("could not resolve XAML image at {URI}", Anonymizer.Anonymize(imageUri));
                return null;
            }
            Stream xamlStream = streamResourceInfo.Stream;
            if (xamlStream == null)
            {
                // these are supposed to be in our assembly, so log as error
                Logger.Error("XAML image not found at {URI}", Anonymizer.Anonymize(imageUri));
                return null;
            }
            using (xamlStream)
            {
                Canvas canvas = (Canvas) XamlReader.Load(xamlStream);
                return RenderXaml(canvas, width, height);
            }
        }

        private ImageSource LoadXamlFile(Uri imageUri, int? width, int? height)
        {
            Logger.Debug("XAML being loaded as vector drawing from {URI}", Anonymizer.Anonymize(imageUri));
            using (Stream xamlStream = new FileStream(imageUri.AbsolutePath, FileMode.Open))
            {
                try
                {
                    Canvas canvas = _xamlFirewall.LoadXamlDefensively<Canvas>(xamlStream);
                    return RenderXaml(canvas, width, height);
                }
                catch (XamlFirewall.DisallowedElementException ex)
                {
                    Logger.Error("attempt to load XAML {URI} that did not contain simple drawing code denied. {Element} is not allowed.",
                        imageUri, ex.ElementName);
                    return null;
                }
            }
        }

        private static ImageSource RenderXaml(Canvas canvas, int? width, int? height)
        {
            int scaledWidth = width.HasValue ? Math.Max(1, width.Value) : (int) canvas.Width;
            int scaledHeight = height.HasValue ? Math.Max(1, height.Value) : (int) canvas.Height;
            RenderTargetBitmap render =
                new RenderTargetBitmap(scaledWidth, scaledHeight, 96d, 96d, PixelFormats.Pbgra32);
            if (width.HasValue || height.HasValue)
            {
                double scaleX = canvas.Width > 0 ? scaledWidth / canvas.Width : 1.0;
                double scaleY = canvas.Height > 0 ? scaledHeight / canvas.Height : 1.0;
                canvas.RenderTransform = new ScaleTransform(scaleX, scaleY);
            }

            canvas.Measure(new Size(canvas.Width, canvas.Height));
            canvas.Arrange(new Rect(new Size(canvas.Width, canvas.Height)));
            render.Render(canvas);
            return render;
        }

        private Uri GetImageUri(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                // empty string is a normal case for unitialized image paths, and so is null,
                // so short circuit here
                return null;
            }

            if (path.StartsWith("pack:") || path.StartsWith("{"))
            {
                string packPath = MakeImagePathAbsolute(path);
                Uri imageUri = new Uri(packPath, UriKind.Absolute);

                if (CanOpenPackUri(imageUri))
                {
                    return imageUri;
                }
            }
            else if (path.StartsWith("http:", true, CultureInfo.CurrentCulture) ||
                     path.StartsWith("https:", true, CultureInfo.CurrentCulture) ||
                     path.StartsWith("ftp:", true, CultureInfo.CurrentCulture) ||
                     path.StartsWith("file:", true, CultureInfo.CurrentCulture))
            {
                return new Uri(path);
            }
            else
            {
                string filePath = MakeImagePathAbsolute(path);
                if (!string.IsNullOrEmpty(filePath))
                {
                    return new Uri("file://" + filePath);
                }
            }

            return null;
        }

        public string MakeImagePathRelative(string filename)
        {
            string newFilename = filename;

            if (filename.Length > 0)
            {
                if (filename.StartsWith("pack://application:,,,/"))
                {
                    int closingIndex = filename.IndexOf(";component", StringComparison.CurrentCulture);
                    if (closingIndex > -1)
                    {
                        string assembly = filename.Substring(23, closingIndex - 23);
                        newFilename = "{" + assembly + "}" + filename.Substring(closingIndex + 10);
                    }
                }
                else
                {
                    string fullFilename = Path.GetFullPath(filename);
                    if (fullFilename.StartsWith(ConfigManager.ImagePath, StringComparison.CurrentCulture))
                    {
                        if (fullFilename.Equals(ConfigManager.ImagePath))
                        {
                            newFilename ="";
                        }
                        else
                        {
                            newFilename = fullFilename.Substring(ConfigManager.ImagePath.Length + 1);
                        }
                    }
                }
            }

            return newFilename;
        }

        public string MakeImagePathAbsolute(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            if (fileName.StartsWith("pack:"))
            {
                return fileName;
            }

            if (Path.IsPathRooted(fileName))
            {
                // absolute file path, we can check this
                if (File.Exists(fileName))
                {
                    return fileName;
                }
            }
            else
            {
                if (fileName.StartsWith("{"))
                {
                    int closingIndex = fileName.IndexOf('}');
                    if (closingIndex > -1)
                    {
                        // reference to an assembly by name, convert to pack URI
                        string assembly = fileName.Substring(1, closingIndex - 1);
                        return "pack://application:,,,/" + assembly + ";component" +
                               fileName.Substring(closingIndex + 1);
                    }
                }

                // First check the users images directory
                string loadName = Path.Combine(_documentImagePath, fileName);
                if (File.Exists(loadName))
                {
                    return loadName;
                }

                // these load failures happen during deserialization, so we have no idea if they should be warnings or not
                // unless we configure this with a control center preference
                //
                // we cannot configure it from the Profile Interface's settings, because that has not been loaded yet
                //
                // When images for animation frames are being loaded, we do not want to issue a message for the 
                // post-ultimate image.
                if (!_suppressMissingImageMessages)
                {
                    Logger.Info("referenced user image not found at {loadName}", Anonymizer.Anonymize(loadName));
                }
            }

            return "";
        }

        public static bool CanOpenPackUri(Uri uri)
        {
            try
            {
                StreamResourceInfo streamResourceInfo = Application.GetResourceStream(uri);
                if (null == streamResourceInfo)
                {
                    // this is the normal case for a resource that is not found
                    return false;
                }
                Stream resourceStream = streamResourceInfo.Stream;
                if (resourceStream.Length < 1)
                {
                    resourceStream.Close();
                    return false;
                }
                resourceStream.Close();
                return true;
            }
            catch (Exception e)
            {
                if (!_suppressMissingImageMessages)
                {
                    Logger.Error(e, "Error loading image from pack reference to assembly {URI}.", uri);
                }
                return false;
            }
        }

        #region IImageManager

        public ImageSource LoadImage(string uri)
        {
            ImageSource result = DoLoadImage(new ImageLoadRequest(uri, null, null, LoadImageOptions.None));
            OnImageLoad(uri, result);
            return result;
        }

        public ImageSource LoadImage(string uri, int width, int height)
        {
            ImageSource result = DoLoadImage(new ImageLoadRequest(uri, width, height, LoadImageOptions.None));
            OnImageLoad(uri, result);
            return result;
        }

        #endregion

        #region IImageManager3

        public ImageSource LoadImage(string uri, LoadImageOptions options)
        {
            ImageSource result = DoLoadImage(new ImageLoadRequest(uri, null, null, options));
            OnImageLoad(uri, result);
            return result;
        }

        public ImageSource LoadImage(string uri, int width, int height, LoadImageOptions options)
        {
            ImageSource result = DoLoadImage(new ImageLoadRequest(uri, width, height, options));
            OnImageLoad(uri, result);
            return result;
        }

        #endregion

        private void OnImageLoad(string path, ImageSource result)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            if (result == null)
            {
                if (!_suppressMissingImageMessages)
                {
                    _failedImagePaths.Add(path);
                    ImageLoadFailure?.Invoke(this, new ImageLoadEventArgs(path));
                }
            }
            else
            {
                _failedImagePaths.Remove(path);
                ImageLoadSuccess?.Invoke(this, new ImageLoadEventArgs(path));
            }
        }

        #region IImageManager2

        public void ClearFailureTracking()
        {
            _failedImagePaths.Clear();
        }

        public void ReplayCurrentFailures(Action<object, ImageLoadEventArgs> imageLoadFailureHandler)
        {
            foreach (string path in _failedImagePaths)
            {
                imageLoadFailureHandler.Invoke(this, new ImageLoadEventArgs(path));
            }
        }

        #endregion
        
        #region IImageManager4

        public void DropObjectCache()
        {
            _objectCache.Clear();
        }

        /// <summary>
        /// true if object caching is enabled
        /// </summary>
        public bool CacheObjects
        {
            get => _cacheObjects;
            set
            {
                if (_cacheObjects == value) return;
                bool oldValue = _cacheObjects;
                _cacheObjects = value;
                if (oldValue && !value)
                {
                    // turned off
                    DropObjectCache();;
                }
            }
        }

        #endregion
    }
}