using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace teszt {
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    internal class CsvToMatrix {
        private readonly StreamReader _sr;

        public CsvToMatrix(string fileName) {
            FileName = fileName;
            var fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.None);
            _sr = new StreamReader(fs, Encoding.GetEncoding("iso-8859-2"), false);
        }

        private string FileName { get; }

        public bool[,] Map { get; private set; }


        private int LineCount() {
            var lineNum = 0;
            while (_sr.ReadLine() != null) lineNum ++;
            return lineNum;
        }

        public void Read() {
            var lineCount = LineCount();
            _sr.DiscardBufferedData();
            _sr.BaseStream.Seek(0, SeekOrigin.Begin);
            var line = _sr.ReadLine();
            var i = 0;
            if (line != null) {
                var splitLine = line.Split(';');
                Map = new bool[lineCount, splitLine.LongLength];
                //MessageBox.Show(lineCount + " " + splitLine.LongLength);
                while (line != null) {
                    var j = 0;
                    foreach (var s in splitLine) {
                        switch (s) {
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

    public partial class MainWindow {
        public MainWindow() {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Image.Width = 640;
            Image.Height = 640;
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

        private void Button1_Click(object sender, RoutedEventArgs e) {
            var myLine1 = new RectangleGeometry(new Rect(0,0,1000,60));

            var drawing = new GeometryDrawing();

            drawing.Pen = new Pen(Brushes.Black, 1);


            drawing.Geometry = myLine1;

            Image.Source = new DrawingImage(drawing);
        }
    }
}