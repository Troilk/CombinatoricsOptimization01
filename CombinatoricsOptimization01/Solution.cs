using System;
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
            int[] otherPath = other.path;
            int pathLength = otherPath.Length;

            for (int i = 0; i < pathLength; ++i)
                this.path[i] = otherPath[i];
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
    }
}