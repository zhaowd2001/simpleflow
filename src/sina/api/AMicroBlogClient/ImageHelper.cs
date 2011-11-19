using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Reflection;
using System.Net;

namespace AMicroblogAPISample
{
    /// <summary>
    /// Provides helper method for manipulating image.
    /// </summary>
    public static class ImageHelper
    {
        static ImageHelper()
        {
            var savedImgFolder = StartupLocation + SavedImageFolder;
            if (!Directory.Exists(savedImgFolder))
                Directory.CreateDirectory(savedImgFolder);
        }

        private static string startupLocation;
        public static string StartupLocation
        {
            get
            {
                if (string.IsNullOrEmpty(startupLocation))
                {
                    var assembly = Assembly.GetExecutingAssembly();
                    var dir = System.IO.Path.GetDirectoryName(assembly.Location);
                    if (!dir.EndsWith(@"\"))
                        dir += @"\";

                    startupLocation = dir;
                }

                return startupLocation;
            }
        }

        /// <summary>
        /// Saves a image from its ImageSource to local "SavedImage" folder.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Save(BitmapImage source)
        {
            var uri = source.UriSource.AbsoluteUri;
            var fileName = System.IO.Path.GetFileName(uri);
            var targetFileLocation = SavedImageFolder + fileName;

            if (ExistsImage(uri))
                return targetFileLocation;

            var ext = System.IO.Path.GetExtension(fileName);

            BitmapEncoder enc = GetEncoderByExt(ext);
            enc.Frames.Add(BitmapFrame.Create(source));


            using (var stream = File.Open(targetFileLocation, FileMode.Create, FileAccess.Write))
            {
                enc.Save(stream);
            }

            return targetFileLocation;
        }

        /// <summary>
        /// Checks whether the specified image exists in local image folder.
        /// </summary>
        /// <param name="imageUri"></param>
        /// <returns></returns>
        public static bool ExistsImage(string imageUri)
        {
            var fileName = System.IO.Path.GetFileName(imageUri);
            var targetFileLocation = StartupLocation + SavedImageFolder + fileName;

            return File.Exists(targetFileLocation);            
        }

        /// <summary>
        /// Retrieves the full local location of the specified image.
        /// </summary>
        /// <remarks>If this file does not exists in local image folder, retrieves it from remote server.</remarks>
        /// <param name="imageUri"></param>
        /// <returns></returns>
        public static string GetImage(string imageUri)
        {
            var fileName = System.IO.Path.GetFileName(imageUri);
            var targetFileLocation = StartupLocation + SavedImageFolder + fileName;

            if (!ExistsImage(targetFileLocation))
            {
                Save(imageUri);
            }

            return targetFileLocation;
        }

        public static string Save(string imageUri)
        {
            var fileName = System.IO.Path.GetFileName(imageUri);
            var ext = System.IO.Path.GetExtension(fileName);
            var targetFileLocation = StartupLocation + SavedImageFolder + fileName;

            using (WebClient client = new WebClient())
            {
                client.DownloadFile(imageUri, targetFileLocation);
            }

            return targetFileLocation;
        }

        public const string SavedImageFolder = @"SavedImage\";

        private static BitmapEncoder GetEncoderByExt(string imgExt)
        {
            switch (imgExt.ToLowerInvariant())
            {
                case ".jpg":
                case ".jpeg":
                    return new JpegBitmapEncoder();
                case ".gif":
                    return new GifBitmapEncoder();
                case ".bmp":
                    return new BmpBitmapEncoder();
                case ".tiff":
                    return new TiffBitmapEncoder();
                case ".wmp":
                    return new WmpBitmapEncoder();

                default:
                    return new BmpBitmapEncoder();
            }
        }
    }
}
