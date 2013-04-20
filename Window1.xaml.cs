using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Collections.Generic;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media;
using System;
using System.Windows;

using System.Windows.Input;

namespace WpfRenderT
{
    public partial class Window1
    {
        public Window1()
        {
            this.InitializeComponent();

            //TestWpfImgUtils();            

            colorEff = new ColorManiPulatorEffect(@"WpfShaders\ShdColorEffect\ShdColorEfect.ps");
            colorEff.AddTo(ImageViewer1, ImageViewer1.Source as BitmapSource);

            TransformGroup tg = new TransformGroup();
            tg.Children.Add(new ScaleTransform());
            tg.Children.Add(new TranslateTransform());
            cvsDraw.RenderTransform = tg;

            AddColorSildersEvents();
            colorEff.SetColorParam(ColorManiPulatorEffect.ColorParams.Contrast, 0.2);


            btnDefaultVal.MouseDown += new MouseButtonEventHandler(btnDefaultVal_MouseDown);
            btnOpenImage.MouseDown += new MouseButtonEventHandler(btnOpenImage_MouseDown);
            btnSaveImage.MouseDown += new MouseButtonEventHandler(btnSaveImage_MouseDown);
            cvsDraw.MouseWheel += new MouseWheelEventHandler(cvsDraw_MouseWheel);

            center = new Point(cvsDraw.Width / 2 - 70, cvsDraw.Height / 2 - 70);
            AddFiguresMainEvents();
            AddGeneralFigManipulationEvents();

            AddPatternEvents();
        }

        private void TestWpfImgUtils()
        {
            DateTime timeBefore = DateTime.Now;

            ImageViewer1.Source = WpfImgUtils.SubSample(ImageViewer1.Source as BitmapSource, 36);

            double msTimeAfter = DateTime.Now.Subtract(timeBefore).TotalMilliseconds;

            lblInfo.Content = "Decode+Subsaple+Convert time : " + msTimeAfter.ToString();
        }

        void cvsDraw_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScaleTransform st = ((TransformGroup)cvsDraw.RenderTransform).Children[0] as ScaleTransform;
            double zoom = e.Delta > 0 ? .1 : -.1;

            if (st.ScaleX + zoom > .4 && st.ScaleX + zoom < 10)
            {
                st.ScaleX += zoom;
                st.ScaleY += zoom;

                Point pt = e.GetPosition(cvsDraw);
                st.CenterX = pt.X;
                st.CenterY = pt.Y;
            }
            
        }

        Point center;

        #region Pattern 

        void AddPatternEvents()
        {

        }
        
        void NewPatternCreationRequested(Object obj, EventArgs eva)
        {

        }



        void sldPat_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        void UpdatePatSldValueLabels()
        {
            foreach (UIElement elem in stkPatSliders.Children)
            {
                ((elem as StackPanel).Children[2] as Label).Content =
                    ((elem as StackPanel).Children[1] as Slider).Value.ToString();
            }
        }

        void chkPat_Checked(object sender, RoutedEventArgs e)
        {
            foreach (UIElement elem in (chkPatSpot.Parent as StackPanel).Children)
            {
                if (elem is CheckBox && (bool)(elem as CheckBox).IsChecked && !elem.Equals(sender))
                {
                    (elem as CheckBox).IsChecked = false;
                }
            }

            e.Handled = true;
        }

        CheckBox GetCheckedPattern()
        {
            foreach (UIElement elem in (chkPatSpot.Parent as StackPanel).Children)
            {
                if (elem is CheckBox && (bool)(elem as CheckBox).IsChecked)
                {
                    return elem as CheckBox;
                }
            }
            return null;
        }


        #endregion

        #region Figures Main

        void AddFiguresMainEvents()
        {
            btnRect.MouseDown += new MouseButtonEventHandler(btnRect_MouseDown);
            btnExcEllipse.MouseDown += new MouseButtonEventHandler(btnExcEllipse_MouseDown);
            btnExcBarbell.MouseDown += new MouseButtonEventHandler(btnExcBarbell_MouseDown);
            btnLine.MouseDown += new MouseButtonEventHandler(btnLine_MouseDown);
            btnText.MouseDown += new MouseButtonEventHandler(btnText_MouseDown);
            btnEllipse.MouseDown += new MouseButtonEventHandler(btnEllipse_MouseDown);
        }

