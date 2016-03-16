using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace teszt {
    public partial class MainWindow {
        public MainWindow() {
            InitializeComponent();
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

                var myBitmapSource = BitmapSource.Create(file.Map.GetLength(1), file.Map.GetLength(0), 96, 96,
                                                         PixelFormats.Pbgra32, null, pixels, file.Map.GetLength(0) * 4);
                Image.Source = myBitmapSource;
                RenderOptions.SetBitmapScalingMode(Image, BitmapScalingMode.NearestNeighbor);
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}