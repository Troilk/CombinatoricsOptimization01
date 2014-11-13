using System;

namespace CombinatoricsOptimization01
{
    public class LocalSearchSolver : Solver
    {
        public override Solution Solve(Solution initialSolution, double[,] graphMatrix)
        {
            Solution currentSolution = new Solution(initialSolution);

            while (this.CitySwapBest(currentSolution, currentSolution, graphMatrix))
            { }

            return currentSolution;
        }
    }
}

