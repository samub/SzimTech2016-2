using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using System.IO;

namespace RobotMover
{
    internal class CreateGraph
    {
        public List<Tuple<int, int>> coordinatesList = new List<Tuple<int, int>>();
        public List<Tuple<int, int, double, int>> tspList = new List<Tuple<int, int, double, int>>();
        public List<Tuple<int, int, double>> fullList = new List<Tuple<int, int, double>>();
        public HashSet<int> nodes = new HashSet<int>();
        private const int m_size = 640;
        private int[,] coormap;
        public double[,] adjmtx;
        private int pos;

        public void creatcoor(int number, double[,] fullmap)
        {
            coormap = new int[2, number];
            int k = 0;
            for (int i = 0; i < m_size; i++)
            {
                for (int j = 0; j < m_size; j++)
                {
                    if ((fullmap[i, j] != 0) && (fullmap[i, j] != 1))
                    {
                        coormap[0, k] = j;
                        coormap[1, k] = i;
                        nodes.Add(k);
                        k++;
                    }
                }
            }
        }

        public List<Tuple<int, int>> bresenHam(int xs, int ys, int xe, int ye)
        {
            int l = 0;
            int x1 = xs;
            int y1 = ys;
            int x2 = xe;
            int y2 = ye;

            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            int sx = (x1 < x2) ? 1 : -1;
            int sy = (y1 < y2) ? 1 : -1;
            int err = dx - dy;
            coordinatesList.Add(new Tuple<int, int>(x1, y1));
            while (!((x1 == x2) && (y1 == y2)))
            {
                l++;
                int e2 = err << 1;
                if (e2 > -dy)
                {
                    err -= dy;
                    x1 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y1 += sy;
                }
                // Set coordinates
                coordinatesList.Add(new Tuple<int, int>(x1, y1));
            }
            return coordinatesList;
        }

        public void adjMatrixCreator(int number, double[,] fullmap, int x, int y)
        {
            adjmtx = new double[number, number];
            bool obstacle = false;

            for (int i = 0; i < number; i++)
            {
                for (int j = 0; j < number; j++)
                {
                    obstacle = false;
                    coordinatesList.Clear();
                    bresenHam(coormap[0, i], coormap[1, i], coormap[0, j], coormap[1, j]);
                    for (int k = 0; k < coordinatesList.Count; k++)
                    {
                        if (fullmap[coordinatesList.ElementAt(k).Item2, coordinatesList.ElementAt(k).Item1] == 1)
                        {
                            obstacle = true;
                        }
                    }
                    if (!obstacle)
                    {
                        adjmtx[i, j] = Math.Sqrt((Math.Pow((coormap[0, j] - coormap[0, i]), 2)) + (Math.Pow((coormap[1, j] - coormap[1, i]), 2)));
                        if ((coormap[0, i] == x) && (coormap[1, i] == y))
                        {
                            pos = i;
                        }
                    }
                    else
                    {
                        adjmtx[i, j] = 9999999;
                    }

                }
            }
        }
        public void runTSP(int x, int y, int number)
        {
            tspList.Add(new Tuple<int, int, double, int>(x, y, 0, pos));

            double max = 0;
            double min = 0;
            int maxPos = 0;
            int minPos = 0;
            double sum = 0;
            bool isInList = false;

            for (int i = 0; i < adjmtx.GetLength(1); i++)
            {
                if ((adjmtx[pos, i] > max) && (adjmtx[pos, i] != 9999999))
                {
                    max = adjmtx[pos, i];
                    maxPos = i;
                }
            }
            tspList.Add(new Tuple<int, int, double, int>(coormap[0, maxPos], coormap[1, maxPos], 0, maxPos));
            max = 0;

            for (int l = 0; l < number - 2; l++)
            {
                max = 0;
                maxPos = 0;
                min = 0;
                minPos = 0;
                for (int i = 0; i < number; i++)
                {
                    isInList = false;
                    for (int j = 0; j < tspList.Count; j++)
                    {
                        if ((i == tspList.ElementAt(j).Item4) || (adjmtx[i, tspList.ElementAt(j).Item4] > 10000))
                        {
                            isInList = true;
                        }
                    }
                    if (!isInList)
                    {
                        min = 9999999;
                        for (int k = 0; k < tspList.Count; k++)
                        {
                            if (min > adjmtx[tspList.ElementAt(k).Item4, i])
                            {

                                min = adjmtx[tspList.ElementAt(k).Item4, i];
                            }
                        }
                    }
                    if ((max < min))
                    {

                        max = min;
                        maxPos = i;

                    }
                }

                if (maxPos != 0)
                {
                    nodes.Remove(maxPos);
                }

                if (maxPos != 0)
                {
                    sum = 0;
                    min = 9999999;
                    minPos = 0;
                    for (int i = 1; i < tspList.Count; i++)
                    {
                        tspList.Insert(i, new Tuple<int, int, double, int>(coormap[0, maxPos], coormap[1, maxPos], 0, maxPos));
                        sum = 0;
                        for (int j = 0; j < tspList.Count - 1; j++)
                        {
                            sum += adjmtx[tspList.ElementAt(j + 1).Item4, tspList.ElementAt(j).Item4];
                        }
                        if (sum <= min)
                        {
                            min = sum;
                            minPos = i;
                        }
                        tspList.RemoveAt(i);
                    }
                    tspList.Insert(minPos, new Tuple<int, int, double, int>(coormap[0, maxPos], coormap[1, maxPos], 0, maxPos));
                }
            }
        }

