using System;
using System.Collections.Generic;
using OfficeOpenXml;

namespace CombinatoricsOptimization01
{
    public class Solution
    {
        double cost;
        int[] path;

        public double Duration;

        public double Cost { get { return this.cost; } set { this.cost = value; } }
        public int[] Path { get { return this.path; } }

        public Solution(int[] path, double cost)
        {
            this.path = path;
            this.cost = cost;
        }

        public Solution(int[] path, double[,] graphMatrix)
        {
            this.path = path;
            this.cost = GetSolutionCost(path, graphMatrix);
        }

        public Solution(Solution other)
        {
            this.cost = other.cost;
            this.path = (int[])other.path.Clone();
        }

        public void CopySolution(Solution other)
        {
            this.cost = other.cost;
            //other.path.CopyTo(this.path, 0);
            Buffer.BlockCopy(other.path, 0, this.path, 0, this.path.Length * sizeof(int));
        }

        public void PrintToXLS(ExcelWorksheet worksheet, int startRow, int startCol)
        {
            int pathLength = this.path.Length;

            for (int i = 0; i < pathLength; ++i)
            {
                worksheet.Cells[startRow, startCol + i].Value = this.path[i];
                worksheet.Column(startCol + i).Width = 4.0;
            }
        }

        public static double GetSolutionCost(int[] path, double[,] graphMatrix)
        {
            int pathLength = path.Length - 1;
            double currentCost = 0.0;

            int i = 0;
            int iVal = path[0];
            for (; i < pathLength; ++i)
                currentCost += graphMatrix[iVal, iVal = path[i + 1]];
            currentCost += graphMatrix[iVal, path[0]];

            return currentCost;
        }

        public void PrintSolution()
        {
            int l = this.path.Length;
            string solutionStr = "{ " + this.path[0];

            for (int i = 1; i < l; ++i)
                solutionStr += ", " + this.path[i];
            Log.LogSolution(solutionStr + " }");
        }

        public bool ValidateSolution(double[,] graphMatrix)
        {
            int l = this.path.Length;
            int node;
            List<int> visitedNodes = new List<int>(l);

            for (int i = 0; i < l; ++i)
            {
                if (visitedNodes.Contains(node = this.path[i]))
                {
                    Log.LogError("Validation error: node " + node + " already visited");
                    this.PrintSolution();
                    return false;
                }
                visitedNodes.Add(node);
            }

            if (Math.Abs(GetSolutionCost(this.path, graphMatrix) - this.cost) > 0.00000001)
            {
                Log.LogError("Validation error: path cost incorrect");
                this.PrintSolution();
                return false;
            }

            return true;
        }
    }
}