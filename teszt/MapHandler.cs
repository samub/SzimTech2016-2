using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace RobotMover
{
    class MapHandler
    {
        //TODO fix Image!!
        private void MapRefresh(Robot _robot, BitmapSource _myBitmapSource, Boolean _isFile, byte[] _pixels, BitmapSource _myOriginalMap,int MyImageSizeX, int MyImageSizeY, Object Image)
        {
            if (_robot != null)
                if (_myBitmapSource != null)
                {
                    if (_isFile)
                    {
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
                                             startPixelY * 4 * _robot.Robot1.Map.GetLength(1)] == 255)
                                {
                                    _pixels[i * 4 + j * 4 * 640] = 255;
                                    _pixels[i * 4 + j * 4 * 640 + 1] = 255;
                                    _pixels[i * 4 + j * 4 * 640 + 2] = 255;
                                    _pixels[i * 4 + j * 4 * 640 + 3] = 255;
                                }
                                else if (_pixels[i * 4 + j * 4 * 640] == 0 &&
                                         robotPix[
                                                  i * 4 - startPixelX * 4 + j * 4 * _robot.Robot1.Map.GetLength(1) -
                                                  startPixelY * 4 * _robot.Robot1.Map.GetLength(1)] == 255)
                                {
                                    _pixels[i * 4 + j * 4 * 640] = 255;
                                    _pixels[i * 4 + j * 4 * 640 + 1] = 255;
                                    _pixels[i * 4 + j * 4 * 640 + 2] = 255;
                                    _pixels[i * 4 + j * 4 * 640 + 3] = 255;
                                }
                                else if (_pixels[i * 4 + j * 4 * 640] == 0 &&
                                         robotPix[
                                                  i * 4 - startPixelX * 4 + j * 4 * _robot.Robot1.Map.GetLength(1) -
                                                  startPixelY * 4 * _robot.Robot1.Map.GetLength(1)] == 0)
                                {
                                    _pixels[i * 4 + j * 4 * 640] = 0;
                                    _pixels[i * 4 + j * 4 * 640 + 1] = 0;
                                    _pixels[i * 4 + j * 4 * 640 + 2] = 0;
                                    _pixels[i * 4 + j * 4 * 640 + 3] = 255;
                                }
                                else if (_pixels[i * 4 + j * 4 * 640] == 255 &&
                                         robotPix[
                                                  i * 4 - startPixelX * 4 +
                                                  j * 4 * _robot.Robot1.Map.GetLength(1) -
                                                  startPixelY * 4 * _robot.Robot1.Map.GetLength(1)] == 0)
                                {
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

                        ShapeDrawer.DrawLine(_robot.X, _robot.Y, (int)Math.Round(xstart), (int)Math.Round(ystart),
                                             ref _pixels);
                        ShapeDrawer.DrawLine(_robot.X, _robot.Y, (int)Math.Round(xend), (int)Math.Round(yend),
                                             ref _pixels);

                        //var r = new Random();
                        var randAngle = _robot.Theta + 0.2 * (_robot.Theta + 270 - _robot.Theta);
                        var randRadius = 0.2 * _robot.Radius;
                        var randX = _robot.X + randRadius * Math.Cos(Math.PI / 180.0 * randAngle);
                        var randY = _robot.Y - randRadius * Math.Sin(Math.PI / 180.0 * randAngle);

                        ShapeDrawer.FloodFill(ref _pixels, new Point((int)Math.Floor(randX), (int)Math.Floor(randY)));

                        _myBitmapSource = BitmapSource.Create(640, 640, 96, 96, PixelFormats.Pbgra32, null, _pixels,
                                                              640 * 4);
                        Image.Source = _myBitmapSource;
                    }
                    _robot.SetCurrentlyCoveredArea(_myBitmapSource);
                }
                else MessageBox.Show("Először töltsön be egy térképet!", "Figyelmeztetés");
            else MessageBox.Show("Kérem válassza ki a robot típusát!", "Figyelmeztetés");
        }
    }
}
