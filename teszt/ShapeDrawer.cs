using System;

namespace teszt {
    internal class ShapeDrawer {
        public static void DrawCircle(int x0, int y0, int radius, ref byte[] pixels) {
            var x = radius;
            var y = 0;
            var decisionOver2 = 1 - x;

            while (x >= y) {
                if (x0 + x >= 0 && x0 + x <= 640 - 1 && y0 + y >= 0 && y0 + y <= 640 - 1) DrawPixel(x + x0, y + y0, ref pixels);
                if (x0 + x >= 0 && x0 + x <= 640 - 1 && y0 - y >= 0 && y0 - y <= 640 - 1) DrawPixel(x + x0, -y + y0, ref pixels);
                if (x0 - x >= 0 && x0 - x <= 640 - 1 && y0 + y >= 0 && y0 + y <= 640 - 1) DrawPixel(-x + x0, y + y0, ref pixels);
                if (x0 - x >= 0 && x0 - x <= 640 - 1 && y0 - y >= 0 && y0 - y <= 640 - 1) DrawPixel(-x + x0, -y + y0, ref pixels);
                if (x0 + y >= 0 && x0 + y <= 640 - 1 && y0 + x >= 0 && y0 + x <= 640 - 1) DrawPixel(y + x0, x + y0, ref pixels);
                if (x0 + y >= 0 && x0 + y <= 640 - 1 && y0 - x >= 0 && y0 - x <= 640 - 1) DrawPixel(y + x0, -x + y0, ref pixels);
                if (x0 - y >= 0 && x0 - y <= 640 - 1 && y0 + x >= 0 && y0 + x <= 640 - 1) DrawPixel(-y + x0, x + y0, ref pixels);
                if (x0 - y >= 0 && x0 - y <= 640 - 1 && y0 - x >= 0 && y0 - x <= 640 - 1) DrawPixel(-y + x0, -x + y0, ref pixels);
                y++;
                if (decisionOver2 <= 0) decisionOver2 += 2 * y + 1;
                else {
                    x--;
                    decisionOver2 += 2 * (y - x) + 1;
                }
            }
        }

        public static void DrawEllipse(int xc, int yc, int width, int height, ref byte[] pixels) {
            var a2 = width * width;
            var b2 = height * height;
            int fa2 = 4 * a2, fb2 = 4 * b2;
            int x, y, sigma;
            for (x = 0, y = height, sigma = 2 * b2 + a2 * (1 - 2 * height); b2 * x <= a2 * y; x++) {
                DrawPixel(xc + x, yc + y, ref pixels);
                DrawPixel(xc - x, yc + y, ref pixels);
                DrawPixel(xc + x, yc - y, ref pixels);
                DrawPixel(xc - x, yc - y, ref pixels);
                if (sigma >= 0) {
                    sigma += fa2 * (1 - y);
                    y--;
                }
                sigma += b2 * (4 * x + 6);
            }
            for (x = width, y = 0, sigma = 2 * a2 + b2 * (1 - 2 * width); a2 * y <= b2 * x; y++) {
                DrawPixel(xc + x, yc + y, ref pixels);
                DrawPixel(xc - x, yc + y, ref pixels);
                DrawPixel(xc + x, yc - y, ref pixels);
                DrawPixel(xc - x, yc - y, ref pixels);
                if (sigma >= 0) {
                    sigma += fb2 * (1 - x);
                    x--;
                }
                sigma += a2 * (4 * y + 6);
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

        public static void DrawRectangle(int x, int y, int length, int breadth, ref byte[] pixels) {
            DrawLine(x - breadth / 2, y + length / 2, x + breadth / 2, y + length / 2, ref pixels);
            DrawLine(x + breadth / 2, y + length / 2, x + breadth / 2, y - length / 2, ref pixels);
            DrawLine(x + breadth / 2, y - length / 2, x - breadth / 2, y - length / 2, ref pixels);
            DrawLine(x - breadth / 2, y - length / 2, x - breadth / 2, y + length / 2, ref pixels);
        }

        /*
        public void DrawTriangle(int a1, int a2, int b1, int b2, int c1, int c2, ref byte[] pixels) {
            DrawLine(a1, a2, b1, b2, ref pixels);
            DrawLine(b1, b2, c1, c2, ref pixels);
            DrawLine(c1, c2, a1, a2, ref pixels);
        }
*/

        private static bool CanDraw(double theta, double startangle, double endangle) {
            return theta >= startangle && theta <= endangle;
        }

        private static void CirclePoints(int x, int y, int xc, int yc, double startangle, double endangle,
                                         ref byte[] pixels) {
            var theta = Math.Atan((double) y / x);
            theta = theta * (180 / Math.PI);


            if (CanDraw(theta, startangle, endangle)) DrawPixel(xc + x, yc - y, ref pixels);
            if (CanDraw(360 - theta, startangle, endangle)) DrawPixel(xc + x, yc + y, ref pixels);

            if (CanDraw(90 - theta, startangle, endangle)) DrawPixel(xc + y, yc - x, ref pixels);
            if (CanDraw(270 + theta, startangle, endangle)) DrawPixel(xc + y, yc + x, ref pixels);

            if (CanDraw(180 - theta, startangle, endangle)) DrawPixel(xc - x, yc - y, ref pixels);
            if (CanDraw(180 + theta, startangle, endangle)) DrawPixel(xc - x, yc + y, ref pixels);

            if (CanDraw(90 + theta, startangle, endangle)) DrawPixel(xc - y, yc - x, ref pixels);
            if (CanDraw(270 - theta, startangle, endangle)) DrawPixel(xc - y, yc + x, ref pixels);
        }

        public static void DrawCircle(int x0, int y0, int radius, double startangle, double endangle, ref byte[] pixels) {
            var x = radius;
            var y = 0;
            var decisionOver2 = 1 - x;

            while (x >= y) {
                CirclePoints(x, y, x0, y0, startangle, endangle, ref pixels);
                y++;
                if (decisionOver2 <= 0) decisionOver2 += 2 * y + 1;
                else {
                    x--;
                    decisionOver2 += 2 * (y - x) + 1;
                }
            }
        }

        public static void DrawLine(int x0, int y0, int x1, int y1, ref byte[] pixels) {
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
    }
}