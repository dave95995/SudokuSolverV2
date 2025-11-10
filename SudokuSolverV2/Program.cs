namespace SudokuSolverV2
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			string puzzle = "000000000000003085001020000000507000004000100090000000500000073002010000000040009";
			Solver s = new(puzzle);
			s.Solve();
			s.Display();
		}
	}
}