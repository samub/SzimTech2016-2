using System;
using System.Collections.Generic;

namespace RobotMover.GA {
    internal struct Chromosome {
        public List<Tuple<int, int, double>> Route;
        public double Fitness;
    }

    internal delegate void Progress(int progress);

    internal class Algo {
        private const int MaxFit = 28;
        private Random _random;

        public Algo() {
            _random = new Random((int) DateTime.Now.Ticks);
        }

        public event Progress progress;

        /*public static void DoMating(ref List<Chromosome> initPopulation, int generations, double probCrossver, double probMutation)
        {
            int totalFitness = 0;
            CalcFitness(ref initPopulation, ref totalFitness);

            for (int generation = 0; generation < generations; generation++)
            {
                PrepareRuletteWheel(ref initPopulation, totalFitness);
                Crossover(ref initPopulation, probCrossver);
                Mutate(ref initPopulation, probMutation);
                CalcFitness(ref initPopulation, ref totalFitness);



                /*if (initPopulation[initPopulation.Count - 1].fitness == 28)
                    break;
                if (progress != null)
                {
                    progress(generation + 1);
                }*/
    }

    //initPopulation.Sort(new FitnessComparator());


    //}*/

    /* public void Crossover(ref List<Chromosome> parents, double probability)
     {
         List<Chromosome> offspring = new List<Chromosome>();

         for (int i = 0; i < parents.Count; i++)
         {
             if (Assay(probability)) //if the chance is to crossover
             {
                 Chromosome parentX = AssayRuletteWheel(parents);
                 Chromosome parentY = AssayRuletteWheel(parents);

                 List<int> child = new List<int>();
                 for (int j = 0; j < 8; j++)
                 {
                     if (Assay(0.5)) //select from parentX
                     {
                         for (int k = 0; k < parentX.genes.Length; k++)
                         {
                             if (!child.Contains(parentX.genes[k]))//instead of deleting the similar genes from parents select the next non-contained number
                             {
                                 child.Add(parentX.genes[k]);
                                 break;
                             }
                         }
                     }
                     else //select from parentY
                     {
                         for (int k = 0; k < parentY.genes.Length; k++)
                         {
                             if (!child.Contains(parentY.genes[k]))//instead of deleting the similar genes from parents select the next non-contained number
                             {
                                 child.Add(parentY.genes[k]);
                                 break;
                             }
                         }
                     }
                 }
                 Chromosome offSpr = new Chromosome();
                 offSpr.genes = child.ToArray();
                 offspring.Add(offSpr);

             }
             else //else the chance is to clonning
             {
                 Chromosome parentX = AssayRuletteWheel(parents);
                 offspring.Add(parentX);
             }
         }

         while (offspring.Count > parents.Count)
         {
             offspring.RemoveAt((int)GetRandomVal(0, offspring.Count - 1));
         }

         parents = offspring;
     }

     private void PrepareRuletteWheel(ref List<Chromosome> parents, int total)
     {
         int currentTotalFitness = 0;
         for (int i = 0; i < parents.Count; i++)
         {
             currentTotalFitness += parents[i].Fitness;
             Chromosome temp = parents[i];
             temp.cumAvgFitness = currentTotalFitness / (double)total;
             parents[i] = temp;
         }
     }

     private Chromosome AssayRuletteWheel(List<Chromosome> parents)
     {
         Chromosome selection = parents[0];
         double probability = random.NextDouble();
         for (int i = 0; i < parents.Count; i++)
         {
             selection = parents[i];
             if (parents[i].cumAvgFitness > probability)
                 break;

         }
         return selection;
     }

     public void Mutate(ref List<Chromosome> parents, double probability)
     {
         List<Chromosome> offsprings = new List<Chromosome>();

         for (int i = 0; i < parents.Count; i++)
         {
             Chromosome offspring = parents[i];
             for (int mutatePosition = 0; mutatePosition < 8; mutatePosition++)
             {
                 if (Assay(probability)) //if the chance is to mutate
                 {
                     int newGeneIndex = (int)(GetRandomVal(0, 6) + 0.5);
                     if (newGeneIndex >= mutatePosition)
                     {
                         newGeneIndex += 1;
                     }
                     int swapTemp = offspring.genes[mutatePosition];
                     offspring.genes[mutatePosition] = offspring.genes[newGeneIndex];
                     offspring.genes[newGeneIndex] = swapTemp;
                 }
             }

             offsprings.Add(offspring);
         }

         parents = offsprings;
     }

     public double GetRandomVal(double min, double max)
     {
         return min + random.NextDouble() * (max - min);
     }

     private bool Assay(double probability)
     {
         if (random.NextDouble() < probability)
             return true;
         else
             return false;
     }

     public void CalcFitness(ref List<Chromosome> chromosome, ref int totalFitness)
     {
         int collisions = 0;
         totalFitness = 0;
         for (int k = 0; k < chromosome.Count; k++)
         {
             for (int i = 0; i < chromosome[k].genes.Length - 1; i++)
             {
                 int x = i;
                 int y = chromosome[k].genes[i];
                 for (int j = i + 1; j < chromosome[k].genes.Length; j++)
                 {
                     if (Math.Abs(j - x) == Math.Abs(chromosome[k].genes[j] - y))
                         collisions++;
                 }
             }

             Chromosome temp = chromosome[k];
             temp.Fitness = MAX_FIT - collisions;
             chromosome[k] = temp;
             totalFitness += chromosome[k].Fitness;
             collisions = 0;
         }
     }
 }

 //Két egyed összehasonlítása
 class FitnessComparator : Comparer<Chromosome>
 {
     public override int Compare(Chromosome x, Chromosome y)
     {
         if (x.Fitness == y.Fitness)
             return 0;
         else if (x.Fitness < y.Fitness)
             return 1;
         else
             return -1;
     }
 }*/
}