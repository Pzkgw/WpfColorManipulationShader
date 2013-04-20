using System;
using System.Windows.Media.Imaging;
using System.Windows;

namespace WpfRenderT
{


    internal enum Interpolation
    {
        /// <summary>
        /// The nearest neighbor algorithm simply selects the color of the nearest pixel.
        /// </summary>
        NearestNeighbor = 0,

        /// <summary>
        /// Linear interpolation in 2D using the average of 3 neighboring pixels.
        /// </summary>
        Bilinear,
    }


    class WpfImgUtilsTransform
    {
        internal static WriteableBitmap Resize(BitmapSource bmp, int width, int height, Interpolation interpolation)
        {
            // Init vars
            Int32[] ps = WpfImgUtilsEncoding.GetEncodedImageDataInt(bmp);
            int ws = bmp.PixelWidth;
            int hs = bmp.PixelHeight;

            WriteableBitmap result = new WriteableBitmap(width, height, 96, 96, bmp.Format, null);
            Int32[] pd = WpfImgUtilsEncoding.GetEncodedImageDataInt(result);
            float xs = (float)ws / width;
            float ys = (float)hs / height;

            float fracx, fracy, ifracx, ifracy, sx, sy, l0, l1;
            int c, x0, x1, y0, y1;
            byte c1a, c1r, c1g, c1b, c2a, c2r, c2g, c2b, c3a, c3r, c3g, c3b, c4a, c4r, c4g, c4b;
            byte a = 0, r = 0, g = 0, b = 0;

            // Nearest Neighbor
            if (interpolation == Interpolation.NearestNeighbor)
            {
                int srcIdx = 0;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        sx = x * xs;
                        sy = y * ys;
                        x0 = (int)sx;
                        y0 = (int)sy;

                        pd[srcIdx++] = ps[y0 * ws + x0];
                    }
                }
            }

               // Bilinear
            else if (interpolation == Interpolation.Bilinear)
            {
                int srcIdx = 0;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        sx = x * xs;
                        sy = y * ys;
                        x0 = (int)sx;
                        y0 = (int)sy;

                        // Calculate coordinates of the 4 interpolation points
                        fracx = sx - x0;
                        fracy = sy - y0;
                        ifracx = 1f - fracx;
                        ifracy = 1f - fracy;
                        x1 = x0 + 1;
                        if (x1 >= ws)
                            x1 = x0;
                        y1 = y0 + 1;
                        if (y1 >= hs)
                            y1 = y0;


                        // Read source color
                        c = ps[y0 * ws + x0];
                        c1a = (byte)(c >> 24);
                        c1r = (byte)(c >> 16);
                        c1g = (byte)(c >> 8);
                        c1b = (byte)(c);

                        c = ps[y0 * ws + x1];
                        c2a = (byte)(c >> 24);
                        c2r = (byte)(c >> 16);
                        c2g = (byte)(c >> 8);
                        c2b = (byte)(c);

                        c = ps[y1 * ws + x0];
                        c3a = (byte)(c >> 24);
                        c3r = (byte)(c >> 16);
                        c3g = (byte)(c >> 8);
                        c3b = (byte)(c);

                        c = ps[y1 * ws + x1];
                        c4a = (byte)(c >> 24);
                        c4r = (byte)(c >> 16);
                        c4g = (byte)(c >> 8);
                        c4b = (byte)(c);


                        // Calculate colors
                        // Alpha
                        l0 = ifracx * c1a + fracx * c2a;
                        l1 = ifracx * c3a + fracx * c4a;
                        a = (byte)(ifracy * l0 + fracy * l1);

                        if (a > 0)
                        {
                            // Red
                            l0 = ifracx * c1r * c1a + fracx * c2r * c2a;
                            l1 = ifracx * c3r * c3a + fracx * c4r * c4a;
                            r = (byte)((ifracy * l0 + fracy * l1) / a);

                            // Green
                            l0 = ifracx * c1g * c1a + fracx * c2g * c2a;
                            l1 = ifracx * c3g * c3a + fracx * c4g * c4a;
                            g = (byte)((ifracy * l0 + fracy * l1) / a);

                            // Blue
                            l0 = ifracx * c1b * c1a + fracx * c2b * c2a;
                            l1 = ifracx * c3b * c3a + fracx * c4b * c4a;
                            b = (byte)((ifracy * l0 + fracy * l1) / a);
                        }

                        // Write destination
                        pd[srcIdx++] = (a << 24) | (r << 16) | (g << 8) | b;
                    }
                }
            }

            result.WritePixels(new Int32Rect(0, 0, result.PixelWidth, result.PixelHeight),
                pd, result.PixelWidth * result.Format.BitsPerPixel / 8, 0, 0);

            return result;
        }

    }
}
