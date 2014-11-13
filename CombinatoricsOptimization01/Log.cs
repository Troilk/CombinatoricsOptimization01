using System;

namespace CombinatoricsOptimization01
{
	public static class Log
	{
		public static void LogError(string message)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(message);
		}

		public static void LogInfo(string message)
		{
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(message);
		}

		public static void LogBlockStart(string message)
		{
            Console.ForegroundColor = ConsoleColor.Blue;
			Console.WriteLine(message);
		}

		public static void LogGlobal(string message)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(message);
		}
	}
}