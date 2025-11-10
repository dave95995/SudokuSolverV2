using System.Drawing;

namespace SudokuSolverV2
{
	internal class Unit
	{
		// Position on the grid
		public Point Position { get; }

		// The cells that affect the domain
		public List<Unit> Constraints { get; }

		// The possible numbers that can be assigned
		public List<int> Domain { get; }

		public Unit(int x, int y, List<int> domain)
		{
			Position = new Point(x, y);
			Domain = domain;
			Constraints = [];
		}

		public List<Unit> GetRowGroup()
		{
			return Constraints.Where(u => u.Position.Y == Position.Y).ToList();
		}

		public List<Unit> GetColGroup()
		{
			return Constraints.Where(u => u.Position.X == Position.X).ToList();
		}

		public List<Unit> GetBoxGroup()
		{
			int boxX = Position.X / 3;
			int boxY = Position.Y / 3;

			return Constraints.Where(u =>
				(u.Position.X / 3 == boxX) &&
				(u.Position.Y / 3 == boxY)).ToList();
		}
	}
}