namespace UdpØvelse
{
	internal static class Main
	{
		public static void ClearPreviousConsoleLine()
		{
			Console.SetCursorPosition(0, Console.CursorTop - 1);
			ClearCurrentConsoleLine();
		}

		public static void ClearCurrentConsoleLine()
		{
			int currentLineCursor = Console.CursorTop;
			Console.SetCursorPosition(0, Console.CursorTop);
			Console.Write(new string(' ', Console.WindowWidth));
			Console.SetCursorPosition(0, currentLineCursor);
		}
	}
}