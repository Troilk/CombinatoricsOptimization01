using System;

namespace CombinatoricsOptimization01
{
    public abstract class Solver
    {
        CyclicArray<int> csPath;

        public abstract Solution Solve(Solution initialSolution, double[,] graphMatrix);

        protected bool CitySwapBest(Solution currentSolution, Solution outSolution, double[,] graph)
        {
            // initial variables initialization
            double bestCost = currentSolution.Cost;
            double deltaCost;
            int n = currentSolution.Path.Length;
            int temp, bestI = 0, bestJ = 0;
            int i0, i1, i2, j0, j1, j2;

            // convert current path to cyclic path
            if (this.csPath == null)
                this.csPath = new CyclicArray<int>(currentSolution.Path);
            else
                this.csPath.SetArray(currentSolution.Path);
            CyclicArray<int> path = this.csPath;

            // swaps
            for (int i = 0; i < n; ++i)
            {
                for (int j = i + 1; j < n; ++j)
                {
                    i0 = path[i - 1];
                    i1 = path[i];
                    i2 = path[i + 1];

                    j0 = path[j - 1];
                    j1 = path[j];
                    j2 = path[j + 1];

                    deltaCost = (graph[i1, j2] + graph[j0, i1] + graph[j1, i2]) - 
                                (graph[i1, i2] + graph[j0, j1] + graph[j1, j2]);

                    if (i != 0)
                        deltaCost += graph[i0, j1] - graph[i0, i1];

                    if (deltaCost < InputData.LESS_INFINITY)
                    {
                        if (deltaCost < 0.0)
                        {
                            bestCost = currentSolution.Cost + deltaCost;
                            bestI = i;
                            bestJ = j;
                        }
                    }
                }
            }

            if (bestCost < currentSolution.Cost)
            {
                outSolution.CopySolution(currentSolution);

                temp = outSolution.Path[bestI];
                outSolution.Path[bestI] = outSolution.Path[bestJ];
                outSolution.Path[bestJ] = temp;
                outSolution.Cost = bestCost;

                return true;
            }
             
            return false;
        }
    }
}