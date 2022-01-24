using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SATProblem;

namespace SATAlgorithms
{

    public class GeneticAlgorithm
    {
        readonly private Random rand = new();
        private List<BitArray> population;
        private List<BitArray> newPopulation;
        private List<double> populationEvaluations;
        private List<double> F;
        private List<double> Fn;
        private List<double> SelectionProb;
        private List<(BitArray indv, double prob)> crossAndProb;
        public int maxT;
        public int popSize;

        public GeneticAlgorithm(int maxt, int popsize)
        {
            if (maxt < 1) throw new ArgumentOutOfRangeException(nameof(maxt));
            if (popsize < 1) throw new ArgumentOutOfRangeException(nameof(popsize));

            maxT = maxt;
            popSize = popsize;
            population = new(popSize);
            newPopulation = new(popSize);
            populationEvaluations = new(popSize);
            F = new(popSize);
            Fn = new(popSize);
            SelectionProb = new(popSize);
            crossAndProb = new(popsize);
        }

        public GeneticAlgorithm() : this(1000, 100) { }
        public void GetTruthValues(CNFSATProblem problemInstance, double selectPresure, double mutationProb, double crossProb, double elitism, out double satisfiabilityProportion, out double generationFound)
        {
            rand.NextDouble();
            int limit = 100;
            int t = 0;
            double bestSolution = 0;
            generationFound = 0;
            int selectedForElitism = (int)elitism * popSize;
            Utils.GeneratePopulation(population, problemInstance.VariableCount, rand);

            EvaluatePop(problemInstance);

            while (t < maxT)
            {
                t++;
                Selection(selectPresure, problemInstance.ClausesCount, selectedForElitism);
                MultiBitAllowWorse(problemInstance, selectedForElitism, mutationProb);
                Crossover(crossProb);
                EvaluatePop(problemInstance);

                double bestInGeneration = populationEvaluations.Max();

                if (bestInGeneration > bestSolution)
                {
                    bestSolution = bestInGeneration;
                    generationFound = t;
                }
                else if (t - generationFound > limit) break;   
            }

            satisfiabilityProportion = bestSolution;
        }

        private void EvaluatePop(CNFSATProblem problemInstance)
        {
            populationEvaluations.Clear();
            populationEvaluations.Capacity = population.Capacity;

            for (int i = 0; i < population.Count; i++)
            {
                populationEvaluations.Add(problemInstance.Evaluate(population[i]));
            }
        }

        private void Selection(double selectPressure, double maxClauses, int selectedForElitism)
        {

            F.Clear();
            F.Capacity = population.Capacity;

            for (int i = 0; i < populationEvaluations.Count; i++)
            {
                F.Add(Math.Pow(populationEvaluations[i]/maxClauses + double.Epsilon + 1, selectPressure));
            }

            double fSum = F.Sum();


            Fn.Clear();
            Fn.Capacity = F.Capacity;

            for (int i = 0; i < F.Count; i++)
            {
                Fn.Add(F[i] / fSum);
            }


            SelectionProb.Clear();
            SelectionProb.Capacity = Fn.Capacity;

            SelectionProb.Add(Fn[0]);
            for (int i = 1; i < F.Count; i++)
            {
                SelectionProb.Add(SelectionProb[i - 1] + Fn[i]);
            }

            SelectionProb[^1] = 1;

            newPopulation.Clear();

            //Adding Elitism
            Utils.QuickSortTwoLists(population, populationEvaluations, 0, population.Count - 1);
            for (int i = 0; i < selectedForElitism; i++)
            {
                newPopulation.Add(population[i]);
            }

            for (int i = 0; i < popSize - selectedForElitism; i++)
            {
                double r = rand.NextDouble();
                bool chosen = false;

                for (int j = 0; j < SelectionProb.Count; j++)
                {
                    if (r <= SelectionProb[j])
                    {
                        newPopulation.Add(population[j]);
                        chosen = true;
                        break;
                    }
                }
                if (!chosen) newPopulation.Add(population[^1]);
            }
            //end, to remove elitism look in the sequnece between comments

            population.Clear();
            population.AddRange(newPopulation);
        }

