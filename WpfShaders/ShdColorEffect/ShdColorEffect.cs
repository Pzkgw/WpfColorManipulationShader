using System.Windows.Controls;
namespace WpfRenderT
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Effects;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;

    // ----------------------------------------------------------------------------
    //
    //      Author: Bogdan Visoiu
    //
    // ColorManiPulatorEffect is a c# wrapper over a compiled shader (ps file).
    // It provides a way to modify color parameters (RGB gains, saturation ..) for a given input image.
    // The ps file is the result of a fx file(shader) compilation
    //
    // Steps for fx compile :
    // 1.Install DxSdk
    // 2.in Visual Studion\Tools\External Tools add:
    // Title : Some name 
    // Command : Path to fxc.exe(fx compiler) in DxSdk instalation
    // Arguments : /DWPF /Od /T ps_2_0 /E $(ItemFileName)PS /Fo$(ProjectDir)$(ItemFileName).ps $(ItemPath)
    // Initial directory : $(TargetDir)
    // Uncheck "Close on exit" to make sure output of fx compilation is visible
    // Add fx file to solution explorer and select it , and use the new added tool
    //
    // Resulted ps file must be added to solution and set "Build action" to Resource(!!!)    
    public class ColorManiPulatorEffect : ShaderEffect, IShaderEffect
    {
        public enum ColorParams
        {
            GainR,
            GainG,
            GainB,
            Saturation,
            Brightness,
            Contrast,
            Gamma
        }

        public ColorManiPulatorEffect(string psFileLocation)
        {
            pixelShader = new PixelShader();
            pixelShader.UriSource = new Uri(psFileLocation, UriKind.RelativeOrAbsolute);
            pixelShader.Freeze();

            Array values = Enum.GetValues(typeof(ColorParams));

            clrParams = new double[values.Length];
            averageRGB = new double[3] { 127.5, 127.5, 127.5 };
            wBmp = new WriteableBitmap(256, 1, 96, 96, PixelFormats.Rgb24, null);

            this.PixelShader = pixelShader;

            UpdateShaderValue(InputProperty);
            UpdateShaderValue(LutColorProperty);
            UpdateShaderValue(SaturationProperty);

            SetDefaultValues();
        }

        ~ColorManiPulatorEffect()
        {

        }

        private PixelShader pixelShader;

        double[] averageRGB;
        double[] clrParams;
        WriteableBitmap wBmp;

        #region Interface

        public BitmapSource GetShadedImage(BitmapSource image)
        {
            return ApplyToImage(image);
        }

        public void AddTo(UIElement elem)
        {
            AddTo(elem, null, 127.5, 127.5, 127.5);
        }

        public void AddTo(UIElement elem, double averageR, double averageG, double averageB)
        {
            AddTo(elem, null, averageR, averageG, averageB);
        }

        public void AddTo(UIElement elem, BitmapSource averageImg)
        {
            AddTo(elem, averageImg, 127.5, 127.5, 127.5);
        }

        public void UpdateRGBAverage(BitmapSource averageImg)
        {
            UpdateAverage(averageImg, 127.5, 127.5, 127.5);
        }

        public void UpdateRGBAverage(double averageR, double averageG, double averageB)
        {
            UpdateAverage(null, averageR, averageG, averageB);
        }

        public void SetColorParam(ColorParams param, double value)
        {
            clrParams[(int)param] = value;

            if (param == ColorParams.Saturation)
            {
                Saturation = clrParams[(int)ColorParams.Saturation];
            }
            else
            {
                UpdateColorLut();
            }
        }

        #endregion
        void SetDefaultValues()
        {
            clrParams[(int)ColorParams.GainR] = 1;
            clrParams[(int)ColorParams.GainG] = 1;
            clrParams[(int)ColorParams.GainB] = 1;

            clrParams[(int)ColorParams.Brightness] = 0;
            clrParams[(int)ColorParams.Contrast] = 1;
            clrParams[(int)ColorParams.Gamma] = 1;
            clrParams[(int)ColorParams.Saturation] = 1;

            averageRGB[0] = 255 / 2.0f;
            averageRGB[1] = 255 / 2.0f;
            averageRGB[2] = 255 / 2.0f;
        }

        BitmapSource ApplyToImage(BitmapSource image)
        {
            Rectangle r = new Rectangle();
            r.Fill = new ImageBrush(image);
            r.Effect = this;

            Size sz = new Size(image.PixelWidth, image.PixelHeight);
            r.Measure(sz);
            r.Arrange(new Rect(sz));

            RenderTargetBitmap rtb = new RenderTargetBitmap(image.PixelWidth, image.PixelHeight, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(r);
            return rtb;
        }

        private void AddTo(UIElement elem, BitmapSource averageImg, double averageR, double averageG, double averageB)
        {
            UpdateAverage(averageImg, averageR, averageG, averageB);
            elem.Effect = this;
        }

        private void UpdateAverage(BitmapSource averageImg, double averageR, double averageG, double averageB)
        {
            if (averageImg != null)
            {
                CalculateAverage(averageImg, ref averageR, ref averageG, ref averageB);
            }

            averageRGB[0] = averageR;
            averageRGB[1] = averageG;
            averageRGB[2] = averageB;
            UpdateColorLut();
        }

        //0-255 retValues
        void CalculateAverage(BitmapSource bmp, ref double averageR, ref double averageG, ref double averageB)
        {
            if (bmp != null)
            {
                //get image buffer
                int height = bmp.PixelHeight;
                int width = bmp.PixelWidth;
                int stride = width * ((bmp.Format.BitsPerPixel + 7) / 8);

                byte[] bits = new byte[height * stride];
                bmp.CopyPixels(bits, stride, 0);

                //calculate average
                int i, j;
                double sumR = 0, sumG = 0, sumB = 0;
                for (j = 0; j < height; j++)
                {
                    for (i = 0; i < width * 3; i += 3)
                    {
                        sumR += bits[i + j * width * 3];
                        sumG += bits[(i + 1) + j * width * 3];
                        sumB += bits[(i + 2) + j * width * 3];
                    }
                }

                if (sumR > 0 && sumG > 0 && sumB > 0)
                {
                    averageR = (sumR / (width * height));
                    averageG = (sumG / (width * height));
                    averageB = (sumB / (width * height));
                }

            }

        }


        void UpdateColorLut()
        {
            // ---------  Lut calculation -----------
            byte[] buff = new byte[768];

            for (int i = 0; i < 768; i += 3)
            {
                double valR, valG, valB, inP = i / 3.0f;

                //Apply Gamma
                inP = Math.Pow(inP / 255, 1 / clrParams[(int)ColorParams.Gamma]);
                inP *= 255;
                //Apply Brightness
                inP += clrParams[(int)ColorParams.Brightness];
                //Apply RBG gains
                valR = inP * clrParams[(int)ColorParams.GainR];
                valG = inP * clrParams[(int)ColorParams.GainG];
                valB = inP * clrParams[(int)ColorParams.GainB];

                //Apply Contrast
                valR -= averageRGB[0];
                valG -= averageRGB[1];
                valB -= averageRGB[2];

                valR *= clrParams[(int)ColorParams.Contrast];
                valG *= clrParams[(int)ColorParams.Contrast];
                valB *= clrParams[(int)ColorParams.Contrast];

                valR += averageRGB[0];
                valG += averageRGB[1];
                valB += averageRGB[2];

                //Clamp and apply values
                buff[i] = Clamp(valR);
                buff[i + 1] = Clamp(valG);
                buff[i + 2] = Clamp(valB);
            }

            // ---------  Lut upload to grapics board -----------

            wBmp.WritePixels(new Int32Rect(0, 0,
                wBmp.PixelWidth, wBmp.PixelHeight),
                buff, wBmp.PixelWidth * wBmp.Format.BitsPerPixel / 8, 0);

            LutColor = new ImageBrush(wBmp);
        }

        byte Clamp(double val)
        {
            if (val > 255)
            {
                val = 255;
            }

            if (val < 0)
            {
                val = 0;
            }

            return (byte)val;
        }


        #region Uniform Variables

        static readonly DependencyProperty InputProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty("InputP",
            typeof(ColorManiPulatorEffect), 0 /* assigned to sampler register S0 */);

        Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }


        static readonly DependencyProperty LutColorProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty("LutColor",
            typeof(ColorManiPulatorEffect), 1 /* assigned to sampler register S1 */);

        Brush LutColor
        {
            get { return (Brush)GetValue(LutColorProperty); }
            set { SetValue(LutColorProperty, value); }
        }

        static readonly DependencyProperty SaturationProperty =
            DependencyProperty.Register("Saturation", typeof(double), typeof(ColorManiPulatorEffect),
            new UIPropertyMetadata(1.0, PixelShaderConstantCallback(0))/* assigned to sampler register C0 */);

        double Saturation
        {
            get { return (double)GetValue(SaturationProperty); }
            set { SetValue(SaturationProperty, value); }
        }

        #endregion
    }
}
