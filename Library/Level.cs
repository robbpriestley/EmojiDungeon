// Current issues: 1) non-square grids either crash or render weird
//                 2) no rooms, but the code for it is in the archive /Users/wizard/Documents/Archive/Game Concepts/New School

using System;
using System.Text;
using System.Collections.Generic;

namespace DigitalWizardry.LevelGenerator
{	
	public class Level
	{
		private int GridWidth;
		private int GridHeight;
		private Random R;
		private int SequenceNumber;
		private Coords StartCoords;
		private List<Cell> Grid;

		public Level(int width, int height, Coords startCoords)
		{
			R = new Random();
			GridWidth = width;
			GridHeight = height;
			StartCoords = startCoords;
			
			Globals.Initialize();
		
			Start();
		}

		private void Start()
		{
			int buildPasses = 0;
			bool levelComplete = false;

			do
			{
				try 
				{
					buildPasses++;
			
					Initialize();
					GenerateLevel();
					LevelSolve();
					levelComplete = true;  // i.e. no exceptions...
				}
				catch (LevelGenerateException) 
				{
					levelComplete = false;   // Try again.
				}

			} while (!levelComplete);
		}

		private void Initialize()
		{
			Grid = new List<Cell>();

			// Fill in each cell with the "empty cell" object.
			for (int i = 0; i < GridWidth * GridHeight; i++)
			{
				Grid.Add(Globals.EmptyCell);
			}

			List<CellType> types = CellTypes.GetTypes(StartCoords, GridWidth, GridHeight);

			CellType newType = RandomCellType(types);
			Cell newCell = new Cell(StartCoords.X, StartCoords.Y, newType);

			SetDungeonCellValue(StartCoords.X, StartCoords.Y, newCell);
		}

		private void GenerateLevel()
		{    
			Cell cell;
			bool modified = false;
			
			// As long as the dungeon is not considered complete, keep adding stuff to it.
			do 
			{
				modified = false;
				
				for (int Y = 0; Y < GridHeight; Y++) 
				{
					for (int X = 0; X < GridWidth; X++) 
					{
						cell = CellAt(X, Y);
						
						if (!cell.Type.IsEmpty && !cell.AttachBlocked && cell.AvailableConnections > 0)
						{
							// Attach a new random cell to current cell, if possible. If the cell has 
							// available connections but nothing can be added to it, consider it blocked.
							if (AttachNewCell(cell)) 
								modified = true;
							else  
								cell.AttachBlocked = true;
						}
					}
				}
				
				//Console.WriteLine(VisualizeAsText());

			} while (!CompleteCheck(modified));
		}

		private bool AttachNewCell(Cell cell)
		{
			bool attachSuccessful = false;
			Coords coords = RandomAttachCoords(cell);
			
			if (coords != null)
			{
				// Get a disposable array of constructed corridor dungeon cell types.
				List<CellType> types = CellTypes.GetTypes(coords, GridWidth, GridHeight);
				
				// Choose a new cell type to attach.
				Cell newCell = null;
				CellType newType = null;
				
				while (newCell == null) 
				{
					if (types.Count == 0) 
						return false;  // Whoops, there are no more possibilities.
					
					newType = RandomCellType(types);
				
					// The new cell needs to be compatible with each adjacent cell.
					if (TypeCompatibleWithAdjacentCells(newType, coords))
						newCell = new Cell(coords.X, coords.Y, newType);
				}
				
				SetDungeonCellValue(coords.X, coords.Y, newCell);
				attachSuccessful = true;
			}
			
			return attachSuccessful;
		}

		// When a new cell is placed in the dungeon, "record" it as such by decrementing the
		// availableConnections count of each adjacent, non-empty cell that connects to it.
		// Also, decrement the availableConnections count of the new cell accordingly.
		private void RecordNewAttachment(Cell cell)
		{
			Cell cellUp, cellDown, cellLeft, cellRight;
			
			if (cell.Y + 1 < GridHeight)
			{
				cellUp = CellAt(cell.X, cell.Y + 1);
				if (cell.Type.ConnectsTo(cellUp.Type, Direction.Up))
				{
					cell.AvailableConnections--;
					cellUp.AvailableConnections--;
				}
			}
			
			if (cell.Y - 1 >= 0)
			{
				cellDown = CellAt(cell.X, cell.Y - 1);
				if (cell.Type.ConnectsTo(cellDown.Type, Direction.Down))
				{
					cell.AvailableConnections--;
					cellDown.AvailableConnections--;
				}
			}
			
			if (cell.X - 1 >= 0)
			{
				cellLeft = CellAt(cell.X - 1, cell.Y);
				if (cell.Type.ConnectsTo(cellLeft.Type, Direction.Left))
				{
					cell.AvailableConnections--;
					cellLeft.AvailableConnections--;
				}
			}
			
			if (cell.X + 1 < GridWidth)
			{
				cellRight = CellAt(cell.X + 1, cell.Y);
				if (cell.Type.ConnectsTo(cellRight.Type, Direction.Right))
				{
					cell.AvailableConnections--;
					cellRight.AvailableConnections--;
				}
			}
		}

