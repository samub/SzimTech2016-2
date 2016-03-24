using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace teszt {
    public partial class MainWindow {
        private const int MyImageSizeX = 640;
        private const int MyImageSizeY = 640;
        private readonly ShapeDrawer sd = new ShapeDrawer();
        private BitmapSource _myOriginalMap;
        private Robot _robot;
        private CsvToMatrix map;
        private BitmapSource myBitmapSource;
        private byte[] pixels;

        public MainWindow() {
            InitializeComponent();

            // változó inicializálása a MessageHandler osztályban
            MessageHandler.TextBoxMessages = TextBoxForMessages;


            if (CheckBoxLogOnOff.IsChecked != null && CheckBoxLogOnOff.IsChecked.Value) TextBoxLogFileName.Visibility = Visibility.Visible;

            pixels = new byte[MyImageSizeX * MyImageSizeY * 4];
            myBitmapSource = BitmapSource.Create(MyImageSizeX, MyImageSizeY, 96, 96, PixelFormats.Pbgra32, null, pixels,
                                                 MyImageSizeX * 4);
            Image.Source = myBitmapSource;
            RenderOptions.SetBitmapScalingMode(Image, BitmapScalingMode.NearestNeighbor);


            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Image.Width = 640;
            Image.Height = 640;
            Image.Stretch = Stretch.None;
        }

        private void button_Click(object sender, RoutedEventArgs e) {
            var openFileDialog1 = new OpenFileDialog {
                                                         Filter = "CSV Files|*.csv",
                                                         Title = "Select a CSV File",
                                                         InitialDirectory = Assembly.GetExecutingAssembly().Location
                                                     };

            if (openFileDialog1.ShowDialog() == true) {
                map = new CsvToMatrix(openFileDialog1.FileName);

                map.Read();

                pixels = new byte[MyImageSizeX * MyImageSizeY * 4];
                var current = 0;

                for (var i = 0; i < MyImageSizeX; i++)
                    for (var j = 0; j < MyImageSizeY; j++)
                        if (map.Map[i, j]) {
                            pixels[current++] = 255;
                            pixels[current++] = 255;
                            pixels[current++] = 255;
                            pixels[current++] = 255;
                        }
                        else {
                            pixels[current++] = 0;
                            pixels[current++] = 0;
                            pixels[current++] = 0;
                            pixels[current++] = 255;
                        }

                myBitmapSource = BitmapSource.Create(MyImageSizeX, MyImageSizeY, 96, 96, PixelFormats.Pbgra32, null,
                                                     pixels, MyImageSizeX * 4);
                _myOriginalMap = BitmapSource.Create(MyImageSizeX, MyImageSizeY, 96, 96, PixelFormats.Pbgra32, null,
                                                     pixels, MyImageSizeX * 4);
                Image.Source = myBitmapSource;
                RenderOptions.SetBitmapScalingMode(Image, BitmapScalingMode.NearestNeighbor);
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e) {
            TextBoxForMessages.Text = "";


            if (RadioButton.IsChecked != null && RadioButton.IsChecked.Value)
                if (TextBoxPositionX.Text.Length != 0 && TextBoxPositionY.Text.Length != 0 &&
                    TextBoxCoveringPercentage.Text.Length != 0)
                    _robot = new Robot((int) SliderViweAngle.Value, Convert.ToInt32(TextBoxPositionX.Text),
                                       Convert.ToInt32(TextBoxPositionY.Text),
                                       Convert.ToInt32(TextBoxCoveringPercentage.Text), 90, "fan50A.csv");
                else MessageBox.Show("Adjon meg kezdőértékeket a robotnak!");
            else if (RadioButton1.IsChecked != null && RadioButton1.IsChecked.Value)
                if (TextBoxPositionX.Text.Length != 0 && TextBoxPositionY.Text.Length != 0 &&
                    TextBoxCoveringPercentage.Text.Length != 0)
                    _robot = new Robot((int) SliderViweAngle.Value, Convert.ToInt32(TextBoxPositionX.Text),
                                       Convert.ToInt32(TextBoxPositionY.Text),
                                       Convert.ToInt32(TextBoxCoveringPercentage.Text), 90, "fan100.csv");
                else MessageBox.Show("Adjon meg kezdőértékeket a robotnak!");

            if (_robot != null) MapRefresh();

            /*if (RadioButtonGenetic.IsChecked != null && RadioButtonGenetic.IsChecked.Value);
                //genetic
                else if (RadioButtonHeuristic1.IsChecked != null && RadioButtonHeuristic1.IsChecked.Value);
                //h1
                else if (RadioButtonHeuristic2.IsChecked != null && RadioButtonHeuristic2.IsChecked.Value);
                //h2*/
            else MessageBox.Show("Kérem válasszon robot típust!");
        }

        private void MapRefresh() {
            if (_robot != null) {
                var stride = myBitmapSource.PixelWidth * 4;
                var size = myBitmapSource.PixelHeight * stride;
                pixels = new byte[size];
                _myOriginalMap.CopyPixels(pixels, stride, 0);


                var stride1 = _robot.MyBitmap.PixelWidth * 4;
                var size1 = _robot.MyBitmap.PixelHeight * stride;
                var robotPix = new byte[size1];
                _robot.MyBitmap.CopyPixels(robotPix, stride1, 0);

                var startPixelX = _robot.X - _robot.Robot1.Map.GetLength(0) / 2;
                var startPixelY = _robot.Y - _robot.Robot1.Map.GetLength(1) / 2;

                for (var i = startPixelX; i < startPixelX + _robot.Robot1.Map.GetLength(0); i++)
                    for (var j = startPixelY; j < startPixelY + _robot.Robot1.Map.GetLength(1); j++)
                        if (pixels[i * 4 + j * 4 * 640] == 255 &&
                            robotPix[
                                     i * 4 - startPixelX * 4 + j * 4 * _robot.Robot1.Map.GetLength(1) -
                                     startPixelY * 4 * _robot.Robot1.Map.GetLength(1)] == 255) {
                            pixels[i * 4 + j * 4 * 640] = 255;
                            pixels[i * 4 + j * 4 * 640 + 1] = 255;
                            pixels[i * 4 + j * 4 * 640 + 2] = 255;
                            pixels[i * 4 + j * 4 * 640 + 3] = 255;
                        }
                        else if (pixels[i * 4 + j * 4 * 640] == 0 &&
                                 robotPix[
                                          i * 4 - startPixelX * 4 + j * 4 * _robot.Robot1.Map.GetLength(1) -
                                          startPixelY * 4 * _robot.Robot1.Map.GetLength(1)] == 255) {
                            pixels[i * 4 + j * 4 * 640] = 255;
                            pixels[i * 4 + j * 4 * 640 + 1] = 255;
                            pixels[i * 4 + j * 4 * 640 + 2] = 255;
                            pixels[i * 4 + j * 4 * 640 + 3] = 255;
                        }
                        else if (pixels[i * 4 + j * 4 * 640] == 0 &&
                                 robotPix[
                                          i * 4 - startPixelX * 4 + j * 4 * _robot.Robot1.Map.GetLength(1) -
                                          startPixelY * 4 * _robot.Robot1.Map.GetLength(1)] == 0) {
                            pixels[i * 4 + j * 4 * 640] = 0;
                            pixels[i * 4 + j * 4 * 640 + 1] = 0;
                            pixels[i * 4 + j * 4 * 640 + 2] = 0;
                            pixels[i * 4 + j * 4 * 640 + 3] = 255;
                        }
                        else if (pixels[i * 4 + j * 4 * 640] == 255 &&
                                 robotPix[
                                          i * 4 - startPixelX * 4 + j * 4 * _robot.Robot1.Map.GetLength(1) -
                                          startPixelY * 4 * _robot.Robot1.Map.GetLength(1)] == 0) {
                            pixels[i * 4 + j * 4 * 640] = 0;
                            pixels[i * 4 + j * 4 * 640 + 1] = 0;
                            pixels[i * 4 + j * 4 * 640 + 2] = 0;
                            pixels[i * 4 + j * 4 * 640 + 3] = 255;
                        }

                myBitmapSource = BitmapSource.Create(MyImageSizeX, MyImageSizeY, 96, 96, PixelFormats.Pbgra32, null,
                                                     pixels, MyImageSizeX * 4);
                Image.Source = myBitmapSource;
                RenderOptions.SetBitmapScalingMode(Image, BitmapScalingMode.NearestNeighbor);
            }
            else MessageBox.Show("Kérem válasszon robot típust!");
        }

        private void ImageButton_Click(object sender, RoutedEventArgs e) {
            var p1 = Mouse.GetPosition(this);
            Console.WriteLine(@"Mouse.GetPosition: {0}, {1}", p1.X - 11, p1.Y - 51);
            var newPoint = new Point(p1.X - 11, p1.Y - 51);

            myBitmapSource.CopyPixels(pixels, 640 * 4, 0);

            sd.AddPointToShape(p1);
            pixels = sd.DrawPoints(pixels, newPoint);


            myBitmapSource = BitmapSource.Create(640, 640, 96, 96, PixelFormats.Pbgra32, null, pixels, 640 * 4);
            Image.Source = myBitmapSource;
        }

        private void checkBoxLogOnOff_Checked(object sender, RoutedEventArgs e) {
            TextBoxLogFileName.Visibility = Visibility.Visible;
        }

        private void checkBoxLogOnOff_Unchecked(object sender, RoutedEventArgs e) {
            TextBoxLogFileName.Visibility = Visibility.Hidden;
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e) {
            if (CheckBoxLogOnOff.IsChecked.Equals(true)) MessageHandler.ToLog(TextBoxLogFileName.Text);
        }

        private void button_Click_1(object sender, RoutedEventArgs e) {
            _robot.Reposition(Convert.ToInt32(TextBoxPositionX.Text), Convert.ToInt32(TextBoxPositionY.Text), Convert.ToInt32(textBox_TESZT.Text));
            MapRefresh();
        }


        private void button1_Click(object sender, RoutedEventArgs e) {
            for (int i=1;i<100;i++)
            {
                _robot.Route.Add(new Tuple<int,int,double>(i,i,Convert.ToDouble(i)));
            }
            for (int i = 1; i < 100; i++)
            {
                _robot.Reposition(_robot.Route[i].Item1, _robot.Route[i].Item2, _robot.Route[i].Item3);
                MapRefresh();
            }
            
        }
    }
}