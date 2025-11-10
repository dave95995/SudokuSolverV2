namespace SudokuSolverV2
{
	internal class Solver
	{
		private readonly List<Unit> _units = [];

		// Store the units for each 9 boxes
		private readonly List<List<Unit>> _boxes = [];

		public int GetTotalCandidates()
		{
			int total = 0;

			foreach (Unit unit in _units)
			{
				total += unit.Domain.Count;
			}
			return total;
		}

		public Solver(string input)
		{
			ParseInput(input);
			SetAllBoxes();
		}

		public bool Solve()
		{
			ApplyPatterns(false);
			return Backtrack();
		}

		private bool Backtrack()
		{
			Unit? unit = null;

			foreach (Unit u in _units)
			{
				if (u.Domain.Count > 1)
				{
					unit = u;
					break;
				}
			}

			// Solved
			if (unit == null)
				return true;

			// Test each value
			foreach (int value in unit.Domain.ToList())
			{
				if (IsValidAssignment(unit, value))
				{
					// Set the value and continue
					var savedDomain = new List<int>(unit.Domain);
					unit.Domain.Clear();
					unit.Domain.Add(value);

					// Recurse
					if (Backtrack())
						return true;

					// Restore the candidates
					unit.Domain.Clear();
					unit.Domain.AddRange(savedDomain);
				}
			}

			// No valid assignment
			return false;
		}

		private bool IsValidAssignment(Unit unit, int value)
		{
			foreach (Unit other in unit.Constraints)
			{
				if (other.Domain.Count == 1 && other.Domain[0] == value)
					return false;
			}
			return true;
		}

		private int CalcDiff(Action func)
		{
			int before = GetTotalCandidates();
			func();
			return before - GetTotalCandidates();
		}

		private void ApplyPatterns(bool verbose = false)
		{
			int iteration = 1;
			while (true)
			{
				int before = GetTotalCandidates();

				int diff1 = CalcDiff(PatternCheckSingleValue);
				int diff2 = CalcDiff(PatternCheckOnlyCandidate);
				int diff3 = CalcDiff(PatternCheckForBoxPairs);

				int totalDiff = diff1 + diff2 + diff3;
				if (verbose)
				{
					Console.WriteLine($"Iteration {iteration++}:");
					Console.WriteLine($"\tSingleValue removed {diff1}");
					Console.WriteLine($"\tOnlyCandidate removed {diff2}");
					Console.WriteLine($"\tBoxPairs removed {diff3}");
					Console.WriteLine($"\tTotal reduction: {totalDiff}");
					Console.WriteLine();
				}

				if (totalDiff == 0)
					break;
			}
		}

		private void SetAllBoxes()
		{
			for (int y = 0; y < 3; y++)
			{
				for (int x = 0; x < 3; x++)
				{
					List<Unit> box = _units
						.Where(u => u.Position.X / 3 == x && u.Position.Y / 3 == y)
						.ToList();
					_boxes.Add(box);
				}
			}
		}

		private void ParseInput(string input)
		{
			// Add the units
			for (int i = 0; i < input.Length; i++)
			{
				int x = i % 9;
				int y = i / 9;
				int number = int.Parse(input.Substring(i, 1));
				if (number == 0)
				{
					_units.Add(new Unit(x, y, Enumerable.Range(1, 9).ToList()));
				}
				else
				{
					_units.Add(new Unit(x, y, [number]));
				}
			}

			// Add the contraints
			foreach (Unit unit in _units)
			{
				foreach (Unit otherUnit in _units)
				{
					if (unit.Position == otherUnit.Position)
					{
						continue;
					}

					bool sameRow = unit.Position.Y == otherUnit.Position.Y;
					bool sameCol = unit.Position.X == otherUnit.Position.X;
					bool sameBox = (unit.Position.X / 3 == otherUnit.Position.X / 3) &&
						(unit.Position.Y / 3 == otherUnit.Position.Y / 3);
					if (sameRow || sameCol || sameBox)
					{
						unit.Constraints.Add(otherUnit);
					}
				}
			}
		}

		// If we have solved a cell, remove the number
		// from the row, col and box
		private void PatternCheckSingleValue()
		{
			bool madeChange = true;

			while (madeChange)
			{
				madeChange = false;

				foreach (Unit unit in _units)
				{
					// We have a solved cell
					if (unit.Domain.Count == 1)
					{
						// Remove the value from the Domain (row, col and box) cells
						int value = unit.Domain.First();
						foreach (Unit otherUnit in unit.Constraints)
						{
							if (otherUnit.Domain.Remove(value))
							{
								madeChange = true;
							}
						}
					}
				}
			}
		}

		// If a number apperas once in a row, col or box then
		// we have found a solved cell.
		private void PatternCheckOnlyCandidate()
		{
			bool madeChange = true;

			while (madeChange)
			{
				madeChange = false;
				foreach (Unit unit in _units)
				{
					if (unit.Domain.Count > 1)
					{
						// Check each candidate value in unit
						foreach (int candidate in unit.Domain.ToList())
						{
							bool rowCheck = !unit.GetRowGroup().Any(u => u.Domain.Contains(candidate));
							bool colCheck = !unit.GetColGroup().Any(u => u.Domain.Contains(candidate));
							bool boxCheck = !unit.GetBoxGroup().Any(u => u.Domain.Contains(candidate));

							if (rowCheck || colCheck || boxCheck)
							{
								unit.Domain.Clear();
								unit.Domain.Add(candidate);
								madeChange = true;
								break;
							}
						}
					}
				}
			}
		}

		private bool RemoveFromRowGroup(Unit firstUnit, int number)
		{
			bool madeChange = false;

			foreach (Unit unit in firstUnit.GetRowGroup())
			{
				// Skip the cells in the same group
				if (!firstUnit.GetBoxGroup().Contains(unit))
				{
					if (unit.Domain.Remove(number))
					{
						madeChange = true;
					}
				}
			}
			return madeChange;
		}

		private bool RemoveFromColGroup(Unit firstUnit, int number)
		{
			bool madeChange = false;

			foreach (Unit unit in firstUnit.GetColGroup())
			{
				// Skip the cells in the same group
				if (!firstUnit.GetBoxGroup().Contains(unit))
				{
					if (unit.Domain.Remove(number))
					{
						madeChange = true;
					}
				}
			}
			return madeChange;
		}

		public void PatternCheckForBoxPairs()
		{
			bool madeChange = true;

			while (madeChange)
			{
				madeChange = false;

				foreach (List<Unit> oneBox in _boxes)
				{
					for (int i = 1; i <= 9; i++)
					{
						List<Unit> numUnits = oneBox.Where(u => u.Domain.Contains(i)).ToList();

						if (numUnits.Count is >= 2 and <= 3)
						{
							// All elements must have the same row or col value
							bool sameRow = numUnits.All(u => u.Position.Y == numUnits[0].Position.Y);
							bool sameCol = numUnits.All(u => u.Position.X == numUnits[0].Position.X);

							if (sameRow)
							{
								if (RemoveFromRowGroup(numUnits[0], i))
								{
									madeChange = true;
								}
							}

							if (sameCol)
							{
								if (RemoveFromColGroup(numUnits[0], i))
								{
									madeChange = true;
								}
							}
						}
					}
				}
			}
		}

		public void Display()
		{
			foreach (Unit unit in _units)
			{
				if (unit.Domain.Count == 1)
				{
					Console.Write(unit.Domain.First() + " ");
				}
				else
				{
					Console.Write("- ");
				}
				if (unit.Position.X == 2 || unit.Position.X == 5)
				{
					Console.Write("| ");
				}

				if (unit.Position.X == 8)
				{
					Console.WriteLine();
					if (unit.Position.Y == 2 || unit.Position.Y == 5)
					{
						Console.WriteLine("------+-------+------");
					}
				}
			}
		}
	}
}