		#region Randomization

		// Find which locations adjacent to the current cell could be populated with a new
		// attaching cell. Then, out of those locations, select one at random and return the 
		// coords for it. If no such location exists, return nil.
		private Coords RandomAttachCoords(Cell cell)
		{
			List<Coords> coordPotentials = new List<Coords>();
			
			// Check each direction for an adjacent cell. Obviously, an adjacent cell has to be within the 
			// coordinate bounds of the dungeon. If the current cell has the capability to join with a cell
			// in the adjacent location, and the cell in the adjacent location is empty, add the coords for 
			// each such location to the coordPotentials array. Then choose one of those potential coords at 
			// random and return it. If there are no potentials at all, return null.
			
			// Cell above.
			if (cell.Type.ConnectsUp && cell.Y + 1 < GridHeight)
				if (CellAt(cell.X, cell.Y + 1).Type.IsEmpty)
					coordPotentials.Add(new Coords(cell.X, cell.Y + 1));
			
			// Cell below.
			if (cell.Type.ConnectsDown && cell.Y - 1 >= 0)
				if (CellAt(cell.X, cell.Y - 1).Type.IsEmpty)
					coordPotentials.Add(new Coords(cell.X, cell.Y - 1));
		
			// Cell left.
			if (cell.Type.ConnectsLeft && cell.X - 1 >= 0)
				if (CellAt(cell.X - 1, cell.Y).Type.IsEmpty)
					coordPotentials.Add(new Coords(cell.X - 1, cell.Y));
			
			// Cell right.
			if (cell.Type.ConnectsRight && cell.X + 1 < GridWidth)
				if (CellAt(cell.X + 1, cell.Y).Type.IsEmpty)
					coordPotentials.Add(new Coords(cell.X + 1, cell.Y));
		
			if (coordPotentials.Count == 0)
				return null;
			else
			{
				int randomIndex = R.Next(coordPotentials.Count);
				return coordPotentials[randomIndex];
			}
		}

		private CellType RandomCellType(List<CellType> types)
		{
			// Pick a cell type randomly, and also eliminate it as a candidate for the current cell to avoid 
			// re-testing it in the future if it is rejected. DungeonCellTypes have weights, so some are more 
			// likely to be picked than others.
			
			int total = 0;
			
			foreach (CellType type in types)
			{
				total += type.Weight;
			}
			
			int threshold = R.Next(total);
			
			CellType selected = null;

			foreach (CellType type in types)
			{
				selected = type;
				threshold -= type.Weight;

				if (threshold < 0)
				{
					types.Remove(type);
					break;
				}
			}
			
			return selected;
		}

		private CellType RandomDungeonCellType(List<CellType> types)
		{
			// Pick a cell type randomly, and also eliminate it as a candidate for the current
			// cell to avoid re-testing it in the future if it is rejected. DungeonCellTypes
			// have weights, so some are more likely to be picked than others.
			
			int total = 0;
			
			foreach (CellType type in types)
			{
				total += type.Weight;
			}	
			
			int threshold = R.Next(total);
			
			CellType selectedType = null;
			
			foreach (CellType type in types)
			{
				threshold -= type.Weight;
				
				if (threshold < 0)
				{
					selectedType = type;
					types.Remove(type);
					break;
				}
			}
			
			return selectedType;
		}

		private Cell RandomForceGrowthCell(List<Cell> forceGrowthCells)
		{
			// Pick a cell randomly, and also eliminate it as a future candidate...
			
			Cell cell = forceGrowthCells[R.Next(forceGrowthCells.Count)];
			forceGrowthCells.Remove(cell);
			return cell;
		}
			
		#endregion

		#region Checks

