using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace teszt
{
    class ShapeDrawer
    {

        List<Point> ShapePoints = new List<Point>();

        public void AddPointToShape (Point p)
        {
            ShapePoints.Add(p);
        }

        public byte[] DrawPoints (byte[] pixels, Point NextPoint)
        {
            int x1 = Convert.ToInt32(ShapePoints[ShapePoints.Count - 1].X);
            int y1 = Convert.ToInt32(ShapePoints[ShapePoints.Count - 1].Y);

            if (ShapePoints.Count == 1)
            {
                int y = Convert.ToInt32(ShapePoints[0].Y) - 51;
                int x = Convert.ToInt32(ShapePoints[0].X) - 11;
                int startPixel = y * 640 * 4;
                pixels[startPixel + x * 4] = 0; // r
                pixels[startPixel + x * 4 + 1] = 0; // g 
                pixels[startPixel + x * 4 + 2] = 0; // b 
                pixels[startPixel + x * 4 + 3] = 255; // alpha 
                
            }
            else if (ShapePoints.Count >= 2)
            {
                LineCalc(Convert.ToInt32(ShapePoints[ShapePoints.Count-2].X), Convert.ToInt32(ShapePoints[ShapePoints.Count - 2].Y), Convert.ToInt32(ShapePoints[ShapePoints.Count - 1].X), Convert.ToInt32(ShapePoints[ShapePoints.Count - 1].Y));
                for (int i = 0; i<=ShapePoints.Count-1; i++)
                {
                    int y = Convert.ToInt32(ShapePoints[i].Y) - 51;
                    int x = Convert.ToInt32(ShapePoints[i].X) - 11;
                    int startPixel = y * 640 * 4;
                    pixels[startPixel + x * 4] = 0; // r
                    pixels[startPixel + x * 4 + 1] = 0; // g 
                    pixels[startPixel + x * 4 + 2] = 0; // b 
                    pixels[startPixel + x * 4 + 3] = 255; // alpha 
                }
            }
            
            else if (((x1+10 <= NextPoint.X) && (y1 + 10 <= NextPoint.Y || y1 - 10 >= NextPoint.Y)) || ((x1 - 10 >= NextPoint.X) && (y1 + 10 <= NextPoint.Y || y1 - 10 >= NextPoint.Y)))
            {
                ShapePoints.Add(ShapePoints[0]);
                LineCalc(Convert.ToInt32(ShapePoints[ShapePoints.Count - 2].X), Convert.ToInt32(ShapePoints[ShapePoints.Count - 2].Y), Convert.ToInt32(ShapePoints[ShapePoints.Count - 1].X), Convert.ToInt32(ShapePoints[ShapePoints.Count - 1].Y));
                for (int i = 0; i <= ShapePoints.Count - 1; i++)
                {
                    int y = Convert.ToInt32(ShapePoints[i].Y) - 51;
                    int x = Convert.ToInt32(ShapePoints[i].X) - 11;
                    int startPixel = y * 640 * 4;
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

        public void LineCalc(int x, int y, int x2, int y2)
        {
            int w = x2 - x;
            int h = y2 - y;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
            int longest = Math.Abs(w);
            int shortest = Math.Abs(h);
            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            int numerator = longest >> 1;
            for (int i = 0; i <= longest; i++)
            {
                ShapePoints.Insert(ShapePoints.Count - 2, new Point(x, y));
                numerator += shortest;
                if (!(numerator < longest))
                {
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
