using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace teszt {
    public partial class MainWindow {
        private const int MyImageSizeX = 640;
        private const int MyImageSizeY = 640;
        private readonly ShapeDrawer _sd = new ShapeDrawer();
        private bool _isFile;
        private CsvToMatrix _map;
        private BitmapSource _myBitmapSource;
        private BitmapSource _myOriginalMap;
        private byte[] _pixels;
        private Robot _robot;

        public MainWindow() {
            InitializeComponent();

            // változó inicializálása a MessageHandler osztályban
            MessageHandler.TextBoxMessages = TextBoxForMessages;

            if (CheckBoxLogOnOff.IsChecked != null && CheckBoxLogOnOff.IsChecked.Value) TextBoxLogFileName.Visibility = Visibility.Visible;
            RenderOptions.SetBitmapScalingMode(Image, BitmapScalingMode.NearestNeighbor);

            _pixels = new byte[0];
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Image.Width = 640;
            Image.Height = 640;
            TextBoxPositionX.MaxLength = 3;
            TextBoxPositionY.MaxLength = 3;
            TextBoxCoveringPercentage.MaxLength = 3;
            Image.Stretch = Stretch.None;
            _isFile = false;
        }

        private void button_Click(object sender, RoutedEventArgs e) {
            var openFileDialog1 = new OpenFileDialog {
                                                         Filter = "CSV Files|*.csv",
                                                         Title = "Select a CSV File",
                                                         InitialDirectory = Assembly.GetExecutingAssembly().Location
                                                     };

            if (openFileDialog1.ShowDialog() == true) {
                _map = new CsvToMatrix(openFileDialog1.FileName);

                _map.Read();

                if (_map.Map.GetLength(0) == 640 && _map.Map.GetLength(1) == 640) {
                    _pixels = new byte[MyImageSizeX * MyImageSizeY * 4];
                    var current = 0;

                    for (var i = 0; i < MyImageSizeX; i++)
                        for (var j = 0; j < MyImageSizeY; j++)
                            if (_map.Map[i, j]) {
                                _pixels[current++] = 255;
                                _pixels[current++] = 255;
                                _pixels[current++] = 255;
                                _pixels[current++] = 255;
                            }
                            else {
                                _pixels[current++] = 0;
                                _pixels[current++] = 0;
                                _pixels[current++] = 0;
                                _pixels[current++] = 255;
                            }

                    _myBitmapSource = BitmapSource.Create(MyImageSizeX, MyImageSizeY, 96, 96, PixelFormats.Pbgra32, null,
                                                          _pixels, MyImageSizeX * 4);
                    _myOriginalMap = BitmapSource.Create(MyImageSizeX, MyImageSizeY, 96, 96, PixelFormats.Pbgra32, null,
                                                         _pixels, MyImageSizeX * 4);
                    Image.Source = _myBitmapSource;
                }
                else MessageBox.Show("A térkép felbontása 640x640 lehet.", "Figyelmeztetés");
            }
        }

        private void IntegerValidationTextBox(object sender, TextCompositionEventArgs e) {
            var regex = new Regex(@"^\d$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e) {
            TextBoxForMessages.Text = "";
            ButtonMapEdit.IsEnabled = false;
            ButtonMapOpen.IsEnabled = false;
            RadioButtonCircle.IsEnabled = false;
            RadioButtonRect.IsEnabled = false;
            RadioButtonEllips.IsEnabled = false;
            if (_isFile) {
                if (RadioButton.IsChecked != null && !RadioButton.IsChecked.Value && RadioButton1.IsChecked != null &&
                    !RadioButton1.IsChecked.Value) MessageBox.Show("Kérem válassza ki a robot típusát!", "Figyelmeztetés");
                else if (RadioButton.IsChecked != null && RadioButton.IsChecked.Value)
                    if (TextBoxPositionX.Text.Length != 0 && TextBoxPositionY.Text.Length != 0 &&
                        TextBoxCoveringPercentage.Text.Length != 0) {
                        _robot = new Robot((int) SliderViweAngle.Value, Convert.ToInt32(TextBoxPositionX.Text),
                                           Convert.ToInt32(TextBoxPositionY.Text),
                                           Convert.ToInt32(TextBoxCoveringPercentage.Text), 90, "fan50A.csv");
                        if (Convert.ToInt32(TextBoxPositionX.Text) < _robot.Robot1.Map.GetLength(0) / 2 ||
                            Convert.ToInt32(TextBoxPositionY.Text) < _robot.Robot1.Map.GetLength(1) / 2 ||
                            Convert.ToInt32(TextBoxPositionX.Text) > MyImageSizeX - _robot.Robot1.Map.GetLength(0) / 2 ||
                            Convert.ToInt32(TextBoxPositionY.Text) > MyImageSizeY - _robot.Robot1.Map.GetLength(1) / 2) {
                            MessageBox.Show("A robot nem lóghat le a térképről, inicializálja újra!", "Figyelmeztetés");
                            _robot = null;
                            Image.Source = _myOriginalMap;
                        }
                    }
                    else MessageBox.Show("Adjon meg kezdőértékeket a robotnak!", "Figyelmeztetés");
                else if (RadioButton1.IsChecked != null && RadioButton1.IsChecked.Value)
                    if (TextBoxPositionX.Text.Length != 0 && TextBoxPositionY.Text.Length != 0 &&
                        TextBoxCoveringPercentage.Text.Length != 0) {
                        _robot = new Robot((int) SliderViweAngle.Value, Convert.ToInt32(TextBoxPositionX.Text),
                                           Convert.ToInt32(TextBoxPositionY.Text),
                                           Convert.ToInt32(TextBoxCoveringPercentage.Text), 90, "fan100.csv");
                        if (Convert.ToInt32(TextBoxPositionX.Text) < _robot.Robot1.Map.GetLength(0) / 2 ||
                            Convert.ToInt32(TextBoxPositionY.Text) < _robot.Robot1.Map.GetLength(1) / 2 ||
                            Convert.ToInt32(TextBoxPositionX.Text) > MyImageSizeX - _robot.Robot1.Map.GetLength(0) / 2 ||
                            Convert.ToInt32(TextBoxPositionY.Text) > MyImageSizeY - _robot.Robot1.Map.GetLength(1) / 2) {
                            MessageBox.Show("A robot nem lóghat le a térképről, inicializálja újra!", "Figyelmeztetés");
                            _robot = null;
                            Image.Source = _myOriginalMap;
                        }
                    }
                    else MessageBox.Show("Adjon meg kezdőértékeket a robotnak!", "Figyelmeztetés");
            }
            else if (TextBoxPositionX.Text.Length != 0 && TextBoxPositionY.Text.Length != 0 &&
                     TextBoxCoveringPercentage.Text.Length != 0) {
                _robot = new Robot((int) SliderViweAngle.Value, Convert.ToInt32(TextBoxPositionX.Text),
                                   Convert.ToInt32(TextBoxPositionY.Text),
                                   Convert.ToInt32(TextBoxCoveringPercentage.Text), 315);
                if (Convert.ToInt32(TextBoxPositionX.Text) < _robot.Radius ||
                    Convert.ToInt32(TextBoxPositionY.Text) < _robot.Radius ||
                    Convert.ToInt32(TextBoxPositionX.Text) > MyImageSizeX - _robot.Radius ||
                    Convert.ToInt32(TextBoxPositionY.Text) > MyImageSizeY - _robot.Radius) {
                    MessageBox.Show("A robot nem lóghat le a térképről, inicializálja újra!", "Figyelmeztetés");
                    _robot = null;
                    Image.Source = _myOriginalMap;
                }
            }
            else MessageBox.Show("Adjon meg kezdőértékeket a robotnak!", "Figyelmeztetés");

            if (_robot != null) MapRefresh(_isFile);

            /*if (RadioButtonGenetic.IsChecked != null && RadioButtonGenetic.IsChecked.Value);
                //genetic
                else if (RadioButtonHeuristic1.IsChecked != null && RadioButtonHeuristic1.IsChecked.Value);
                //h1
                else if (RadioButtonHeuristic2.IsChecked != null && RadioButtonHeuristic2.IsChecked.Value);
                //h2*/
        }

        private void MapRefresh(bool method) {
            if (_robot != null)
                if (_myBitmapSource != null)
                    if (method) {
                        var stride = _myBitmapSource.PixelWidth * 4;
                        var size = _myBitmapSource.PixelHeight * stride;
                        _pixels = new byte[size];
                        _myOriginalMap.CopyPixels(_pixels, stride, 0);


                        var stride1 = _robot.MyBitmap.PixelWidth * 4;
                        var size1 = _robot.MyBitmap.PixelHeight * stride;
                        var robotPix = new byte[size1];
                        _robot.MyBitmap.CopyPixels(robotPix, stride1, 0);

                        var startPixelX = _robot.X - _robot.Robot1.Map.GetLength(0) / 2;
                        var startPixelY = _robot.Y - _robot.Robot1.Map.GetLength(1) / 2;

                        for (var i = startPixelX; i < startPixelX + _robot.Robot1.Map.GetLength(0); i++)
                            for (var j = startPixelY; j < startPixelY + _robot.Robot1.Map.GetLength(1); j++)
                                if (_pixels[i * 4 + j * 4 * 640] == 255 &&
                                    robotPix[
                                             i * 4 - startPixelX * 4 + j * 4 * _robot.Robot1.Map.GetLength(1) -
                                             startPixelY * 4 * _robot.Robot1.Map.GetLength(1)] == 255) {
                                    _pixels[i * 4 + j * 4 * 640] = 255;
                                    _pixels[i * 4 + j * 4 * 640 + 1] = 255;
                                    _pixels[i * 4 + j * 4 * 640 + 2] = 255;
                                    _pixels[i * 4 + j * 4 * 640 + 3] = 255;
                                }
                                else if (_pixels[i * 4 + j * 4 * 640] == 0 &&
                                         robotPix[
                                                  i * 4 - startPixelX * 4 + j * 4 * _robot.Robot1.Map.GetLength(1) -
                                                  startPixelY * 4 * _robot.Robot1.Map.GetLength(1)] == 255) {
                                    _pixels[i * 4 + j * 4 * 640] = 255;
                                    _pixels[i * 4 + j * 4 * 640 + 1] = 255;
                                    _pixels[i * 4 + j * 4 * 640 + 2] = 255;
                                    _pixels[i * 4 + j * 4 * 640 + 3] = 255;
                                }
                                else if (_pixels[i * 4 + j * 4 * 640] == 0 &&
                                         robotPix[
                                                  i * 4 - startPixelX * 4 + j * 4 * _robot.Robot1.Map.GetLength(1) -
                                                  startPixelY * 4 * _robot.Robot1.Map.GetLength(1)] == 0) {
                                    _pixels[i * 4 + j * 4 * 640] = 0;
                                    _pixels[i * 4 + j * 4 * 640 + 1] = 0;
                                    _pixels[i * 4 + j * 4 * 640 + 2] = 0;
                                    _pixels[i * 4 + j * 4 * 640 + 3] = 255;
                                }
                                else if (_pixels[i * 4 + j * 4 * 640] == 255 &&
                                         robotPix[
                                                  i * 4 - startPixelX * 4 +
                                                  j * 4 * _robot.Robot1.Map.GetLength(1) -
                                                  startPixelY * 4 * _robot.Robot1.Map.GetLength(1)] == 0) {
                                    _pixels[i * 4 + j * 4 * 640] = 0;
                                    _pixels[i * 4 + j * 4 * 640 + 1] = 0;
                                    _pixels[i * 4 + j * 4 * 640 + 2] = 0;
                                    _pixels[i * 4 + j * 4 * 640 + 3] = 255;
                                }

                        _myBitmapSource = BitmapSource.Create(MyImageSizeX, MyImageSizeY, 96, 96, PixelFormats.Pbgra32,
                                                              null, _pixels, MyImageSizeX * 4);
                        Image.Source = _myBitmapSource;
                    }
                    else {
                        _myOriginalMap.CopyPixels(_pixels, 640 * 4, 0);

                        _myBitmapSource = BitmapSource.Create(MyImageSizeX, MyImageSizeY, 96, 96, PixelFormats.Pbgra32,
                                                              null, _pixels, MyImageSizeX * 4);


                        ShapeDrawer.DrawCircle(_robot.X, _robot.Y, _robot.Radius, _robot.Theta, _robot.Theta + 270,
                                               ref _pixels);

                        var xstart = _robot.X + _robot.Radius * Math.Cos(Math.PI / 180.0 * _robot.Theta);
                        var ystart = _robot.Y - _robot.Radius * Math.Sin(Math.PI / 180.0 * _robot.Theta);

                        var xend = _robot.X + _robot.Radius * Math.Cos(Math.PI / 180.0 * (_robot.Theta + 270));
                        var yend = _robot.Y - _robot.Radius * Math.Sin(Math.PI / 180.0 * (_robot.Theta + 270));

                        if (_robot.Theta + 270 > 360)
                            ShapeDrawer.DrawCircle(_robot.X, _robot.Y, _robot.Radius, 0, _robot.Theta + 270 - 360,
                                                   ref _pixels);

                        ShapeDrawer.DrawLine(_robot.X, _robot.Y, (int) xstart, (int) ystart, ref _pixels);
                        ShapeDrawer.DrawLine(_robot.X, _robot.Y, (int) xend, (int) yend, ref _pixels);

                        if (_robot.Theta >= 300 && _robot.Theta <= 360 || _robot.Theta >= 0 && _robot.Theta <= 150) ShapeDrawer.FloodFill(ref _pixels, new Point(_robot.X - 10, _robot.Y));

                        _myBitmapSource = BitmapSource.Create(640, 640, 96, 96, PixelFormats.Pbgra32, null, _pixels,
                                                              640 * 4);
                        Image.Source = _myBitmapSource;
                    }
                else MessageBox.Show("Először töltsön be egy térképet!", "Figyelmeztetés");
            else MessageBox.Show("Kérem válassza ki a robot típusát!", "Figyelmeztetés");
        }

        private void ImageButton_Click(object sender, RoutedEventArgs e) {
            if (ButtonMapEdit.IsEnabled) {
                var p1 = Mouse.GetPosition(this);
                _myBitmapSource.CopyPixels(_pixels, 640 * 4, 0);
                var dialog = new Dialog();
                if (RadioButtonCircle.IsChecked != null && RadioButtonCircle.IsChecked.Value) {
                    dialog.Label.Content = "Sugár";
                    if (dialog.ShowDialog() == true) {
                        ShapeDrawer.DrawCircle((int) p1.X, (int) p1.Y, Convert.ToInt32(dialog.ResponseText), ref _pixels);
                        ShapeDrawer.FloodFill(ref _pixels, p1);
                    }
                }
                else if (RadioButtonRect.IsChecked != null && RadioButtonRect.IsChecked.Value) {
                    dialog.Label.Content = "Magasság";
                    dialog.Label1.Visibility = Visibility.Visible;
                    dialog.TextBox1.Visibility = Visibility.Visible;
                    if (dialog.ShowDialog() == true) {
                        ShapeDrawer.DrawRectangle((int) p1.X, (int) p1.Y, Convert.ToInt32(dialog.ResponseText),
                                                  Convert.ToInt32(dialog.ResponseText1), ref _pixels);
                        ShapeDrawer.FloodFill(ref _pixels, p1);
                    }
                }
                else if (RadioButtonEllips.IsChecked != null && RadioButtonEllips.IsChecked.Value) {
                    dialog.Label.Content = "Magasság";
                    dialog.Label1.Visibility = Visibility.Visible;
                    dialog.TextBox1.Visibility = Visibility.Visible;
                    if (dialog.ShowDialog() == true) {
                        ShapeDrawer.DrawEllipse((int) p1.X, (int) p1.Y, Convert.ToInt32(dialog.ResponseText),
                                                Convert.ToInt32(dialog.ResponseText1), ref _pixels);
                        ShapeDrawer.FloodFill(ref _pixels, p1);
                    }
                }

                _myBitmapSource = BitmapSource.Create(640, 640, 96, 96, PixelFormats.Pbgra32, null, _pixels, 640 * 4);
                _myOriginalMap = BitmapSource.Create(MyImageSizeX, MyImageSizeY, 96, 96, PixelFormats.Pbgra32, null,
                                                     _pixels, MyImageSizeX * 4);
                Image.Source = _myBitmapSource;
            }
        }

        private void checkBoxLogOnOff_Checked(object sender, RoutedEventArgs e) {
            TextBoxLogFileName.Visibility = Visibility.Visible;
        }

        private void checkBoxLogOnOff_Unchecked(object sender, RoutedEventArgs e) {
            TextBoxLogFileName.Visibility = Visibility.Hidden;
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e) {
            if (CheckBoxLogOnOff.IsChecked.Equals(true)) MessageHandler.ToLog(TextBoxLogFileName.Text);

            ButtonMapEdit.IsEnabled = true;
            ButtonMapOpen.IsEnabled = true;

            RadioButtonCircle.IsEnabled = true;
            RadioButtonRect.IsEnabled = true;
            RadioButtonEllips.IsEnabled = true;
        }

        private void button_Click_1(object sender, RoutedEventArgs e) {
            //TODO Nullpointer exceptions if I click
            _robot.Reposition(Convert.ToInt32(TextBoxPositionX.Text), Convert.ToInt32(TextBoxPositionY.Text),
                              Convert.ToInt32(TextBoxTeszt.Text), _isFile);
            MapRefresh(_isFile);
        }


        private async void button1_Click(object sender, RoutedEventArgs e) {
            for (var i = 100; i < 250; i++) _robot.Route.Add(new Tuple<int, int, double>(i, i + 10, Convert.ToDouble(i * 0.05)));
            foreach (var t in _robot.Route) {
                _robot.Reposition(t.Item1, t.Item2, t.Item3, _isFile);
                await Task.Delay(1);
                MapRefresh(_isFile);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e) {
            _myOriginalMap = null;
            _map = null;
            _robot = null;
            _myBitmapSource = null;
            RadioButtonCircle.IsEnabled = true;
            RadioButtonRect.IsEnabled = true;
            RadioButtonEllips.IsEnabled = true;
            _pixels = new byte[MyImageSizeX * MyImageSizeY * 4];

            for (var i = 0; i < MyImageSizeX; i++)
                for (var j = 0; j < MyImageSizeY; j++) {
                    _pixels[i * 4 + j * 4 * 640] = 0;
                    _pixels[i * 4 + j * 4 * 640 + 1] = 0;
                    _pixels[i * 4 + j * 4 * 640 + 2] = 0;
                    _pixels[i * 4 + j * 4 * 640 + 3] = 255;
                }

            _myBitmapSource = BitmapSource.Create(MyImageSizeX, MyImageSizeY, 96, 96, PixelFormats.Pbgra32, null,
                                                  _pixels, MyImageSizeX * 4);
            _myOriginalMap = BitmapSource.Create(MyImageSizeX, MyImageSizeY, 96, 96, PixelFormats.Pbgra32, null, _pixels,
                                                 MyImageSizeX * 4);
            Image.Source = _myBitmapSource;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e) {
            _isFile = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e) {
            _isFile = false;
        }
    }
}