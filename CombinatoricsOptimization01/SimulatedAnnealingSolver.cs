using System;

namespace CombinatoricsOptimization01
{
    public class SimulatedAnnealingSolver : Solver
    {
        public override Solution Solve(Solution initialSolution, InputData data)
        {
            // settings
            // should initially eccept 50%-60% of worse moves
            double av = Math.Sqrt(data.AverageVariance);
            double t0 = (av * 0.5) / Math.Log(2.0);
            double t = t0;
            // dependent on the neighbourhood size
            int iterationsPerT;

            Solution currentSolution = new Solution(initialSolution);
            Solution recordSolution = new Solution(initialSolution);
            double deltaCost;
            int tIterations;
            int globalIterations = 0;
            int i, j;

            // caches
            int n = initialSolution.Path.Length;
            double logN = Math.Log(n);
            double nLogN = n * logN;
            double tMin = 1.0;

            do
            {

                ++globalIterations;
                tIterations = 0;
                // dynamic: small number of iterations at hight temperature, larger at lower
                iterationsPerT = (int)(nLogN * (1.0 + (0.5 - ((t - tMin) / (t0 - tMin))) * 1.5));

                do
                {

                    ++tIterations;
                    deltaCost = this.CitySwap(currentSolution, data.GraphMatrix, out i, out j);

                    if(deltaCost <= 0.0 || this.rand.NextDouble() < Math.Exp(-deltaCost * 0.5 / t))
                    {
                        this.PermutateSolution(currentSolution, i, j, deltaCost);
                        // update record
                        if(currentSolution.Cost < recordSolution.Cost)
                            recordSolution.CopySolution(currentSolution);
                    }

                }while(tIterations < iterationsPerT);

                // cooling
                //long but optimizes all
                //t /= (1.0 + 0.001 * t);
                //t /= (1.0 + (1.0 / initialSolution.Path.Length) * t);
                t /= (1.0 + (1.0 / nLogN) * t);

            } while(tMin < t);
                
            return recordSolution;
        }
    }
}