// Current issues: 1) non-square grids either crash or render weird (unsolved areas)
//                 2) no rooms, but the code for it is in the archive /Users/wizard/Documents/Archive/Game Concepts/New School
// Strategy: square grids currently work, so for demonstration purposes this is OK. The issues with the non-
//           square grids is concerning, but it probably makes sense to go ahead and port the rooms code in
//           and double-check everything, then debug everything later if this is really something that should
//           get built out into a product.

using System;
using System.Text;
using System.Collections.Generic;

namespace DigitalWizardry.LevelGenerator
{	
	public class Level
	{
		private List<Cell> Grid;
		private Random R;
		private Coords StartCoords;
		private int SequenceNumber;
		private Cell DownCell;  // The dead-end cell chosen to be replaced with a down stairs.
		private Double MaxDistance;
		private Double DownCellDistance;
		private int RoomCount;
		private int MinesCount;
		private int CatacombsCount;
		private List<Room> Rooms;
		private List<Cell> CellsWithLockedDoors;
		private int BuildPasses;  // Number of discarded attempts before arriving at a completed level.
		private TimeSpan BuildTime;  // How long in total did it take to build this level?

		public Level(Coords startCoords)
		{
			R = new Random();
			StartCoords = startCoords;
			Globals.Initialize();
			Start();
		}

		private void Start()
		{
			bool levelComplete = false;
			DateTime start = DateTime.Now;

			do
			{
				try 
				{
					BuildPasses++;
					
					Initialize();
					PlaceRooms();
					GenerateLevel();
					//PlaceDoors();
					LevelSolve();
					//PlaceKeys();
					//PlaceDownStairs();
					//AddDescriptions();
					levelComplete = true;  // i.e. no exceptions...
				}
				catch (LevelGenerateException) 
				{
					levelComplete = false;   // Try again.
				}

			} while (!levelComplete);

			BuildTime = DateTime.Now - start;
		}

		private void Initialize()
		{
			Grid = new List<Cell>();

			// Fill in each cell with the "empty cell" object.
			for (int i = 0; i < Constants.GridWidth * Constants.GridHeight; i++)
			{
				Grid.Add(Globals.EmptyCell);
			}

			List<CellType> types = CellTypes.GetTypes(StartCoords);

			CellType newType = RandomCellType(types);
			// RP: Should the next be CellDescriptions.Corridor_TBD? Or something else?
			Cell newCell = new Cell(StartCoords.X, StartCoords.Y, newType, CellDescriptions.Corridor_TBD);

			SetDungeonCellValue(StartCoords.X, StartCoords.Y, newCell);
		}

		private void GenerateLevel()
		{    
			Cell cell;
			bool modified = false;
			CellDescription descr = CellDescriptions.Corridor_TBD;
			
			// As long as the dungeon is not considered complete, keep adding stuff to it.
			do 
			{
				modified = false;
				
				for (int y = 0; y < Constants.GridHeight; y++) 
				{
					for (int x = 0; x < Constants.GridWidth; x++) 
					{
						cell = CellAt(x, y);
						
						if (!cell.Type.IsEmpty && !cell.AttachBlocked && cell.AvailableConnections > 0)
						{
							// Attach a new random cell to current cell, if possible. If the cell has 
							// available connections but nothing can be added to it, consider it blocked.
							if (AttachNewCell(cell, descr)) 
							{
								modified = true;
							}
							else  
							{
								cell.AttachBlocked = true;
							}
						}
					}
				}
			} while (!CompleteCheck(modified));
		}