        void btnEllipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            /*
            //shpDraw.ShpFigAdd(center, new ShpFigAttrib(ShpFigType.ELLIPSE, ShpFigStyle.SIMPLE, 140, 140));
            ShpDBEllipseItem dummy = new ShpDBEllipseItem(null);
            dummy.setTopLeftX((int)center.X);
            dummy.setTopLeftY((int)center.Y);
            dummy.setWidth(140);
            dummy.setHeight(140);
            dummy.setFigureStyle((int)ShpFigStyle.NONE);

            shpDraw.ShpFigAdd(dummy);*/
        }

        void btnText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //shpDraw.ShpFigAdd(center, new ShpFigAttrib(ShpFigType.TEXT, ShpFigStyle.SIMPLE, 140, 140));
        }

        void btnLine_MouseDown(object sender, MouseButtonEventArgs e)
        {
            /*
            //shpDraw.ShpFigAdd(center, new ShpFigAttrib(ShpFigType.LINE, ShpFigStyle.SIMPLE, 140, 140));
            ShpDBLineItem dummy = new ShpDBLineItem(null);
            dummy.setPoint1X((int)center.X);
            dummy.setPoint1Y((int)center.Y);
            dummy.setPoint2X((int)center.X + 140);
            dummy.setPoint2Y(140);
            dummy.setFigureStyle((int)ShpFigStyle.NONE);

            shpDraw.ShpFigAdd(dummy);*/
        }

        void btnExcBarbell_MouseDown(object sender, MouseButtonEventArgs e)
        {
            /*
            Point barbellSt = new Point(center.X - 70, center.Y + 20);
            //shpDraw.ShpFigAdd(barbellSt, new ShpFigAttrib(ShpFigType.BARBELL, ShpFigStyle.SIMPLE, 280, 100));
            ShpDBBarbellItem dummy = new ShpDBBarbellItem(null);
            dummy.setLeft((int)barbellSt.X);
            dummy.setTop((int)barbellSt.Y);
            dummy.setWidth(80);
            dummy.setHeight(80);

            dummy.setLeft1((int)barbellSt.X + 200);
            dummy.setTop1((int)barbellSt.Y);
            dummy.setWidth1(80);
            dummy.setHeight1(80);

            dummy.setDistance(30);

            dummy.setFigureStyle((int)ShpFigStyle.NONE);

            shpDraw.ShpFigAdd(dummy);*/
        }

        void btnExcEllipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            /*
            //for (int i = 0; i < 10000; i++)
            {
                //shpDraw.ShpFigAdd(center, new ShpFigAttrib(ShpFigType.ELLIPSE, ShpFigStyle.CROSSEDLINEDRAW, 140, 140));
                ShpDBEllipseItem dummy = new ShpDBEllipseItem(null);
                dummy.setTopLeftX((int)center.X);
                dummy.setTopLeftY((int)center.Y);
                dummy.setWidth(140);
                dummy.setHeight(140);
                dummy.setFigureStyle((int)ShpFigStyle.CROSSEDLINEDRAW);

                shpDraw.ShpFigAdd(dummy);
            }*/
        }

        void btnRect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            /*
            //for (int i = 0; i < 10000; i++)
            {
                ShpDBRectangleItem dummy = new ShpDBRectangleItem(null);
                dummy.setTopLeftX((int)center.X);
                dummy.setTopLeftY((int)center.Y);
                dummy.setWidth(140);
                dummy.setHeight(140);
                dummy.setFigureStyle((int)ShpFigStyle.NONE);

                shpDraw.ShpFigAdd(dummy);
            }*/
        }

        #endregion

        #region GeneralFigManipulation

        void AddGeneralFigManipulationEvents()
        {
            this.btnDelActiveFig.MouseDown += new MouseButtonEventHandler(btnDelActiveFig_MouseDown);
            this.btnDeleteAllFig.MouseDown += new MouseButtonEventHandler(btnDeleteAllFig_MouseDown);
            this.btnColor.MouseDown += new MouseButtonEventHandler(btnColor_MouseDown);
            this.sldLineThick.ValueChanged += new RoutedPropertyChangedEventHandler<double>(sldLineThick_ValueChanged);
            this.chkFigProperties.Checked += new RoutedEventHandler(chkFigProperties_Checked);
            this.chkFigProperties.Unchecked += new RoutedEventHandler(chkFigProperties_Unchecked);
        }

        void chkFigProperties_Unchecked(object sender, RoutedEventArgs e)
        {
        }

        void chkFigProperties_Checked(object sender, RoutedEventArgs e)
        {
        }

        void sldLineThick_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }

        void btnColor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Forms.ColorDialog cd = new System.Windows.Forms.ColorDialog();
            cd.ShowDialog();

            SolidColorBrush brush = new SolidColorBrush(
                Color.FromArgb(cd.Color.A, cd.Color.R, cd.Color.G, cd.Color.B));
            //shpDraw.ShpFigSetColorSelected(brush);
        }

        void btnDeleteAllFig_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        void btnDelActiveFig_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        #endregion

        #region Color Manipulation

        ColorManiPulatorEffect colorEff;

        void AddColorSildersEvents()
        {
            sldGainR.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(sldGainR_ValueChanged);
            sldGainG.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(sldGainG_ValueChanged);
            sldGainB.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(sldGainB_ValueChanged);
            sldSaturation.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(sldSaturation_ValueChanged);
            sldBrightness.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(sldBrightness_ValueChanged);
            sldContrast.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(sldContrast_ValueChanged);
            sldGamma.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(sldGamma_ValueChanged);
        }

        void sldGainR_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            colorEff.SetColorParam(ColorManiPulatorEffect.ColorParams.GainR, e.NewValue);
            lblInfo.Content = e.NewValue.ToString();
        }

        void sldGainG_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            colorEff.SetColorParam(ColorManiPulatorEffect.ColorParams.GainG, e.NewValue);
            lblInfo.Content = e.NewValue.ToString();
        }

        void sldGainB_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            colorEff.SetColorParam(ColorManiPulatorEffect.ColorParams.GainB, e.NewValue);
            lblInfo.Content = e.NewValue.ToString();
        }

        void sldSaturation_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            colorEff.SetColorParam(ColorManiPulatorEffect.ColorParams.Saturation, e.NewValue);
            lblInfo.Content = e.NewValue.ToString();
        }

        void sldBrightness_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            colorEff.SetColorParam(ColorManiPulatorEffect.ColorParams.Brightness, e.NewValue);
            lblInfo.Content = e.NewValue.ToString();
        }

        void sldContrast_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            colorEff.SetColorParam(ColorManiPulatorEffect.ColorParams.Contrast, e.NewValue);
            lblInfo.Content = e.NewValue.ToString();
        }

        void sldGamma_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            colorEff.SetColorParam(ColorManiPulatorEffect.ColorParams.Gamma, e.NewValue);
            lblInfo.Content = e.NewValue.ToString();
        }

        void btnDefaultVal_MouseDown(object sender, MouseButtonEventArgs e)
        {
            foreach (object child in stackColorControls.Children)
            {
                if (child is StackPanel &&
                    (child as StackPanel).Children != null &&
                    (child as StackPanel).Children.Count == 2 &&
                    (child as StackPanel).Children[1] is Slider)
                {
                    Slider sld = (child as StackPanel).Children[1] as Slider;

                    sld.Value = (sld.Minimum + sld.Maximum) / 2;
                }
            }
        }

        #endregion

        void btnSaveImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Stream myStream = null;
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.InitialDirectory = "c:\\";
            saveFileDialog.Filter = "All files (*.*)|*.*";
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.FileName = "Image.bmp";

            if ((bool)saveFileDialog.ShowDialog())
            {
                try
                {
                    if ((myStream = saveFileDialog.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                            encoder.Frames.Add(BitmapFrame.Create(colorEff.GetShadedImage(ImageViewer1.Source as BitmapSource)));
                            encoder.Save(myStream);
                        }
                    }
                }
                catch
                {
                    //MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        void btnOpenImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if ((bool)openFileDialog.ShowDialog())
            {
                try
                {
                    if ((myStream = openFileDialog.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            BitmapImage bi = new BitmapImage();
                            bi.BeginInit();
                            bi.CacheOption = BitmapCacheOption.OnLoad;
                            bi.UriSource = new Uri(openFileDialog.FileName);
                            bi.EndInit();
                            ImageViewer1.Source = bi;

                            colorEff.UpdateRGBAverage(ImageViewer1.Source as BitmapSource);
                        }
                    }
                }
                catch
                {
                    //MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

    }
}