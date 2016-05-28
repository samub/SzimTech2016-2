using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace RobotMover {
    public partial class MainWindow {
        private const int MyImageSizeX = 640;
        private const int MyImageSizeY = 640;
        private readonly CreateGraph _creategraph = new CreateGraph();
        private bool _isFile;
        private CsvToMatrix _map;
        private readonly Mapcover _mapcover = new Mapcover();
        private bool[,] _mapToBool;
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
            RadioButton.IsEnabled = false;
            RadioButton1.IsEnabled = false;
        }

        /// <summary>
        ///     Megnyitás gomb hatására kapunk egy fájlmegnyitó dialógus ablakot melyben tetszőles
        ///     CSV formátumú térképet tudunk megnyitni melyet a program megfelelően lekezel és kirajzol az image-re
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Click(object sender, RoutedEventArgs e) {
            var openFileDialog1 = new OpenFileDialog {
                                                         Filter = "CSV Files|*.csv",
                                                         Title = "Select a CSV File",
                                                         InitialDirectory = Assembly.GetExecutingAssembly().Location
                                                     };

            if (openFileDialog1.ShowDialog() == true) {
                _map = new CsvToMatrix(openFileDialog1.FileName);

                _map.Read();

                RadioButtonCircle.IsEnabled = false;
                RadioButtonRect.IsEnabled = false;
                RadioButtonEllips.IsEnabled = false;

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

        /// <summary>
        ///     Start gomb lenyomására létrehozunk egy robot referenciát.
        ///     A robot megkapja a formon megadott kezdőértékeket és lerakásra került a térképre.
        ///     Ezt követően a kiválasztott algoritmus hívása következik majd.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonStart_Click(object sender, RoutedEventArgs e) {
            if (_myBitmapSource != null) {
                TextBoxForMessages.Text = "";
                ButtonMapEdit.IsEnabled = false;
                ButtonMapOpen.IsEnabled = false;
                RadioButtonCircle.IsEnabled = false;
                RadioButtonRect.IsEnabled = false;
                RadioButtonEllips.IsEnabled = false;
                SliderViweAngle.IsEnabled = false;
                TextBoxCoveringPercentage.IsEnabled = false;
                TextBoxPositionX.IsEnabled = false;
                TextBoxPositionY.IsEnabled = false;
                CheckBox.IsEnabled = false;
            }
            if (_isFile) {
                if (RadioButton.IsChecked != null && !RadioButton.IsChecked.Value && RadioButton1.IsChecked != null &&
                    !RadioButton1.IsChecked.Value) MessageBox.Show("Kérem válassza ki a robot típusát!", "Figyelmeztetés");
                else if (RadioButton.IsChecked != null && RadioButton.IsChecked.Value)
                    if (TextBoxPositionX.Text.Length != 0 && TextBoxPositionY.Text.Length != 0 &&
                        TextBoxCoveringPercentage.Text.Length != 0) {
                        _robot = new Robot(20, Convert.ToInt32(TextBoxPositionX.Text),
                                           Convert.ToInt32(TextBoxPositionY.Text),
                                           Convert.ToInt32(TextBoxCoveringPercentage.Text), 90, "fan50A.csv", MapRefresh,
                                           _isFile);
                        Console.WriteLine(@"Robooot");
                        SimulateAlgos.setRobot(ref _robot);
                        Console.WriteLine(@"Cover in robot.cs" + _robot.Cover);

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
                        _robot = new Robot(41, Convert.ToInt32(TextBoxPositionX.Text),
                                           Convert.ToInt32(TextBoxPositionY.Text),
                                           Convert.ToInt32(TextBoxCoveringPercentage.Text), 90, "fan100.csv", MapRefresh,
                                           _isFile);
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
                                   Convert.ToInt32(TextBoxCoveringPercentage.Text), 223, MapRefresh, _isFile);
                Console.WriteLine(@"Robooot1");
                SimulateAlgos.setRobot(ref _robot);
                Console.WriteLine(@"Cover in robot.cs " + _robot.Cover);
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

            if (_robot != null) {
                MapRefresh(_isFile);

                if (RadioButtonGenetic.IsChecked != null && RadioButtonGenetic.IsChecked.Value) MessageBox.Show("Generikus");
                else if (RadioButtonHeuristic1.IsChecked != null && RadioButtonHeuristic1.IsChecked.Value) {
                    _mapToBool = BitmapToBools(_myOriginalMap);
                    Alg.start(ref _robot, ref _mapToBool, ref _myBitmapSource, MapRefresh);
                    //System.Threading.Thread.Sleep(500);
                }
                else if (RadioButtonHeuristic2.IsChecked != null && RadioButtonHeuristic2.IsChecked.Value) {
                    _mapToBool = BitmapToBools(_myOriginalMap);

                    _mapcover.setStart(_robot.X, _robot.Y);
                    _mapcover.setRadius(_robot.Radius);
                    _mapcover.setCover(_robot.Cover);

                    _mapcover.fullMapCover();
                    _mapcover.obstacleCover(_mapToBool);
                    _mapcover.createMap(_mapToBool);
                    _mapcover.isTheMapOptimized(_mapToBool);
                    _mapcover.mapOptimizedCover();
                    _mapcover.createFullmap(_mapToBool);

                    _creategraph.creatcoor(_mapcover.getNumber(), _mapcover.fullmap);
                    _creategraph.adjMatrixCreator(_mapcover.getNumber(), _mapcover.fullmap, _robot.X, _robot.Y);
                    _creategraph.runTSP(_robot.X, _robot.Y, _mapcover.getNumber());
                    _creategraph.runTSPmodif();
                    _creategraph.calculateAngle();

                    _robot.Route = _creategraph.fullList;
                    _robot.ExecuteRobot();

                    _mapcover.cleanUp();
                    _creategraph.cleanUp();
                }
            }
        }

        private static bool[,] BitmapToBools(BitmapSource map) {
            var stride = map.PixelWidth * 4;
            var size = map.PixelHeight * stride;
            var pixels = new byte[size];
            map.CopyPixels(pixels, stride, 0);
            var retVal = new bool[MyImageSizeX, MyImageSizeY];

            for (var i = 0; i < MyImageSizeX; i++)
                for (var j = 0; j < MyImageSizeY; j++)
                    if (pixels[i * 4 + j * 640 * 4] == 255 && pixels[i * 4 + j * 640 * 4 + 1] == 255 &&
                        pixels[i * 4 + j * 640 * 4 + 2] == 255 && pixels[i * 4 + j * 640 * 4 + 3] == 255) retVal[j, i] = true;
                    else retVal[j, i] = false;
            return retVal;
        }

        /// <summary>
        ///     Bármilyen robot mozgás után a térkép frissítésére van szükség. Az előző helyről eltünteni illetve
        ///     az új helyre lerakni a robotot. A method paramétertől függően forgatás és mozgatás következik be illetve
        ///     ha nem fájlból olvastuk a robotot akkor újra lesz rajzolva.
        /// </summary>
        private void MapRefresh(bool nothing) {
            if (_robot != null)
                if (_myBitmapSource != null) {
                    if (nothing) {
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
                        //_myOriginalMap.CopyPixels(_pixels, 640 * 4, 0);

                        //_myBitmapSource = BitmapSource.Create(MyImageSizeX, MyImageSizeY, 96, 96, PixelFormats.Pbgra32,
                        //                                   null, _pixels, MyImageSizeX * 4);


                        ShapeDrawer.DrawCircle(_robot.X, _robot.Y, _robot.Radius, _robot.Theta, _robot.Theta + 270,
                                               ref _pixels, true);

                        var xstart = _robot.X + _robot.Radius * Math.Cos(Math.PI / 180.0 * _robot.Theta);
                        var ystart = _robot.Y - _robot.Radius * Math.Sin(Math.PI / 180.0 * _robot.Theta);

                        var xend = _robot.X + _robot.Radius * Math.Cos(Math.PI / 180.0 * (_robot.Theta + 270));
                        var yend = _robot.Y - _robot.Radius * Math.Sin(Math.PI / 180.0 * (_robot.Theta + 270));

                        if (_robot.Theta + 270 > 360)
                            ShapeDrawer.DrawCircle(_robot.X, _robot.Y, _robot.Radius, 0, _robot.Theta + 270 - 360,
                                                   ref _pixels, true);

                        ShapeDrawer.DrawLine(_robot.X, _robot.Y, (int) Math.Round(xstart), (int) Math.Round(ystart),
                                             ref _pixels, true);
                        ShapeDrawer.DrawLine(_robot.X, _robot.Y, (int) Math.Round(xend), (int) Math.Round(yend),
                                             ref _pixels, true);

                        //var r = new Random();
                        var randAngle = _robot.Theta + 0.2 * (_robot.Theta + 270 - _robot.Theta);
                        var randRadius = 0.2 * _robot.Radius;
                        var randX = _robot.X + randRadius * Math.Cos(Math.PI / 180.0 * randAngle);
                        var randY = _robot.Y - randRadius * Math.Sin(Math.PI / 180.0 * randAngle);

                        if ((int) randX > 638) randX = 638;
                        if ((int) randY > 638) randY = 638;

                        if ((int) randX <= 0) randX = 1;
                        if ((int) randY <= 0) randY = 1;

                        ShapeDrawer.FloodFill(ref _pixels, new Point((int) Math.Floor(randX), (int) Math.Floor(randY)),
                                              true);

                        _myBitmapSource = BitmapSource.Create(640, 640, 96, 96, PixelFormats.Pbgra32, null, _pixels,
                                                              640 * 4);

                        Image.Source = _myBitmapSource;
                    }
                    _robot.SetCurrentlyCoveredArea(_myBitmapSource);
                }
                else MessageBox.Show("Először töltsön be egy térképet!", "Figyelmeztetés");
            else MessageBox.Show("Kérem válassza ki a robot típusát!", "Figyelmeztetés");
        }

        /// <summary>
        ///     Mikor a szerkesztés gombot lenyomtuk és mikor ez a gomb aktív akkor az image kattintható.
        ///     Ekkor kört, téglalapot , ellipszist tudunk rajzolni a térképre melynek a közzéppontja az
        ///     image-en kattintott pont lesz. Előjön egy dialógusablak melyben a különféle alakzatokat tudjuk paraméterezni.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageButton_Click(object sender, RoutedEventArgs e) {
            if (ButtonMapEdit.IsEnabled) {
                var p1 = Mouse.GetPosition(this);
                _myBitmapSource.CopyPixels(_pixels, 640 * 4, 0);
                var dialog = new Dialog();
                if (RadioButtonCircle.IsChecked != null && RadioButtonCircle.IsChecked.Value) {
                    dialog.Label.Content = "Sugár";
                    if (dialog.ShowDialog() == true) {
                        ShapeDrawer.DrawCircle((int) p1.X, (int) p1.Y, Convert.ToInt32(dialog.ResponseText), ref _pixels);
                        ShapeDrawer.FloodFill(ref _pixels, p1, false);
                    }
                }
                else if (RadioButtonRect.IsChecked != null && RadioButtonRect.IsChecked.Value) {
                    dialog.Label.Content = "Magasság";
                    dialog.Label1.Visibility = Visibility.Visible;
                    dialog.TextBox1.Visibility = Visibility.Visible;
                    if (dialog.ShowDialog() == true) {
                        ShapeDrawer.DrawRectangle((int) p1.X, (int) p1.Y, Convert.ToInt32(dialog.ResponseText),
                                                  Convert.ToInt32(dialog.ResponseText1), ref _pixels, false);
                        ShapeDrawer.FloodFill(ref _pixels, p1, false);
                    }
                }
                else if (RadioButtonEllips.IsChecked != null && RadioButtonEllips.IsChecked.Value) {
                    dialog.Label.Content = "Magasság";
                    dialog.Label1.Visibility = Visibility.Visible;
                    dialog.TextBox1.Visibility = Visibility.Visible;
                    if (dialog.ShowDialog() == true) {
                        ShapeDrawer.DrawEllipse((int) p1.X, (int) p1.Y, Convert.ToInt32(dialog.ResponseText1),
                                                Convert.ToInt32(dialog.ResponseText), ref _pixels);
                        ShapeDrawer.FloodFill(ref _pixels, p1, false);
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

            SliderViweAngle.IsEnabled = true;
            TextBoxCoveringPercentage.IsEnabled = true;
            TextBoxPositionX.IsEnabled = true;
            TextBoxPositionY.IsEnabled = true;
            CheckBox.IsEnabled = true;
            _robot = null;
            _myBitmapSource = null;
            _myOriginalMap = null;
            _map = null;

            Image.Source = null;
        }

        private void button_Click_1(object sender, RoutedEventArgs e) {
            //TODO Nullpointer exceptions if I click
            double angle;
            if (!_isFile) angle = Convert.ToInt32(TextBoxTeszt.Text);
            else angle = Convert.ToInt32(TextBoxTeszt.Text) * Math.PI / 180.0;

            _robot.Reposition(Convert.ToInt32(TextBoxPositionX.Text), Convert.ToInt32(TextBoxPositionY.Text), angle);
            MapRefresh(_isFile);
        }


        private void button1_Click(object sender, RoutedEventArgs e) {
            _robot.ExecuteRobot();
        }

        /// <summary>
        ///     Szerkeszt gomb megnyomására visszaállítunk mindent "alaphelyzetbe" és egy teljesen
        ///     fekete térképet kapunk. ERre tudunk akadályokat rakni.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            RadioButton.IsEnabled = true;
            RadioButton1.IsEnabled = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e) {
            _isFile = false;
            RadioButton.IsEnabled = false;
            RadioButton1.IsEnabled = false;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e) {
            var dlg = new SaveFileDialog();

            dlg.FileName = "map";
            dlg.DefaultExt = ".csv";
            dlg.Filter = "CSV Files|*.csv";
            dlg.Title = "Save MAP";
            dlg.InitialDirectory = Assembly.GetExecutingAssembly().Location;

            var result = dlg.ShowDialog();

            if (result == true) {
                if (_myOriginalMap != null) {
                    var writer = new MapToCsv(dlg.FileName, _myOriginalMap);

                    writer.Write();
                }
                else MessageBox.Show("Nem lehet menteni! Nincs kirajzolva aktuális térkép!", "Figyelmeztetés");
            }
        }

        private void BtnQuit_Click(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }
    }
}