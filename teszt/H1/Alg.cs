using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace RobotMover
{
    class Alg
    {
        public static int[,] myIntMap;
        public static BitmapSource myBitMap;
        public static Action<bool> _MapRefresh;

        /// <summary>
        ///     Bresenham algoritmust megvalosito metodus.
        ///     Parameterei a kezdo- es vegpont kordinatai, egy pontlista, es a terkep (int, jelenleg nem hasznalja az algoritmus)
        ///     Az algoritmus a ket koordinata kozti pontokat a listaba helyezi.
        /// </summary>
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
                //TODO: fix theta
                list.Add(new PointHOne(x, y, 0));

                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    x += dx1;
                    y += dy1;
                }
                else
                {
                    x += dx2;
                    y += dy2;
                }
            }
        }

        /// <summary>
        ///     H1 csapat kodjat elindito metodus.
        ///     A metodust a foprogrambol hibjuk meg (MainWindow.xaml.cs).
        ///     Innen kerul meghivasra az utkereses, es innen adjuk at a szukseges valtozokat/peldanyokat (map, robot)
        ///     Paramterei: robot referencia, terkep referencia (bool), bitmap terkep referencia, MapRefresh metodus ref (jelenleg nem hasznalt)
        /// </summary>
        public static void start(ref Robot robot, ref bool[,] map, ref BitmapSource bms, Action<bool> MapRefresh)
        {
            Alg.myBitMap = bms;
            Alg.myIntMap = BoolToIntMap(map);
            Alg._MapRefresh = MapRefresh;
            MessageHandler.Write("A robot elindult.");
            new Way(ref myIntMap, ref myBitMap, ref robot);
            MessageHandler.Write("A térkép bejárása véget ért.");
        }

        /// <summary>
        ///     Egyszeru konvertalo metodus, ami a bool terkepunket alakitja at int terkeppe.
        ///     Ez azert szukseges, mert a mi csapatunk sulyozza a bejart utat, igy szukseg van 1-nel nagyobb
        ///     szamokra is az utkereses soran.
        ///     Parametere a bool tipusu terkep, visszateresi erteke az eloallitott int terkep.
        /// </summary>
        public static int[,] BoolToIntMap(bool[,] boolmap)
        {
            var intmap = new int[640, 640];
            for (int i = 0; i < 640; i++)
            {
                for (int j = 0; j < 640; j++)
                {
                    if (boolmap[i, j]) intmap[i, j] = 1;
                    else intmap[i, j] = 0;
                }
            }
            return intmap;
        }
    }
}
