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
        private BitmapSource myBitmapSource;

        public MainWindow() {
            InitializeComponent();

            // Üzenetek tesztelése
            MessageHandler Messages = new MessageHandler(textBoxMessages);
            Messages.Write("Uzenet1");
            Messages.Write("Uzenet2");
            Messages.Write("Uzenet3");
            Messages.Write("Uzenet4");
            Messages.Write("Uzenet5");
            Messages.Write("Uzenet6");

            byte[] pixels = new byte[640 * 640 * 4];
            myBitmapSource = BitmapSource.Create(640, 640, 96, 96, PixelFormats.Pbgra32, null, pixels, 640 * 4);
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
                var file = new CsvToMatrix(openFileDialog1.FileName);

                file.Read();

                var pixels = new byte[file.Map.GetLength(0) * file.Map.GetLength(1) * 4];
                var current = 0;

                for (var i = 0; i < file.Map.GetLength(0); i++)
                    for (var j = 0; j < file.Map.GetLength(1); j++)
                        if (file.Map[i, j]) {
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

                //Image.Width = file.Map.GetLength(1);
                //Image.Height = file.Map.GetLength(0);

                //  MessageBox.Show(file.Map.GetLength(0) + "x" + file.Map.GetLength(1));

                myBitmapSource = BitmapSource.Create(640, 640, 96, 96,
                                                     PixelFormats.Pbgra32, null, pixels, 640 * 4);
                Image.Source = myBitmapSource;
                RenderOptions.SetBitmapScalingMode(Image, BitmapScalingMode.NearestNeighbor);
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e) {
            var openFileDialog1 = new OpenFileDialog {
                                                         Filter = "CSV Files|*.csv",
                                                         Title = "Select a CSV File",
                                                         InitialDirectory = Assembly.GetExecutingAssembly().Location
                                                     };
            if (openFileDialog1.ShowDialog() == true) {
                var robot = new Robot((int) SliderViweAngle.Value, Convert.ToInt32(TextBoxPositionX.Text),
                                      Convert.ToInt32(TextBoxPositionY.Text),
                                      Convert.ToInt32(TextBoxCoveringPercentage.Text), 90, openFileDialog1.FileName);


                var stride = myBitmapSource.PixelWidth * 4;
                var size = myBitmapSource.PixelHeight * stride;
                byte[] pixels = new byte[size];
                myBitmapSource.CopyPixels(pixels, stride, 0);
                //MessageBox.Show(myBitmapSource.PixelWidth + " " + myBitmapSource.PixelHeight);


                var current = 0;
                byte[] robotPixels = new byte[robot.Robot1.Map.GetLength(0) * robot.Robot1.Map.GetLength(1) * 4];

                for (var i = 0; i < robot.Robot1.Map.GetLength(0); i++)
                    for (var j = 0; j < robot.Robot1.Map.GetLength(1); j++)
                        if (robot.Robot1.Map[i, j])
                        {
                            robotPixels[current++] = 255;
                            robotPixels[current++] = 255;
                            robotPixels[current++] = 255;
                            robotPixels[current++] = 255;
                        }
                        else {
                            robotPixels[current++] = 0;
                            robotPixels[current++] = 0;
                            robotPixels[current++] = 0;
                            robotPixels[current++] = 255;
                        }


                //combine the two arrays






                myBitmapSource = BitmapSource.Create(640, 640, 96, 96,
                                                     PixelFormats.Pbgra32, null, pixels, 640 * 4);
                Image.Source = myBitmapSource;
                RenderOptions.SetBitmapScalingMode(Image, BitmapScalingMode.NearestNeighbor);


                /*if (RadioButtonGenetic.IsChecked != null && RadioButtonGenetic.IsChecked.Value);
                //genetic
                else if (RadioButtonHeuristic1.IsChecked != null && RadioButtonHeuristic1.IsChecked.Value);
                //h1
                else if (RadioButtonHeuristic2.IsChecked != null && RadioButtonHeuristic2.IsChecked.Value);
                //h2*/
            }
        }

        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            Point p1 = Mouse.GetPosition(this);
            
            Console.WriteLine(string.Format("Mouse.GetPosition: {0}, {1}", p1.X-11, p1.Y-51));
        }

    }
}