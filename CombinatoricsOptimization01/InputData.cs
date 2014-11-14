using System;
using System.IO;
using System.Xml;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CombinatoricsOptimization01
{
	public class InputData
	{
		const int INITIAL_SOLUTIONS_COUNT = 10;
        const bool USE_FIXED_SOLUTIONS_COUNT = true;
        public const double INFIINITY = 99999.0;
        public const double LESS_INFINITY = INFIINITY * 0.5;
		static Regex numberInString = new Regex(@"\d+", RegexOptions.Compiled);

		public int CitiesCount;
        public Solution[] InitialSolutions;
        public double AverageEdgeLength;
        public double AverageVariance;
        public double BestInitialSolutionCost = INFIINITY;
		public double[,] GraphMatrix;

        public int InitalSolutionsCount { get { return USE_FIXED_SOLUTIONS_COUNT ? INITIAL_SOLUTIONS_COUNT : this.CitiesCount; } }

		public InputData(string pathToInputFile)
		{
			if(!File.Exists(pathToInputFile))
				throw new Exception("Input file not found: " + pathToInputFile);

			Log.LogBlockStart("Parsing input file: " + pathToInputFile);

			XmlReader subReader;
			int[] initialSolution = null;

			using(XmlReader reader = XmlTextReader.Create(pathToInputFile))
			{
				// reading count of cities
				reader.ReadToFollowing("name");

				using (subReader = reader.ReadSubtree())
				{
					subReader.ReadStartElement();
					this.CitiesCount = int.Parse(numberInString.Match(reader.Value).Value);
					this.GraphMatrix = new double[this.CitiesCount, this.CitiesCount];
				}

				// reading initial solutions
                int initialSolutionsCount = USE_FIXED_SOLUTIONS_COUNT ? INITIAL_SOLUTIONS_COUNT : this.CitiesCount;
                int[][] initialSolutionsPaths = new int[initialSolutionsCount][];

                for(int i = 0; i < initialSolutionsCount; ++i)
				{
					reader.ReadToFollowing("path");

					initialSolution = new int[this.CitiesCount];

					for(int j = 0; j < this.CitiesCount; ++j)
					{
						while (reader.NodeType != XmlNodeType.Text)
							reader.Read();
						initialSolution[j] = int.Parse(reader.Value);
						reader.Read();
					}

					initialSolutionsPaths[i] = initialSolution;
				}

				// reading graph
				reader.ReadToFollowing("graph");
				double value;
				int currentNode, previousNode = -1;

				for (int i = 0; i < this.CitiesCount; ++i)
				{
					reader.ReadToFollowing("vertex");
					previousNode = -1;

					for (int j = 0; j < this.CitiesCount; ++j)
					{
						reader.ReadToFollowing("edge");
						reader.ReadAttributeValue();

						// check if need to skip node
						if (reader.NodeType == XmlNodeType.None)
						{
                            this.GraphMatrix[i, j] = INFIINITY;
							continue;
						}

						value = double.Parse(reader.GetAttribute(0), NumberStyles.Float);

						// check if need to skip node
						reader.Read();
						currentNode = int.Parse(reader.Value);
						if (currentNode - previousNode == 2)
						{
                            this.GraphMatrix[i, j] = INFIINITY;
							++j;
						}
						previousNode = currentNode;
				
                        this.GraphMatrix[i, j] = value == 9999.0 ? INFIINITY : value;
					}
				}

                // validate self transitions to be infinity
                for (int i = 0; i < this.CitiesCount; ++i)
                    this.GraphMatrix[i, i] = INFIINITY;

                // creating initial solutions with costs
                this.InitialSolutions = new Solution[initialSolutionsCount];
                Solution sol;

                for (int i = 0; i < initialSolutionsCount; ++i)
                {
                    sol = new Solution(initialSolutionsPaths[i], this.GraphMatrix);
                    this.InitialSolutions[i] = sol;
                    if (sol.Cost < this.BestInitialSolutionCost)
                        this.BestInitialSolutionCost = sol.Cost;
                }

                // finding avarage edge length
                double averageLength = 0.0;
                int totalEdges = 0;

                for (int i = 0; i < this.CitiesCount; ++i)
                {
                    for(int j = 0; j < this.CitiesCount; ++j)
                    {
                        value = this.GraphMatrix[i, j];
                        if (value != INFIINITY)
                        {
                            averageLength += value;
                            ++totalEdges;
                        }
                    }
                }

                averageLength /= totalEdges;

                this.AverageEdgeLength = averageLength;

                double averageVariance = 0.0;
                for (int i = 0; i < this.CitiesCount; ++i)
                {
                    for (int j = 0; j < this.CitiesCount; ++j)
                    {
                        value = this.GraphMatrix[i, j];
                        if (value != INFIINITY)
                            averageVariance += (value - averageLength) * (value - averageLength);
                    }
                }
                this.AverageVariance = averageVariance / totalEdges;
			}
		}

        public void PrintGraph()
        {
            Console.Write("[ ");
            for (int i = 0; i < this.CitiesCount; ++i)
            {
                Console.Write("[ " + this.GraphMatrix[i, 0]);
                for (int j = 1; j < this.CitiesCount; ++j)
                    Console.Write(", " + this.GraphMatrix[i, j]);
                Console.Write(" ],\n");
            }
            Console.Write("] ");
            Console.WriteLine();
        }
	}
}