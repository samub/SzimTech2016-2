using System;
using System.Collections.Generic;

namespace RobotMover.GA {
    internal class GA {
        //private bool m_elitism;

        private static readonly Random MRandom = new Random(); //for new Random number generator
        private static int _mPopulationSize; //POPULÁCIÓ NAGYSÁGA
        private static double _mCrossoverRate; //KERESZTEZÉS %
        private static int _mGenerationSize; //GENERÁCIÓK SZÁMA
        private static Action<Robot> _refresh;
        private static double _mMutationRate; //MUTÁCIÓ %
        private static int _cover;

        //static private GAFunction getFitness;

        private static int[,] _myIntMap;
        private int _mGenomeSize; //Egy egyed nagysága

        /// Set the GA rates and sizes
        public GA(Action<Robot> met, int cov) {
            _mMutationRate = 0.05;
            _mCrossoverRate = 0.80;
            _mPopulationSize = 5;
            _mGenerationSize = 2000;
            _cover = cov;
            _refresh = met;
        }

        /// Set the GA properties
        public GA(double crossoverRate, double mutationRate, int populationSize, int generationSize, int genomeSize) {
            //InitialValues();
            _mMutationRate = mutationRate;
            _mCrossoverRate = crossoverRate;
            _mPopulationSize = populationSize;
            _mGenerationSize = generationSize;
            _mGenomeSize = genomeSize;
        }


        /// GA program indulása
        //
        public static void Go(bool[,] map) {
            //Int térképet kapunk belőle amivel dolgozunk a bejárás során.
            _myIntMap = Map_BoolToInt(map);
            //Populációs listát létrehozzuk az "m_populationSize"-ból
            var initPopulation = GetInitialPopulation(_mPopulationSize);

            //Számítás (Fitness, Szelekció, Keresztezés, Mutáció, Fitness)
            Algo.DoMating(ref initPopulation, _mGenerationSize, _mCrossoverRate, _mMutationRate, ref _myIntMap);
        }


        //Bool map --> Int map konvertálás
        private static int[,] Map_BoolToInt(bool[,] boolMap) {
            var intMap = new int[640, 640];
            for (var i = 0; i < 640; i++) {
                for (var j = 0; j < 640; j++) {
                    if (boolMap[i, j]) intMap[i, j] = 1;
                    else intMap[i, j] = 0;
                }
            }
            return intMap;
        }


        //Populáció inicializálás
        private static List<Chromosome> GetInitialPopulation(int population) {
            var initPop = new List<Chromosome>();
            int x, y;
            for (var i = 0; i < population; i++) {
                var egyed = new Chromosome {
                                               Robot =
                                                   new Robot(20, MRandom.Next(640), MRandom.Next(640), _cover, 90,
                                                             _refresh, false)
                                           };

                bool ok;
                do {
                    ok = true;
                    x = MRandom.Next(640);
                    y = MRandom.Next(640);
                    var koztesPontok = CalcPoints(egyed.Robot.X, egyed.Robot.Y, x, y);
                    foreach (var t in koztesPontok) {
                        if (_myIntMap[t.Item1, t.Item1] == 1) {
                            ok = false;
                            break;
                        }
                    }
                    if (ok) {
                        foreach (var t in koztesPontok) {
                            egyed.Robot.Route.Add(new Tuple<int, int, double>(t.Item1, t.Item2, 0));
                        }
                    }
                }
                while (!ok);

                initPop.Add(egyed);
            }
            return initPop;
        }

        private static List<Tuple<int, int>> CalcPoints(int x0, int y0, int x1, int y1) {
            var retval = new List<Tuple<int, int>>();
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
                    retval.Add(new Tuple<int, int>(y, x));
                }
                else {
                    retval.Add(new Tuple<int, int>(x, y));
                }
                err = err - dY;
                if (err < 0) {
                    y += ystep;
                    err += dX;
                }
            }
            return retval;
        }

        private static void Swap<T>(ref T lhs, ref T rhs) {
            var temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
    }
}