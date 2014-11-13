using System;
using System.IO;
using System.Xml;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CombinatoricsOptimization01
{
	public class InputData
	{
		public const int INITIAL_SOLUTIONS_COUNT = 17;
        public const double INFIINITY = 99999.0;
        public const double LESS_INFINITY = INFIINITY * 0.5;
		static Regex numberInString = new Regex(@"\d+", RegexOptions.Compiled);

		public int CitiesCount;
        public Solution[] InitialSolutions = new Solution[INITIAL_SOLUTIONS_COUNT];
        public double BestInitialSolutionCost = INFIINITY;
		public double[,] GraphMatrix;

		public InputData(string pathToInputFile)
		{
			if(!File.Exists(pathToInputFile))
				throw new Exception("Input file not found: " + pathToInputFile);

			Log.LogBlockStart("Parsing input file: " + pathToInputFile);

			XmlReader subReader;
            int[][] initialSolutionsPaths = new int[INITIAL_SOLUTIONS_COUNT][];
			int[] initialSolution = null;

			//int parseNodeTypesMask = (1 << (int)XmlNodeType.Element) | (1 << (int)XmlNodeType.Text);

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
				for(int i = 0; i < INITIAL_SOLUTIONS_COUNT; ++i)
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
                Solution sol;
                for (int i = 0; i < INITIAL_SOLUTIONS_COUNT; ++i)
                {
                    sol = new Solution(initialSolutionsPaths[i], this.GraphMatrix);
                    this.InitialSolutions[i] = sol;
                    if (sol.Cost < this.BestInitialSolutionCost)
                        this.BestInitialSolutionCost = sol.Cost;
                }
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