		// With "coords" representing the new, empty dungeon location, check that each of the 
		// adjacent cells is compatible with the proposed (randomly determined) new cell type.
		private bool TypeCompatibleWithAdjacentCells(CellType newCellType, Coords coords)
		{
			// This is an innocent-until-proven guilty scenario. However, if any of the cells
			// is proven to be incompatible, that's enough to eliminate it as a prospect.
			
			Cell cellUp, cellDown, cellLeft, cellRight;
			
			if (coords.Y + 1 < GridHeight)
			{
				cellUp = CellAt(coords.X, coords.Y + 1);
				if (!newCellType.CompatibleWith(cellUp.Type, Direction.Up))
					return false;
			}
			
			if (coords.Y - 1 >= 0)
			{
				cellDown = CellAt(coords.X, coords.Y - 1);
				if (!newCellType.CompatibleWith(cellDown.Type, Direction.Down))
					return false;
			}
			
			if (coords.X - 1 >= 0)
			{
				cellLeft = CellAt(coords.X - 1, coords.Y);
				if (!newCellType.CompatibleWith(cellLeft.Type, Direction.Left))
					return false;
			}
			
			if (coords.X + 1 < GridWidth)
			{
				cellRight = CellAt(coords.X + 1, coords.Y);
				if (!newCellType.CompatibleWith(cellRight.Type, Direction.Right))
					return false;
			}
			
			return true;
		}

		// If the dungeon level was modified on the last pass, it cannot yet be considered 
		// complete. If it was not modified on the last pass, check to see if the dungeon 
		// level is filled to completion. If it is not, modify a cell to allow the dungeon 
		// to grow some more.
		bool CompleteCheck(bool modified)
		{
			bool complete = false;
			
			if (!modified)
			{
				int percentFilled = CalcPercentFilled();
				
				if (percentFilled >= 100)
					complete = true;
				else
				{
					if (!ForceGrowth())  // Modify a random cell to allow more growth.
					{
						// Sometimes, forceGrowth doesn't work to fill in all the cells in the dungeon level.
						// Frequently, this is because several rooms "block" growth into empty "pockets" near
						// the edge. One option at this point would be to perform some sort of additional 
						// "dungeon augmentation" by swapping corridor types for empty cells. But, that would 
						// involve a bunch of programming. Before I resort to that, I'm going to see how
						// feasible it is to simply abandon this "failed" dungeon level and start fresh...
						throw new LevelGenerateException();
					}
				}
			}

			return complete;
		}
			
		#endregion
		
		#region Accessors

		private Cell CellAt(int X, int Y)
		{
			return Grid[GridWidth * X + Y];
		}

		private void SetDungeonCellValue(int X, int Y, Cell cell)
		{
			int i = GridWidth * X + Y;
			Grid[i] = cell;
			RecordNewAttachment(cell);
		}
			
		#endregion

		#region Force Growth
			
		// Replace a random, non-empty, compatible cell with a different cell to see if that makes the
		// dungeon level grow any bigger. But, don't waste any cycles doing it, it could be a lost cause...
		private bool ForceGrowth()
		{
			bool success = false;
			bool typeMatch = false;
			
			CellType newType = null;
			List<Cell> cells = ForceGrowthCells();
			
			while (!success && cells.Count > 0) 
			{
				Cell cell = RandomForceGrowthCell(cells);
				Coords coords = new Coords(cell.X, cell.Y);
				
				// Attempt to replace it from the standard types.
				List<CellType> types = CellTypes.GetTypes(coords, GridWidth, GridHeight);
				types.Remove(cell.Type);  // ...but replace it with something different.
				
				while (!typeMatch) 
				{
					if (types.Count == 0) 
						break;  // If nothing replaces it, start over.
					
					newType = RandomDungeonCellType(types);  // Candidate new cell type.
					
					// The new cell needs to be compatible with each adjacent cell.
					if (TypeCompatibleWithAdjacentCells(newType, coords))
					{
						// It is? Cool.
						typeMatch = true;
					}
				}
				
				if (typeMatch) 
				{
					// Now set the new cell.
					Cell newCell = new Cell(coords.X, coords.Y, newType);
					SetDungeonCellValue(coords.X, coords.Y, newCell);
					success = true;
				}
			}

			return success;
		}

		// Gets all the cells in the dungeon that are not empty, and forceGrowthCompatible.
		private List<Cell> ForceGrowthCells()
		{
			List<Cell> cells = new List<Cell>();
			
			foreach (Cell cell in Grid)
			{
				if (cell.Type.ForceGrowthCompatible && !cell.Type.IsEmpty) 
				{
					cells.Add(cell);
				}
			}
			
			return cells;
		}

		#endregion

		#region Solve
		
