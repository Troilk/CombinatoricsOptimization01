using System;

namespace CombinatoricsOptimization01
{
    public class SimulatedAnnealingSolver : Solver
    {
        double TMax = 100.0;

        public override Solution Solve(Solution initialSolution, double[,] graphMatrix)
        {
            Solution currentSolution = new Solution(initialSolution);

            return currentSolution;
        }
    }
}

