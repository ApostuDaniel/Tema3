using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SATAlgorithms
{
    static public class Utils
    {
        public static BitArray Initiallize(int variablesCount, Random rand)
        {
            var solArray = new BitArray(variablesCount);

            for (int i = 0; i < variablesCount; ++i)
            {
                solArray[i] = rand.NextDouble() < 0.5;
            }

            return solArray;
        }

        public static string PrintArray(BitArray arr)
        {
            var sb = new StringBuilder("[");
            foreach (bool elm in arr)
            {
                sb.Append(elm ? 1 : 0);
                sb.Append(", ");
            }
            sb.Append("]\n");

            return sb.ToString();
        }

        public static void GeneratePopulation(List<BitArray> pop, int variablesCount, Random rand)
        {
            pop.Clear();
            int popSize = pop.Capacity;

            for (int i = 0; i < popSize; i++)
            {
                pop.Add(Initiallize(variablesCount, rand));
            }
        }

    }
}
