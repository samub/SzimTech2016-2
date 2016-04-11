using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teszt 
{
    class Mapcover
    {
        private bool[,] fcmap = new bool[640 , 640];
        private double[,] optmap = new double[640 , 640];
        private double[,] fullmap = new double[640, 640];

        private void fullMapCover()
        {
            MessageHandler.Write("Középpontok lehelyezése.");

            for (int i = 1; i < 10; i += 2)
            {
                for (int j = 1; j < 10; j += 2)
                {
                    fcmap[i, j] = true;


                }
            }
            MessageHandler.Write("Középpontok lehelyezése, megtörtént.");
        }

        private bool[,] obstacleCover()
        {
            MessageHandler.Write("Akadályoknál lévő középpontok eltávolítása.");

            for (int i = 1; i < 10; i += 1)
            {
                for (int j = 1; j < 10; j += 1)
                {
                    if (Map[i, j] == fcmap[i, j])
                    {
                        fcmap[i, j] = false;
                    }
                }
            }

            MessageHandler.Write("Akadályoknál lévő középpontok eltávolítása, megtörtént.");
            return fcmap;
        }

         private double[,] createMap()
        {
            MessageHandler.Write("Lefedési térkép létrehozása");
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (fcmap[i, j] == true)
                    {
                        for (int k = i - Radius; (k < 10) && (k < i + Radius + 1); k++)
                        {
                            for (int l = j - Radius; (l < 10) && (l < j + Radius + 1); l++)
                            {
                                optmap[k, l] = 0.5;
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if ((Map[i, j]) == true)
                    {
                        optmap[i, j] = 1;
                    }
                }
            }
            MessageHandler.Write("Lefedési térkép létrehozása, megtörtént.");
            return optmap;
        }

        private double isTheMapOptimized()
        {
            int freearea = 0;
            int coveredarea = 0;
            double percent = 0;

            MessageHandler.Write("Lefedés optimalizáltságának vizsgálata.");
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (optmap[i, j] == 0.5)
                    {
                        coveredarea += 1;
                    }
                }
            }
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (fcmap[i, j] == false)
                    {
                        freearea += 1;
                    }
                }
            }
            percent = (100 * ((double)coveredarea / (double)freearea));
            MessageHandler.Write("Lefedés optimalizáltságának vizsgálata, megtörtént");
            return percent;

        }
        private double[,] mapOptimizedCover()
        {
            int x = 0, y = 0;
            bool ok;
            int Radius = 1;

            MessageHandler.Write("Meghatározott lefedési százalék elérése a középpontok optimalizálásával.");

            if ((isTheMapOptimized() < 90))
            {
                do
                {
                    ok = false;
                    for (int i = 0; i < 10; i++)
                    {
                        for (int j = 0; j < 10; j++)
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
                    if ((x >= Radius) && (y >= Radius))
                    {
                        for (int k = x - Radius; (k < 10) && (k < x + Radius + 1); k++)
                        {
                            for (int l = y - Radius; (l < 10) && (l < y + Radius + 1); l++)
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
                        for (int k = x; (k < 10) && (k < x + Radius + 1); k++)
                        {
                            for (int l = y; (l < 10) && (l < y + Radius + 1); l++)
                            {
                                if (optmap[k, l] == 0)
                                {
                                    optmap[k, l] = 0.5;
                                }
                            }
                        }
                    }
                } while ((isTheMapOptimized() < 90));

            }
            MessageHandler.Write("Meghatározott lefedési százalék elérése a középpontok optimalizálásával, megtörtént.");
            return optmap;
        }
        private void createFullmap()
        {
            MessageHandler.Write("Gráfbejárásnál szükséges mátrix létrehozása.");
            for (int i = 0; i < 640; i++)
            {
                for(int j = 0; j < 640; j++)
                {
                    if(fcmap[i,j] == true)
                    {
                        fullmap[i,j] = 0.5;
                    }
                    if(Map[i,j] == true)
                    {
                        fullmap[i, j] = 1;
                    }
                }
            }
            MessageHandler.Write("Gráfbejárásnál szükséges mátrix létrehozása, megtörtént");
        }
    }
}
