using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Rasterization.Engine;
using System.Drawing;
using Point = System.Drawing.Point;
using Color = System.Drawing.Color;
using System.Windows.Controls.Primitives;
using Rasterization.UI.ConversionExtension;
using Microsoft.Win32;
using System.IO;
using System.Xml.Serialization;
using Rectangle = Rasterization.Engine.Rectangle;

namespace Rasterization.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    //TODO: CLEAN THIS UP WHEN U HAVE TIME IT GOT VERY MESSY. YOU WILL USE IT FOR PROJ 4 
    public partial class MainWindow : Window
    {
        List<Point> CachedPoints = new List<Point>();
        IGraphicsEngine GE;
        List<IDrawable> Drawables = new List<IDrawable>();
        Color tempColor = Color.FromArgb(0, 0, 0);
        Color oldc = Color.FromArgb(250, 250, 250, 250);
        Point MousePointRegister;

        IDrawable SelectedDrawable;
        public MainWindow()
        {
            InitializeComponent();

        }


        private void ToolSetClick(object sender, RoutedEventArgs e)
        {
            LineDrawingButton.IsChecked = false;
            GrabToolButton.IsChecked = false;
            CircleDrawingButton.IsChecked = false;
            PolygonDrawingButton.IsChecked = false;
            ArcDrawingButton.IsChecked = false;
            PolyFill.IsChecked = false;
            RectangleDrawingButton.IsChecked = false;
            //WuLineDrawingButton.IsChecked = false;
            //WuCircleDrawingButton.IsChecked = false;
            try
            {
                var btn = (ToggleButton)sender;
                btn.IsChecked = true;
                if (SelectedDrawable != null)
                {
                    SelectedDrawable.Erase(GE);
                    SelectedDrawable.Draw(GE);
                    SelectedDrawable = null;
                }

            }
            catch { }
        }

        private void ToolSetUnClick(object sender, RoutedEventArgs e)
        {
            CachedPoints.Clear();
        }

        private void ImageCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(ImageCanvas);
            var pointWithInts = new Point((int)p.X, (int)p.Y);
            //used when mouse is moved
            try
            {
                if ((bool)LineDrawingButton.IsChecked)
                {
                    CachedPoints.Add(pointWithInts);

                    if (CachedPoints.Count == 2)
                    {
                        Line line = new Line(CachedPoints[0], CachedPoints[1], ((System.Windows.Media.Color)ColorPicker.SelectedColor).ToColor(), (int)BrushInput.Value -1);
                        CachedPoints.Clear();
                        line.CalculatePoints();
                        Drawables.Add(line);
                        line.Draw(GE);

                    }
                }
                //if ((bool)WuLineDrawingButton.IsChecked)
                //{
                //    CachedPoints.Add(pointWithInts);

                //    if (CachedPoints.Count == 2)
                //    {
                //        Line line = new Line(CachedPoints[0], CachedPoints[1], ((System.Windows.Media.Color)ColorPicker.SelectedColor).ToColor(), (int)BrushInput.Value);
                //        CachedPoints.Clear();
                //        Drawables.Add(line);
                //        line.DrawAA(GE);

                //    }
                //}
                if ((bool)CircleDrawingButton.IsChecked)
                {
                    CachedPoints.Add(pointWithInts);

                    if (CachedPoints.Count == 2)
                    {
                        Circle circle = new Circle(CachedPoints[0], CachedPoints[1], ((System.Windows.Media.Color)ColorPicker.SelectedColor).ToColor(), (int)BrushInput.Value -1);
                        CachedPoints.Clear();
                        circle.CalculatePoints();
                        Drawables.Add(circle);
                        circle.Draw(GE);

                    }
                }
                if((bool)PolyFill.IsChecked)
                {
                    CachedPoints.Add(pointWithInts);

                    if (CachedPoints.Count == 1)
                    {
                       GE.Bitmap.boundaryFill4(CachedPoints[0].X,CachedPoints[0].Y, ((System.Windows.Media.Color)ColorPicker.SelectedColor).ToColor(), ((System.Windows.Media.Color)ColorPicker2.SelectedColor).ToColor(), (bool)eightcon.IsChecked);
                        CachedPoints.Clear();
                        PolyFill.IsChecked = false;
                        
                    }

                }
                //if ((bool)WuCircleDrawingButton.IsChecked)
                //{
                //    CachedPoints.Add(pointWithInts);

                //    if (CachedPoints.Count == 2)
                //    {
                //        Circle circle = new Circle(CachedPoints[0], CachedPoints[1], ((System.Windows.Media.Color)ColorPicker.SelectedColor).ToColor(), (int)BrushInput.Value);
                //        CachedPoints.Clear();
                //        Drawables.Add(circle);
                //        circle.CalculatePoints();
                //        circle.DrawAA(GE);

                //    }
                //}
                if ((bool)PolygonDrawingButton.IsChecked)
                {
                    var initialPoint = CachedPoints.FirstOrDefault();
                    if (((int)Math.Sqrt(Math.Pow(initialPoint.X - pointWithInts.X, 2) + Math.Pow(initialPoint.Y - pointWithInts.Y, 2)) < 50))
                    {
                        //CachedPoints.Add(CachedPoints.First());
                        Polygon polygon = new Polygon(CachedPoints, ((System.Windows.Media.Color)ColorPicker.SelectedColor).ToColor(), (int)BrushInput.Value -1);
                        polygon.CalculatePoints();
                        Drawables.Add(polygon);
                        polygon.Draw(GE);
                        CachedPoints.Clear();
                    }
                    else
                        CachedPoints.Add(pointWithInts);

                }
                if ((bool)RectangleDrawingButton.IsChecked)
                {
                    CachedPoints.Add(pointWithInts);

                    if (CachedPoints.Count == 2)
                    {
                        Rectangle rec = new Rectangle(CachedPoints[0], CachedPoints[1]);
                        CachedPoints.Clear();
                        rec.CalculatePoints();
                        Drawables.Add(rec);
                        rec.Draw(GE);

                    }
                }
                if ((bool)GrabToolButton.IsChecked)
                {
                    foreach (var drawable in Drawables)
                    {
                        if (drawable.HitTest(pointWithInts))
                        {
                            if (SelectedDrawable != null)
                            {
                                SelectedDrawable.Erase(GE);
                                SelectedDrawable.Draw(GE);
                            }
                            SelectedDrawable = drawable;
                            SelectedDrawable.IndicateSelection(GE);
                            //MessageBox.Show("Found Something");
                        }
                    }
                }

                GC.Collect();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                MousePointRegister = pointWithInts;
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PixelFormat pf = PixelFormats.Bgra32;
            int width = 1200 / 2;
            int height = 1200 / 2;
            int rawStride = (width * pf.BitsPerPixel + 7) / 8;
            byte[] rawImage = new byte[rawStride * height];
            rawImage = Enumerable.Repeat((byte)250, rawImage.Length).ToArray();

            BitmapSource bitmap = BitmapSource.Create(width, height,
                96, 96, pf, null,
                rawImage, rawStride);
            var writeable = new WriteableBitmap(bitmap);
            ImageCanvas.Source = writeable;
            GE = new GraphicsEngine(writeable);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedDrawable != null)
            {
                ToolSetClick(sender, e);

                SelectedDrawable.Erase(GE);
                Drawables.Remove(SelectedDrawable);
                SelectedDrawable = null;
            }
        }

        private void ImageCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var p = e.GetPosition(ImageCanvas);
            var pointWithInts = new Point((int)p.X, (int)p.Y);
            if (SelectedDrawable != null && e.LeftButton == MouseButtonState.Pressed && SelectedDrawable.StretchablePoints.Any(point => Math.Sqrt(Math.Pow(pointWithInts.X - point.X, 2) + Math.Pow(pointWithInts.Y - point.Y, 2)) < 25))
            {
                var min = 35000;
                int closestPointIdx = 0;
                foreach (var pt in SelectedDrawable.StretchablePoints)
                {
                    var minToCheck = (int)Math.Sqrt(Math.Pow(pointWithInts.X - pt.X, 2) + Math.Pow(pointWithInts.Y - pt.Y, 2));
                    var isNewMin = minToCheck < min;
                    if (isNewMin)
                    {
                        closestPointIdx = SelectedDrawable.StretchablePoints.IndexOf(pt);
                        min = minToCheck;
                    }
                }

                var dx = (pointWithInts.X - MousePointRegister.X);
                var dy = (pointWithInts.Y - MousePointRegister.Y);
                //dx *= dx / dy;
                //dy *= dx / dy;

                SelectedDrawable.Erase(GE);
                SelectedDrawable.Stretch(GE, dx, dy, closestPointIdx);
                Drawables.Where(x => x != SelectedDrawable).ToList().ForEach(x => x.Draw(GE));
                MousePointRegister = pointWithInts;

            }

            if (SelectedDrawable != null && e.RightButton == MouseButtonState.Pressed && SelectedDrawable.Points.Any(p => Math.Sqrt(Math.Pow(pointWithInts.X - p.X, 2) + Math.Pow(pointWithInts.Y - p.Y, 2)) < 25))
            {
                var min = 35000;
                int closestPointIdx = 0;
                foreach (var pt in SelectedDrawable.Points)
                {
                    var minToCheck = (int)Math.Sqrt(Math.Pow(pointWithInts.X - pt.X, 2) + Math.Pow(pointWithInts.Y - pt.Y, 2));
                    var isNewMin = minToCheck < min;
                    if (isNewMin)
                    {
                        closestPointIdx = SelectedDrawable.Points.IndexOf(pt);
                        min = minToCheck;
                    }
                }

                var dx = (pointWithInts.X - MousePointRegister.X);
                var dy = (pointWithInts.Y - MousePointRegister.Y);

                SelectedDrawable.Erase(GE);
                SelectedDrawable.Move(GE, dx, dy, closestPointIdx);
                MousePointRegister = pointWithInts;
                Drawables.Where(x => x != SelectedDrawable).ToList().ForEach(x => x.Draw(GE));

            }
        }

        private void ImageCanvas_ButtonDown(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(ImageCanvas);
            var pointWithInts = new Point((int)p.X, (int)p.Y);
            //used when mouse is moved
            MousePointRegister = pointWithInts;
        }

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            if (SelectedDrawable != null)
            {
                SelectedDrawable.Color = ((System.Windows.Media.Color)ColorPicker.SelectedColor).ToColor();
                SelectedDrawable.Brush.Color = ((System.Windows.Media.Color)ColorPicker.SelectedColor).ToColor();
                SelectedDrawable.IndicateSelection(GE);

            }
        }

        private void BrushInput_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (SelectedDrawable != null)
            {
                SelectedDrawable.Erase(GE);
                SelectedDrawable.Brush.Radius = (int)e.NewValue;
                SelectedDrawable.CalculatePoints();
                SelectedDrawable.IndicateSelection(GE);
            }
        }

        private void SuperSample_Click(object sender, RoutedEventArgs e)
        {

            PixelFormat pf = PixelFormats.Bgra32;
            int width3 = 1200;
            int height3 = 1200;
            int rawStride2 = (width3 * pf.BitsPerPixel + 7) / 8;
            byte[] rawImage2 = new byte[rawStride2 * height3];
            rawImage2 = Enumerable.Repeat((byte)240, rawImage2.Length).ToArray();

            BitmapSource bitmap = BitmapSource.Create(width3, height3,
                96, 96, pf, null,
                rawImage2, rawStride2);
            var writeable = new WriteableBitmap(bitmap);
            WriteableBitmap readBmp = (WriteableBitmap)ImageCanvas.Source;


            GE.Bitmap = writeable;
            foreach (var vector in Drawables)
            {

                vector.UpScale(GE);

            }



            int width = 1200 / 2;
            int height = 1200 / 2;
            int rawStride = (width * pf.BitsPerPixel + 7) / 8;
            byte[] rawImage = new byte[rawStride * height];
            rawImage = Enumerable.Repeat((byte)240, rawImage.Length).ToArray();

            BitmapSource bitmap2 = BitmapSource.Create(width, height,
                96, 96, pf, null,
                rawImage, rawStride);
            var writeable2 = new WriteableBitmap(bitmap2);
            readBmp = writeable.Clone();
            List<Point> pts = new();
            try
            {
                readBmp.Lock();
                writeable2.Lock();

                unsafe
                {
                    var width2 = readBmp.PixelWidth;
                    var height2 = readBmp.PixelHeight;

                    unsafe
                    {
                        for (int col = 0; col < 1200; col++)
                        {
                            for (int row = 0; row < 1200; row++)
                            {
                                Color pixelColor = Color.Empty;
                                List<int> redChannelValues = new();
                                List<int> blueChannelValues = new();
                                List<int> greenChannelValues = new();


                                for (int kernelCol = 0; kernelCol < 2; kernelCol++)
                                {
                                    for (int kernelRow = 0; kernelRow < 2; kernelRow++)
                                    {
                                        //TODO: If you have time after doing everything try MIRRORING for edges
                                        var pixelX = col + kernelCol - 1;
                                        var pixelY = row + kernelRow - 1;
                                        //pixelColor = writeableBitmap.GetPixel(pixelX, pixelY);

                                        //var kernelIntensity = kernel.Matrix[kernelRow, kernelCol];

                                        //Extend for out of bound ones

                                        if (pixelX < 0)
                                            pixelX = 0;

                                        if (pixelX >= readBmp.PixelWidth)
                                            pixelX = readBmp.PixelWidth - 1;

                                        if (pixelY < 0)
                                            pixelY = 0;

                                        if (pixelY >= readBmp.PixelHeight)
                                            pixelY = readBmp.PixelHeight - 1;


                                        pixelColor = readBmp.GetPixel(pixelX, pixelY);
                                        //Console.WriteLine($"Kernel({kernelCol},{kernelRow})*Bitmap({pixelX},{pixelY})");
                                        redChannelValues.Add(pixelColor.R);
                                        blueChannelValues.Add(pixelColor.B);
                                        greenChannelValues.Add(pixelColor.G);


                                    }
                                }

                                var red = redChannelValues.Average();
                                var blue = blueChannelValues.Average();
                                var green = greenChannelValues.Average();
                                //here get median
                                writeable2.SetPixel(col / 2, row / 2, Color.FromArgb((int)red, (int)green, (int)blue));
                            }
                        }
                    }

                }

            }
            finally
            {
                readBmp.Unlock();
                writeable2.Unlock();

            }

            ImageCanvas.Source = writeable2;
            GE.Bitmap = (WriteableBitmap)ImageCanvas.Source;

        }

        private void AntiAliasing_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var drawable in Drawables)
            {
                drawable.Erase(GE);
                drawable.CalculateAntiAliased(GE);
                drawable.IsAntiAliased = true;
            }
        }

        private void AntiAliasing_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (IDrawable drawable in Drawables)
            {
                drawable.IsAntiAliased = false;
                drawable.Erase(GE);
                drawable.Draw(GE);
            }
        }

        double currentScale = 1.0;
        private void ImageCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var position = e.MouseDevice.GetPosition(ImageCanvas);
            var renderTransformValue = ImageCanvas.RenderTransform.Value;
            if (e.Delta > 0)
            {
                currentScale += 0.1;
            }
            else if (e.Delta < 0)
            {
                currentScale -= 0.1;
                if (currentScale < 1.0)
                    currentScale = 1.0;
            }
            ImageCanvas.RenderTransform = new ScaleTransform(currentScale, currentScale, position.X, position.Y);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                OpenFileDialog openFileDialog = new OpenFileDialog();
                {
                    openFileDialog.Filter = "xml files (*.xml) | *.xml";
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.RestoreDirectory = true;
                    if (openFileDialog.ShowDialog(this) == true)
                    {
                        if (openFileDialog.FileName != "")
                        {
                            var stream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read);
                            var xmlSerializer = new XmlSerializer(new List<Serializable>().GetType());

                            //XmlDictionaryReader reader =
                            //    XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas());



                            ////RESET THE APP
                            //ProgressBar.Value = 0;
                            //stopwatch.Reset();
                            //timer.Stop();
                            //Canvas.Children.Clear();
                            //Circles.Clear();
                            //NewCircleLocation = new Point(Canvas.ActualWidth / 2, Canvas.ActualHeight / 2);

                            var items = (List<Serializable>)xmlSerializer.Deserialize(stream);
                            stream.Close();

                            foreach(var item in items)
                            {
                                switch (item.Name)
                                {
                                    case "Line":
                                        var ln = new Line();
                                        ln.Points = item.Points;
                                        ln.BasePoints = item.BasePoints;
                                        Drawables.Add(ln);
                                        break;
                                    case "Circle":
                                        var c = new Circle();
                                        c.Points = item.Points;
                                        c.BasePoints = item.BasePoints;
                                        Drawables.Add(c);
                                        break;
                                    case "Polygon":
                                        var p = new Polygon();
                                        p.Points = item.Points;
                                        p.BasePoints = item.BasePoints;
                                        Drawables.Add(p);
                                        break;
                                }

                            }

                            Drawables.ForEach(x => x.Draw(GE));


                        }
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK);
            }
        }

        public struct Serializable
        {
            public List<ColoredPoint> Points;
            public List<Point> BasePoints;
            public string Name;
        }

        private void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            var serList = new List<Serializable>();
            foreach(var item in Drawables)
            {
                serList.Add(new Serializable { Points = item.Points, BasePoints = item.BasePoints , Name = item.Name});
                
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Image files (*.xml)|*.xml|All Files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                if (saveFileDialog.FileName != string.Empty)
                {
                    using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.OpenOrCreate))
                    {
                        //PngBitmapEncoder encoder = new PngBitmapEncoder();
                        //encoder.Frames.Add(BitmapFrame.Create((WriteableBitmap)modifiedImageCanvas.Source));
                        //encoder.Save(stream);
                        var xmlSerializer = new XmlSerializer(serList.GetType());
                        xmlSerializer.Serialize(stream, serList);
                        stream.Close();
                    }
                }

            }
        }

        private void Clip_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedDrawable == null)
                return;
            foreach(var item in Drawables)
            {
                if (item == SelectedDrawable)
                    continue;
                if (item.Name != "Polygon" && item.Name != "Line")
                    continue;

                if(SelectedDrawable.Name == "Polygon")
                    item.Clip(GE,SelectedDrawable);

            }
        }

        private void Fill_Click(object sender, RoutedEventArgs e)
        {
            if(SelectedDrawable != null)
            if(SelectedDrawable.Name == "Polygon")
            {
                    SelectedDrawable.IsFilledImage = false;
                SelectedDrawable.IsFilled = true;
                ((ILinePolygon)SelectedDrawable).FillPolygon(GE, ((System.Windows.Media.Color)ColorPicker.SelectedColor).ToColor());
                    SelectedDrawable.Draw(GE);

                }
        }

        private void FillImage_Click(object sender, RoutedEventArgs e)
        {
            if(SelectedDrawable != null && SelectedDrawable.Name == "Polygon")
            {
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.Filter = "Image files (*.jpg)|*.jpg|All Files (*.*)|*.*";
                fileDialog.RestoreDirectory = true;

                if (fileDialog.ShowDialog() == true)
                {
                    BitmapImage bitmap = new BitmapImage(new Uri(fileDialog.FileName));
                    WriteableBitmap writeable = new WriteableBitmap(bitmap);
                    SelectedDrawable.IsFilled = false;
                    SelectedDrawable.IsFilledImage = true;
                    SelectedDrawable.FillBitmap = writeable;
                    ((ILinePolygon)SelectedDrawable).FillImage(GE, writeable);
                    SelectedDrawable.Draw(GE);
                }
            }
        }
    }
}
