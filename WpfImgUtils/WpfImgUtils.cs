using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Media;

namespace WpfRenderT
{
    class WpfImgUtils
    {



        public static BitmapSource SubSample(BitmapSource bmp, int fac)
        {
            BitmapSource retVal = null;

            int iWidth = (int)bmp.PixelWidth;
            int iHeight = (int)bmp.PixelHeight;
            int factor = fac;
            int tWidth = iWidth / factor;
            int tHeight = iHeight / factor;
            int bpp = ((bmp.Format.BitsPerPixel + 7) / 8);
            int iStride = iWidth * bpp;
            int tStride = iStride / factor;
            int dpi = 96;

            byte[] imgBuffer = WpfImgUtilsEncoding.GetEncodedImageData(bmp);
            byte[] thumbnailBuffer = new byte[tWidth * tHeight * bpp];


            int imgBufferOff = 0, thumbnailBufferOff = 0;

            //subsample image (same code as RSA)
            for (int i = 0; i < tHeight; i++)
            {
                for (int j = 0; j < tWidth * bpp; j += bpp)
                {
                    if (bpp > 0)
                    {
                        thumbnailBuffer[thumbnailBufferOff + j] = imgBuffer[imgBufferOff + j * factor];
                        if (bpp > 1)
                        {
                            thumbnailBuffer[thumbnailBufferOff + j + 1] = imgBuffer[imgBufferOff + j * factor + 1];
                            if (bpp > 2)
                            {
                                thumbnailBuffer[thumbnailBufferOff + j + 2] = imgBuffer[imgBufferOff + j * factor + 2];
                                if (bpp > 3)
                                {
                                    thumbnailBuffer[thumbnailBufferOff + j + 3] = imgBuffer[imgBufferOff + j * factor + 3];
                                }
                            }
                        }
                    }
                }

                imgBufferOff += iWidth * factor * bpp;
                thumbnailBufferOff += tWidth * bpp;
            }

            //for (int i = 0; i < tWidth; i++)
            //{
            //    for (int j = 0; j < tHeight * bpp; j++)
            //    {
            //        int srcOff = imgBufferOff + j * factor * bpp;
            //        int dstOff = thumbnailBufferOff + j * bpp;
            //        if (srcOff + bpp < imgBuffer.Length && dstOff + bpp < thumbnailBuffer.Length)
            //        {
            //            Buffer.BlockCopy(imgBuffer, srcOff, thumbnailBuffer, dstOff, bpp);
            //        }
            //    }

            //    imgBufferOff += iWidth * bpp * factor;
            //    thumbnailBufferOff += tWidth * bpp;
            //}


            WriteableBitmap bs = new WriteableBitmap(tWidth, tHeight, dpi, dpi, bmp.Format, null);
            bs.WritePixels(new Int32Rect(0, 0, tWidth, tHeight),
                thumbnailBuffer, tWidth * bpp, 0, 0);
            bs.Freeze();


            retVal = bs;

            return retVal;
        }

        public static BitmapSource SubSampleSmooth(BitmapSource bmp, int fac)
        {
            BitmapSource retVal = null;

            int iWidth = (int)bmp.PixelWidth;
            int iHeight = (int)bmp.PixelHeight;
            int factor = fac;
            int tWidth = iWidth / factor;
            int tHeight = iHeight / factor;
            int bpp = ((bmp.Format.BitsPerPixel + 7) / 8);
            int iStride = iWidth * bpp;
            int tStride = iStride / factor;
            int dpi = 96;

            byte[] imgBuffer = WpfImgUtilsEncoding.GetEncodedImageData(bmp);
            byte[] thumbnailBuffer = new byte[tWidth * tHeight * bpp];          



            WriteableBitmap bs = new WriteableBitmap(iWidth, iHeight, dpi, dpi, bmp.Format, null);
            bs.WritePixels(new Int32Rect(0, 0, iWidth, iHeight),
                imgBuffer, iWidth * bpp, 0, 0);


            //bs = WpfImgUtilsTransform.Resize(bmp, bmp.PixelWidth / factor, bmp.PixelHeight / factor, Interpolation.Bilinear);
            //retVal = bs;

            retVal = new TransformedBitmap(bs, new ScaleTransform(1.0 / factor, 1.0 / factor));

            bs.Freeze();

            return retVal;
        }

    }
}
