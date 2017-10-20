using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using DigitalWizardry.Dungeon.Models;

namespace DigitalWizardry.Dungeon
{	
	public class Dungeon
	{
		private int _level;
		private Cell[,] _grid;                     // Master grid data structure, a "simulated 2-D array".
		private Random _r;                         // Re-usable random number generator.
		private Cell _emptyCell;                   // This empty cell instance is re-used everywhere. It exists outside of "normal space" because its coords are -1,-1.
		private Coords _startCoords;               // Where the entrance to the dungeon is located.
		private int _sequenceNumber;               // Used when solving the dungeon to "stamp" cells with their sequence number.
		private Double _maxDistance;               // Maximum possible distance between points in the grid.
		private Cell _downStairsCell;              // The dead-end cell chosen to be replaced with a down stairs.
		private Double _downStairsCellDistance;    // Distance from the start cell to the DownStairsCell.
		private int _roomsCount;                   // Number of rooms randomly determined to be in the dungeon.
		private List<Room> _rooms;                 // The collection of rooms added to the dungeon.
		private List<Cell> _cellsWithDoors;        // Convenience collection of door objects.
		private int _iterations;                   // Number of discarded attempts before arriving at a completed dungeon.
		private TimeSpan _elapsedTime;             // How long in total did it take to build this dungeon?

		public Dungeon(int level, int startX, int startY, Direction start)
		{	
			_level = level;
			_r = new Random();
			_startCoords = new Coords(startX, startY);
			_emptyCell = new Cell(-1, -1, CellTypes.EmptyCell);
			_maxDistance = Math.Abs(Math.Sqrt(Math.Pow(Reference.GridWidth - 1, 2) + Math.Pow(Reference.GridHeight - 1, 2)));
			Start(start);
		}

		private void Start(Direction start)
		{
			bool dungeonComplete = false;
			DateTime startTime = DateTime.Now;

			do
			{
				try 
				{
					_iterations++;
					
					Initialize(start);
					PlaceRooms();
					GenerateDungeon();
					PlaceDoors();
					DungeonSolve();
					PlaceKeys();
					PlaceGems();
					PlaceHearts();
					PlaceSwords();
					PlaceGoblins();
					PlaceDownStairs();
					dungeonComplete = true;
				}
				catch (DungeonGenerateException) 
				{
					dungeonComplete = false;
				}

			} while (!dungeonComplete);

			_elapsedTime = DateTime.Now - startTime;
		}

		private void Initialize(Direction start)
		{
			_grid = new Cell[Reference.GridWidth, Reference.GridHeight];

			// Fill in each cell with the "empty cell" object.
			for (int y = 0; y < Reference.GridHeight; y++)
			{
				for (int x = 0; x < Reference.GridWidth; x++)
				{
					_grid[x, y] = _emptyCell;
				}
			}

			// The first level (level 1) dungeon is outfitted with the start cell at bottom center.
			if (_level == 1) 
			{
				PlaceEntrance();
			}
			else  // Otherwise, it "inherits" start cell (i.e. stairs) leading up to "earlier" level.
			{
				PlaceStairs(start);
			}

			List<CellType> types = CellTypes.GetTypes(_startCoords);

			_cellsWithDoors = new List<Cell>();
		}

		private void PlaceEntrance()
		{
			Cell entrance = new Cell(_startCoords.X, _startCoords.Y, CellTypes.Entrance);
			SetCellValue(_startCoords.X, _startCoords.Y, entrance);
		}

		private void PlaceStairs(Direction start)
		{
			CellType startType = null;

			switch (start)
			{
				case Direction.Up:
					startType = CellTypes.UpStairsD;
					break;
				
				case Direction.Down:
					startType = CellTypes.UpStairsU;
					break;

				case Direction.Left:
					startType = CellTypes.UpStairsR;
					break;

				case Direction.Right:
					startType = CellTypes.UpStairsL;
					break;

				default:
					break;
			}

			Cell stairs = new Cell(_startCoords.X, _startCoords.Y, startType);
			SetCellValue(_startCoords.X, _startCoords.Y, stairs);
		}