        public void runTSPmodif()
        {
            double min = 0;
            int maxPos = 0;
            int minPos = 0;
            double sum = 0;

            for (int k = 0; k < nodes.Count; k++)
            {
                maxPos = nodes.ElementAt(k);
                min = 99999999999999;
                minPos = 0;
                sum = 0;

                for (int i = 1; i < tspList.Count; i++)
                {

                    tspList.Insert(i, new Tuple<int, int, double, int>(coormap[0, maxPos], coormap[1, maxPos], 0, maxPos));
                    sum = 0;

                    for (int j = 0; j < tspList.Count - 1; j++)
                    {
                        sum += adjmtx[tspList.ElementAt(j).Item4, tspList.ElementAt(j + 1).Item4];
                    }
                    if (sum <= min)
                    {
                        min = sum;
                        minPos = i;
                    }
                    tspList.RemoveAt(i);
                }
                tspList.Insert(minPos, new Tuple<int, int, double, int>(coormap[0, maxPos], coormap[1, maxPos], 0, maxPos));
            }
        }

        public void calculateAngle()
        {
            int x1 = 0;
            int y1 = 0;
            int x2 = 0;
            int y2 = 0;
            double angle = 0;
            coordinatesList.Clear();
            fullList.Add(new Tuple<int, int, double>(tspList.ElementAt(0).Item1, tspList.ElementAt(0).Item2, tspList.ElementAt(0).Item3));

            for ( int i = 0; i< tspList.Count - 2; i++)
            {
                coordinatesList.Clear();
                bresenHam(tspList.ElementAt(i).Item1, tspList.ElementAt(i).Item2, tspList.ElementAt(i + 1).Item1, tspList.ElementAt(i + 1).Item2);
                foreach (var t in coordinatesList)
                {
                    fullList.Add(new Tuple<int, int, double>(t.Item1, t.Item2, 0));
                }
                y1 = (tspList.ElementAt(i+1).Item2 - tspList.ElementAt(i).Item2);
                x1 = (tspList.ElementAt(i+1).Item1 - tspList.ElementAt(i).Item1);
                y2 = (tspList.ElementAt(i + 2).Item2 - tspList.ElementAt(i+1).Item2);
                x2 = (tspList.ElementAt(i + 2).Item1 - tspList.ElementAt(i+1).Item1);
                angle = Math.Acos((x1 * x2 + y1 * y2) / ((Math.Sqrt(Math.Pow(x1, 2) + Math.Pow(y1, 2))) * (Math.Sqrt(Math.Pow(x2, 2) + Math.Pow(y2, 2)))))*180/Math.PI;
                if ((Math.Sqrt(Math.Pow(x1, 2) + Math.Pow(y1, 2))) * (Math.Sqrt(Math.Pow(x2, 2) + Math.Pow(y2, 2))) != 0)
                {
                   fullList.Add(new Tuple<int, int, double>(tspList.ElementAt(i +1).Item1, tspList.ElementAt(i+1).Item2, angle));
                }
                else
                {
                   fullList.Add(new Tuple<int, int, double>(tspList.ElementAt(i+1 ).Item1, tspList.ElementAt(i+1 ).Item2, 0));
                }

            }
        }

        public void cleanUp()
        {
            coormap = null;
            adjmtx = null;
            coordinatesList.Clear();
            tspList.Clear();
            coormap = null;
            pos = 0;
            nodes.Clear();

            MessageHandler.Write("A gráf bejárás véget ért!");
        }
    }
}



