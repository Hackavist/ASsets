using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Assets.Helpers
{
    public static class ImageHelpers
    {
        public static BitmapImage BitmapFromBase64String(string base64Image)
        {
            BitmapImage bi = new BitmapImage();
            byte[] binaryData = Convert.FromBase64String(base64Image);
            bi.BeginInit();
            bi.StreamSource = new MemoryStream(binaryData);
            bi.EndInit();
            return bi;
        }
    }
}