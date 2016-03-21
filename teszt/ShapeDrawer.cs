using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        public void DrawPoints (Point p)
        {
            //ennek olyan visszatérési érték kell, amit egyből rá lehet tenni az Image-re
        }

    }
}
