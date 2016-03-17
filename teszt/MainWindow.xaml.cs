using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace WpfMapTest
{

    internal class CsvToMatrix
    {
        private readonly StreamReader _sr;

        public CsvToMatrix(string fileName)
        {
            FileName = fileName;
            var fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.None);
            _sr = new StreamReader(fs, Encoding.GetEncoding("iso-8859-2"), false);
        }

        private string FileName { get; }

        public bool[,] Map { get; private set; }


        private int LineCount()
        {
            var lineNum = 0;
            while (_sr.ReadLine() != null) lineNum++;
            return lineNum;
        }

        public void Read()
        {
            var lineCount = LineCount();
            _sr.DiscardBufferedData();
            _sr.BaseStream.Seek(0, SeekOrigin.Begin);
            var line = _sr.ReadLine();
            var i = 0;
            if (line != null)
            {
                var splitLine = line.Split(';');
                Map = new bool[lineCount, splitLine.LongLength];
                //MessageBox.Show(lineCount + " " + splitLine.LongLength);
                while (line != null)
                {
                    var j = 0;
                    foreach (var s in splitLine)
                    {
                        switch (s)
                        {
                            case "1":
                                Map[i, j] = true;
                                break;
                            case "0":
                                Map[i, j] = false;
                                break;
                        }
                        j++;
                    }
                    line = _sr.ReadLine();
                    splitLine = line?.Split(';');
                    i++;
                }
            }
            _sr.Close();
        }
    }

    public partial class MainWindow : Window
    {
        int MyImageSizeX = 20, MyImageSizeY = 20;
        double RotAngle = 0.1;
        BitmapSource MyBitmapSource;
        public MainWindow()
        {

            InitializeComponent();
            MyImageSizeX = (int)SliderImSize.Value;
            var random = new Random();
            var pixels = new byte[MyImageSizeX * MyImageSizeY * 4];
            random.NextBytes(pixels);
            MyBitmapSource = BitmapSource.Create(MyImageSizeX, MyImageSizeY, 96, 96, PixelFormats.Pbgra32, null, pixels, MyImageSizeX * 4);
            ImageMap.Source = MyBitmapSource;
            RenderOptions.SetBitmapScalingMode(ImageMap, BitmapScalingMode.NearestNeighbor);

        }
        private void SliderImSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MyImageSizeX = (int)SliderImSize.Value;
            MyImageSizeY = (int)SliderImSize.Value;
            LabelStatus.Content = "New image size is: " + MyImageSizeX + " by " + MyImageSizeY + ".";
        }
        private void SliderRot_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RotAngle = SliderRot.Value;
            LabelStatus.Content = "New rotation angle is: " + RotAngle * (180.0 / Math.PI) + "°";
        }
        private void Window_Closed(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// Creates and apllies random image source.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRand_Click(object sender, RoutedEventArgs e)
        {
            var random = new Random();
            var pixels = new byte[MyImageSizeX * MyImageSizeY * 4];
            random.NextBytes(pixels);
            MyBitmapSource = BitmapSource.Create(MyImageSizeX, MyImageSizeY, 96, 96, PixelFormats.Pbgra32, null, pixels, MyImageSizeX * 4);
            ImageMap.Source = MyBitmapSource;
        }
        private void BtnRenderMode_Click(object sender, RoutedEventArgs e)
        {
            BitmapScalingMode bitmapScalingMode = RenderOptions.GetBitmapScalingMode(ImageMap);
            if (bitmapScalingMode != BitmapScalingMode.NearestNeighbor)
            {
                RenderOptions.SetBitmapScalingMode(ImageMap, BitmapScalingMode.NearestNeighbor);
                LabelStatus.Content = "BitmapScalingMode is NearestNeighbor";
            }
            else
            {
                RenderOptions.SetBitmapScalingMode(ImageMap, BitmapScalingMode.LowQuality);
                LabelStatus.Content = "BitmapScalingMode is LowQuality";
            }
        }
        private void BtnRotate_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Refactor and test
            int Stride = MyBitmapSource.PixelWidth * 4;
            int Size = MyBitmapSource.PixelHeight * Stride;
            byte[] PixArray = new byte[Size];
            float[] CoordX = new float[0];
            float[] CoordY = new float[0];
            MyBitmapSource.CopyPixels(PixArray, Stride, 0);
            for (var i = 0; i < MyImageSizeX * 4; i += 4)
                for (var j = 0; j < MyImageSizeY; j = j + 1)
                    // Collect the black pixels into CoordX and CoordY
                    if (PixArray[i + j * 4 * MyImageSizeY] == 0)
                    {
                        Array.Resize(ref CoordX, CoordX.Length + 1);
                        Array.Resize(ref CoordY, CoordY.Length + 1);
                        CoordX[CoordX.Length - 1] = i / 4 - MyBitmapSource.PixelWidth / 2;
                        CoordY[CoordY.Length - 1] = j - MyBitmapSource.PixelHeight / 2;
                    }
            // CoordX and CoordY >> OriginalCoordinates which contains all the black coordinates
            float[,] OriginalCoordinates = new float[CoordX.Length, 2];
            for (var i = 0; i < CoordX.Length; i++)
            {
                OriginalCoordinates[i, 0] = CoordX[i];
                OriginalCoordinates[i, 1] = CoordY[i];
            }
            float[,] RotMatrix = { { (float)Math.Cos(RotAngle), (float)-Math.Sin(RotAngle) }, { (float)Math.Sin(RotAngle), (float)Math.Cos(RotAngle) } };
            float[,] ResultMatrix;
            // The matrix multiplication
            ResultMatrix = MatrixMult(OriginalCoordinates, RotMatrix);
            // Fill the entire image with white pixels
            for (var i = 0; i < MyImageSizeX * 4; i += 4)
                for (var j = 0; j < MyImageSizeY; j = j + 1)
                {
                    PixArray[i + j * 4 * MyImageSizeY] = 255; //r
                    PixArray[i + j * 4 * MyImageSizeY + 1] = 255; // g 
                    PixArray[i + j * 4 * MyImageSizeY + 2] = 255; // b 
                    PixArray[i + j * 4 * MyImageSizeY + 3] = 255; // alpha 
                }
            // Draw the black pixels according the rotation
            for (var i = 0; i < ResultMatrix.GetLength(0); i++)
            {
                //System.Diagnostics.Debug.WriteLine((int)c[i, 0] + ";" + (int)c[i, 1]);
                //int Pixel1 = (int)(ResultMatrix[i, 0] + MyBitmapSource.PixelHeight / 2 ) * 4 + (int)(ResultMatrix[i, 1] + MyBitmapSource.PixelWidth / 2) * 4 * MyImageSizeY;
                int PixelUp = (int)Math.Ceiling(ResultMatrix[i, 0] + MyBitmapSource.PixelHeight / 2) * 4 +
                             (int)Math.Ceiling(ResultMatrix[i, 1] + MyBitmapSource.PixelWidth / 2) * 4 * MyImageSizeY;
                int PixelDown = (int)Math.Floor(ResultMatrix[i, 0] + MyBitmapSource.PixelHeight / 2) * 4 +
                             (int)Math.Floor(ResultMatrix[i, 1] + MyBitmapSource.PixelWidth / 2) * 4 * MyImageSizeY;
                try // TODO: remove try-catch
                {
                    if (PixelUp > 0 && PixelUp <= MyBitmapSource.PixelWidth * 4 + MyBitmapSource.PixelHeight * 4 * MyImageSizeY)
                    {
                        PixArray[PixelUp] = 0; // r
                        PixArray[PixelUp + 1] = 20; // g 
                        PixArray[PixelUp + 2] = 20; // b 
                        PixArray[PixelUp + 3] = 255; // alpha 
                    }
                    if (PixelDown > 0 && PixelDown <= MyBitmapSource.PixelWidth * 4 + MyBitmapSource.PixelHeight * 4 * MyImageSizeY)
                    {
                        PixArray[PixelDown] = 0; // r
                        PixArray[PixelDown + 1] = 20; // g 
                        PixArray[PixelDown + 2] = 20; // b 
                        PixArray[PixelDown + 3] = 255; // alpha 
                    }
                }
                catch { }

            }
            MyBitmapSource = BitmapSource.Create(MyImageSizeX, MyImageSizeX, 96, 96, PixelFormats.Pbgra32, null, PixArray, MyImageSizeX * 4);
            ImageMap.Source = MyBitmapSource;

        }
        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog1 = new OpenFileDialog
            {
                Filter = "CSV Files|*.csv",
                Title = "Select a CSV File",
                InitialDirectory = Assembly.GetExecutingAssembly().Location
            };

            if (openFileDialog1.ShowDialog() == true)
            {
                var file = new CsvToMatrix(openFileDialog1.FileName);

                file.Read();

                var pixels = new byte[file.Map.GetLength(0) * file.Map.GetLength(1) * 4];
                MyImageSizeX = file.Map.GetLength(0);
                MyImageSizeY = file.Map.GetLength(1);
                LabelStatus.Content = "Image size is: " + MyImageSizeX + " by " + MyImageSizeY;
                var current = 0;

                for (var i = 0; i < file.Map.GetLength(0); i++)
                    for (var j = 0; j < file.Map.GetLength(1); j++)
                        if (file.Map[i, j])
                        {
                            pixels[current++] = 255;
                            pixels[current++] = 255;
                            pixels[current++] = 255;
                            pixels[current++] = 255;
                        }
                        else
                        {
                            pixels[current++] = 0;
                            pixels[current++] = 0;
                            pixels[current++] = 0;
                            pixels[current++] = 255;
                        }

                MyBitmapSource = BitmapSource.Create(file.Map.GetLength(1), file.Map.GetLength(0), 96, 96, PixelFormats.Pbgra32, null, pixels, file.Map.GetLength(0) * 4);
                ImageMap.Source = MyBitmapSource;
                RenderOptions.SetBitmapScalingMode(ImageMap, BitmapScalingMode.NearestNeighbor);
            }
        }
        /// <summary>
        /// "Drawing" on the map
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed || Mouse.RightButton == MouseButtonState.Pressed)
            {
                try
                {
                    int Xpixel = (int)(e.GetPosition(ImageMap).X / ImageMap.ActualWidth * MyImageSizeX);
                    int Ypixel = Ypixel = (int)(e.GetPosition(ImageMap).Y / ImageMap.ActualHeight * MyImageSizeX);
                    int Stride = MyBitmapSource.PixelWidth * 4;
                    int Size = MyBitmapSource.PixelHeight * Stride;
                    byte[] PixArray = new byte[Size];
                    MyBitmapSource.CopyPixels(PixArray, Stride, 0);
                    int Pixel = Xpixel * 4 + Ypixel * 4 * MyImageSizeY;
                    if (Mouse.LeftButton == MouseButtonState.Pressed)
                    {
                        PixArray[Pixel] = 0; // r
                        PixArray[Pixel + 1] = 0; // g 
                        PixArray[Pixel + 2] = 0; // b 
                        PixArray[Pixel + 3] = 255; // alpha 
                    }
                    else
                    {
                        PixArray[Pixel] = 255; // r
                        PixArray[Pixel + 1] = 255; // g 
                        PixArray[Pixel + 2] = 255; // b 
                        PixArray[Pixel + 3] = 255; // alpha 
                    }
                    MyBitmapSource = BitmapSource.Create(MyImageSizeX, MyImageSizeY, 96, 96, PixelFormats.Pbgra32, null, PixArray, MyImageSizeX * 4);
                    ImageMap.Source = MyBitmapSource;
                    LabelStatus.Content = "x: " + Xpixel + " y: " + Ypixel + " is colored.";
                    LabelStatus.Background = Brushes.MediumSeaGreen;
                }
                catch (Exception ex)
                {
                    LabelStatus.Background = Brushes.Firebrick;
                    LabelStatus.Content = ex.ToString();
                }
            }
            else
            {
                LabelStatus.Background = Brushes.DarkMagenta;
            }
        }
        private void ImageMap_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            /// <summary>
            /// Item1: x, Item2: y, Item3: theta
            /// </summary>
            var Route = new List<Tuple<int, int, double>>();

            Route.Add(new Tuple<int, int, double>(320, 240, 180));
            Route.Add(new Tuple<int, int, double>(639, 479, 4.32));
            foreach (Tuple<int, int, double> t in Route)

                System.Diagnostics.Debug.WriteLine("x: " + t.Item1 + " y: " + t.Item2 + " theta: " + t.Item3);
        }

        /// <summary>
        /// Matrix multiplier 
        /// each i, j entry is given by multiplying the entries Aik (across row i of A) 
        /// by the entries Bkj (down column j of B), 
        /// for k = 1, 2, ..., m, and summing the results over k
        /// </summary>
        /// <param name="a">A matrix</param>
        /// <param name="b">B matrix</param>
        float[,] MatrixMult(float[,] a, float[,] b)
        {
            float[,] c = { };
            if (a.GetLength(1) == b.GetLength(0))
            {
                c = new float[a.GetLength(0), b.GetLength(1)];
                for (int i = 0; i < c.GetLength(0); i++)
                {
                    for (int j = 0; j < c.GetLength(1); j++)
                    {
                        c[i, j] = 0;
                        for (int k = 0; k < a.GetLength(1); k++) // vagy k<b.GetLength(0)
                            c[i, j] = c[i, j] + a[i, k] * b[k, j];
                    }
                }
            }
            else
            {
                MessageBox.Show("Matrix size mismatch.");
            }
            return c;
        }

        void MatrixMultDemo()
        {
            //// Matix multiplication demo
            //float[,] a = { { 2.9F, 3, 4 }, { 1, 0, 0 }, { 0, 0, 4 } };
            //float[,] b = { { 1000 }, { 100 }, { 10 } };
            //float[,] c;
            //c = MatrixMult(a, b);
            //for (int i = 0; i < c.GetLength(0); i++)
            //{
            //    System.Diagnostics.Debug.WriteLine("");
            //    for (int j = 0; j < c.GetLength(1); j++)
            //    {
            //        System.Diagnostics.Debug.Write(c[i, j] + " ");
            //    }
            //}
        }
    }
}
