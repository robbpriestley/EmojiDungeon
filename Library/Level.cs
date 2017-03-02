using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalWizardry.LevelGenerator
{	
	public class Level
	{
		public int LevelNumber;
		private int GridWidth;
		private int GridHeight;
		private Coords StartCoords;
		private List<Cell> Grid;

		public Level(int levelNumber, int width, int height, Coords startCoords)
		{
			GridWidth = width;
			GridHeight = height;
			LevelNumber = levelNumber;
			StartCoords = startCoords;
			
			Globals.Initialize();
		
			Generate();
		}

		private void Generate()
		{
			int buildPasses = 0;
			bool levelComplete = false;

			do
			{
				try 
				{
					buildPasses++;
			
					Initialize();
					//[self generateLevel];
					//[self levelSolve];
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

			List<CellType> types = CellTypes.GetTypes(StartCoords);

			CellType newType = RandomCellType(types);
			Cell newCell = new Cell(StartCoords.X, StartCoords.Y, newType);

			SetDungeonCellValue(StartCoords.X, StartCoords.Y, newCell);
		}

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

		private CellType RandomCellType(List<CellType> types)
		{
			// Pick a cell type randomly, and also eliminate it as a candidate for the current
			// cell to avoid re-testing it in the future if it is rejected. DungeonCellTypes
			// have weights, so some are more likely to be picked than others.
			
			int total = 0;
			
			foreach (CellType type in types)
			{
				total += type.Weight;
			}
			
			Random r = new Random(); 
			int threshold = r.Next(total);
			
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

		public string VisualizeAsText()
		{
			int X;
			Cell cell;
			string padding;
			StringBuilder line, grid = new StringBuilder();

			grid.AppendLine();  // Initial spacer.
	
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
			
			grid.Append("\n\n");

			return grid.ToString();
		}
	}

	public class LevelGenerateException : Exception
	{
		public LevelGenerateException() : base() {}
	}
}