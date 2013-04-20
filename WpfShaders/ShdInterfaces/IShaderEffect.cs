
using System.Windows.Media.Imaging;

namespace WpfRenderT
{
    public interface IShaderEffect
    {
        void UpdateRGBAverage(BitmapSource averageImg);
        void SetColorParam(WpfRenderT.ColorManiPulatorEffect.ColorParams param, double value);
        BitmapSource GetShadedImage(BitmapSource image);
    }
}
