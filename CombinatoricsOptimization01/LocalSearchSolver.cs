using System;

namespace CombinatoricsOptimization01
{
    public class LocalSearchSolver : Solver
    {
        public override Solution Solve(Solution initialSolution, InputData data)
        {
            Solution currentSolution = new Solution(initialSolution);

            while (this.CitySwapBest(currentSolution, currentSolution, data.GraphMatrix))
            { }

            return currentSolution;
        }
    }
}

