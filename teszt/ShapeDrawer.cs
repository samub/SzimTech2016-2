using System;
using System.Collections.Generic;
using System.Windows;

namespace RobotMoverGUI {
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
                pixels[startPixel + x * 4] = 255; // r
                pixels[startPixel + x * 4 + 1] = 255; // g 
                pixels[startPixel + x * 4 + 2] = 255; // b 
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
                    pixels[startPixel + x * 4] = 255; // r
                    pixels[startPixel + x * 4 + 1] = 255; // g 
                    pixels[startPixel + x * 4 + 2] = 255; // b 
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
                    pixels[startPixel + x * 4] = 255; // r
                    pixels[startPixel + x * 4 + 1] = 255; // g 
                    pixels[startPixel + x * 4 + 2] = 255; // b 
                    pixels[startPixel + x * 4 + 3] = 255; // alpha 
                }
            }
            return pixels;
            //ha 3 pont van --> lezárható esetleg az alakzat és ki is lehet tölteni

            //klikk számolás!!! hogy tudjam hány sarka vagy éle vagy akármilye van az alaknak
        }

        public static void DrawCircle(int x0, int y0, int radius, ref byte[] pixels) {
            var x = radius;
            var y = 0;
            var decisionOver2 = 1 - x;

            while (x >= y) {
                DrawPixel(x + x0, y + y0, ref pixels);
                DrawPixel(y + x0, x + y0, ref pixels);
                DrawPixel(-x + x0, y + y0, ref pixels);
                DrawPixel(-y + x0, x + y0, ref pixels);
                DrawPixel(-x + x0, -y + y0, ref pixels);
                DrawPixel(-y + x0, -x + y0, ref pixels);
                DrawPixel(x + x0, -y + y0, ref pixels);
                DrawPixel(y + x0, -x + y0, ref pixels);
                y++;
                if (decisionOver2 <= 0) decisionOver2 += 2 * y + 1;
                else {
                    x--;
                    decisionOver2 += 2 * (y - x) + 1;
                }
            }
        }

        public static void BasicFill(ref byte[] array, int x, int y) {
            array[y * 4 * 640 + x * 4] = 255;
            array[y * 4 * 640 + x * 4 + 1] = 255;
            array[y * 4 * 640 + x * 4 + 2] = 255;
            array[y * 4 * 640 + x * 4 + 3] = 255;
            if (y > 0 && array[y * 4 * 640 - 1 * 4 * 640 + x * 4] != 255) BasicFill(ref array, x, y - 1);
            if (x > 0 && array[y * 4 * 640 + x * 4 - 1 * 4] != 255) BasicFill(ref array, x - 1, y);
            if (x < 640 - 1 && array[y * 4 * 640 + x * 4 + 1 * 4] != 255) BasicFill(ref array, x + 1, y);
            if (y < 640 - 1 && array[y * 4 * 640 + 1 * 4 * 640 + x * 4] != 255) BasicFill(ref array, x, y + 1);
        }

        private static void DrawPixel(int x, int y, ref byte[] pixels) {
            var idx = x * 4 + y * 4 * 640;
            pixels[idx] = 255;
            pixels[idx + 1] = 255;
            pixels[idx + 2] = 255;
            pixels[idx + 3] = 255;
        }

        public void DrawRectangle(int x, int y, int length, int breadth, ref byte[] pixels) {
            DrawLine(x - breadth / 2, y + length / 2, x + breadth / 2, y + length / 2, ref pixels);
            DrawLine(x + breadth / 2, y + length / 2, x + breadth / 2, y - length / 2, ref pixels);
            DrawLine(x + breadth / 2, y - length / 2, x - breadth / 2, y - length / 2, ref pixels);
            DrawLine(x - breadth / 2, y - length / 2, x - breadth / 2, y + length / 2, ref pixels);
            /*DrawLine(x, y, x + length, y, ref pixels);
            DrawLine(x + length, y, x + length, y - breadth, ref pixels);
            DrawLine(x + length, y - breadth, x, y - breadth, ref pixels);
            DrawLine(x, y - breadth, x, y, ref pixels);*/
        }

        public void DrawTriangle(int a1, int a2, int b1, int b2, int c1, int c2, ref byte[] pixels) {
            DrawLine(a1, a2, b1, b2, ref pixels);
            DrawLine(b1, b2, c1, c2, ref pixels);
            DrawLine(c1, c2, a1, a2, ref pixels);
        }

        private void DrawLine(int x0, int y0, int x1, int y1, ref byte[] pixels) {
            var dx = Math.Abs(x1 - x0);
            var sx = x0 < x1 ? 1 : -1;
            var dy = Math.Abs(y1 - y0);
            var sy = y0 < y1 ? 1 : -1;
            var err = (dx > dy ? dx : -dy) / 2;

            while (true) {
                DrawPixel(x0, y0, ref pixels);
                if (x0 == x1 && y0 == y1) break;
                var e2 = err;
                if (e2 > -dx) {
                    err -= dy;
                    x0 += sx;
                }
                if (e2 < dy) {
                    err += dx;
                    y0 += sy;
                }
            }
        }

        private void LineCalc(int x, int y, int x2, int y2) {
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