		// Ensure that every cell in the dungeon is "reachable". If not, start fresh.
		// Also, check for anomalous disconnected cells. If any are found, scrap the level.
		private void LevelSolve()
		{
			SequenceNumber = 0;
			
			Solve(CellAt(StartCoords.X, StartCoords.Y));
			
			for (int Y = 0; Y < GridHeight; Y++) 
				for (int X = 0; X < GridWidth; X++)
					if (!CellAt(X, Y).Visited)
						throw new LevelGenerateException();
			
			CircularPassagewayCheck();
		}

		private void Solve(Cell cell)
		{
			SequenceNumber++;
			cell.Sequence = SequenceNumber;
			cell.Visited = true;
			
			// Cell above.
			if (cell.Type.TraversableUp && cell.Y + 1 < GridHeight)
			{
				Cell cellAbove = CellAt(cell.X, cell.Y + 1);
				// Method connectsCheck was commented out here. Required for rooms?
				if (!cellAbove.Visited)
					Solve(cellAbove);
			}

			// Cell below.
			if (cell.Type.TraversableDown && cell.Y - 1 >= 0)
			{
				Cell cellBelow = CellAt(cell.X, cell.Y - 1);
				// Method connectsCheck was commented out here. Required for rooms?
				if (!cellBelow.Visited)
					Solve(cellBelow);
			}
			
			// Cell left.
			if (cell.Type.TraversableLeft && cell.X - 1 >= 0)
			{
				Cell cellLeft = CellAt(cell.X - 1, cell.Y);
				// Method connectsCheck was commented out here. Required for rooms?
				if (!cellLeft.Visited)
					Solve(cellLeft);
			}
			
			// Cell right.
			if (cell.Type.TraversableRight && cell.X + 1 < GridWidth)
			{
				Cell cellRight = CellAt(cell.X + 1, cell.Y);
				// Method connectsCheck was commented out here. Required for rooms?
				if (!cellRight.Visited)
					Solve(cellRight);
			}
		}

		private void CircularPassagewayCheck()
		{
			for (int Y = 0; Y < GridHeight - 1; Y++) 
			{
				for (int X = 0; X < GridWidth - 1; X++)
				{
					Cell cell1 = CellAt(X, Y);
					Cell cell2 = CellAt(X, Y + 1);
					Cell cell3 = CellAt(X + 1, Y + 1);
					Cell cell4 = CellAt(X + 1, Y);
					
					if 
					(
						cell1.Type.ConnectsTo(cell2.Type, Direction.Up) &&
						cell2.Type.ConnectsTo(cell3.Type, Direction.Right) &&
						cell3.Type.ConnectsTo(cell4.Type, Direction.Down) &&
						cell4.Type.ConnectsTo(cell1.Type, Direction.Left)
					)
					{
						throw new LevelGenerateException();
					}
				}
			}
		}	

		#endregion

		#region Utility

		private int CalcPercentFilled()
		{
			int filledCellCount = 0;
			
			for (int X = 0; X < GridWidth; X++)
			{
				for (int Y = 0; Y < GridHeight; Y++) 
				{
					if (!CellAt(X, Y).Type.IsEmpty)
						filledCellCount++;
				}
			}
			
			return (filledCellCount * 100) / (GridWidth * GridHeight);
		}
		
		public string VisualizeAsText()
		{
			int X;
			Cell cell;
			string padding;
			StringBuilder line, grid = new StringBuilder();
	
			// Because it is console printing, start with the "top" of the dungeon, and work down.
			for (int Y = GridHeight - 1; Y >= 0; Y--) 
			{
				line = new StringBuilder();
				
				for (X = 0; X < GridWidth * 2; X++) 
				{
					cell = CellAt(X / 2, Y);
					line.Append(X % 2 == 0 ? cell.Type.TextRep : cell.Type.TextRep2);
				}
				
				padding = (Y < 10) ? "0" : "";  // For co-ordinate printing.
				
				grid.AppendLine(padding + Y + line.ToString());
			}
			
			// Now print X co-ordinate names at the bottom.
			
			grid.Append("  ");
			
			for (X = 0; X < GridWidth * 2; X++) 
			{
				if (X % 2 == 0)
					grid.Append((X/2)/10);
				else
					grid.Append(" "); 
			}
			
			grid.Append("\n  ");
			
			for (X = 0; X < GridWidth * 2; X++) 
			{
				if (X % 2 == 0)
					grid.Append((X/2) % 10);
				else
					grid.Append(" "); 
			}

			return grid.ToString();
		}

		#endregion
	}

	public class LevelGenerateException : Exception
	{
		public LevelGenerateException() : base() {}
	}
}