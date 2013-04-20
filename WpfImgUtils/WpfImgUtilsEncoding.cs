using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;

namespace WpfRenderT
{
    class WpfImgUtilsEncoding
    {
        internal static byte[] GetEncodedImageData(BitmapSource bmp)
        {
            int width = bmp.PixelWidth;
            int height = bmp.PixelHeight;
            int stride = width * ((bmp.Format.BitsPerPixel + 7) / 8);

            byte[] pixels = new byte[height * stride];

            bmp.CopyPixels(pixels, stride, 0);

            return pixels;
        }

        internal static int[] GetEncodedImageDataInt(BitmapSource bmp)
        {
            int width = bmp.PixelWidth;
            int height = bmp.PixelHeight;
            int stride = width * ((bmp.Format.BitsPerPixel + 7) / 8);

            int[] pixels = new int[height * stride / 4];

            bmp.CopyPixels(pixels, stride, 0);

            return pixels;
        }
    }
}
