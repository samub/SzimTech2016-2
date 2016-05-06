using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace RobotMover
{
    internal class Mapcover
    {
        private const int m_size = 640;
        private bool[,] fcmap;
        private double[,] optmap;
        public double[,] fullmap;
        private int number = 0;
        private int freearea = 0;
        private int radius = 0;
        private int cover = 0;
        private int sx = 0;
        private int sy = 0;

        public void setStart(int x, int y)
        {
            sx = x;
            sy = y;
        }
        public void setRadius(int rad)
        {
            radius = rad;
        }

        public void setCover(int cov)
        {
            cover = cov;
        }

        public int getNumber()
        {
            return number;
        }

        public void fullMapCover()
        {
            fcmap = new bool[m_size, m_size];
            MessageHandler.Write("Középpontok lehelyezése.");
            fcmap[sx, sy] = true;
            MessageHandler.Write("Kezdőpontok: X:"+sx + " Y" + sy) ;
            for (int i = radius; i < m_size; i += (2 * radius))
            {
                for (int j = radius; j < m_size; j += (2 * radius))
                {
                    fcmap[i, j] = true;
                }
            }
            MessageHandler.Write("Középpontok lehelyezése, megtörtént.");
        }

        public bool[,] obstacleCover(bool[,] obst)
        {
            MessageHandler.Write("Akadályoknál lévő középpontok eltávolítása.");

            for (int i = 0; i < m_size; i++)
            {
                for (int j = 0; j < m_size; j++)
                {
                    if (obst[i, j] == fcmap[i, j])
                    {
                        fcmap[i, j] = false;
                    }
                }
            }

            MessageHandler.Write("Akadályoknál lévő középpontok eltávolítása, megtörtént.");
            return fcmap;
        }
        
        public double[,] createMap(bool[,] obst)
        {
            optmap = new double[m_size, m_size];
            MessageHandler.Write("Lefedési térkép létrehozása");
            for (int i = 0; i < m_size; i++)
            {
                for (int j = 0; j < m_size; j++)
                {
                    if (fcmap[i, j] == true)
                    {
                        for (int k = i - radius; (k < m_size) && (k <= i + radius); k++)
                        {
                            for (int l = j - radius; (l < m_size) && (l <= j + radius); l++)
                            {
                                optmap[k, l] = 0.5;
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < m_size; i++)
            {
                for (int j = 0; j < m_size; j++)
                {
                    if ((obst[i, j]) == true)
                    {
                        optmap[i, j] = 1;
                    }
                }
            }
            MessageHandler.Write("Lefedési térkép létrehozása, megtörtént.");
            return optmap;
        }

        public double isTheMapOptimized()
        {
            int coveredarea = 0;
            double percent = 0;

            for (int i = 0; i < m_size; i++)
            {
                for (int j = 0; j < m_size; j++)
                {
                    if (optmap[i, j] == 0.5)
                    {
                        coveredarea += 1;
                    }
                }
            }
            
            percent = (100 * ((double)coveredarea / (double)freearea));  
            return percent;

        }

        public double isTheMapOptimized(bool[,] obst)
        {
            int coveredarea = 0;
            double percent = 0;

            MessageHandler.Write("Lefedés optimalizáltságának vizsgálata.");
            for (int i = 0; i < m_size; i++)
            {
                for (int j = 0; j < m_size; j++)
                {
                    if (optmap[i, j] == 0.5)
                    {
                        coveredarea += 1;
                    }
                }
            }
            for (int i = 0; i < m_size; i++)
            {
                for (int j = 0; j < m_size; j++)
                {
                    if (obst[i, j] == false)
                    {
                        freearea += 1;
                    }
                }
            }
            percent = (100 * ((double)coveredarea / (double)freearea));
            MessageHandler.Write("Lefedés optimalizáltságának vizsgálata megtörtént: " + percent + "%");
            return percent;

        }
        
        public double[,] mapOptimizedCover()
        {
            int x = 0, y = 0;
            bool ok;

            if ((isTheMapOptimized() < cover))
            {
                MessageHandler.Write("Meghatározott lefedési százalék elérése a középpontok optimalizálásával.");
                do
                {
                    ok = false;
                    for (int i = 0; i < m_size; i++)
                    {
                        for (int j = 0; j < m_size; j++)
                        {
                            if ((optmap[i, j] == 0) && (!ok))
                            {
                                x = i;
                                y = j;
                                fcmap[i, j] = true;
                                ok = true;
                            }
                        }
                    }
                    if ((x >= radius) && (y >= radius))
                    {
                        for (int k = x - radius; (k < m_size) && (k <= x + radius); k++)
                        {
                            for (int l = y - radius; (l < m_size) && (l <= y + radius); l++)
                            {
                                if (optmap[k, l] == 0)
                                {
                                    optmap[k, l] = 0.5;
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int k = x; (k < m_size) && (k < x + radius + 1); k++)
                        {
                            for (int l = y; (l < m_size) && (l < y + radius + 1); l++)
                            {
                                if (optmap[k, l] == 0)
                                {
                                    optmap[k, l] = 0.5;
                                }
                            }
                        }
                    }
                } while ((isTheMapOptimized() < cover));
                MessageHandler.Write("Meghatározott lefedési százalék elérése a középpontok optimalizálásával, megtörtént: " + isTheMapOptimized() + "%");
            }
            return optmap;
        }
    
        public void createFullmap(bool[,] obst)
        {
            fullmap = new double[m_size, m_size];
            MessageHandler.Write("Gráfbejáráshoz szükséges mátrix létrehozása.");
            for (int i = 0; i < m_size; i++)
            {
                for(int j = 0; j < m_size; j++)
                {
                    if(fcmap[i,j] == true)
                    {
                        fullmap[i,j] = 0.5;
                        number += 1;
                    }
                    if(obst[i,j] == true)
                    {
                        fullmap[i, j] = 1;
                    }
                }
            }
            MessageHandler.Write("Gráfbejáráshoz szükséges mátrix létrehozása, megtörtént");
        }
        public void cleanUp()
        {
            fcmap = null;
            optmap = null;
            fullmap = null;
            freearea = 0;
            number = 0;
        }

    }
}