        private void Crossover(double crossProb)
        {

            crossAndProb.Clear();

            foreach (var individ in population)
            {
                crossAndProb.Add((individ, rand.NextDouble()));
            }

            crossAndProb.Sort((x, y) => x.prob.CompareTo(y.prob));

            int i = 0;
            while (i + 1 < crossAndProb.Count && crossAndProb[i].prob < crossProb)
            {
                if (crossAndProb[i + 1].prob < crossProb || rand.NextDouble() < 0.5)
                {
                    var temp = TwoPointCrossoverGene(crossAndProb[i].indv, crossAndProb[i + 1].indv);
                    population.Add(temp.Item1);
                    population.Add(temp.Item2);
                }
                i += 2;
            }
        }

        private (BitArray, BitArray) CrossoverGene(BitArray parent1, BitArray parent2)
        {
            int crossPos = 1 + rand.Next(parent1.Length - 2);

            var child1 = new BitArray(parent1);
            var child2 = new BitArray(parent2);

            for (int i = crossPos; i < parent1.Length; i++)
            {
                child1[i] = parent2[i];
                child2[i] = parent1[i];
            }
            return (child1, child2);
        }

        private (BitArray, BitArray) TwoPointCrossoverGene(BitArray parent1, BitArray parent2)
        {
            int crossPos = 1 + rand.Next(parent1.Length - 2);
            int crossPos2;

            do
            {
                crossPos2 = 1 + rand.Next(parent1.Length - 2);
            } while (Math.Abs(crossPos-crossPos2) <= 1);

            if (crossPos2 < crossPos)
            {
                var temp = crossPos2;
                crossPos2 = crossPos;
                crossPos = temp;
            }

            var child1 = new BitArray(parent1);
            var child2 = new BitArray(parent2);

            for (int i = 0; i < crossPos; i++)
            {
                child1[i] = parent2[i];
                child2[i] = parent1[i];
            }
            for (int i = crossPos2; i < parent1.Length; i++)
            {
                child1[i] = parent2[i];
                child2[i] = parent1[i];
            }

            return (child1, child2);
        }

        private void Mutate(double mutationProb)
        {
            int arrSize = population[0].Length;
            for (int i = 0; i < population.Count; i++)
            {
                for (int j = 0; j < arrSize; j++)
                {
                    if (rand.NextDouble() < mutationProb)
                    {
                        population[i][j] = !population[i][j];
                    }
                }
            }
        }

        private void MultiBitMutate(CNFSATProblem problemInstance, int selectedForElitism)
        {
            int arrSize = population[0].Length;
            for (int i = selectedForElitism; i < population.Count; i++)
            {
                double initialEval = populationEvaluations[i];
                for (int j = 0; j < arrSize; j++)
                {
                    population[i][j] = !population[i][j];
                    double currentEval = problemInstance.Evaluate(population[i]);

                    if (initialEval < currentEval) initialEval = currentEval;
                    else population[i][j] = !population[i][j];                 
                }
            }
        }
        private void MultiBitAllowWorse(CNFSATProblem problemInstance, int selectedForElitism, double mutationProb)
        {
            int arrSize = population[0].Length;
            for (int i = selectedForElitism; i < population.Count; i++)
            {
                double initialEval = populationEvaluations[i];
                for (int j = 0; j < arrSize; j++)
                {
                    population[i][j] = !population[i][j];
                    double currentEval = problemInstance.Evaluate(population[i]);

                    if (initialEval < currentEval) initialEval = currentEval;
                    else if(initialEval == currentEval)
                    {
                        if(rand.NextDouble() < 0.5) initialEval = currentEval;
                        else population[i][j] = !population[i][j];
                    }
                    else
                    {
                        if(rand.NextDouble() < mutationProb) initialEval = currentEval;
                        else population[i][j] = !population[i][j];
                    }
                }
            }
        }
    }
}
