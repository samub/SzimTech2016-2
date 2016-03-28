using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace teszt {
    internal class Robot {
        public readonly List<Tuple<int, int, double>> Route = new List<Tuple<int, int, double>>();

        public Robot(int radius, int x, int y, int cover, double theta, string robotName) {
            Robot1 = new CsvToMatrix(robotName);
            Robot1.Read();
            BoolMatrixToBitmap();
            Radius = radius;
            X = x;
            Y = y;
            Cover = cover;
            Theta = theta;
            OriginalCoordinates = new float[0, 0];
        }

        public int X { get; private set; }

        public int Y { get; private set; }

        private float[,] OriginalCoordinates { get; set; }

        public BitmapSource MyBitmap { get; private set; }

        public CsvToMatrix Robot1 { get; }
        public double Theta { get; set; }

        public int Cover { get; set; }

        public int Radius { get; set; }

        private void BoolMatrixToBitmap() {
            var pixels = new byte[Robot1.Map.GetLength(0) * Robot1.Map.GetLength(1) * 4];
            var current = 0;
            for (var i = 0; i < Robot1.Map.GetLength(0); i++)
                for (var j = 0; j < Robot1.Map.GetLength(1); j++)
                    if (Robot1.Map[i, j]) {
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
            MyBitmap = BitmapSource.Create(Robot1.Map.GetLength(0), Robot1.Map.GetLength(1), 96, 96,
                                           PixelFormats.Pbgra32, null, pixels, Robot1.Map.GetLength(0) * 4);
        }

        public void Reposition(int x, int y, double rotAngle) {
            if (x < Robot1.Map.GetLength(0) / 2 || y < Robot1.Map.GetLength(1) / 2 ||
                x > 640 - Robot1.Map.GetLength(0) / 2 || y > 640 - Robot1.Map.GetLength(1) / 2) ;
            else {
                X = x;
                Y = y;

                var stride = MyBitmap.PixelWidth * 4;
                var size = MyBitmap.PixelHeight * stride;
                var pixArray = new byte[size];
                var coordX = new float[0];
                var coordY = new float[0];
                MyBitmap.CopyPixels(pixArray, stride, 0);
                for (var i = 0; i < Robot1.Map.GetLength(0) * 4; i += 4)
                    for (var j = 0; j < Robot1.Map.GetLength(1); j += 1)
                        if (pixArray[i + j * 4 * Robot1.Map.GetLength(1)] == 255) {
                            Array.Resize(ref coordX, coordX.Length + 1);
                            Array.Resize(ref coordY, coordY.Length + 1);

                            coordX[coordX.Length - 1] = i / 4 - MyBitmap.PixelWidth / 2;
                            coordY[coordY.Length - 1] = j - MyBitmap.PixelHeight / 2;
                        }

                if (OriginalCoordinates.Length == 0) {
                    OriginalCoordinates = new float[coordX.Length, 2];
                    for (var i = 0; i < coordX.Length; i++) {
                        OriginalCoordinates[i, 0] = coordX[i];
                        OriginalCoordinates[i, 1] = coordY[i];
                    }
                }

                float[,] rotMatrix = {
                                         {(float) Math.Cos(rotAngle), (float) -Math.Sin(rotAngle)},
                                         {(float) Math.Sin(rotAngle), (float) Math.Cos(rotAngle)}
                                     };

                var resultMatrix = MatrixMult(OriginalCoordinates, rotMatrix);


                for (var i = 0; i < Robot1.Map.GetLength(0) * 4; i += 4)
                    for (var j = 0; j < Robot1.Map.GetLength(1); j = j + 1) {
                        pixArray[i + j * 4 * Robot1.Map.GetLength(1)] = 0; //r
                        pixArray[i + j * 4 * Robot1.Map.GetLength(1) + 1] = 0; // g 
                        pixArray[i + j * 4 * Robot1.Map.GetLength(1) + 2] = 0; // b 
                        pixArray[i + j * 4 * Robot1.Map.GetLength(1) + 3] = 255; // alpha 
                    }


                for (var i = 0; i < resultMatrix.GetLength(0); i++) {
                    var pixelUp = (int) Math.Ceiling(resultMatrix[i, 0] + MyBitmap.PixelHeight / 2) * 4 +
                                  (int) Math.Ceiling(resultMatrix[i, 1] + MyBitmap.PixelWidth / 2) * 4 *
                                  Robot1.Map.GetLength(1);
                    var pixelDown = (int) Math.Floor(resultMatrix[i, 0] + MyBitmap.PixelHeight / 2) * 4 +
                                    (int) Math.Floor(resultMatrix[i, 1] + MyBitmap.PixelWidth / 2) * 4 *
                                    Robot1.Map.GetLength(1);

                    try // TODO: remove try-catch
                    {
                        if (pixelUp > 0 &&
                            pixelUp <= MyBitmap.PixelWidth * 4 + MyBitmap.PixelHeight * 4 * Robot1.Map.GetLength(1)) {
                            pixArray[pixelUp] = 255; // r
                            pixArray[pixelUp + 1] = 255; // g 
                            pixArray[pixelUp + 2] = 255; // b 
                            pixArray[pixelUp + 3] = 255; // alpha 
                        }
                        if (pixelDown > 0 &&
                            pixelDown <= MyBitmap.PixelWidth * 4 + MyBitmap.PixelHeight * 4 * Robot1.Map.GetLength(1)) {
                            pixArray[pixelDown] = 255; // r
                            pixArray[pixelDown + 1] = 255; // g 
                            pixArray[pixelDown + 2] = 255; // b 
                            pixArray[pixelDown + 3] = 255; // alpha 
                        }
                    }
                    catch {
                        // ignored
                    }
                }
                MyBitmap = BitmapSource.Create(Robot1.Map.GetLength(0), Robot1.Map.GetLength(1), 96, 96,
                                               PixelFormats.Pbgra32, null, pixArray, Robot1.Map.GetLength(0) * 4);
            }
        }

        private static float[,] MatrixMult(float[,] a, float[,] b) {
            float[,] c = {};
            if (a.GetLength(1) == b.GetLength(0)) {
                c = new float[a.GetLength(0), b.GetLength(1)];
                for (var i = 0; i < c.GetLength(0); i++)
                    for (var j = 0; j < c.GetLength(1); j++) {
                        c[i, j] = 0;
                        for (var k = 0; k < a.GetLength(1); k++) // vagy k<b.GetLength(0)
                            c[i, j] = c[i, j] + a[i, k] * b[k, j];
                    }
            }
            else MessageBox.Show("Matrix size mismatch.");
            return c;
        }
    }
}