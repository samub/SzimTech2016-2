using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teszt
{
    class GA
    {
        private double m_mutationRate; //MUTÁCIÓ %
        private double m_crossoverRate; //KERESZTEZÉS %
        private int m_populationSize; //POPULÁCIÓ NAGYSÁGA
        private int m_generationSize; //GENERÁCIÓK SZÁMA
        private int m_genomeSize; //Egy egyed nagysága
        //private double m_totalFitness;
        private string m_strFitness; //FITNESS értéke string ként
                                     //private bool m_elitism;

        static Random m_random = new Random(); //for new Random number generator
        //static private GAFunction getFitness;

        static int[,] myIntMap = new int[640, 640];



        ///Set the GA rates and sizes
        public GA()
        {
            ///InitialValues();
            m_mutationRate = 0.05;
            m_crossoverRate = 0.80;
            m_populationSize = 100;
            m_generationSize = 2000;
            m_strFitness = "";
        }

        ///Set the GA properties
        public GA(double crossoverRate, double mutationRate, int populationSize, int generationSize, int genomeSize)
        {
            //InitialValues();
            m_mutationRate = mutationRate;
            m_crossoverRate = crossoverRate;
            m_populationSize = populationSize;
            m_generationSize = generationSize;
            m_genomeSize = genomeSize;
            m_strFitness = "";
        }

        ///Set the GA genom size
        public GA(int genomeSize)
        {
            ///InitialValues();
            m_genomeSize = genomeSize;
        }



        ///GA program indulása
        //
        public static void Go(Robot robi, bool[,] map)
        {
            //Int térképet kapunk belőle amivel dolgozunk a bejárás során.
            myIntMap = Map_BoolToInt(map);

            //Populációs listát létrehozzuk az "m_populationSize"-ból
            List<Chromosome> initPopulation = GetInitialPopulation(m_populationSize);

            //Számítás (Fitness, Szelekció, Keresztezés, Mutáció, Fitness)
            Algo.DoMating(ref initPopulation, m_generationSize, m_crossoverRate, m_mutationRate);

        }


        //Bool map --> Int map konvertálás
        public static int[,] Map_BoolToInt(bool[,] boolMap)
        {
            var intMap = new int[640, 640];
            for (int i = 0; i < 640; i++)
            {
                for (int j = 0; j < 640; j++)
                {
                    if (boolMap[i, j]) intMap[i, j] = 1;
                    else intMap[i, j] = 0;
                }
            }
            return intMap;
        }


        //Populáció inicializálás
        private List<Chromosome> GetInitialPopulation(int population)
        {
            List<Chromosome> initPop = new List<Chromosome>();
            Algo RandomGen = new Algo();
            for (int i = 0; i < population; i++)
            {
                List<int> genes = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });
                Chromosome chromosome = new Chromosome();
                chromosome.genes = new int[8];
                for (int j = 0; j < 8; j++)
                {
                    int geneIndex = (int)(RandomGen.GetRandomVal(0, genes.Count - 1) + 0.5);
                    chromosome.genes[j] = genes[geneIndex];
                    genes.RemoveAt(geneIndex);
                }

                initPop.Add(chromosome);
            }
            return initPop;
        }
    }
}