		private bool AttachNewCell(Cell cell, CellDescription descr)
		{
			bool attachSuccessful = false;
			Coords coords = RandomAttachCoords(cell);
			
			if (coords != null)
			{
				// Get a disposable array of constructed corridor dungeon cell types.
				List<CellType> types = CellTypes.GetTypes(coords);
				
				// Choose a new cell type to attach.
				Cell newCell = null;
				CellType newType = null;
				
				while (newCell == null) 
				{
					if (types.Count == 0) 
					{
						return false;  // Whoops, there are no more possibilities.
					}
					
					newType = RandomCellType(types);
				
					// The new cell needs to be compatible with each adjacent cell.
					if (TypeCompatibleWithAdjacentCells(newType, coords))
					{
						newCell = new Cell(coords.X, coords.Y, newType, descr);
					}
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
			
			if (cell.Y + 1 < Constants.GridHeight)
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
			
			if (cell.X + 1 < Constants.GridWidth)
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
			if (cell.Type.ConnectsUp && cell.Y + 1 < Constants.GridHeight)
			{
				if (CellAt(cell.X, cell.Y + 1).Type.IsEmpty)
				{
					coordPotentials.Add(new Coords(cell.X, cell.Y + 1));
				}
			}
			
			// Cell below.
			if (cell.Type.ConnectsDown && cell.Y - 1 >= 0)
			{
				if (CellAt(cell.X, cell.Y - 1).Type.IsEmpty)
				{
					coordPotentials.Add(new Coords(cell.X, cell.Y - 1));
				}
			}
		
			// Cell left.
			if (cell.Type.ConnectsLeft && cell.X - 1 >= 0)
			{
				if (CellAt(cell.X - 1, cell.Y).Type.IsEmpty)
				{
					coordPotentials.Add(new Coords(cell.X - 1, cell.Y));
				}
			}
			
			// Cell right.
			if (cell.Type.ConnectsRight && cell.X + 1 < Constants.GridWidth)
			{
				if (CellAt(cell.X + 1, cell.Y).Type.IsEmpty)
				{
					coordPotentials.Add(new Coords(cell.X + 1, cell.Y));
				}
			}
		
			if (coordPotentials.Count == 0)
			{
				return null;
			}
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
			
			if (coords.Y + 1 < Constants.GridHeight)
			{
				cellUp = CellAt(coords.X, coords.Y + 1);
				if (!newCellType.CompatibleWith(cellUp.Type, Direction.Up))
				{
					return false;
				}
			}
			
			if (coords.Y - 1 >= 0)
			{
				cellDown = CellAt(coords.X, coords.Y - 1);
				if (!newCellType.CompatibleWith(cellDown.Type, Direction.Down))
				{
					return false;
				}
			}
			
			if (coords.X - 1 >= 0)
			{
				cellLeft = CellAt(coords.X - 1, coords.Y);
				if (!newCellType.CompatibleWith(cellLeft.Type, Direction.Left))
				{
					return false;
				}
			}
			
			if (coords.X + 1 < Constants.GridWidth)
			{
				cellRight = CellAt(coords.X + 1, coords.Y);
				if (!newCellType.CompatibleWith(cellRight.Type, Direction.Right))
				{
					return false;
				}
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
				{
					complete = true;
				}
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
		#region Rooms

		private void PlaceRooms()
		{	
			Rooms = new List<Room>();
			
			CalcRooms();
			PlaceMines();
			// [self placeRegularRooms:regularCount];
			// [self mergeRooms];
			// [self placeRoundRoom];
			// [self cleanRoomScraps];
			// [self cleanRoomsArray];
			// [self connectRooms];
			// [self connectMines];
			// [self cleanRoomsArray];
			// [self convertRoomsToCatacombs:catacombsCount];
		}

		private void CalcRooms()
		{
			int rand = 0;
			MinesCount = 0;

			rand = R.Next(100);
			
			if (rand >= 85 && rand <= 99)
			{
				MinesCount = 1;
			}
	 
			CatacombsCount = 0;

			rand = R.Next(100);
			
			if (rand >= 70 && rand < 92)
			{
				CatacombsCount = 1;
			}
			else if (rand >= 92 && rand < 98)
			{	
				CatacombsCount = 2;
			}
			else if (rand >= 98)
			{
				CatacombsCount = 3;
			}

			int rooms = R.Next(Constants.MaxRoom - Constants.MinRooms + 1) + Constants.MinRooms;  // MinRooms ~ MaxRooms
			
			RoomCount = rooms - MinesCount;
		}

		private void PlaceMines()
		{
			int attempts = 0;
			int count = MinesCount;  // Preserve original count.
			int maxAttempts = Constants.GridWidth * Constants.GridHeight;
			
			while (count > 0 && attempts <= maxAttempts) 
			{
				bool horiz = RandomBool();
			
				if (horiz)
				{
					if (RandomRoom(CellDescriptions.Mines_Horiz, Constants.MaxMinesHeight, Constants.MaxMinesWidth, Constants.MinMinesHeight, Constants.MinMinesWidth))
					{
						count--;
						attempts = 0;
					}
					else
					{
						attempts++;
					}
				}
				else
				{
					if (RandomRoom(CellDescriptions.Mines_Vert, Constants.MaxMinesWidth, Constants.MaxMinesHeight, Constants.MinMinesWidth, Constants.MinMinesHeight))
					{
						count--;
						attempts = 0;
					}
					else
					{
						attempts++;
					}
				}
			}
		}

		private bool RandomRoom(CellDescription descr, int maxWidth, int maxHeight, int minWidth, int minHeight)
		{
			int width = R.Next(maxWidth - minWidth + 1) + minWidth;
			int height = R.Next(maxHeight - minHeight + 1) + minHeight;
			
			Coords coords = RandomCell(true);  // Get a random empty cell as the attach point.
			
			if (!RoomFits(coords, width, height, false))
			{
				return false;
			}
			else
			{
				if (descr == CellDescriptions.Room_TBD)
				{
					bool irregularlyShapedRoom = RandomBool();
					
					// if (irregularlyShapedRoom && width > 2 && height > 2)
					// 	[self buildIrregularlyShapedRoom:coords width:width height:height];
					// else
					// 	[self buildRoom:Regular coords:coords width:width height:height];
				}
				else 
				{
					// [self buildMines:coords width:width height:height desc:desc];
					BuildMines(coords, width, height, descr);
				}
			}
			return true;
		}

		// Returns YES if the room fits into the dungeon entirely within allowable, currently-empty cells.
		private bool RoomFits(Coords coords, int width, int height, bool round)
		{
			Cell cell = null;
			bool fits = true;  // Innocent until proven guilty.
			
			if (coords.X + width >  Constants.GridWidth || coords.Y + height > Constants.GridHeight)
			{
				fits = false;
			}
			else
			{
				for (int y = coords.Y; y < coords.Y + height && fits == true; y++) 
				{
					for (int x = coords.X; x < coords.X + width; x++) 
					{
						cell = CellAt(x, y);
						
						if 
						(
							CellDescriptions.IsMines(cell.Descr) ||
							(
								y > StartCoords.Y - 2 && y < StartCoords.Y + 2 && 
								x > StartCoords.X - 2 && x < StartCoords.X + 2
								// Maintain a 2-cell margin around the start cell to allow it to be connected.
							) 
						) 
						{
							fits = false;
							break;
						}
						else if (round && !cell.Type.IsEmpty)
						{
							fits = false;
							break;
						}
					}
				}
			}
			return fits;
		}

		private void BuildMines(Coords coords, int width, int height, CellDescription descr)
		{
			CellType newType = null;
			
			for (int y = coords.Y; y < coords.Y + height; y++) 
			{
				for (int x = coords.X; x < coords.X + width; x++) 
				{
					if (x == coords.X && y == coords.Y)                                // Bottom-left corner.
					{
						newType = CellTypes.elbUR;
					}
					else if (x == coords.X + width - 1 && y == coords.Y)               // Bottom-right corner.
					{
						newType = CellTypes.elbUL;
					}
					else if (x == coords.X && y == coords.Y + height - 1)              // Top-left corner.
					{
						newType = CellTypes.elbDR;
					}
					else if (x == coords.X + width - 1 && y == coords.Y + height - 1)  // Top-right corner.
					{
						newType = CellTypes.elbDL;
					}
					else if (x == coords.X)                                            // Left wall.
					{
						newType = MinesWall(x, y, Direction.Left, descr);
					}
					else if (x == coords.X + width - 1)                                // Right wall.
					{
						newType = MinesWall(x, y, Direction.Right, descr);   
					}
					else if (y == coords.Y)                                            // Bottom wall.
					{
						newType = MinesWall(x, y, Direction.Down, descr);
					}
					else if (y == coords.Y + height - 1)                               // Top wall.
					{
						newType = MinesWall(x, y, Direction.Up, descr);
					}
					else if (descr == CellDescriptions.Mines_Horiz)
					{
						newType = CellTypes.horiz;
					}
					else if (descr == CellDescriptions.Mines_Vert)
					{
						newType = CellTypes.vert;
					}
					
					Cell currentCell = CellAt(x, y);
					
					if (currentCell.Type.IsEmpty)
					{
						Cell newCell = new Cell(x, y, newType, descr);
						SetDungeonCellValue(x, y, newCell);
					}
				}
			}
		}

		private CellType MinesWall(int x, int y, Direction dir, CellDescription desc)
		{
			CellType type = null;
		
			// Room exits can only occur if the room wall in question is at least 1 cell
			// from the dungeon edge, to allow the resulting corridors to grow or be "capped".
			
			if (dir == Direction.Up)                             
			{
				if (desc == CellDescriptions.Mines_Horiz)
				{
					type = CellTypes.horiz;    // No exit for horizontal mine.
				}
				else if (y + 1 < Constants.GridHeight)
				{
					type = CellTypes.inter;    // Exit for vertical mine.
				}
				else
				{
					type = CellTypes.juncDLR;  // No exit for vertical mine.
				}
			}
			else if (dir == Direction.Down)                                            
			{
				if (desc == CellDescriptions.Mines_Horiz)
				{
					type = CellTypes.horiz;    // No exit for horizontal mine.
				}
				else if (y - 1 >= 0)
				{
					type = CellTypes.inter;    // Exit for vertical mine.
				}
				else
				{
					type = CellTypes.juncULR;  // No exit for vertical mine.
				}
			}
			else if (dir == Direction.Left)                                              
			{        
				if (desc == CellDescriptions.Mines_Vert)
				{
					type = CellTypes.vert;    // No exit for vertical mine.
				}
				else if (x - 1 >= 0)
				{
					type = CellTypes.inter;    // Exit for horizontal mine.
				}
				else
				{
					type = CellTypes.juncUDR;  // No exit for horizontal mine.
				}
			}                                
			else if (dir == Direction.Right)                                  
			{
				if (desc == CellDescriptions.Mines_Vert)
				{
					type = CellTypes.vert;    // No exit for vertical mine.
				}
				else if (x + 1 < Constants.GridHeight)
				{
					type = CellTypes.inter;    // Exit for horizontal mine.
				}
				else
				{
					type = CellTypes.juncUDL;  // No exit for horizontal mine.
				}
			}
			
			return type;
		}

		#endregion
		#region Accessors

		private Cell CellAt(int x, int y)
		{
			return Grid[Constants.GridWidth * x + y];
		}

		private void SetDungeonCellValue(int x, int y, Cell cell)
		{
			int i = Constants.GridWidth * x + y;
			Grid[i] = cell;
			RecordNewAttachment(cell);
		}

		// Returns a random cell from the dungeon level. If empty == YES, then the random
		// cell will be empty. If empty == NO, then the random cell will be occupied.
		private Coords RandomCell(bool empty)
		{
			Coords coords = null;
			
			while (coords == null) 
			{
				int x = R.Next(Constants.GridWidth);
				int y = R.Next(Constants.GridHeight);
				
				Cell cell = CellAt(x, y);
				
				if (empty)  // Cell must be occupied.
				{
					if (cell.Type.IsEmpty) 
					{
						coords = new Coords(x, y);
					}
				}
				else  // Cell must be occupied.
				{
					if (!cell.Type.IsEmpty) 
					{
						coords = new Coords(x, y);
					}
				}
			}
			
			return coords;
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
			CellDescription descr = null;
			List<Cell> cells = ForceGrowthCells();
			
			while (!success && cells.Count > 0) 
			{
				Cell cell = RandomForceGrowthCell(cells);
				Coords coords = new Coords(cell.X, cell.Y);
				
				// Attempt to replace it from the standard types.
				List<CellType> types = CellTypes.GetTypes(coords);
				types.Remove(cell.Type);  // ...but replace it with something different.
				
				while (!typeMatch) 
				{
					if (types.Count == 0) 
					{
						break;  // If nothing replaces it, start over.
					}
					
					newType = RandomDungeonCellType(types);  // Candidate new cell type.
					
					// The new cell needs to be compatible with each adjacent cell.
					if (TypeCompatibleWithAdjacentCells(newType, coords))
					{
						// It is? Cool.
						descr = cell.Descr;
						typeMatch = true;
					}
				}
				
				if (typeMatch) 
				{
					// Now set the new cell.
					Cell newCell = new Cell(coords.X, coords.Y, newType, descr);
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
			
			for (int Y = 0; Y < Constants.GridHeight; Y++) 
			{
				for (int X = 0; X < Constants.GridWidth; X++)
				{
					if (!CellAt(X, Y).Visited)
					{
						throw new LevelGenerateException();
					}
				}
			}
			
			CircularPassagewayCheck();
		}

		private void Solve(Cell cell)
		{
			SequenceNumber++;
			cell.Sequence = SequenceNumber;
			cell.Visited = true;
			
			// Cell above.
			if (cell.Type.TraversableUp && cell.Y + 1 < Constants.GridHeight)
			{
				Cell cellAbove = CellAt(cell.X, cell.Y + 1);
				// Method connectsCheck was commented out here. Required for rooms?
				if (!cellAbove.Visited)
				{
					Solve(cellAbove);
				}
			}

			// Cell below.
			if (cell.Type.TraversableDown && cell.Y - 1 >= 0)
			{
				Cell cellBelow = CellAt(cell.X, cell.Y - 1);
				// Method connectsCheck was commented out here. Required for rooms?
				if (!cellBelow.Visited)
				{
					Solve(cellBelow);
				}
			}
			
			// Cell left.
			if (cell.Type.TraversableLeft && cell.X - 1 >= 0)
			{
				Cell cellLeft = CellAt(cell.X - 1, cell.Y);
				// Method connectsCheck was commented out here. Required for rooms?
				if (!cellLeft.Visited)
				{
					Solve(cellLeft);
				}
			}
			
			// Cell right.
			if (cell.Type.TraversableRight && cell.X + 1 < Constants.GridWidth)
			{
				Cell cellRight = CellAt(cell.X + 1, cell.Y);
				// Method connectsCheck was commented out here. Required for rooms?
				if (!cellRight.Visited)
				{
					Solve(cellRight);
				}
			}
		}

		private void CircularPassagewayCheck()
		{
			for (int Y = 0; Y < Constants.GridHeight - 1; Y++) 
			{
				for (int X = 0; X < Constants.GridWidth - 1; X++)
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

		private bool RandomBool()
		{
			return R.Next(2) == 0 ? false : true;
		}

		private int CalcPercentFilled()
		{
			int filledCellCount = 0;
			
			for (int x = 0; x < Constants.GridWidth; x++)
			{
				for (int y = 0; y < Constants.GridHeight; y++) 
				{
					if (!CellAt(x, y).Type.IsEmpty)
					{
						filledCellCount++;
					}
				}
			}
			
			return (filledCellCount * 100) / (Constants.GridWidth * Constants.GridHeight);
		}
		
		public string VisualizeAsText()
		{
			int x;
			Cell cell;
			string padding;
			StringBuilder line, grid = new StringBuilder();
	
			// Because it is console printing, start with the "top" of the dungeon, and work down.
			for (int y = Constants.GridHeight - 1; y >= 0; y--) 
			{
				line = new StringBuilder();
				
				for (x = 0; x < Constants.GridWidth * 2; x++) 
				{
					cell = CellAt(x / 2, y);
					line.Append(x % 2 == 0 ? cell.Type.TextRep : cell.Type.TextRep2);
				}
				
				padding = (y < 10) ? "0" : "";  // For co-ordinate printing.
				
				grid.AppendLine(padding + y + line.ToString());
			}
			
			// Now print X co-ordinate names at the bottom.
			
			grid.Append("  ");
			
			for (x = 0; x < Constants.GridWidth * 2; x++) 
			{
				if (x % 2 == 0)
					grid.Append((x/2)/10);
				else
					grid.Append(" "); 
			}
			
			grid.Append("\n  ");
			
			for (x = 0; x < Constants.GridWidth * 2; x++) 
			{
				if (x % 2 == 0)
					grid.Append((x/2) % 10);
				else
					grid.Append(" "); 
			}

			return grid.ToString();
		}

		public string Stats()
		{
			return Environment.NewLine +
			       "Build Passes: " + BuildPasses.ToString() + Environment.NewLine +
				   "Build Time: " + BuildTime.ToString() + Environment.NewLine +
				   "Room Count: " + RoomCount.ToString() + Environment.NewLine +
				   "Mines Count: " + MinesCount.ToString() + Environment.NewLine +
				   "Catacombs Count: " + CatacombsCount.ToString();
		}

		#endregion
	}

	public class LevelGenerateException : Exception
	{
		public LevelGenerateException() : base() {}
	}
}