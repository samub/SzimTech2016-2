using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace RobotMover {
    internal class MapToCsv {
        private readonly BitmapSource _map;
        private readonly StreamWriter _sw;

        public MapToCsv(string filename, BitmapSource map) {
            var fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            _sw = new StreamWriter(fs);
            _map = map;
        }

        public void Write() {
            var stride = _map.PixelWidth * 4;
            var size = _map.PixelHeight * stride;
            var pixels = new byte[size];
            _map.CopyPixels(pixels, stride, 0);
            for (var i = 0; i < 640; i++) {
                for (var j = 0; j < 640; j++) {
                    if (pixels[j * 4 + i * 4 * 640] == 255 && pixels[j * 4 + i * 4 * 640 + 1] == 255 &&
                        pixels[j * 4 + i * 4 * 640 + 2] == 255 && pixels[j * 4 + i * 4 * 640 + 3] == 255) {
                        _sw.Write(j < 639 ? "1;" : "1");
                    }
                    else {
                        _sw.Write(j < 639 ? "0;" : "0");
                    }
                }
                _sw.Write(Environment.NewLine);
            }
            _sw.Close();
        }
    }
}