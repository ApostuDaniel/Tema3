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

        public static void QuickSortTwoLists<T>(List<T> corespondent, List<double> sortValues, int start, int end)
        {
            int i;
            if (start < end)
            {
                i = Partition(corespondent, sortValues, start, end);

                QuickSortTwoLists(corespondent, sortValues, start, i - 1);
                QuickSortTwoLists(corespondent, sortValues, i + 1, end);
            }
        }

        private static int Partition<T>(List<T> corespondent, List<double> sortValues, int start, int end)
        {
            double temp;
            T temp2;
            double p = sortValues[end];
            int i = start - 1;

            for (int j = start; j <= end - 1; j++)
            {
                if (sortValues[j] <= p)
                {
                    i++;
                    temp = sortValues[i];
                    sortValues[i] = sortValues[j];
                    sortValues[j] = temp;

                    temp2 = corespondent[i];
                    corespondent[i] = corespondent[j];
                    corespondent[j] = temp2;
                }
            }

            temp = sortValues[i + 1];
            sortValues[i + 1] = sortValues[end];
            sortValues[end] = temp;

            temp2 = corespondent[i + 1];
            corespondent[i + 1] = corespondent[end];
            corespondent[end] = temp2;
            return i + 1;
        }

    }
}
