using System;
using SATProblem;
using SATAlgorithms;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

namespace Tema3
{
    class Program
    {
        static void Main(string[] args)
        {
            var testInstaces = Directory.EnumerateFiles(@"F:\C#\Facultate\AlgGenetici\Tema3\Data");
            Parallel.ForEach(testInstaces, instance =>
            {
                //var algorithm = new GeneticAlgorithm();
                var problemInstance = new CNFSATProblem(instance);
                //TestGA(problemInstance, algorithm, 30, 0.5, 0, 0.9, 0.5);
                //TestGA(problemInstance, algorithm, 10, 1, 0.1, 0.6, 0.05);
                TestSA(problemInstance, 30);
            });

            //var algorithm = new GeneticAlgorithm();
            //var problemInstance = new CNFSATProblem(@"F:\C#\Facultate\AlgGenetici\Tema3\Data\uf250-0100.txt");
            //TestGA(problemInstance, algorithm, 10, 1, 0.01, 0.7, 0.05);

            //var watch = new Stopwatch();
            //double evalSum = 0;
            //double timeSum = 0;
            //for (int i = 0; i < 5; i++)
            //{
            //    watch.Start();
            //    SimulatedAnnealing.FindEval(problemInstance, out double evaluation);
            //    watch.Stop();
            //    evalSum += evaluation;
            //    timeSum += watch.ElapsedMilliseconds;
            //    Console.WriteLine($"Evaluation: {evaluation}, Actual: {problemInstance.ClausesCount}, Time: {watch.ElapsedMilliseconds} ms");
            //    watch.Reset();
            //}
            //Console.WriteLine($"\n Average: Evaluation: {evalSum/5}, Actual: {problemInstance.ClausesCount}, Time: {timeSum/5} ms");

            Console.WriteLine("\nDone, press any key to end program");
            Console.ReadLine();
        }

        public static void TestSA(CNFSATProblem problemInstance, int repeats)
        {
            Stopwatch watch = new();
            List<double> resultsPerRun = new(repeats);
            List<double> timePerRun = new(repeats);
            for (int i = 0; i < repeats; i++)
            {
                watch.Restart();
                SimulatedAnnealing.FindEval(problemInstance, out double result);
                watch.Stop();
                timePerRun.Add(watch.ElapsedMilliseconds);
                resultsPerRun.Add(result);
            }

            var evalMean = resultsPerRun.Sum() / repeats;
            var timeMean = timePerRun.Sum() / repeats;
            double sdEval = Math.Sqrt(resultsPerRun.Sum(number => Math.Pow(number - evalMean, 2)) / repeats);
            double sdTime = Math.Sqrt(timePerRun.Sum(number => Math.Pow(number - timeMean, 2)) / repeats);

            Console.WriteLine($"File:{problemInstance.FileName}  Satisfied_Clauses:{evalMean}    Clauses:{problemInstance.ClausesCount}" +
                $"  Time:{timeMean}    SD:{sdEval}    SDTime:{sdTime}");
        }

        public static void TestGA(CNFSATProblem problemInstance, GeneticAlgorithm alg, int repeats, double selectionPressure, double mutationStrength, double crossoverProbability, double elitism)
        {
            double generationsSum = 0;
            Stopwatch watch = new();
            List<double> resultsPerRun = new(repeats);
            List<double> timePerRun = new(repeats);
            for (int i = 0; i < repeats; i++)
            {
                watch.Restart();
                alg.GetTruthValues(problemInstance, selectionPressure, mutationStrength, crossoverProbability, elitism, out double result, out double generation);
                watch.Stop();
                timePerRun.Add(watch.ElapsedMilliseconds);
                resultsPerRun.Add(result);
                generationsSum += generation;
            }

            var evalMean = resultsPerRun.Sum() / repeats;
            var timeMean = timePerRun.Sum() / repeats;
            double sdEval = Math.Sqrt(resultsPerRun.Sum(number => Math.Pow(number - evalMean, 2)) / repeats);
            double sdTime = Math.Sqrt(timePerRun.Sum(number => Math.Pow(number - timeMean, 2)) / repeats);

            Console.WriteLine($"File:{problemInstance.FileName}  Satisfied_Clauses:{evalMean}    Clauses:{problemInstance.ClausesCount}    Generation:{generationsSum / repeats}" +
                $"Time:{timeMean}    SD:{sdEval}    SDTime:{sdTime}" +
                $"\nstats: select {selectionPressure}, elitism {elitism}, cross {crossoverProbability}, mut: {mutationStrength}");
        }

        public static void ParallelTestGA(CNFSATProblem problemInstance, GeneticAlgorithm alg, int repeats, double selectionPressure, double mutationStrength, double crossoverProbability, double elitism)
        {
            double generationsSum = 0;
            double clausesSum = 0;
            object genLock = new object();
            object initializationLock = new object();
            object clauseLock = new object();
            Parallel.For(0, repeats, i =>
            {
                CNFSATProblem newProblem;
                GeneticAlgorithm newAlg;

                lock (initializationLock)
                {
                    newProblem = new CNFSATProblem(problemInstance);
                    newAlg = new GeneticAlgorithm(alg.maxT, alg.popSize);
                }
                newAlg.GetTruthValues(newProblem, selectionPressure, mutationStrength, crossoverProbability, elitism, out double result, out double generations);
                lock (genLock)
                {
                    generationsSum += generations;
                }
                lock (clauseLock)
                {
                    clausesSum += result;
                }
            });
            Console.WriteLine($"Paralel On average: found {clausesSum / repeats} satisfied clauses at generation {generationsSum / repeats}" +
                $" stats: select {selectionPressure}, mutation {mutationStrength}, cross {crossoverProbability}");
        }    
    }
}
