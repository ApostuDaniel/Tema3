﻿using System;
using SATProblem;
using SATAlgorithms;
using System.Collections;
using System.Diagnostics;

namespace Tema3
{
    class Program
    {
        static void Main(string[] args)
        {

            var problemInstance = new CNFSATProblem(@"F:\C#\Facultate\AlgGenetici\Tema3\Data\satisfiableHardInstance.txt");
            var algorithm = new GeneticAlgorithm();
            var watch = new Stopwatch();
            watch.Start();
            TestGA(problemInstance, algorithm, 1,1,0.1,0.5, 0.6);
            watch.Stop();
            Console.WriteLine("time: {0}", watch.ElapsedMilliseconds / 1000);
            //for (int i = 1; i < 6; i++)
            //{
            //    for (double j = 0.01; j < 0.25; j+=0.05)
            //    {
            //        for (double k = 0.1; k < 0.7; k+=0.1)
            //        {
            //            TestGA(problemInstance, algorithm, 10, i, j, k);
            //        }
            //    }
            //}
        }

        public static void TestGA(CNFSATProblem problemInstance, GeneticAlgorithm alg, int repeats, double selectionPressure, double mutationStrength, double crossoverProbability, double elitism)
        {
            double generationsSum = 0;
            double clausesSum = 0;
            for (int i = 0; i < repeats; i++)
            {
                generationsSum += alg.GetTruthValues(problemInstance, selectionPressure, mutationStrength, crossoverProbability, elitism, out double result);
                clausesSum += result;
            }
            Console.WriteLine($"On average: found {clausesSum / repeats} satisfied clauses at generation {generationsSum / repeats}" +
                $" stats: select {selectionPressure}, mutation {mutationStrength}, cross {crossoverProbability}");
        }
    }
}
