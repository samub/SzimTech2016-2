using System;
using System.Collections.Generic;
using System.Windows;

namespace teszt {
    internal static class ShapeDrawer {
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
                if (xc + x >= 0 && xc + x <= 640 - 1 && yc + y >= 0 && yc + y <= 640 - 1) DrawPixel(xc + x, yc + y, ref pixels);
                if (xc - x >= 0 && xc - x <= 640 - 1 && yc + y >= 0 && yc + y <= 640 - 1) DrawPixel(xc - x, yc + y, ref pixels);
                if (xc + x >= 0 && xc + x <= 640 - 1 && yc - y >= 0 && yc - y <= 640 - 1) DrawPixel(xc + x, yc - y, ref pixels);
                if (xc - x >= 0 && xc - x <= 640 - 1 && yc - y >= 0 && yc - y <= 640 - 1) DrawPixel(xc - x, yc - y, ref pixels);
                if (sigma >= 0) {
                    sigma += fa2 * (1 - y);
                    y--;
                }
                sigma += b2 * (4 * x + 6);
            }
            for (x = width, y = 0, sigma = 2 * a2 + b2 * (1 - 2 * width); a2 * y <= b2 * x; y++) {
                if (xc + x >= 0 && xc + x <= 640 - 1 && yc + y >= 0 && yc + y <= 640 - 1) DrawPixel(xc + x, yc + y, ref pixels);
                if (xc - x >= 0 && xc - x <= 640 - 1 && yc + y >= 0 && yc + y <= 640 - 1) DrawPixel(xc - x, yc + y, ref pixels);
                if (xc + x >= 0 && xc + x <= 640 - 1 && yc - y >= 0 && yc - y <= 640 - 1) DrawPixel(xc + x, yc - y, ref pixels);
                if (xc - x >= 0 && xc - x <= 640 - 1 && yc - y >= 0 && yc - y <= 640 - 1) DrawPixel(xc - x, yc - y, ref pixels);
                if (sigma >= 0) {
                    sigma += fb2 * (1 - x);
                    x--;
                }
                sigma += a2 * (4 * y + 6);
            }
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

        public static void FloodFill(ref byte[] pixels, Point pt) {
            var q = new Queue<Point>();
            q.Enqueue(pt);
            while (q.Count > 0) {
                var n = q.Dequeue();
                if (pixels[(int) (n.X * 4 + n.Y * 4 * 640)] == 255) continue;
                Point w = n, e = new Point(n.X + 1, n.Y);
                while ((w.X >= 0) && pixels[(int) (w.X * 4 + w.Y * 4 * 640)] != 255) {
                    DrawPixel((int) w.X, (int) w.Y, ref pixels);
                    if ((w.Y > 0) && pixels[(int) (w.X * 4 + w.Y * 4 * 640 - 1 * 4 * 640)] != 255) q.Enqueue(new Point(w.X, w.Y - 1));
                    if ((w.Y < 640 - 1) && pixels[(int) (w.X * 4 + w.Y * 4 * 640 + 1 * 4 * 640)] != 255) q.Enqueue(new Point(w.X, w.Y + 1));
                    w.X--;
                }
                while ((e.X <= 640 - 1) && pixels[(int) (e.X * 4 + e.Y * 4 * 640)] != 255) {
                    DrawPixel((int) e.X, (int) e.Y, ref pixels);
                    if ((e.Y > 0) && pixels[(int) (e.X * 4 + e.Y * 4 * 640 - 1 * 4 * 640)] != 255) q.Enqueue(new Point(e.X, e.Y - 1));
                    if ((e.Y < 640 - 1) && pixels[(int) (e.X * 4 + e.Y * 4 * 640 + 1 * 4 * 640)] != 255) q.Enqueue(new Point(e.X, e.Y + 1));
                    e.X++;
                }
            }
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
            var steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep) {
                Swap(ref x0, ref y0);
                Swap(ref x1, ref y1);
            }
            if (x0 > x1) {
                Swap(ref x0, ref x1);
                Swap(ref y0, ref y1);
            }
            int dX = x1 - x0, dY = Math.Abs(y1 - y0), err = dX / 2, ystep = y0 < y1 ? 1 : -1, y = y0;

            for (var x = x0; x <= x1; ++x) {
                if (steep) {
                    if (x >= 0 && x <= 640 - 1 && y >= 0 && y <= 640 - 1) DrawPixel(y, x, ref pixels);
                    if (x >= 0 && x <= 640 - 1 && y >= 0 && y <= 640 - 1) DrawPixel(y, x, ref pixels);
                    if (x >= 0 && x <= 640 - 1 && y >= 0 && y <= 640 - 1) DrawPixel(y, x, ref pixels);
                    if (x >= 0 && x <= 640 - 1 && y >= 0 && y <= 640 - 1) DrawPixel(y, x, ref pixels);
                    if (y >= 0 && y <= 640 - 1 && x >= 0 && x <= 640 - 1) DrawPixel(y, x, ref pixels);
                    if (y >= 0 && y <= 640 - 1 && x >= 0 && x <= 640 - 1) DrawPixel(y, x, ref pixels);
                    if (y >= 0 && y <= 640 - 1 && x >= 0 && x <= 640 - 1) DrawPixel(y, x, ref pixels);
                    if (y >= 0 && y <= 640 - 1 && x >= 0 && x <= 640 - 1) DrawPixel(y, x, ref pixels);
                }
                else {
                    if (x >= 0 && x <= 640 - 1 && y >= 0 && y <= 640 - 1) DrawPixel(x, y, ref pixels);
                    if (x >= 0 && x <= 640 - 1 && y >= 0 && y <= 640 - 1) DrawPixel(x, y, ref pixels);
                    if (x >= 0 && x <= 640 - 1 && y >= 0 && y <= 640 - 1) DrawPixel(x, y, ref pixels);
                    if (x >= 0 && x <= 640 - 1 && y >= 0 && y <= 640 - 1) DrawPixel(x, y, ref pixels);
                    if (y >= 0 && y <= 640 - 1 && x >= 0 && x <= 640 - 1) DrawPixel(x, y, ref pixels);
                    if (y >= 0 && y <= 640 - 1 && x >= 0 && x <= 640 - 1) DrawPixel(x, y, ref pixels);
                    if (y >= 0 && y <= 640 - 1 && x >= 0 && x <= 640 - 1) DrawPixel(x, y, ref pixels);
                    if (y >= 0 && y <= 640 - 1 && x >= 0 && x <= 640 - 1) DrawPixel(x, y, ref pixels);
                }
                err = err - dY;
                if (err < 0) {
                    y += ystep;
                    err += dX;
                }
            }
        }

        private static void Swap<T>(ref T lhs, ref T rhs) {
            var temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
    }
}