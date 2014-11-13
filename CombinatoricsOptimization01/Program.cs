using System;
using System.IO;
using OfficeOpenXml;
using System.Diagnostics;

namespace CombinatoricsOptimization01
{
	class MainClass
	{
		const string inputDataRoot = "../../data/";
        static Tuple<string, double>[] inputFiles = new Tuple<string, double>[17]
        {
            new Tuple<string, double>("br17", 39.0),
            new Tuple<string, double>("bays29", 2020.0),
            new Tuple<string, double>("ftv33", 1286.0),
            new Tuple<string, double>("ftv35", 1473.0),
            new Tuple<string, double>("swiss42", 1273.0),
            new Tuple<string, double>("p43", 5620.0),
            new Tuple<string, double>("ftv44", 1613.0),
            new Tuple<string, double>("ftv47", 1776.0),
            new Tuple<string, double>("att48", 10628.0),
            new Tuple<string, double>("ry48p", 14422.0),
            new Tuple<string, double>("eil51", 426.0),
            new Tuple<string, double>("berlin52", 7542.0),
            new Tuple<string, double>("ft53", 6905.0),
            new Tuple<string, double>("ftv55", 1608.0),
            new Tuple<string, double>("ftv64", 1839.0),
            new Tuple<string, double>("eil76", 538.0),
            new Tuple<string, double>("eil101", 629.0)
        };

        static void PutHeadersToWorksheet(ExcelWorksheet worksheet, string[] headers, int startRow, int startCol,
                                          int mergeVerticalCount, int mergeHorizontalCount)
        {
            for (int i = 0; i < headers.Length; ++i)
            {
                string tableHeaderStr = ExcelRange.GetAddress(startRow, startCol + i * mergeHorizontalCount,
                    startRow + mergeVerticalCount - 1, startCol + (i + 1) * mergeHorizontalCount - 1);

                worksheet.Cells[tableHeaderStr].Merge = true;
                worksheet.Cells[tableHeaderStr].Value = headers[i];
                worksheet.Cells[tableHeaderStr].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.Cells[tableHeaderStr].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            }
        }

        static void PutValuesRow(ExcelWorksheet worksheet, object[] values, int row, int startCol)
        {
            for (int i = 0; i < values.Length; ++i)
                worksheet.Cells[row, startCol + i].Value = values[i];
        }

		public static void Main(string[] args)
		{
			Log.LogGlobal ("Program started");

            DirectoryInfo outDirInfo = new DirectoryInfo(Environment.CurrentDirectory);
            FileInfo newFile = new FileInfo(outDirInfo.FullName + @"/results.xlsx");
            if (newFile.Exists)
            {
                newFile.Delete();
                newFile = new FileInfo(outDirInfo.FullName + @"/results.xlsx");
            }

            using (ExcelPackage package = new ExcelPackage(newFile))
            {
                // setting global xls file settings/properties
                package.Workbook.Properties.Title = "TSP problem solutions";
                package.Workbook.Properties.Author = "Vitalii Maslikov, IS-43m";
                package.Workbook.Properties.Comments = "";

                // generating first (results) worksheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Результати");
                int table1StartRow = 1, table1StartCol = 2;

                // printing headers
                PutHeadersToWorksheet(worksheet, new string[]{ "", "Назва задачі", "n", "f*", "f0" }, 
                    table1StartRow, table1StartCol - 1, 2, 1);
                PutHeadersToWorksheet(worksheet, new string[]{ "Детермінований локальний пошук", "Інший алгоритм ЛП" }, 
                    table1StartRow, table1StartCol + 4, 1, 3);
                PutValuesRow(worksheet, new string[]{ "f", "E", "t", "f", "E", "t" }, 
                    table1StartRow + 1, table1StartCol + 4);

                // generating second (solutions) worksheet
                ExcelWorksheet worksheet2 = package.Workbook.Worksheets.Add("Розв’язки");
                PutHeadersToWorksheet(worksheet2, new string[]{ "", "Назва задачі", "n", "f*", "f record",
                    "Найкращий розв'язок (перестановка)" }, 1, 1, 2, 1);

                string fileName;
                string filePath;
                double exactBestCost;
                int currentRowIdx;
                Stopwatch stopwatch = Stopwatch.StartNew();
                stopwatch.Stop();
                InputData inputData;

                Solution solution1 = null, solution2 = null;
                Solution recordSolution = null;
                LocalSearchSolver lsSolver = new LocalSearchSolver();

                for (int i = 0; i < inputFiles.Length; ++i)
                {
                    // reading input data
                    fileName = inputFiles[i].Item1;
                    exactBestCost = inputFiles[i].Item2;
                    filePath = inputDataRoot + fileName + ".xml";

                    try
                    {
                        inputData = new InputData(filePath);
                    }
                    catch (Exception e)
                    {
                        Log.LogError(e.Message + "\n" + e.TargetSite);
                        continue;
                    }

                    //debug
//                    if (i == 0)
//                    inputData.PrintGraph();
                    //Solution sl = new Solution(new int[]{ 2, 13, 1, 10, 12, 9, 7, 8, 0, 11, 16, 15, 6, 5, 14, 4, 3 }, inputData.GraphMatrix);
                     
                    // run algorithms and measure time
                    for (int j = 0; j < InputData.INITIAL_SOLUTIONS_COUNT; ++j)
                    {
                        GC.Collect();
                        stopwatch.Restart();

                        solution1 = lsSolver.Solve(inputData.InitialSolutions[j], inputData.GraphMatrix);

                        stopwatch.Stop();
                        solution1.Duration = stopwatch.Elapsed.TotalSeconds;

                        if (recordSolution == null)
                            recordSolution = solution1;
                        else if (recordSolution.Cost > solution1.Cost)
                            recordSolution = solution1;
                    }

                    GC.Collect();
                    stopwatch.Restart();
                    //TODO: solve the staff, method 2
                    stopwatch.Stop();
                    //time2 = stopwatch.Elapsed.TotalSeconds;

                    // print results to worksheet
                    currentRowIdx = table1StartRow + 2 + i;
                    PutValuesRow(worksheet, new object[]{ (i + 1), fileName, inputData.CitiesCount, exactBestCost,
                        inputData.BestInitialSolutionCost, solution1.Cost, RelativeError(solution1.Cost, exactBestCost), solution1.Duration, "", "", "" },
                        currentRowIdx, table1StartCol - 1);
                    PutValuesRow(worksheet2, new object[]{ (i + 1), fileName, inputData.CitiesCount, exactBestCost, solution1.Cost }, i + 2, 1);
                }

                package.Save();
            }

            Process.Start(newFile.FullName);
		}

        static double RelativeError(double f, double fExact)
        {
            return 100.0 * (f - fExact) / fExact;
        }
	}
}
