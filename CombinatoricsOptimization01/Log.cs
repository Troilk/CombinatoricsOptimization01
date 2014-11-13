using System;

namespace CombinatoricsOptimization01
{
	public static class Log
	{
        public static void LogError(object message)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(message);
		}

        public static void LogInfo(object message)
		{
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(message);
		}

        public static void LogBlockStart(object message)
		{
            Console.ForegroundColor = ConsoleColor.Blue;
			Console.WriteLine(message);
		}

        public static void LogGlobal(object message)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(message);
		}
	}
}