		private void GenerateDungeon()
		{    
			Cell cell;
			bool modified = false;
			
			// As long as the dungeon is not considered complete, keep adding stuff to it.
			do 
			{
				modified = false;
				
				for (int y = 0; y < Reference.GridHeight; y++) 
				{
					for (int x = 0; x < Reference.GridWidth; x++) 
					{
						cell = _grid[x, y];
						
						if (!cell.Type.IsEmpty && !cell.AttachBlocked && cell.AvailableConnections > 0)
						{
							// Attach a new random cell to current cell, if possible. If the cell has 
							// available connections but nothing can be added to it, consider it blocked.
							if (AttachNewCell(cell)) 
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

		private bool AttachNewCell(Cell cell)
		{
			bool attachSuccessful = false;
			Coords coords = RandomAttachCoords(cell);
			
			if (coords != null)
			{
				// Get a disposable array of constructed corridor cell types.
				List<CellType> types = CellTypes.GetTypes(coords);
				
				// Choose a new cell type to attach.
				Cell newCell = null;
				CellType newType = null;
				
				while (newCell == null) 
				{
					if (types.Count == 0) 
					{
						return false;  // There are no more possibilities.
					}
					
					newType = RandomCellType(types);
				
					// The new cell needs to be compatible with each adjacent cell.
					if (TypeCompatibleWithAdjacentCells(newType, coords))
					{
						newCell = new Cell(coords.X, coords.Y, newType);
					}
				}
				
				SetCellValue(coords.X, coords.Y, newCell);
				attachSuccessful = true;
			}
			
			return attachSuccessful;
		}

		// When a new cell is placed in the dungeon, "record" it as such by decrementing the 
		//availableConnections count of each adjacent, non-empty cell that connects to it.
		// Also, decrement the availableConnections count of the new cell accordingly.
		private void RecordNewAttachment(Cell cell)
		{
			Cell cellUp, cellDown, cellLeft, cellRight;
			
			if (cell.Y + 1 < Reference.GridHeight)
			{
				cellUp = _grid[cell.X, cell.Y + 1];

				if (cell.Type.ConnectsTo(cellUp.Type, Direction.Up))
				{
					cell.AvailableConnections--;
					cellUp.AvailableConnections--;
				}
			}
			
			if (cell.Y - 1 >= 0)
			{
				cellDown = _grid[cell.X, cell.Y - 1];

				if (cell.Type.ConnectsTo(cellDown.Type, Direction.Down))
				{
					cell.AvailableConnections--;
					cellDown.AvailableConnections--;
				}
			}
			
			if (cell.X - 1 >= 0)
			{
				cellLeft = _grid[cell.X - 1, cell.Y];

				if (cell.Type.ConnectsTo(cellLeft.Type, Direction.Left))
				{
					cell.AvailableConnections--;
					cellLeft.AvailableConnections--;
				}
			}
			
			if (cell.X + 1 < Reference.GridWidth)
			{
				cellRight = _grid[cell.X + 1, cell.Y];

				if (cell.Type.ConnectsTo(cellRight.Type, Direction.Right))
				{
					cell.AvailableConnections--;
					cellRight.AvailableConnections--;
				}
			}
		}

		// Set a down stairs at the farthest possible location from the start cell.
		private void PlaceDownStairs()
		{
			Cell newCell;
			CellType newType;
			
			newType = CellTypes.ConvertDeadEndToDownStairs(_downStairsCell.Type);
			newCell = new Cell(_downStairsCell.X, _downStairsCell.Y, newType);
			SetCellValue(_downStairsCell.X, _downStairsCell.Y, newCell);
		}

		#region Randomization

		// Find which locations adjacent to the current cell could be populated with a new attaching cell. 
		// Then, from those locations, select one at random and return the coords for it. If no such 
		// location exists, return nil.
		private Coords RandomAttachCoords(Cell cell)
		{
			List<Coords> coordPotentials = new List<Coords>();
			
			// Check each direction for an adjacent cell. Obviously, an adjacent cell has to be within the 
			// coordinate bounds of the dungeon. If the current cell has the capability to join with a cell
			// in the adjacent location, and the cell in the adjacent location is empty, add the coords for 
			// each such location to the coordPotentials array. Then choose one of those potential coords at 
			// random and return it. If there are no potentials at all, return null.
			
			// Cell above.
			if (cell.Type.ConnectsUp && cell.Y + 1 < Reference.GridHeight)
			{
				if (_grid[cell.X, cell.Y + 1].Type.IsEmpty)
				{
					coordPotentials.Add(new Coords(cell.X, cell.Y + 1));
				}
			}
			
			// Cell below.
			if (cell.Type.ConnectsDown && cell.Y - 1 >= 0)
			{
				if (_grid[cell.X, cell.Y - 1].Type.IsEmpty)
				{
					coordPotentials.Add(new Coords(cell.X, cell.Y - 1));
				}
			}
		
			// Cell left.
			if (cell.Type.ConnectsLeft && cell.X - 1 >= 0)
			{
				if (_grid[cell.X - 1, cell.Y].Type.IsEmpty)
				{
					coordPotentials.Add(new Coords(cell.X - 1, cell.Y));
				}
			}
			
			// Cell right.
			if (cell.Type.ConnectsRight && cell.X + 1 < Reference.GridWidth)
			{
				if (_grid[cell.X + 1, cell.Y].Type.IsEmpty)
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
				int randomIndex = _r.Next(coordPotentials.Count);
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
			
			int threshold = _r.Next(total);
			
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

		private Cell RandomForceGrowthCell(List<Cell> forceGrowthCells)
		{
			// Pick a cell randomly, and also eliminate it as a future candidate...
			Cell cell = forceGrowthCells[_r.Next(forceGrowthCells.Count)];
			forceGrowthCells.Remove(cell);
			return cell;
		}
			
		#endregion

		#region Checks

		// With "coords" representing the new, empty dungeon location, check that each of the adjacent cells 
		// is compatible with the proposed (randomly determined) new cell type.
		private bool TypeCompatibleWithAdjacentCells(CellType newCellType, Coords coords)
		{
			// This is an innocent-until-proven guilty scenario. However, if any of the cells is proven to be
			// incompatible, that's enough to eliminate it as a prospect.
			
			Cell cellUp, cellDown, cellLeft, cellRight;
			
			if (coords.Y + 1 < Reference.GridHeight)
			{
				cellUp = _grid[coords.X, coords.Y + 1];
				if (!newCellType.CompatibleWith(cellUp.Type, Direction.Up))
				{
					return false;
				}
			}
			
			if (coords.Y - 1 >= 0)
			{
				cellDown = _grid[coords.X, coords.Y - 1];
				if (!newCellType.CompatibleWith(cellDown.Type, Direction.Down))
				{
					return false;
				}
			}
			
			if (coords.X - 1 >= 0)
			{
				cellLeft = _grid[coords.X - 1, coords.Y];
				if (!newCellType.CompatibleWith(cellLeft.Type, Direction.Left))
				{
					return false;
				}
			}
			
			if (coords.X + 1 < Reference.GridWidth)
			{
				cellRight = _grid[coords.X + 1, coords.Y];
				if (!newCellType.CompatibleWith(cellRight.Type, Direction.Right))
				{
					return false;
				}
			}
			
			return true;
		}

		// If the dungeon was modified on the last pass, it cannot yet be considered complete. If it was 
		// not modified on the last pass, check to see if the dungeon is filled to completion. If it is
		// not, modify a cell to allow the dungeon to grow some more.
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
					if (!ForceGrowth())  // Try to modify random cells to allow more growth.
					{
						throw new DungeonGenerateException();
					}
				}
			}

			return complete;
		}
			
		#endregion
		#region Rooms

		private void PlaceRooms()
		{	
			_rooms = new List<Room>();
			
			CalcRooms();
			PlaceRegularRooms();
			MergeRooms();
			CleanRoomScraps();
			CleanRoomsArray();
			ConnectRooms();
			CleanRoomsArray();
		}

		private void CalcRooms()
		{
			_roomsCount = _r.Next(Reference.MaxRooms - Reference.MinRooms + 1) + Reference.MinRooms;  // MinRooms ~ MaxRooms
		}
		
		private void PlaceRegularRooms()
		{
			int attempts = 0;
			int roomsCount = _roomsCount;
			int maxAttempts = Reference.GridWidth * Reference.GridHeight;
			
			while (roomsCount > 0 && attempts <= maxAttempts) 
			{
				if (RandomRoom(Reference.MaxRoomWidth, Reference.MaxRoomHeight, Reference.MinRoomWidth, Reference.MinRoomHeight))
				{
					roomsCount--;
					attempts = 0;
				}
				else
				{
					attempts++;
				}
			}
		}

		private bool RandomRoom(int maxWidth, int maxHeight, int minWidth, int minHeight)
		{
			int width = _r.Next(maxWidth - minWidth + 1) + minWidth;
			int height = _r.Next(maxHeight - minHeight + 1) + minHeight;
			
			Coords coords = RandomCell(true);  // Get a random empty cell as the attach point.
			
			if (!RoomFits(coords, width, height))
			{
				return false;
			}
			else
			{
				bool irregularlyShapedRoom = RandomBool();
				
				if (irregularlyShapedRoom && width > 2 && height > 2)
				{
					BuildIrregularlyShapedRoom(coords, width, height);
				}
				else
				{
					BuildRoom(RoomType.Regular, coords, width, height);
				}
			}
			return true;
		}

		// Returns true if the room fits into the dungeon entirely within allowable, currently-empty cells.
		private bool RoomFits(Coords coords, int width, int height)
		{
			Cell cell = null;
			bool fits = true;  // Innocent until proven guilty.
			
			if (coords.X + width >  Reference.GridWidth || coords.Y + height > Reference.GridHeight)
			{
				fits = false;
			}
			else
			{
				for (int y = coords.Y; y < coords.Y + height && fits == true; y++) 
				{
					for (int x = coords.X; x < coords.X + width; x++) 
					{
						cell = _grid[x, y];
						
						if 
						(
							(
								y > _startCoords.Y - 2 && y < _startCoords.Y + 2 && 
								x > _startCoords.X - 2 && x < _startCoords.X + 2
								// Maintain a 2-cell margin around the start cell to allow it to be connected.
							) 
						) 
						{
							fits = false;
							break;
						}
					}
				}
			}
			return fits;
		}

		private Room BuildRoom(RoomType roomType, Coords coords, int width, int height)
		{	
			Room room = new Room(coords.X, coords.Y);
			
			int widthReduce = 0, heightReduce = 0;
			
			if (roomType != RoomType.Regular) 
			{
				widthReduce = _r.Next(width - 2) + 1;
				heightReduce = _r.Next(height - 2) + 1;
			}
			
			for (int y = coords.Y; y < coords.Y + height; y++) 
			{
				for (int x = coords.X; x < coords.X + width; x++) 
				{
					CellType newType = null;
					
					switch (roomType) 
					{                
						case RoomType.IrregularUL:
							newType = IrregRoomTypeUL(x, y, coords, width, height, widthReduce, heightReduce);
							break;
							
						case RoomType.IrregularUR:
							newType = IrregRoomTypeUR(x, y, coords, width, height, widthReduce, heightReduce);
							break;
						
						case RoomType.IrregularDL:
							newType = IrregRoomTypeDL(x, y, coords, width, height, widthReduce, heightReduce);
							break;
							
						case RoomType.IrregularDR:
							newType = IrregRoomTypeDR(x, y, coords, width, height, widthReduce, heightReduce);
							break;
							
						default:
							newType = RegRoomType(x, y, coords, width, height);
							break;
					}
					
					if (newType == null)
						continue;
					
					Cell currentCell = _grid[x, y];
					
					if (IncompatibleCornerTypes(currentCell.Type, newType))
					{
						DeleteRoom(room);
						return null;
					}
					
					Cell newCell = new Cell(x, y, newType);
					SetCellValue(x, y, newCell);
					
					if (newType == CellTypes.RoomSpace)
					{
						room.Space.Add(newCell);
					}
					else
					{
						room.Walls.Add(newCell);
					}
				}
			}
			
			return room;
		}

		private CellType RegRoomType(int x, int y, Coords coords, int width, int height)
		{
			CellType newType = null;
			
			if (x == coords.X && y == coords.Y)                                // Bottom-left corner.
			{
				newType = CellTypes.RoomWallDL;
			}
			else if (x == coords.X + width - 1 && y == coords.Y)               // Bottom-right corner.
			{
				newType = CellTypes.RoomWallDR;
			}
			else if (x == coords.X && y == coords.Y + height - 1)              // Top-left corner.
			{
				newType = CellTypes.RoomWallUL;
			}
			else if (x == coords.X + width - 1 && y == coords.Y + height - 1)  // Top-right corner.
			{
				newType = CellTypes.RoomWallUR;
			}
			else if (x == coords.X)                                            // Left wall.
			{
				newType = CellTypes.RoomWallL;
			}
			else if (x == coords.X + width - 1)                                // Right wall.
			{
				newType = CellTypes.RoomWallR;   
			}
			else if (y == coords.Y)                                            // Bottom wall.
			{
				newType = CellTypes.RoomWallD;
			}
			else if (y == coords.Y + height - 1)                               // Top wall.
			{
				newType = CellTypes.RoomWallU;
			}
			else
			{
				newType = CellTypes.RoomSpace;
			}
			
			return newType;
		}

		private void BuildIrregularlyShapedRoom(Coords coords, int width, int height)
		{
			int quadrant = _r.Next(4);
			
			switch (quadrant) 
			{
				case 0:
					/*
					   +-+
					   | |
					+--+ |
					|    |
					+----+
					*/
					BuildRoom(RoomType.IrregularUL, coords, width, height);
					break;
				
				case 1:
					/*
					+-+
					| |
					| +--+
					|    |
					+----+
					*/
					BuildRoom(RoomType.IrregularUR, coords, width, height);
					break;
					
				case 2:
					/*
					+----+
					|    |
					+--+ |
					   | |
					   +-+
					*/
					BuildRoom(RoomType.IrregularDL, coords, width, height);
					break;
					
				case 3:
					/*
					+----+
					|    |
					| +--+
					| |
					+-+
					*/
					BuildRoom(RoomType.IrregularDR, coords, width, height);
					break;
			}
		}

		/*
		   +-+
		   | |
		+--+ |
		|    |
		+----+
		*/
		private CellType IrregRoomTypeUL(int x, int y, Coords coords, int width, int height, int widthReduce, int heightReduce) 
		{
			CellType newType = null;
			
			if (y >= coords.Y && y < coords.Y + height - heightReduce - 1)         // Normal room section.
			{
				if (x == coords.X && y == coords.Y)                                // Bottom-left corner.
				{
					newType = CellTypes.RoomWallDL;
				}
				else if (x == coords.X + width - 1 && y == coords.Y)               // Bottom-right corner.
				{
					newType = CellTypes.RoomWallDR;
				}
				else if (x == coords.X && y == coords.Y + height - 1)              // Top-left corner.
				{
					newType = CellTypes.RoomWallUL;
				}
				else if (x == coords.X + width - 1 && y == coords.Y + height - 1)  // Top-right corner.
				{
					newType = CellTypes.RoomWallUR;
				}
				else if (x == coords.X)                                            // Left wall.
				{
					newType = CellTypes.RoomWallL;
				}
				else if (x == coords.X + width - 1)                                // Right wall.
				{
					newType = CellTypes.RoomWallR;   
				}
				else if (y == coords.Y)                                            // Bottom wall.
				{
					newType = CellTypes.RoomWallD;
				}
				else if (y == coords.Y + height - 1)                               // Top wall.
				{
					newType = CellTypes.RoomWallU;
				}
				else
				{
					newType = CellTypes.RoomSpace;
				}
			}
			else if (y == coords.Y + height - heightReduce - 1)  
			{
				if (x == coords.X)                                                
				{
					newType = CellTypes.RoomWallUL;
				}
				else if (x == coords.X + width - widthReduce - 1)                
				{
					newType = CellTypes.RoomWallDRinv;
				}
				else if (x == coords.X + width - 1)                                
				{
					newType = CellTypes.RoomWallR;
				}
				else if (x > coords.X && x < coords.X + width - widthReduce - 1) 
				{
					newType = CellTypes.RoomWallU;
				}
				else
				{
					newType = CellTypes.RoomSpace;
				}
			}
			else if (y > coords.Y + height - heightReduce - 1 && y < coords.Y + height - 1) 
			{
				if (x == coords.X + width - 1)                                            
				{
					newType = CellTypes.RoomWallR;
				}
				else if (x == coords.X + width - widthReduce - 1)                
				{
					newType = CellTypes.RoomWallL; 
				}
				else if (x > coords.X + width - widthReduce - 1)   
				{
					newType = CellTypes.RoomSpace;
				}
				//else Empty.
			}
			else
			{
				if (x == coords.X + width - 1)                                            
				{
					newType = CellTypes.RoomWallUR;
				}
				else if (x == coords.X + width - widthReduce - 1)               
				{
					newType = CellTypes.RoomWallUL;
				}
				else if (x > coords.X + width - widthReduce - 1)  
				{
					newType = CellTypes.RoomWallU;
				}
				//else Empty.
			}
			
			return newType;
		}

		/*
		+-+
		| |
		| +--+
		|    |
		+----+
		*/
		private CellType IrregRoomTypeUR(int x, int y, Coords coords, int width, int height, int widthReduce, int heightReduce)
		{
			CellType newType = null;
			
			if (y >= coords.Y && y < coords.Y + height - heightReduce - 1)         // Normal room section.
			{
				if (x == coords.X && y == coords.Y)                                // Bottom-left corner.
				{
					newType = CellTypes.RoomWallDL;
				}
				else if (x == coords.X + width - 1 && y == coords.Y)               // Bottom-right corner.
				{
					newType = CellTypes.RoomWallDR;
				}
				else if (x == coords.X && y == coords.Y + height - 1)              // Top-left corner.
				{
					newType = CellTypes.RoomWallUL;
				}
				else if (x == coords.X + width - 1 && y == coords.Y + height - 1)  // Top-right corner.
				{
					newType = CellTypes.RoomWallUR;
				}
				else if (x == coords.X)                                            // Left wall.
				{
					newType = CellTypes.RoomWallL;
				}
				else if (x == coords.X + width - 1)                                // Right wall.
				{
					newType = CellTypes.RoomWallR;   
				}
				else if (y == coords.Y)                                            // Bottom wall.
				{
					newType = CellTypes.RoomWallD;
				}
				else if (y == coords.Y + height - 1)                               // Top wall.
				{
					newType = CellTypes.RoomWallU;
				}
				else
				{
					newType = CellTypes.RoomSpace;
				}
			}
			else if (y == coords.Y + height - heightReduce - 1)  
			{
				if (x == coords.X)                                                 // Left wall.
				{
					newType = CellTypes.RoomWallL;
				}
				else if (x == coords.X + width - widthReduce - 1)                  // Bottom-left corner inv.
				{
					newType = CellTypes.RoomWallDLinv;
				}
				else if (x == coords.X + width - 1)                                // Top-right corner.
				{
					newType = CellTypes.RoomWallUR;
				}
				else if (x > coords.X + width - widthReduce - 1 && x < coords.X + width - 1)   // Top wall.
				{
					newType = CellTypes.RoomWallU;
				}
				else
				{
					newType = CellTypes.RoomSpace;
				}
			}
			else if (y > coords.Y + height - heightReduce - 1 && y < coords.Y + height - 1) 
			{
				if (x == coords.X)                                                 // Left wall.
				{
					newType = CellTypes.RoomWallL;
				}
				else if (x == coords.X + width - widthReduce - 1)                  // Right wall.
				{
					newType = CellTypes.RoomWallR; 
				}
				else if (x > coords.X && x < coords.X + width - widthReduce - 1)   // Room space.
				{
					newType = CellTypes.RoomSpace;
				}
				//else Empty.
			}
			else
			{
				if (x == coords.X)                                                 // Top-left corner.
				{
					newType = CellTypes.RoomWallUL;
				}
				else if (x == coords.X + width - widthReduce - 1)                  // Top-right corner.
				{
					newType = CellTypes.RoomWallUR;
				}
				else if (x > coords.X && x < coords.X + width - widthReduce - 1)   // Top wall.
				{
					newType = CellTypes.RoomWallU;
				}
				//else Empty.
			}
			
			return newType;
		}


		/*
		+----+
		|    |
		+--+ |
		   | |
		   +-+
		*/
		private CellType IrregRoomTypeDL(int x, int y, Coords coords, int width, int height, int widthReduce, int heightReduce)
		{
			CellType newType = null;
			
			if (y > coords.Y + height - heightReduce - 1)                          // Normal room section.
			{
				if (x == coords.X && y == coords.Y)                                // Bottom-left corner.
				{
					newType = CellTypes.RoomWallDL;
				}
				else if (x == coords.X + width - 1 && y == coords.Y)               // Bottom-right corner.
				{
					newType = CellTypes.RoomWallDR;
				}
				else if (x == coords.X && y == coords.Y + height - 1)              // Top-left corner.
				{
					newType = CellTypes.RoomWallUL;
				}
				else if (x == coords.X + width - 1 && y == coords.Y + height - 1)  // Top-right corner.
				{
					newType = CellTypes.RoomWallUR;
				}
				else if (x == coords.X)                                            // Left wall.
				{
					newType = CellTypes.RoomWallL;
				}
				else if (x == coords.X + width - 1)                                // Right wall.
				{
					newType = CellTypes.RoomWallR;   
				}
				else if (y == coords.Y)                                            // Bottom wall.
				{
					newType = CellTypes.RoomWallD;
				}
				else if (y == coords.Y + height - 1)                               // Top wall.
				{
					newType = CellTypes.RoomWallU;
				}
				else
				{
					newType = CellTypes.RoomSpace;
				}
			}
			else if (y == coords.Y + height - heightReduce - 1)  
			{
				if (x == coords.X)                                                
				{
					newType = CellTypes.RoomWallDL;
				}
				else if (x == coords.X + width - widthReduce - 1)                
				{
					newType = CellTypes.RoomWallURinv;
				}
				else if (x == coords.X + width - 1)                              
				{
					newType = CellTypes.RoomWallR;
				}
				else if (x > coords.X && x < coords.X + width - widthReduce - 1)   
				{
					newType = CellTypes.RoomWallD;
				}
				else
				{
					newType = CellTypes.RoomSpace;
				}
			}
			else if (y > coords.Y && y < coords.Y + height - heightReduce - 1) 
			{
				if (x == coords.X + width - 1)                                                
				{
					newType = CellTypes.RoomWallR;
				}
				else if (x == coords.X + width - widthReduce - 1)               
				{
					newType = CellTypes.RoomWallL; 
				}
				else if (x > coords.X + width - widthReduce - 1 && x < coords.X + width - 1)  
				{
					newType = CellTypes.RoomSpace;
				}
				//else Empty.
			}
			else
			{
				if (x == coords.X + width - 1)                                                 
				{
					newType = CellTypes.RoomWallDR;
				}
				else if (x == coords.X + width - widthReduce - 1)           
				{
					newType = CellTypes.RoomWallDL;
				}
				else if (x > coords.X + width - widthReduce - 1 && x < coords.X + width - 1)  
				{
					newType = CellTypes.RoomWallD;
				}
				//else Empty.
			}
			
			return newType;
		}


		/*
		+----+
		|    |
		| +--+
		| |
		+-+
		*/
		private CellType IrregRoomTypeDR(int x, int y, Coords coords, int width, int height, int widthReduce, int heightReduce) 
		{
			CellType newType = null;
			
			if (y > coords.Y + height - heightReduce - 1)                          // Normal room section.
			{
				if (x == coords.X && y == coords.Y)                                // Bottom-left corner.
				{
					newType = CellTypes.RoomWallDL;
				}
				else if (x == coords.X + width - 1 && y == coords.Y)               // Bottom-right corner.
				{
					newType = CellTypes.RoomWallDR;
				}
				else if (x == coords.X && y == coords.Y + height - 1)              // Top-left corner.
				{
					newType = CellTypes.RoomWallUL;
				}
				else if (x == coords.X + width - 1 && y == coords.Y + height - 1)  // Top-right corner.
				{
					newType = CellTypes.RoomWallUR;
				}
				else if (x == coords.X)                                            // Left wall.
				{
					newType = CellTypes.RoomWallL;
				}
				else if (x == coords.X + width - 1)                                // Right wall.
				{
					newType = CellTypes.RoomWallR;   
				}
				else if (y == coords.Y)                                            // Bottom wall.
				{
					newType = CellTypes.RoomWallD;
				}
				else if (y == coords.Y + height - 1)                               // Top wall.
				{
					newType = CellTypes.RoomWallU;
				}
				else
				{
					newType = CellTypes.RoomSpace;
				}
			}
			else if (y == coords.Y + height - heightReduce - 1)  
			{
				if (x == coords.X)                                                
				{
					newType = CellTypes.RoomWallL;
				}
				else if (x == coords.X + width - widthReduce - 1)                
				{
					newType = CellTypes.RoomWallULinv;
				}
				else if (x == coords.X + width - 1)                              
				{
					newType = CellTypes.RoomWallDR;
				}
				else if (x > coords.X + width - widthReduce - 1 && x < coords.X + width - 1)   
				{
					newType = CellTypes.RoomWallD;
				}
				else
				{
					newType = CellTypes.RoomSpace;
				}
			}
			else if (y > coords.Y && y < coords.Y + height - heightReduce - 1) 
			{
				if (x == coords.X)                                                
				{
					newType = CellTypes.RoomWallL;
				}
				else if (x == coords.X + width - widthReduce - 1)               
				{
					newType = CellTypes.RoomWallR; 
				}
				else if (x > coords.X && x < coords.X + width - widthReduce - 1)  
				{
					newType = CellTypes.RoomSpace;
				}
				//else Empty.
			}
			else
			{
				if (x == coords.X)                                                 
				{
					newType = CellTypes.RoomWallDL;
				}
				else if (x == coords.X + width - widthReduce - 1)           
				{
					newType = CellTypes.RoomWallDR;
				}
				else if (x > coords.X && x < coords.X + width - widthReduce - 1)  
				{
					newType = CellTypes.RoomWallD;
				}
				//else Empty.
			}
			
			return newType;
		}

		// Kerplunk, the rooms are all superimposed. Each room needs to be outlined properly with no gaps.
		private void MergeRooms()
		{
			for (int y = 0; y < Reference.GridHeight; y++)
			{
				for (int x = 0; x < Reference.GridWidth; x++)
				{
					Cell cell = _grid[x, y];
					
					if (cell.Type == CellTypes.RoomWallDL && !cell.Merged)
					{
						try 
						{
							Room room = TraverseRoomOutline(x, y);
							
							if (room != null)
							{
								CleanRoom(x, y, room);
							}
						}
						catch (Exception) 
						{
							// Anything that's this bad that happens here, start over...
							throw new DungeonGenerateException();
						}
					}
				}
			}
		}

		private Room TraverseRoomOutline(int startX, int startY)
		{
			Direction dir = Direction.Up;
			int newX = 0, newY = 0;
			int x = startX, y = startY;
			Room room = new Room(startX, startY);
			Cell currentCell, cellUp, cellDown, cellLeft, cellRight;
			
			do
			{        
				// Since the room "plunking" process, and the outline traverse process aren't perfect 
				// systems, a protection is required for the infrequent, but typical, times when the 
				// traverse attempts to exceed the size of the dungeon.
				if (y < 0 || y >= Reference.GridHeight || x < 0 || x >= Reference.GridWidth)
				{
					throw new DungeonGenerateException();
				}
				
				currentCell = _grid[x, y];
				CellType newType = currentCell.Type;  // Default.
				
				// Arriving back at an already merged cell in the same room means that the rooms cannot be 
				// properly merged. This is usually due to a "missing" wall cells occurring due to the way 
				// the "sub-rooms" were "plunked" down on the dungeon. Irrecoverable. Abort.
				if (currentCell.Merged && room.Walls.Contains(currentCell))
				{
					throw new DungeonGenerateException();
				}
				
				cellUp = y + 1 >= 0 && y + 1 < Reference.GridHeight && x >= 0 && x < Reference.GridWidth 
						? _grid[x, y + 1] : null;
				
				cellDown  = y - 1 >= 0 && y - 1 < Reference.GridHeight && x >= 0 && x < Reference.GridWidth             
						? _grid[x, y - 1] : null;
				
				cellLeft  = x - 1 >= 0 && x - 1 < Reference.GridWidth && y >= 0 && y < Reference.GridWidth 
						? _grid[x - 1, y] : null;
				
				cellRight = x + 1 >= 0 && x + 1 < Reference.GridWidth && y >= 0 && y < Reference.GridWidth 
						? _grid[x + 1, y] : null;
				
				if (x == startX && y == startY)
				{
					newType = RandomRoomCorner(x, y, Direction.DownLeft);
					newX = x;
					newY = y + 1;
					dir = Direction.Up;
				}
				else if (dir == Direction.Up && cellLeft != null && cellLeft.Type.RoomConnectsRight)
				{
					newType = CellTypes.RoomWallURinv;
					newX = x - 1;
					newY = y;
					dir = Direction.Left;
				}
				else if (dir == Direction.Up && cellUp != null && cellUp.Type.RoomConnectsDown)
				{
					newType = RandomRoomWall(x, y, Direction.Left);
					newX = x;
					newY = y + 1;
					dir = Direction.Up;
				}
				else if (dir == Direction.Up && cellRight != null && cellRight.Type.RoomConnectsLeft)
				{
					newType = RandomRoomCorner(x, y, Direction.UpLeft);
					newX = x + 1;
					newY = y;
					dir = Direction.Right;
				}
				else if (dir == Direction.Up && (currentCell.Type == CellTypes.RoomWallUL || currentCell.Type == CellTypes.RoomWallULinv))
				{
					newType = RandomRoomCorner(x, y, Direction.UpLeft);
					newX = x + 1;
					newY = y;
					dir = Direction.Right;
				}
				else if (dir == Direction.Up)
				{
					newType = RandomRoomWall(x, y, Direction.Left);
					newX = x;
					newY = y + 1;
					dir = Direction.Up;
				}
				else if (dir == Direction.Left && cellDown != null && cellDown.Type.RoomConnectsUp)
				{
					newType = CellTypes.RoomWallULinv;
					newX = x;
					newY = y - 1;
					dir = Direction.Down;
				}
				else if (dir == Direction.Left && cellLeft != null && cellLeft.Type.RoomConnectsRight)
				{
					newType = RandomRoomWall(x, y, Direction.Down);
					newX = x - 1;
					newY = y;
					dir = Direction.Left;
				}
				else if (dir == Direction.Left && cellUp != null && cellUp.Type.RoomConnectsDown)
				{
					newType = RandomRoomCorner(x, y, Direction.DownLeft);
					newX = x;
					newY = y + 1;
					dir = Direction.Up;
				}
				else if (dir == Direction.Left && (currentCell.Type == CellTypes.RoomWallDL || currentCell.Type == CellTypes.RoomWallDLinv))
				{
					newType = RandomRoomCorner(x, y, Direction.DownLeft);
					newX = x;
					newY = y + 1;
					dir = Direction.Up;
				}
				else if (dir == Direction.Left)
				{
					newType = RandomRoomWall(x, y, Direction.Down);
					newX = x - 1;
					newY = y;
					dir = Direction.Left;
				}
				else if (dir == Direction.Right && cellUp != null && cellUp.Type.RoomConnectsDown)
				{
					newType = CellTypes.RoomWallDRinv;
					newX = x;
					newY = y + 1;
					dir = Direction.Up;
				}
				else if (dir == Direction.Right && cellRight != null && cellRight.Type.RoomConnectsLeft)
				{
					newType = RandomRoomWall(x, y, Direction.Up);
					newX = x + 1;
					newY = y;
					dir = Direction.Right;
				}
				else if (dir == Direction.Right && cellDown != null && cellDown.Type.RoomConnectsUp)
				{
					newType = RandomRoomCorner(x, y, Direction.UpRight);
					newX = x;
					newY = y - 1;
					dir = Direction.Down;
				}
				else if (dir == Direction.Right && (currentCell.Type == CellTypes.RoomWallUR || currentCell.Type == CellTypes.RoomWallURinv))
				{
					newType = RandomRoomCorner(x, y, Direction.UpRight);
					newX = x;
					newY = y - 1;
					dir = Direction.Down;
				}
				else if (dir == Direction.Right)
				{
					newType = RandomRoomWall(x, y, Direction.Up);
					newX = x + 1;
					newY = y;
					dir = Direction.Right;
				}
				else if (dir == Direction.Down && cellRight != null && cellRight.Type.RoomConnectsLeft)
				{
					newType = CellTypes.RoomWallDLinv;
					newX = x + 1;
					newY = y;
					dir = Direction.Right;
				}
				else if (dir == Direction.Down && cellDown != null && cellDown.Type.RoomConnectsUp)
				{
					newType = RandomRoomWall(x, y, Direction.Right);
					newX = x;
					newY = y - 1;
					dir = Direction.Down;
				}
				else if (dir == Direction.Down && cellLeft != null && cellLeft.Type.RoomConnectsRight)
				{
					newType = RandomRoomCorner(x, y, Direction.DownRight);
					newX = x - 1;
					newY = y;
					dir = Direction.Left;
				}
				else if (dir == Direction.Down && (currentCell.Type == CellTypes.RoomWallDR || currentCell.Type == CellTypes.RoomWallDRinv))
				{
					newType = RandomRoomCorner(x, y, Direction.DownRight);
					newX = x - 1;
					newY = y;
					dir = Direction.Left;
				}
				else if (dir == Direction.Down)
				{
					newType = RandomRoomWall(x, y, Direction.Right);
					newX = x;
					newY = y - 1;
					dir = Direction.Down;
				}
				
				Cell newCell = new Cell(x, y, newType);
				newCell.Merged = true;
				SetCellValue(x, y, newCell);
				room.Walls.Add(newCell);
				
				x = newX;
				y = newY;
				
			} while (!(x == startX && y == startY));
			
			_rooms.Add(room);
			return room;
		}

		// After traversing the room outline to merge a set of rooms, immediately "clean" the room to remove
		// any old remnants of room walls. It is especially important to clean any roomWallDL sections, or
		// mergeRooms will think that it is the start point of a new room, which it actually isn't...
		private void CleanRoom(int startX, int startY, Room room)
		{
			Direction dir = Direction.Up;
			int newX = 0, newY = 0;
			int x = startX, y = startY + 1;
			Cell currentCell, cellUp, cellDown, cellLeft, cellRight;
			
			while (!(x == startX && y == startY))
			{
				int up = 1;
				currentCell = y < Reference.GridHeight ? _grid[x, y] : null;
				cellUp = y + up < Reference.GridHeight ? _grid[x, y + up] : null;
				
				if (currentCell.Type.IsCleanStartWall)
				{
					while (!cellUp.Merged)
					{
						CellType newType = CellTypes.RoomSpace;
						Cell newCell = new Cell(x, y + up, newType);
						newCell.Merged = true;
						SetCellValue(x, y + up, newCell);
						room.Space.Add(newCell);
						up += 1;
						currentCell = _grid[x, y + up];
						cellUp = y + up < Reference.GridHeight ? _grid[x, y + up] : null;
					}
				}
				
				cellUp    = y + 1 < Reference.GridHeight ? _grid[x, y + 1] : null;
				cellDown  = y - 1 >= 0                   ? _grid[x, y - 1] : null;
				cellLeft  = x - 1 >= 0                   ? _grid[x - 1, y] : null;
				cellRight = x + 1 < Reference.GridWidth  ? _grid[x + 1, y] : null;
				
				if (dir != Direction.Right && cellLeft != null && cellLeft.Type.RoomConnectsRight && cellLeft.Merged)
				{
					newX = x - 1;
					newY = y;
					dir = Direction.Left;
				}
				else if (dir != Direction.Down && cellUp != null && cellUp.Type.RoomConnectsDown && cellUp.Merged)
				{
					newX = x;
					newY = y + 1;
					dir = Direction.Up;
				}
				else if (dir != Direction.Left && cellRight != null && cellRight.Type.RoomConnectsLeft && cellRight.Merged)
				{
					newX = x + 1;
					newY = y;
					dir = Direction.Right;
				}
				else if (dir != Direction.Up && cellDown != null && cellDown.Type.RoomConnectsUp && cellDown.Merged)
				{
					newX = x;
					newY = y - 1;
					dir = Direction.Down;
				}
				
				if (x == newX && y == newY)  // Abandon attempt if the process is going nowhere.
				{
					throw new DungeonGenerateException();
				}
				else
				{
					x = newX;
					y = newY;
				}
			}
		}

		private void CleanRoomScraps()
		{
			for (int y = 0; y < Reference.GridHeight; y++) 
			{
				for (int x = 0; x < Reference.GridWidth; x++) 
				{
					Cell cell = _grid[x, y];
					
					if (cell.Type.IsRoomType && !cell.Merged)
					{
						ReplaceCellValue(x, y, _emptyCell);
					}
				}
			}
		}

		// Remove any bits and pieces left over from the room placement and merge process.
		private void CleanRoomsArray()
		{
			List<Cell> junk = new List<Cell>();
			
			foreach (Room room in _rooms) 
			{
				foreach (Cell wall in room.Walls) 
				{
					if (!GridContains(wall))
					{
						junk.Add(wall);
					}
				}
				
				foreach (Cell space in room.Space) 
				{
					if (!GridContains(space))
					{
						junk.Add(space);
					}
				}
			}
			
			foreach (Room room in _rooms) 
			{
				foreach (Cell junkCell in junk) 
				{
					room.Walls.Remove(junkCell);
					room.Space.Remove(junkCell);
				}
			}
		}

		// Need a random piece of wall? Then you've come to the right place!
		private CellType RandomRoomWall(int x, int y, Direction dir)
		{
			CellType type = null;
			bool exit = _r.Next(100) + 1 <= Reference.RoomExitProb;
			
			// Room exits can only occur if the room wall in question is at least 1 cell
			// from the dungeon edge, to allow the resulting corridors to grow or be "capped".
			
			if (dir == Direction.Up)                             
			{
				if (exit && y + 1 < Reference.GridHeight)
					type = CellTypes.RoomExitU; 
				else
					type = CellTypes.RoomWallU;
			}
			else if (dir == Direction.Down)                                            
			{
				if (exit && y - 1 >= 0)
					type = CellTypes.RoomExitD;
				else
					type = CellTypes.RoomWallD;
			}
			else if (dir == Direction.Left)                                              
			{
				if (exit && x - 1 >= 0) 
					type = CellTypes.RoomExitL; 
				else
					type = CellTypes.RoomWallL;
			}                                
			else if (dir == Direction.Right)                                  
			{
				if (exit && x + 1 < Reference.GridHeight) 
					type = CellTypes.RoomExitR;
				else
					type = CellTypes.RoomWallR;
			}
			
			return type;
		}

		private CellType RandomRoomCorner(int x, int y, Direction dir)
		{
			CellType type = null;
			bool exit = _r.Next(100) + 1 <= Reference.RoomExitProb;
			List<CellType> possibleExitTypes = new List<CellType>();
			
			// Room exits can only occur if the room wall in question is at least 1 cell
			// from the dungeon edge, to allow the resulting corridors to grow or be "capped".
			
			if (dir == Direction.UpLeft)                             
			{
				if (!exit) 
				{
					return CellTypes.RoomWallUL;
				}
				
				if (y + 1 < Reference.GridHeight)
				{
					possibleExitTypes.Add(CellTypes.RoomExitUL_U);
				}
				
				if (x - 1 >= 0)
				{
					possibleExitTypes.Add(CellTypes.RoomExitUL_L);
				}
				
				if (possibleExitTypes.Count > 0)
				{
					type = possibleExitTypes[_r.Next(possibleExitTypes.Count)];
				}
				else
				{
					type = CellTypes.RoomWallUL;  // No exits fit.
				}
			}
			else if (dir == Direction.UpRight)                                            
			{
				if (!exit) 
				{
					return CellTypes.RoomWallUR;
				}
				
				if (y + 1 < Reference.GridHeight)
				{
					possibleExitTypes.Add(CellTypes.RoomExitUR_U);
				}
				
				if (x + 1 < Reference.GridHeight)
				{
					possibleExitTypes.Add(CellTypes.RoomExitUR_R);
				}
				
				if (possibleExitTypes.Count > 0)
				{
					type = possibleExitTypes[_r.Next(possibleExitTypes.Count)];
				}
				else
				{
					type = CellTypes.RoomWallUR;  // No exits fit.
				}
			}
			else if (dir == Direction.DownLeft)                                              
			{
				if (!exit) 
				{
					return CellTypes.RoomWallDL;
				}
				
				if (y - 1 >= 0)
				{
					possibleExitTypes.Add(CellTypes.RoomExitDL_D);
				}
				
				if (x - 1 >= 0)
				{
					possibleExitTypes.Add(CellTypes.RoomExitDL_L);
				}
				
				if (possibleExitTypes.Count > 0)
				{
					type = possibleExitTypes[_r.Next(possibleExitTypes.Count)];
				}
				else
				{
					type = CellTypes.RoomWallDL;  // No exits fit.
				}
			}                                
			else if (dir == Direction.DownRight)                                  
			{
				if (!exit) 
				{
					return CellTypes.RoomWallDR;
				}
				
				if (y - 1 >= 0)
				{
					possibleExitTypes.Add(CellTypes.RoomExitDR_D);
				}
				
				if (x + 1 < Reference.GridHeight)
				{
					possibleExitTypes.Add(CellTypes.RoomExitDR_R);
				}
				
				if (possibleExitTypes.Count > 0)
				{
					type = possibleExitTypes[_r.Next(possibleExitTypes.Count)];
				}
				else
				{
					type = CellTypes.RoomWallDR;  // No exits fit.
				}
			}
			
			return type;
		}

		private bool IncompatibleCornerTypes(CellType currentType, CellType newType)
		{
			return 
			(currentType == CellTypes.RoomWallUL && newType == CellTypes.RoomWallDR) ||
			(currentType == CellTypes.RoomWallUR && newType == CellTypes.RoomWallDL) ||
			(currentType == CellTypes.RoomWallDL && newType == CellTypes.RoomWallUR) ||
			(currentType == CellTypes.RoomWallDR && newType == CellTypes.RoomWallUL);
		}

		// Any existing room exit should be connected to any adjacent room section belonging to another room.
		// We don't need to worry about existing exits that, just by chance, happened to be adjacent to one of
		// the exits from another room, because SetCellValue() already takes care of that! 👍
		private void ConnectRooms()
		{
			foreach (Room room in _rooms) 
			{
				Direction dir = Direction.Up;
				int x = room.X, y = room.Y;
				int newX = x, newY = y;
				Cell cellUp, cellDown, cellLeft, cellRight;
				
				AssertExit(room);
				
				do
				{
					ConnectCells(x, y);
					
					cellUp    = y + 1 < Reference.GridHeight ? _grid[x, y + 1] : null;
					cellDown  = y - 1 >= 0                   ? _grid[x, y - 1] : null;
					cellLeft  = x - 1 >= 0                   ? _grid[x - 1, y] : null;
					cellRight = x + 1 < Reference.GridWidth  ? _grid[x + 1, y] : null;
					
					if (dir != Direction.Right && cellLeft != null && cellLeft.Type.RoomConnectsRight)
					{
						newX = x - 1;
						newY = y;
						dir = Direction.Left;
					}
					else if (dir != Direction.Down && cellUp != null && cellUp.Type.RoomConnectsDown)
					{
						newX = x;
						newY = y + 1;
						dir = Direction.Up;
					}
					else if (dir != Direction.Left && cellRight != null && cellRight.Type.RoomConnectsLeft)
					{
						newX = x + 1;
						newY = y;
						dir = Direction.Right;
					}
					else if (dir != Direction.Up && cellDown != null && cellDown.Type.RoomConnectsUp)
					{
						newX = x;
						newY = y - 1;
						dir = Direction.Down;
					}
					
					if (x == newX && y == newY)  // Going nowhere? This room is bitched.
					{
						throw new DungeonGenerateException();
					}
					else
					{
						x = newX;
						y = newY;
					}
				}
				while (!(x == room.X && y == room.Y));
			}
		}

		private void ConnectCells(int x, int y)
		{
			Cell cell = _grid[x, y];
			
			if (!cell.Type.IsRoomExit)
			{
				return;
			}
			
			// The room wall section is an unconnected exit if the cell has an available connection.
			if (cell.Type.ConnectsLeft && x - 1 >= 0 && cell.AvailableConnections > 0)                                 
			{
				ConnectRoomCells(cell, x - 1, y, Direction.Left);
			}
			
			if (cell.Type.ConnectsRight && x + 1 < Reference.GridWidth && cell.AvailableConnections > 0)
			{
				ConnectRoomCells(cell, x + 1, y, Direction.Right);
			}
			
			if (cell.Type.ConnectsDown && y - 1 >= 0 && cell.AvailableConnections > 0)
			{
				ConnectRoomCells(cell, x, y - 1, Direction.Down);
			}
			
			if (cell.Type.ConnectsUp && y + 1 < Reference.GridHeight && cell.AvailableConnections > 0)
			{
				ConnectRoomCells(cell, x, y + 1, Direction.Up);
			}
		}

		private bool AssertExit(Room room)
		{
			List<Cell> walls = new List<Cell>();
			
			// Populate a disposable array of wall cells from the room. If any of 
			// those walls are exits, the exit is obviously asserted already.
			
			bool exitPossible = false;
			
			foreach (Cell cell in room.Walls)
			{
				if (cell.Type.IsRoomExit)
				{
					return true;
				}
				else if (GridContains(cell))
				// Some cells are still in the room.walls array even though they are no longer in the dungeon
				// because they were "plunked upon" by other room cells. Do not consider those for exits.
				{
					if (cell.Type.RoomExitCompatible)
					{
						if (!cell.ExitImpossible)
						{
							exitPossible = true;
							walls.Add(cell);
						}
					}
				}
			}
			
			// So, there's no exit to the room, but an exit hasn't been ruled out. Try to force one.
			bool exitForced = false;

			while (!exitForced) 
			{
				// If none of the walls is an exit, and all have been previously recorded as it being 
				// impossible to place an exit there, the room cannot be reached. Start over.
				if (!exitPossible || walls.Count == 0) 
				{
					throw new DungeonGenerateException();
				}
				
				Cell cell = walls[_r.Next(walls.Count)];
				
				Direction directionOK = Direction.NoDir;
				
				if (cell.Type.IsRoomCorner)
				{
					directionOK = RoomCellsAdjacentOK(cell);
				}
				else
				{
					directionOK = RoomCellAdjacentOK(cell);
				}
				
				if (directionOK != Direction.NoDir) 
				{
					directionOK = FilterDirection(directionOK);
					CellType newType = CellTypes.ConvertRoomWallToExit(cell.Type, directionOK);
					
					Cell newCell = new Cell(cell.X, cell.Y, newType);
					
					SetCellValue(cell.X, cell.Y, newCell);
					room.Walls.Add(newCell);
					
					exitForced = true;
				}
				else
				{
					cell.ExitImpossible = true;
				}
				
				walls.Remove(cell);
			}

			return true;
		}


		// When forcing a room exit and the room wall section in question is a corner, and both directions
		// are available, there is a random chance of either or both the directions being receiving an exit.
		private Direction FilterDirection(Direction dir)
		{
			Direction filterDir = dir;
			int r = _r.Next(3);
			
			if (dir == Direction.Up || dir == Direction.Down || dir == Direction.Left || dir == Direction.Right)
			{
				filterDir = dir;  // No change.
			}
			else if (dir == Direction.UpLeft)
			{
				if (r == 1)
				{
					filterDir = Direction.Up;
				}
				else if (r == 2)
				{
					filterDir = Direction.Left;
				}
				else  // rando == 3
				{
					filterDir = Direction.UpLeft;
				}
			}
			else if (dir == Direction.UpRight)
			{
				if (r == 1)
				{
					filterDir = Direction.Up;
				}
				else if (r == 2)
				{
					filterDir = Direction.Right;
				}
				else  // r == 3
				{
					filterDir = Direction.UpRight;
				}
			}
			else if (dir == Direction.DownLeft)
			{
				if (r == 1)
				{
					filterDir = Direction.Down;
				}
				else if (r == 2)
				{
					filterDir = Direction.Left;
				}
				else  // r == 3
				{
					filterDir = Direction.DownLeft;
				}
			}
			else if (dir == Direction.DownRight)
			{
				if (r == 1)
				{
					filterDir = Direction.Down;
				}
				else if (r == 2)
				{
					filterDir = Direction.Right;
				}
				else  // r == 3
				{
					filterDir = Direction.DownRight;
				}
			}
			
			return filterDir;
		}

		private Direction RoomCellAdjacentOK(Cell cell)
		{
			Cell adjacentCell = null;
			Direction dir = cell.Type.RoomWallDirection;
				
			if (dir == Direction.Up && cell.Y + 1 < Reference.GridHeight)
			{
				adjacentCell = _grid[cell.X, cell.Y + 1];
			}
			else if (dir == Direction.Down && cell.Y - 1 >= 0)
			{
				adjacentCell = _grid[cell.X, cell.Y - 1];
			}
			else if (dir == Direction.Left && cell.X - 1 >= 0)
			{
				adjacentCell = _grid[cell.X - 1, cell.Y];
			}
			else if (dir == Direction.Right && cell.X + 1 < Reference.GridWidth)
			{
				adjacentCell = _grid[cell.X + 1, cell.Y];
			}
				
			if (adjacentCell != null && (adjacentCell.Type.IsEmpty || adjacentCell.Type.RoomExitCompatible))
			{
				return dir;
			}
			else
			{
				return Direction.NoDir;
			}
		}

		// RP: 2017-03-22. Fixed what appeared to be a copy-paste bug in this method where the _grid[) calls 
		// didn't match coords with the conditional checks immediately above them.
		private Direction RoomCellsAdjacentOK(Cell cell)
		{
			bool okUp = false, okDown = false, okLeft = false, okRight = false;
			Cell adjCellUp, adjCellDown, adjCellLeft, adjCellRight;
			
			Direction dir = cell.Type.RoomWallDirection;
				
			if (dir == Direction.UpLeft || dir == Direction.UpRight)
			{
				if (cell.Y + 1 < Reference.GridHeight)
				{
					adjCellUp = _grid[cell.X, cell.Y + 1];

					if (adjCellUp.Type.IsEmpty || adjCellUp.Type.RoomExitCompatible)
					{
						okUp = true;
					}
				}
			}
			
			if (dir == Direction.DownLeft || dir == Direction.DownRight)
			{
				if (cell.Y - 1 >= 0)
				{
					adjCellDown = _grid[cell.X, cell.Y - 1];

					if (adjCellDown.Type.IsEmpty || adjCellDown.Type.RoomExitCompatible)
					{
						okDown = true;
					}
				}
			}
			
			if (dir == Direction.UpLeft || dir == Direction.DownLeft)
			{
				if (cell.X - 1 >= 0)
				{
					adjCellLeft = _grid[cell.X - 1, cell.Y];
					if (adjCellLeft.Type.IsEmpty || adjCellLeft.Type.RoomExitCompatible)
					{
						okLeft = true;
					}
				}
			}
			
			if (dir == Direction.UpRight || dir == Direction.DownRight)
			{
				if (cell.X + 1 < Reference.GridWidth)
				{
					adjCellRight = _grid[cell.X + 1, cell.Y];
					if (adjCellRight.Type.IsEmpty || adjCellRight.Type.RoomExitCompatible)
					{
						okRight = true;
					}
				}
			}
			
			if (okUp && okLeft)
			{
				dir = Direction.UpLeft;
			}
			else if (okUp && okRight)
			{
				dir = Direction.UpRight;
			}
			else if (okDown && okLeft)
			{
				dir = Direction.DownLeft;
			}
			else if (okDown && okRight)
			{
				dir = Direction.DownRight;
			}
			else if (okUp)
			{
				dir = Direction.Up;
			}
			else if (okDown)
			{
				dir = Direction.Down;
			}
			else if (okLeft)
			{
				dir = Direction.Left;
			}
			else if (okRight)
			{
				dir = Direction.Right;
			}
			else
			{
				dir = Direction.NoDir;
			}
			
			return dir;
		}

		// Given a cell representing a section of room wall, it must connect to any adjacent section of 
		// room wall belonging to another room. The adjacent section is not an exit section, or we would
		// not even be here, so force the other section to have an exit and connect the two sections.
		// Now, if the adjacent section happens to be a room corner or something that the existing exit
		// cannot connect to, the exit must be converted into an ordinary room section to avoid clashing.
		private void ConnectRoomCells(Cell cell, int adjX, int adjY, Direction dir)
		{
			Cell newCell;
			CellType newType;
			Cell adjacent = _grid[adjX, adjY];
			
			if (adjacent.Type.IsEmpty || cell.Type.ConnectsTo(adjacent.Type, dir))
			{
				return;
			}
			else if (adjacent.Type.RoomExitCompatible)
			{
				newType = CellTypes.ConvertRoomWallToExit(adjacent.Type, OppositeDir(dir));
				newCell = new Cell(adjX, adjY, newType);
				SetCellValue(adjX, adjY, newCell);
				AddNewCellToRoom(adjacent, newCell);
			}
			else
			{
				newType = CellTypes.ConvertRoomExitToWall(cell.Type, dir);
				newCell = new Cell(cell.X, cell.Y, newType);
				SetCellValue(cell.X, cell.Y, newCell);
				AddNewCellToRoom(cell, newCell);
			}
		}

		// Need to convert direction from the context of connectRoomCells when altering the OTHER room's cell.
		private Direction OppositeDir(Direction dir)
		{
			Direction opposite;
			
			if (dir == Direction.Up)
			{
				opposite = Direction.Down;
			}
			else if (dir == Direction.Down)
			{
				opposite = Direction.Up;
			}
			else if (dir == Direction.Left)
			{
				opposite = Direction.Right;
			}
			else //if (dir == Direction.Right)
			{
				opposite = Direction.Left;
			}
		
			return opposite;
		}

		// Add a new cell to a room without knowing exactly which room it is to begin with...
		private void AddNewCellToRoom(Cell existingCell, Cell newCell)
		{
			foreach (Room room in _rooms) 
			{
				if (room.Walls.Contains(existingCell))
				{
					room.Walls.Add(newCell);
					return;
				}
			}
		}

		// Make a deep copy clone of the Rooms list.
		private List<Room> CloneRoomsList()
		{
			List<Room> rooms = null;
			
			if (_rooms != null)
			{
				rooms = new List<Room>();

				foreach (Room room in _rooms)
				{
					rooms.Add(new Room(room));
				}
			}

			return rooms;
		}

		private void DeleteRoom(Room room)
		{
			foreach (Cell wall in room.Walls) 
			{
				_grid[wall.X, wall.Y] = _emptyCell;
			}
		}

		#endregion
		#region Doors, Keys, Gems, and Hearts
			
		private void  PlaceDoors()
		{
			Cell cell;
			
			for (int x = 0; x < Reference.GridWidth; x++)
			{
				for (int y = 0; y < Reference.GridHeight; y++) 
				{
					cell = _grid[x, y];
					cell.Door = SimpleDoorPlacement(cell);

					// HERE
					// if (!SuppressDoor(cell)) 
					// {
					// 	cell.Door = RandomDoorSetup(cell);
					// }
				}
			}
		}

		// Quite simply put: all rooms exits have doors. This is as opposed to more complex randomized door placement
		// rules. See commented-out code block marked "HERE" in above subroutine. BUG FIX: don't allow two adjacent cells
		// or occasionally the dungeon will have "doubled up" doors that mess up the flow of control.
		private Door SimpleDoorPlacement(Cell cell)
		{    
			Door door = null;
			
			if (cell.Type == CellTypes.RoomExitU || cell.Type == CellTypes.RoomExitUL_U || cell.Type == CellTypes.RoomExitUR_U)
			{
				Cell adjacentCell = _grid[cell.X, cell.Y + 1];
				if (!_cellsWithDoors.Contains(adjacentCell))
				{
					_cellsWithDoors.Add(cell);
					door = new Door(Direction.Up);
				}
			}
			else if (cell.Type == CellTypes.RoomExitD || cell.Type == CellTypes.RoomExitDL_D || cell.Type == CellTypes.RoomExitDR_D)
			{
				Cell adjacentCell = _grid[cell.X, cell.Y - 1];
				if (!_cellsWithDoors.Contains(adjacentCell))
				{
					_cellsWithDoors.Add(cell);
					door = new Door(Direction.Down);
				}
			}
			else if (cell.Type == CellTypes.RoomExitL || cell.Type == CellTypes.RoomExitUL_L || cell.Type == CellTypes.RoomExitDL_L)
			{
				Cell adjacentCell = _grid[cell.X - 1, cell.Y];
				if (!_cellsWithDoors.Contains(adjacentCell))
				{
					_cellsWithDoors.Add(cell);
					door = new Door(Direction.Left);
				}
			}
			else if (cell.Type == CellTypes.RoomExitR || cell.Type == CellTypes.RoomExitUR_R || cell.Type == CellTypes.RoomExitDR_R)
			{
				Cell adjacentCell = _grid[cell.X + 1, cell.Y];
				if (!_cellsWithDoors.Contains(adjacentCell))
				{
					_cellsWithDoors.Add(cell);
					door = new Door(Direction.Right);
				}
			}

			return door;
		}

		// Don't allow two doors to appear side-by-side if the door is to be placed in a corridor.
		// Also, don't allow a door to appear in the viscinity of the start cell, because it could
		// end up being a locked door with no key before it in the dungeon...
		private bool SuppressDoor(Cell cell)
		{
			// Maintain a 2-cell margin around the start cell.
			if (cell.Y > _startCoords.Y - 2 && cell.Y < _startCoords.Y + 2 && 
				cell.X > _startCoords.X - 2 && cell.X < _startCoords.X + 2) 
			{
				return true;
			}

			// Cell above.
			if (cell.Y + 1 < Reference.GridHeight)
			{
				Cell cellAbove = _grid[cell.X, cell.Y + 1];
				
				if (cellAbove.Door != null && cellAbove.Door != null)
				{
					return true;
				}
			}
			
			// Cell below.
			if (cell.Y - 1 >= 0)
			{
				Cell cellBelow = _grid[cell.X, cell.Y - 1];
				
				if (cellBelow.Door != null && cellBelow.Door != null)
				{
					return true;
				}
			}
			
			// Cell left.
			if (cell.X - 1 >= 0)
			{
				Cell cellLeft = _grid[cell.X - 1, cell.Y];
				
				if (cellLeft.Door != null && cellLeft.Door != null)
				{
					return true;
				}
			}
			
			// Cell right.
			if (cell.X + 1 < Reference.GridWidth)
			{
				Cell cellRight = _grid[cell.X + 1, cell.Y];
				
				if (cellRight.Door != null && cellRight.Door != null)
				{
					return true;
				}
			}
			
			return false;  // Made it this far...
		}

		private Door RandomDoorSetup(Cell cell)
		{    
			Door door = null;
			
			if (cell.Type.ConnectsUp)
			{
				door = RandomDoor(cell, Direction.Up);
			}
			
			if (cell.Type.ConnectsDown)
			{
				door = RandomDoor(cell, Direction.Down);
			}
			
			if (cell.Type.ConnectsLeft)
			{
				door = RandomDoor(cell, Direction.Left);
			}
			
			if (cell.Type.ConnectsRight)
			{
				door = RandomDoor(cell, Direction.Right);
			}
			
			return door;
		}

		private Door RandomDoor(Cell cell, Direction dir)
		{    
			Door door = null;
			
			if (RandomPercent() < cell.Type.DoorProbability)
			{
				_cellsWithDoors.Add(cell);
				door = new Door(dir);
			}
			
			return door;
		}

		// Cell available for placement of item (key, heart, gem, etc).
		private bool CellAvailableForItem(Cell cell)
		{
			return

			cell.Door == null &&
			!cell.HasKey &&
			!cell.HasGem &&
			!cell.HasHeart &&
			!cell.HasSword && 
			!cell.HasGoblin &&
			!cell.Type.IsStairsUp && 
			!cell.Type.IsStairsDown && 
			cell.Type != CellTypes.Entrance;
		}

		private void PlaceKeys()
		{
			Cell keyCell;
			List<Cell> potentials;

			_cellsWithDoors = _cellsWithDoors.OrderBy(cell => cell.Sequence).ToList();
			
			foreach (Cell cell in _cellsWithDoors) 
			{
				potentials = KeyLocationPotentials(cell.Sequence);
				keyCell = potentials[_r.Next(potentials.Count)];  // Pick a key cell at random.
				keyCell.HasKey = true;
			}
		}

		private List<Cell> KeyLocationPotentials(int endSequence)
		{
			Cell cell;
			List<Cell> potentials = new List<Cell>();
			
			for (int x = 0; x < Reference.GridWidth; x++)
			{
				for (int y = 0; y < Reference.GridHeight; y++) 
				{
					cell = _grid[x, y];
					
					if (cell.Sequence >= 0 && cell.Sequence < endSequence && CellAvailableForItem(cell))
					{
						potentials.Add(cell);
					}
				}
			}
			
			return potentials;
		}

		private void PlaceGems()
		{
			Cell cell;
			
			for (int x = 0; x < Reference.GridWidth; x++)
			{
				for (int y = 0; y < Reference.GridHeight; y++) 
				{
					cell = _grid[x, y];
					
					if (CellAvailableForItem(cell) && RandomPercent() < Reference.GemProb)
					{
						cell.HasGem = true;
					}
				}
			}
		}

		private void PlaceHearts()
		{
			Cell cell;
			
			for (int x = 0; x < Reference.GridWidth; x++)
			{
				for (int y = 0; y < Reference.GridHeight; y++) 
				{
					cell = _grid[x, y];
					
					if (CellAvailableForItem(cell) && RandomPercent() < Reference.HeartProb)
					{
						cell.HasHeart = true;
					}
				}
			}
		}

		private void PlaceSwords()
		{		
			int swordCount = _level + 2;

			while (swordCount > 0)
			{
				Coords coords = RandomCell(false);
				Cell cell = _grid[coords.X, coords.Y];

				if (CellAvailableForItem(cell))
				{
					cell.HasSword = true;
					swordCount--;
				}
			}
		}

		private void PlaceGoblins()
		{		
			int goblinCount = _level + 2;

			while (goblinCount > 0)
			{
				Coords coords = RandomCell(false);
				Cell cell = _grid[coords.X, coords.Y];

				if(CellAvailableForItem(cell))
				{
					cell.HasGoblin = true;
					goblinCount--;
				}
			}
		}

		#endregion
		#region Accessors

		private void SetCellValue(int x, int y, Cell cell)
		{
			_grid[x, y] = cell;
			RecordNewAttachment(cell);
		}

		private bool GridContains(object value)
		{
			bool contains = false;
			
			for (int y = 0; y < Reference.GridHeight; y++)
			{
				for (int x = 0; x < Reference.GridWidth; x++)
				{
					if (_grid[x, y] == value)
					{
						contains = true;
						break;
					}
				}
			}

			return contains;
		}

		// This is intended to be used only for special rooms durring the rooms connect phase to 
		// avoid screwing up the available connections count of when converting room exits to walls.
		private void ReplaceCellValue(int x, int y, Cell cell)
		{
			_grid[x, y] = cell;
		}

		// Returns a random cell from the dungeon. If empty == true, then the random
		// cell will be empty. If empty == false, then the random cell will be occupied.
		private Coords RandomCell(bool empty)
		{
			Coords coords = null;
			
			while (coords == null) 
			{
				int x = _r.Next(Reference.GridWidth);
				int y = _r.Next(Reference.GridHeight);
				
				Cell cell = _grid[x, y];
				
				if (empty)  // Cell must be unoccupied.
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
		// dungeon grow any bigger. But, don't waste any cycles doing it, it could be a lost cause...
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
				List<CellType> types = CellTypes.GetTypes(coords);
				types.Remove(cell.Type);  // ...but replace it with something different.
				
				while (!typeMatch) 
				{
					if (types.Count == 0) 
					{
						break;  // If nothing replaces it, start over.
					}
					
					newType = RandomCellType(types);  // Candidate new cell type.
					
					// The new cell needs to be compatible with each adjacent cell.
					if (TypeCompatibleWithAdjacentCells(newType, coords))
					{
						typeMatch = true;
					}
				}
				
				if (typeMatch) 
				{
					// Now set the new cell.
					Cell newCell = new Cell(coords.X, coords.Y, newType);
					SetCellValue(coords.X, coords.Y, newCell);
					success = true;
				}
			}

			return success;
		}

		// Gets all the cells in the dungeon that are not empty, and forceGrowthCompatible.
		private List<Cell> ForceGrowthCells()
		{
			List<Cell> cells = new List<Cell>();
			
			foreach (Cell cell in _grid)
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
		
		// Ensure that every cell in the dungeon is "reachable". If not, start fresh. Solving also involves 
		// placing keys in the dungeon prior to corresponding locked doors, and positioning the down stairs 
		// at a suitable location.
		private void DungeonSolve()
		{
			_downStairsCell = null;
			_sequenceNumber = 0;
			_downStairsCellDistance = 0;
			
			Solve(_grid[_startCoords.X, _startCoords.Y]);
			
			for (int y = 0; y < Reference.GridHeight; y++) 
			{
				for (int x = 0; x < Reference.GridWidth; x++)
				{
					if (!_grid[x, y].Visited)
					{
						throw new DungeonGenerateException();
					}
				}
			}
		}

		// Recursive solve algorithm.
		private void Solve(Cell cell)
		{
			_sequenceNumber++;

			cell.Visited = true;
			cell.Sequence = _sequenceNumber;
			
			// Cell above.
			if (cell.Type.TraversableUp && cell.Y + 1 < Reference.GridHeight)
			{
				Cell cellAbove = _grid[cell.X, cell.Y + 1];
				
				if (!cellAbove.Visited)
				{
					Solve(cellAbove);
				}
			}

			// Cell below.
			if (cell.Type.TraversableDown && cell.Y - 1 >= 0)
			{
				Cell cellBelow = _grid[cell.X, cell.Y - 1];
				
				if (!cellBelow.Visited)
				{
					Solve(cellBelow);
				}
			}
			
			// Cell left.
			if (cell.Type.TraversableLeft && cell.X - 1 >= 0)
			{
				Cell cellLeft = _grid[cell.X - 1, cell.Y];
				
				if (!cellLeft.Visited)
				{
					Solve(cellLeft);
				}
			}
			
			// Cell right.
			if (cell.Type.TraversableRight && cell.X + 1 < Reference.GridWidth)
			{
				Cell cellRight = _grid[cell.X + 1, cell.Y];
				
				if (!cellRight.Visited)
				{
					Solve(cellRight);
				}
			}

			if (cell.Type.IsDeadEnd) 
        	{
				CheckForDownStairsPlacement(cell);
			}
		}

		private void CheckForDownStairsPlacement(Cell cell)
		{
			if
			(
				(cell.Type == CellTypes.DeadU && cell.Y == 0)                       ||
				(cell.Type == CellTypes.DeadR && cell.X == 0)                       ||
				(cell.Type == CellTypes.DeadL && cell.X + 1 == Reference.GridWidth) ||
				(cell.Type == CellTypes.DeadD && cell.Y + 1 == Reference.GridHeight)
			)
			{
				return;  // Stairs down cannot be at the edge of the dungeon (leading nowhere).
			}
			
			double distance = DistanceFromStartCell(cell);
			
			if (distance >= _downStairsCellDistance) 
			{
				int placementChance = (int)Math.Round((distance * 100) / _maxDistance);
				int random = _r.Next(100 + 1);
				
				if (random < placementChance && _downStairsCell != null)
				{
					return;
				}
				
				_downStairsCell = cell;
				_downStairsCellDistance = distance;
			}
		}

		private double DistanceFromStartCell(Cell cell)
		{
			// distance = SQRT[(x2 - x1)^2 + (y2 - y1)^2]
			return Math.Abs(Math.Sqrt(Math.Pow(cell.X - _startCoords.X, 2) + Math.Pow(cell.Y - _startCoords.Y, 2)));
		}

		#endregion
		#region Utility

		// Returns a random number 0 >= x < 100, representing percent.
		private int RandomPercent()
		{
			return _r.Next(100);
		}

		private bool RandomBool()
		{
			return _r.Next(2) == 0 ? false : true;
		}

		private int CalcPercentFilled()
		{
			int filledCellCount = 0;
			
			for (int x = 0; x < Reference.GridWidth; x++)
			{
				for (int y = 0; y < Reference.GridHeight; y++) 
				{
					if (!_grid[x, y].Type.IsEmpty)
					{
						filledCellCount++;
					}
				}
			}
			
			return (filledCellCount * 100) / (Reference.GridWidth * Reference.GridHeight);
		}
		
		public string VisualizeAsText(bool showDoors, bool showKeys)
		{
			int x;
			Cell cell;
			string padding;
			StringBuilder line, grid = new StringBuilder();
	
			// Because it is console printing, start with the "top" of the dungeon, and work down.
			for (int y = Reference.GridHeight - 1; y >= 0; y--) 
			{
				line = new StringBuilder();
				
				for (x = 0; x < Reference.GridWidth * 2; x++) 
				{
					cell = _grid[x / 2, y];

					if (x % 2 == 0)
					{  
						if (cell.Door != null && showDoors)
						{
							line.Append("D");
						}
						else if (cell.HasKey && showKeys)
						{
							line.Append("K");
						}
						else if (cell.Type.IsStairsUp)
						{
							line.Append("u");
						}
						else if (cell.Type.IsStairsDown)
						{
							line.Append("d");
						}
						else
						{
							line.Append(cell.Type.TextRep);
						}
					}
					else
					{
						line.Append(cell.Type.TextRep2);
					}

					// Or, quite simply:
					// line.Append(x % 2 == 0 ? cell.Type.TextRep : cell.Type.TextRep2);
				}
				
				padding = (y < 10) ? "0" : "";  // For co-ordinate printing.
				
				grid.AppendLine(padding + y + line.ToString());
			}
			
			// Now print X co-ordinate names at the bottom.
			
			grid.Append("  ");
			
			for (x = 0; x < Reference.GridWidth * 2; x++) 
			{
				if (x % 2 == 0)
					grid.Append((x/2)/10);
				else
					grid.Append(" "); 
			}
			
			grid.Append("\n  ");
			
			for (x = 0; x < Reference.GridWidth * 2; x++) 
			{
				if (x % 2 == 0)
					grid.Append((x/2) % 10);
				else
					grid.Append(" "); 
			}

			return grid.ToString();
		}

		public string BuildStats()
		{
			return Environment.NewLine +
			       "Iterations: " + _iterations.ToString() + Environment.NewLine +
				   "Elapsed Time: " + _elapsedTime.ToString() + Environment.NewLine +
				   "Room Count: " + _roomsCount.ToString() + Environment.NewLine;
		}

		public string DungeonView
		{
			get
			{
				DungeonViewModelCell[,] modelCells = new DungeonViewModelCell[Reference.GridWidth, Reference.GridHeight];

				foreach (Cell cell in _grid)
				{
					DungeonViewModelCell modelCell = new DungeonViewModelCell();

					modelCell.CssName = cell.CssName;
					modelCell.CssLocation = cell.CssLocation;
					modelCell.HasKey = cell.HasKey;
					modelCell.HasGem = cell.HasGem;
					modelCell.HasHeart = cell.HasHeart;
					modelCell.HasSword = cell.HasSword;
					modelCell.HasGoblin = cell.HasGoblin;
					modelCell.DoorDirection = DungeonViewDoor(cell.Door);
					modelCell.TraversableUp = cell.Type.TraversableUp;
					modelCell.TraversableDown = cell.Type.TraversableDown;
					modelCell.TraversableLeft = cell.Type.TraversableLeft;
					modelCell.TraversableRight = cell.Type.TraversableRight;
					modelCell.IsStairsUp = cell.Type.IsStairsUp;
					modelCell.IsStairsDown = cell.Type.IsStairsDown;

					modelCells[cell.X, cell.Y] = modelCell;
				}

				return Utility.Json(modelCells);
			}
		}

		private string DungeonViewDoor(Door door)
		{
			string viewDoor = "";

			if (door != null)
			{
				switch (door.Dir)
				{
					case Direction.Up:
						viewDoor += "U";
						break;
					
					case Direction.Down:
						viewDoor += "D";
						break;
					
					case Direction.Left:
						viewDoor += "L";
						break;
					
					case Direction.Right:
						viewDoor += "R";
						break;

					default:
						break;
				}
			}

			return viewDoor;
		}

		#endregion
	}

	public class DungeonGenerateException : Exception
	{
		public DungeonGenerateException() : base() {}
	}
}