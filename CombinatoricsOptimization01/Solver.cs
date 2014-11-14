using System;

namespace CombinatoricsOptimization01
{
    public abstract class Solver
    {
        CyclicArray<int> csPath;
        protected Random rand = new Random();

        public abstract Solution Solve(Solution initialSolution, InputData data);

        protected double Lerp(double from, double to, double t)
        {
            return from + (to - from) * t;
        }

        protected bool CitySwapBest(Solution currentSolution, Solution outSolution, double[,] graph)
        {
            // initial variables initialization
            double bestCost = currentSolution.Cost;
            double deltaCost = 0.0;
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

                    if (i == 0 && j == n - 1)
                    {
                        deltaCost = (graph[j0, i1] + graph[i1, j1] + graph[j1, i2]) -
                                    (graph[j0, j1] + graph[j1, i1] + graph[i1, i2]);
                    }
                    else
                    {
                        if (j - i == 1)
                        {
                            deltaCost = (graph[i1, j2] + graph[j1, i1]) - (graph[i1, j1] + graph[j1, j2]);
                        }
                        else
                        {
                            deltaCost = (graph[j0, i1] + graph[i1, j2] + graph[j1, i2]) -
                                        (graph[i1, i2] + graph[j0, j1] + graph[j1, j2]);
                        }

                        deltaCost += graph[i0, j1] - graph[i0, i1];
                    }

                    // hard test
//                    int[] testS = (int[])currentSolution.Path.Clone();
//                    testS[i] = currentSolution.Path[j];
//                    testS[j] = currentSolution.Path[i];
//                    if (Solution.GetSolutionCost(testS, graph) - (currentSolution.Cost + deltaCost) > 0.000000001)
//                    {
//                        Log.LogError(Solution.GetSolutionCost(testS, graph) - (currentSolution.Cost + deltaCost));
//                    }
                    //deltaCost = Solution.GetSolutionCost(testS, graph) - currentSolution.Cost;
                    //

                    if (deltaCost < InputData.LESS_INFINITY)
                    {
                        if (deltaCost < 0.0)
                        {
                            bestCost = currentSolution.Cost + deltaCost;
                            bestI = i;
                            bestJ = j;
                            //goto FirstBetter;
                        }
                    }
                }
            }
                
            //FirstBetter:
            if (bestCost < currentSolution.Cost)
            {
                if (outSolution != currentSolution)
                    outSolution.CopySolution(currentSolution);

                temp = outSolution.Path[bestI];
                outSolution.Path[bestI] = outSolution.Path[bestJ];
                outSolution.Path[bestJ] = temp;
                outSolution.Cost = bestCost;

                return true;
            }
             
            return false;
        }

        protected double CitySwap(Solution currentSolution, double[,] graph, out int outI, out int outJ)
        {
            // initial variables initialization
            double deltaCost = 0.0;
            int n = currentSolution.Path.Length;
            int temp;
            int i0, i1, i2, j0, j1, j2;

            // convert current path to cyclic path
            if (this.csPath == null)
                this.csPath = new CyclicArray<int>(currentSolution.Path);
            else
                this.csPath.SetArray(currentSolution.Path);
            CyclicArray<int> path = this.csPath;

            // swap
            int i = 0, j = 0;
            bool foundSolution = false;

            while (!foundSolution)
            {
                i = this.rand.Next(n - 1);
                j = this.rand.Next(i + 1, n);

                i0 = path[i - 1];
                i1 = path[i];
                i2 = path[i + 1];

                j0 = path[j - 1];
                j1 = path[j];
                j2 = path[j + 1];

                if (i == 0 && j == n - 1)
                {
                    deltaCost = (graph[j0, i1] + graph[i1, j1] + graph[j1, i2]) -
                                (graph[j0, j1] + graph[j1, i1] + graph[i1, i2]);
                }
                else
                {
                    if (j - i == 1)
                    {
                        deltaCost = (graph[i1, j2] + graph[j1, i1]) - (graph[i1, j1] + graph[j1, j2]);
                    }
                    else
                    {
                        deltaCost = (graph[j0, i1] + graph[i1, j2] + graph[j1, i2]) -
                                    (graph[i1, i2] + graph[j0, j1] + graph[j1, j2]);
                    }

                    deltaCost += graph[i0, j1] - graph[i0, i1];
                }

                if (deltaCost < InputData.LESS_INFINITY)
                    foundSolution = true;
            }

            outI = i;
            outJ = j;
            return deltaCost;
        }

        protected void PermutateSolution(Solution solution, int i, int j, double deltaCost)
        {
            solution.Cost = solution.Cost + deltaCost;

            int temp = solution.Path[i];
            solution.Path[i] = solution.Path[j];
            solution.Path[j] = temp;
        }
    }
}