using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace MangMana.Helpers
{
    internal static class ImageHelpers
    {
        public static BitmapImage CreateBitmapImage(string imagePath, int width = 0, int height = 0)
        {
            var img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri(imagePath, UriKind.RelativeOrAbsolute);
            if (width > 0) img.DecodePixelWidth = width;
            if (height > 0) img.DecodePixelWidth = height;
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.EndInit();
            img.Freeze();
            return img;
        }

        public static void SaveImage(BitmapSource img, string location)
        {
            if (File.Exists(location))
                File.Delete(location);
            using var filestream = File.Create(location);

            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(img));
            encoder.Save(filestream);
        }
    }
}