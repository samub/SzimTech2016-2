using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teszt 
{
    class Mapcover
    {
        private const int m_size = 640;
        private bool[,] fcmap = new bool[m_size, m_size];
        private double[,] optmap = new double[m_size, m_size];
        private double[,] fullmap = new double[m_size, m_size];
        private Robot _robot;
        private CsvToMatrix _csvtomatrix;

        private void fullMapCover()
        {
            MessageHandler.Write("Középpontok lehelyezése.");

            for (int i = _robot.Radius; i < m_size; i += (2 * _robot.Radius))
            {
                for (int j = _robot.Radius; j < m_size; j += (2 * _robot.Radius))
                {
                    fcmap[i, j] = true;


                }
            }
            MessageHandler.Write("Középpontok lehelyezése, megtörtént.");
        }

        private bool[,] obstacleCover()
        {
            MessageHandler.Write("Akadályoknál lévő középpontok eltávolítása.");

            for (int i = 0; i < m_size; i++)
            {
                for (int j = 0; j < m_size; j++)
                {
                    if (_csvtomatrix.Map[i, j] == fcmap[i, j])
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
            for (int i = 0; i < m_size; i++)
            {
                for (int j = 0; j < m_size; j++)
                {
                    if (fcmap[i, j] == true)
                    {
                        for (int k = i - _robot.Radius; (k < m_size) && (k <= i + _robot.Radius); k++)
                        {
                            for (int l = j - _robot.Radius; (l < m_size) && (l <= j + _robot.Radius); l++)
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
                    if ((_csvtomatrix.Map[i, j]) == true)
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

            MessageHandler.Write("Meghatározott lefedési százalék elérése a középpontok optimalizálásával.");

            if ((isTheMapOptimized() < _robot.Cover))
            {
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
                    if ((x >= _robot.Radius) && (y >= _robot.Radius))
                    {
                        for (int k = x - _robot.Radius; (k < m_size) && (k <= x + _robot.Radius); k++)
                        {
                            for (int l = y - _robot.Radius; (l < m_size) && (l <= y + _robot.Radius ); l++)
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
                        for (int k = x; (k < m_size) && (k < x + _robot.Radius + 1); k++)
                        {
                            for (int l = y; (l < m_size) && (l < y + _robot.Radius + 1); l++)
                            {
                                if (optmap[k, l] == 0)
                                {
                                    optmap[k, l] = 0.5;
                                }
                            }
                        }
                    }
                } while ((isTheMapOptimized() < _robot.Cover));

            }
            MessageHandler.Write("Meghatározott lefedési százalék elérése a középpontok optimalizálásával, megtörtént.");
            return optmap;
        }
        private void createFullmap()
        {
            MessageHandler.Write("Gráfbejárásnál szükséges mátrix létrehozása.");
            for (int i = 0; i < m_size; i++)
            {
                for(int j = 0; j < m_size; j++)
                {
                    if(fcmap[i,j] == true)
                    {
                        fullmap[i,j] = 0.5;
                    }
                    if(_csvtomatrix.Map[i,j] == true)
                    {
                        fullmap[i, j] = 1;
                    }
                }
            }
            MessageHandler.Write("Gráfbejárásnál szükséges mátrix létrehozása, megtörtént");
        }
    }
}
