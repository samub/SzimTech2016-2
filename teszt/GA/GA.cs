using System;
using System.Collections.Generic;
using System.Windows;

namespace RobotMover.GA
{
    internal class GA
    {
        //private bool m_elitism;

        private static readonly Random _mRandom = new Random(); //for new Random number generator
        private static Robot _myRobot;
        private readonly double _mCrossoverRate; //KERESZTEZÉS %
        private readonly int _mGenerationSize; //GENERÁCIÓK SZÁMA
        private readonly double _mMutationRate; //MUTÁCIÓ %
        private readonly int _mPopulationSize; //POPULÁCIÓ NAGYSÁGA
        private int _mGenomeSize; //Egy egyed nagysága
        //private double m_totalFitness;
        private string _mStrFitness; //FITNESS értéke string ként

        /// Set the GA rates and sizes
        public GA(Robot r)
        {
            _mMutationRate = 0.05;
            _mCrossoverRate = 0.80;
            _mPopulationSize = 100;
            _mGenerationSize = 2000;
            _mStrFitness = "";
            _myRobot = r;
            MessageBox.Show(r.Theta + "");
        }

        /// Set the GA properties
        public GA(double crossoverRate, double mutationRate, int populationSize, int generationSize, int genomeSize)
        {
            //InitialValues();
            _mMutationRate = mutationRate;
            _mCrossoverRate = crossoverRate;
            _mPopulationSize = populationSize;
            _mGenerationSize = generationSize;
            _mGenomeSize = genomeSize;
            _mStrFitness = "";
        }

        //static private GAFunction getFitness;

        private static int[,] MyIntMap { get; set; } = new int[640, 640];


        /// GA program indulása
        //
        public static void Go(Robot robi, bool[,] map)
        {
            //Int térképet kapunk belőle amivel dolgozunk a bejárás során.
            MyIntMap = Map_BoolToInt(map);

            //Populációs listát létrehozzuk az "m_populationSize"-ból
            var initPopulation = GetInitialPopulation(1);

            //Számítás (Fitness, Szelekció, Keresztezés, Mutáció, Fitness)
            //Algo.DoMating(ref initPopulation, _mGenerationSize, _mCrossoverRate, _mMutationRate);
        }


        //Bool map --> Int map konvertálás
        private static int[,] Map_BoolToInt(bool[,] boolMap)
        {
            var intMap = new int[640, 640];
            for (var i = 0; i < 640; i++)
            {
                for (var j = 0; j < 640; j++)
                {
                    if (boolMap[i, j]) intMap[i, j] = 1;
                    else intMap[i, j] = 0;
                }
            }
            return intMap;
        }


        //Populáció inicializálás
        private static List<Chromosome> GetInitialPopulation(int population)
        {
            var initPop = new List<Chromosome>();
            var RandomGen = new Algo();
            int x, y;
            for (var i = 0; i < population; i++)
            {
                var egyed = new Chromosome();

                egyed.Route = new List<Tuple<int, int, double>>();
                bool ok;
                do
                {
                    ok = true;
                    x = _mRandom.Next(640);
                    y = _mRandom.Next(640);
                    var koztesPontok = CalcPoints(_myRobot.X, _myRobot.Y, x, y);
                    foreach (var t in koztesPontok)
                    {
                        if (MyIntMap[t.Item1, t.Item1] == 1)
                        {
                            ok = false;
                            break;
                        }
                        MessageBox.Show(t.Item1 + " " + t.Item2);
                    }
                    MessageBox.Show(ok + "");
                }
                while (!ok);


                /*List<int> genes = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });
                Chromosome chromosome = new Chromosome();
                chromosome.genes = new int[8];
                for (int j = 0; j < 8; j++)
                {
                    int geneIndex = (int)(RandomGen.GetRandomVal(0, genes.Count - 1) + 0.5);
                    chromosome.genes[j] = genes[geneIndex];
                    genes.RemoveAt(geneIndex);
                }

                initPop.Add(chromosome);*/
            }
            return initPop;
        }

        public static List<Tuple<int, int>> CalcPoints(int x0, int y0, int x1, int y1)
        {
            var retval = new List<Tuple<int, int>>();
            var steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                Swap(ref x0, ref y0);
                Swap(ref x1, ref y1);
            }
            if (x0 > x1)
            {
                Swap(ref x0, ref x1);
                Swap(ref y0, ref y1);
            }
            int dX = x1 - x0, dY = Math.Abs(y1 - y0), err = dX / 2, ystep = y0 < y1 ? 1 : -1, y = y0;

            for (var x = x0; x <= x1; ++x)
            {
                if (steep) { retval.Add(new Tuple<int, int>(y, x)); }
                else { retval.Add(new Tuple<int, int>(x, y)); }
                err = err - dY;
                if (err < 0)
                {
                    y += ystep;
                    err += dX;
                }
            }
            return retval;
        }

        private static void Swap<T>(ref T lhs, ref T rhs)
        {
            var temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
    }
}