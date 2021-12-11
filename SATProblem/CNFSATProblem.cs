using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace SATProblem
{
    public class CNFSATProblem
    {
        public int VariableCount { get;}
        public int ClausesCount { get; }
        public string FileName { get; }
        private List<int[]> clauses = new List<int[]>();

        public CNFSATProblem(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            FileName = Path.GetFileName(filePath);

            using StreamReader reader = new StreamReader(filePath);
            string line;

            do
            {
                line = reader.ReadLine();
            } while (line[0] == 'c');

            if (line[0] == 'p')
            {
                string[] problemData = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                VariableCount = int.Parse(problemData[2]);
                ClausesCount = int.Parse(problemData[3]);
                clauses.Capacity = ClausesCount;
            }
            else throw new ArgumentException("File doesn't respect format", nameof(filePath));

            string clausesString = reader.ReadToEnd();
            int end = clausesString.IndexOf('%');
            if(end != -1)
            {
                clausesString = clausesString[..end];
            }

            clausesString = clausesString.Replace('\n', ' ');
            string[] individualClausesData = clausesString.Split(" 0 ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (individualClausesData.Length != ClausesCount) throw new FormatException("File doesn't contain the declared number of clauses");

            foreach(string clause in individualClausesData)
            {
                string[] clauseVariables = clause.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                int[] intClauseVariables = new int[clauseVariables.Length];
                for (int i = 0; i < clauseVariables.Length; i++)
                {
                    intClauseVariables[i] = int.Parse(clauseVariables[i]);
                }

                clauses.Add(intClauseVariables);
            } 
        }

        public int Evaluate(BitArray truthValAssignmets)
        {
            int satisfiedClauses = 0;

            foreach(int[] clause in clauses)
            {
                foreach(int variable in clause)
                {
                    if((variable > 0 && truthValAssignmets[variable - 1])||(variable < 0 && !truthValAssignmets[-(variable) - 1]))
                    {
                        satisfiedClauses++;
                        break;
                    }
                }
            }

            return satisfiedClauses;
        }
    }
}
