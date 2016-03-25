using System;
using System.Collections.Generic;
using System.Windows;

namespace teszt {
    internal class ShapeDrawer {
        private readonly List<Point> _shapePoints = new List<Point>();

        public void AddPointToShape(Point p) {
            _shapePoints.Add(p);
        }

        public byte[] DrawPoints(byte[] pixels, Point nextPoint) {
            var x1 = Convert.ToInt32(_shapePoints[_shapePoints.Count - 1].X);
            var y1 = Convert.ToInt32(_shapePoints[_shapePoints.Count - 1].Y);

            if (_shapePoints.Count == 1) {
                var y = Convert.ToInt32(_shapePoints[0].Y);
                var x = Convert.ToInt32(_shapePoints[0].X);
                var startPixel = y * 640 * 4;
                pixels[startPixel + x * 4] = 0; // r
                pixels[startPixel + x * 4 + 1] = 0; // g 
                pixels[startPixel + x * 4 + 2] = 0; // b 
                pixels[startPixel + x * 4 + 3] = 255; // alpha 
            }
            else if (_shapePoints.Count >= 2) {
                LineCalc(Convert.ToInt32(_shapePoints[_shapePoints.Count - 2].X),
                         Convert.ToInt32(_shapePoints[_shapePoints.Count - 2].Y),
                         Convert.ToInt32(_shapePoints[_shapePoints.Count - 1].X),
                         Convert.ToInt32(_shapePoints[_shapePoints.Count - 1].Y));
                for (var i = 0; i <= _shapePoints.Count - 1; i++) {
                    var y = Convert.ToInt32(_shapePoints[i].Y);
                    var x = Convert.ToInt32(_shapePoints[i].X);
                    var startPixel = y * 640 * 4;
                    pixels[startPixel + x * 4] = 0; // r
                    pixels[startPixel + x * 4 + 1] = 0; // g 
                    pixels[startPixel + x * 4 + 2] = 0; // b 
                    pixels[startPixel + x * 4 + 3] = 255; // alpha 
                }
            }

            else if (((x1 + 10 <= nextPoint.X) && (y1 + 10 <= nextPoint.Y || y1 - 10 >= nextPoint.Y)) ||
                     ((x1 - 10 >= nextPoint.X) && (y1 + 10 <= nextPoint.Y || y1 - 10 >= nextPoint.Y))) {
                _shapePoints.Add(_shapePoints[0]);
                LineCalc(Convert.ToInt32(_shapePoints[_shapePoints.Count - 2].X),
                         Convert.ToInt32(_shapePoints[_shapePoints.Count - 2].Y),
                         Convert.ToInt32(_shapePoints[_shapePoints.Count - 1].X),
                         Convert.ToInt32(_shapePoints[_shapePoints.Count - 1].Y));
                for (var i = 0; i <= _shapePoints.Count - 1; i++) {
                    var y = Convert.ToInt32(_shapePoints[i].Y);
                    var x = Convert.ToInt32(_shapePoints[i].X);
                    var startPixel = y * 640 * 4;
                    pixels[startPixel + x * 4] = 0; // r
                    pixels[startPixel + x * 4 + 1] = 0; // g 
                    pixels[startPixel + x * 4 + 2] = 0; // b 
                    pixels[startPixel + x * 4 + 3] = 255; // alpha 
                }
            }
            return pixels;
            //ha 3 pont van --> lezárható esetleg az alakzat és ki is lehet tölteni

            //klikk számolás!!! hogy tudjam hány sarka vagy éle vagy akármilye van az alaknak
        }

        public void LineCalc(int x, int y, int x2, int y2) {
            var w = x2 - x;
            var h = y2 - y;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1;
            else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1;
            else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1;
            else if (w > 0) dx2 = 1;
            var longest = Math.Abs(w);
            var shortest = Math.Abs(h);
            if (!(longest > shortest)) {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1;
                else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            var numerator = longest >> 1;
            for (var i = 0; i <= longest; i++) {
                _shapePoints.Insert(_shapePoints.Count - 2, new Point(x, y));
                numerator += shortest;
                if (!(numerator < longest)) {
                    numerator -= longest;
                    x += dx1;
                    y += dy1;
                }
                else {
                    x += dx2;
                    y += dy2;
                }
            }
        }
    }
}