using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using SATProblem;

namespace SATAlgorithms
{
    public static class SimulatedAnnealing
    {
        static readonly private Random rand = new();
        public static BitArray FindEval(CNFSATProblem problem, out double evaluation)
        {
            rand.NextDouble();

            double T = 10;
            const double haltValue = 0.00000001;

            BitArray vc = Utils.Initiallize(problem.VariableCount, rand);

            do
            {
                BitArray vn = (BitArray)vc.Clone();
                double evalvn, evalvc;
                int iteration = 0;
                do
                {
                    int index;

                    index = rand.Next(0, vc.Length);
                    vn[index] = !vn[index];
                    evalvn = problem.Evaluate(vn);
                    evalvc = problem.Evaluate(vc);

                    if (evalvn > evalvc)
                    {
                        vc[index] = vn[index];
                    }
                    else if (rand.NextDouble() < Math.Exp(-Math.Abs(evalvn - evalvc) / T))
                    {
                        vc[index] = vn[index];
                    }
                    else
                    {
                        vn[index] = !vn[index];
                        ++iteration;
                    }
                } while (iteration < vc.Length);

                T *= 0.9;

            } while (T > haltValue);

            evaluation = problem.Evaluate(vc);
            return vc;
        }
    }
}
