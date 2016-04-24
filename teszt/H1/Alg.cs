using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotMover
{
    class Alg
    {
        static int[,] myIntMap = new int[640, 640];
        
        public static void line(int x, int y, int x2, int y2, List<PointHOne> list, int[,] map)
        {
            
            //x and y represents the starting postition of the line
            //x2 and y2 represents the ending postition of the line
            int xOriginal = x;
            int yOriginal = y;

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
                Console.SetCursorPosition(x, y);
               // if((x==xOriginal && y==yOriginal)) Console.Write("K");
               //else if (x == x2 && y == y2) Console.Write("V");
               //else Console.Write("x");
                //TODO: fix theta
                list.Add(new PointHOne(x, y,map[x,y],0));

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

        public static void start(Robot robot, bool[,] map) {

            MessageBox.Show("H1 start method");
            myIntMap = BoolToIntMap(map); 
			new Way(0.8f, ref myIntMap, ref robot);

        }
        public static int [,] BoolToIntMap(bool[,] boolmap) {
            var intmap = new int[640, 640];
            for (int i = 0; i < 640; i++) {
                for (int j = 0; j < 640; j++) {
                    if (boolmap[i, j]) intmap[i, j] = 1;
                    else intmap[i, j] = 0;
                }
            }
            return intmap;
        }

        public double sulyozas(int x1, int y1, int x2, int y2, int theta) {
            double d = Math.Sqrt(Math.Pow(Math.Abs(x2 - x1), 2) + Math.Pow(Math.Abs(y2 - y1),2));
            List<PointHOne> pontok = new List<PointHOne>();

            line(x1, y1, x2, y2, pontok, Alg.myIntMap);

            int nullak = 0;
            int nemNullaParos = 0;
            foreach (var pont in pontok)
            {
                if (pont.ertek == 0) nullak++;
                if ((pont.ertek != 0) && (pont.ertek % 2 == 0)) nemNullaParos+=Alg.myIntMap[pont.x,pont.y];
            }

            return d + (theta / 4) + nullak - nemNullaParos;
        }
    }
}
