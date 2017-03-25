using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace DigitalWizardry.Dungeon
{	
	public class Dungeon
	{
		private List<Cell> Grid;                 // Master grid data structure, a "simulated 2-D array".
		private int LevelNumber;                 // Zero-indexed level identifier for multi-level dungeons.
		private Random R;                        // Re-usable random number generator.
		private Cell EmptyCell;                  // This empty cell instance is re-used everywhere. It exists outside of "normal space" because its coords are -1,-1.
		private Coords StartCoords;               // Where the entrance to the dungeon is located.
		private int SequenceNumber;               // Used when solving the dungeon to "stamp" cells with their sequence number.
		private Double MaxDistance;               // Maximum possible distance between points in the grid.
		private Cell DownStairsCell;              // The dead-end cell chosen to be replaced with a down stairs.
		private Double DownStairsCellDistance;    // Distance from the start cell to the DownStairsCell.
		private int RoomsCount;                   // Number of rooms randomly determined to be in the dungeon.
		private int MinesCount;                   // Number of mines randomly determined to be in the dungeon.
		private int CatacombsCount;               // Number of catacombs randomly determined to be in the dungeon.
		private List<Room> Rooms;                 // The collection of rooms added to the dungeon.
		private List<Cell> CellsWithLockedDoors;  // Convenience collection of door object.
		private int BuildPasses;                  // Number of discarded attempts before arriving at a completed level.
		private TimeSpan BuildTime;               // How long in total did it take to build this level?

		public Dungeon(int levelNumber)
		{
			Types.Initialize();
			Descriptions.Initialize();
			
			this.R = new Random();
			this.LevelNumber = levelNumber;
			this.EmptyCell = new Cell(-1, -1, Types.EmptyCell, Descriptions.Empty);
			this.MaxDistance = Math.Abs(Math.Sqrt(Math.Pow(Constants.GridWidth - 1, 2) + Math.Pow(Constants.GridHeight - 1, 2)));

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
					this.BuildPasses++;
					
					Initialize();
					PlaceRooms();
					GenerateLevel();
					PlaceDoors();
					LevelSolve();
					PlaceKeys();
					PlaceDownStairs();
					AddDescriptions();
					levelComplete = true;  // i.e. No exceptions...
				}
				catch (DungeonBuildException) 
				{
					levelComplete = false;  // Try again.
				}

			} while (!levelComplete);

			this.BuildTime = DateTime.Now - start;
		}

		private void Initialize()
		{
			this.Grid = new List<Cell>();

			// Fill in each cell with the "empty cell" object.
			for (int i = 0; i < Constants.GridWidth * Constants.GridHeight; i++)
			{
				this.Grid.Add(this.EmptyCell);
			}

			// The level 0 dungeon is outfitted with the start cell at bottom center.
			if (this.LevelNumber == 0) 
			{
				this.StartCoords = PlaceEntrance();
			}
			else  // Otherwise, it "inherits" start cell (i.e. stairs down) from above.
			{
				// this.StartCoords = THIS CODE NEEDS TO BE COMPLETED;
			}

			List<Type> types = Types.GetTypes(this.StartCoords);

			this.CellsWithLockedDoors = new List<Cell>();
		}

		// Set start cell at bottom centre.
		private Coords PlaceEntrance()
		{
			int x = Constants.GridWidth / 2;
			Type startType = Types.Entrance;
			Cell entrance = new Cell(x, 0, startType, Descriptions.Constructed);
			entrance.DescrWeight = 100;
			SetDungeonCellValue(x, 0, entrance);
			return new Coords(x, 0);
		}

		private void GenerateLevel()
		{    
			Cell cell;
			bool modified = false;
			Description descr = Descriptions.Corridor_TBD;
			
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

		private bool AttachNewCell(Cell cell, Description descr)
		{
			bool attachSuccessful = false;
			Coords coords = RandomAttachCoords(cell);
			
			if (coords != null)
			{
				// Get a disposable array of constructed corridor cell types.
				List<Type> types = Types.GetTypes(coords);
				
				// Choose a new cell type to attach.
				Cell newCell = null;
				Type newType = null;
				
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
						newCell = new Cell(coords.X, coords.Y, newType, descr);
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

		// Set a down stairs at the farthest possible location from the start cell.
		private void PlaceDownStairs()
		{
			Cell newCell;
			Type newType;
			
			newType = Types.ConvDeadEndToDownStairs(this.DownStairsCell.Type);
			newCell = new Cell(this.DownStairsCell.X, this.DownStairsCell.Y, newType, this.DownStairsCell.Descr);
			SetCellValue(this.DownStairsCell.X, this.DownStairsCell.Y, newCell);
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
				int randomIndex = this.R.Next(coordPotentials.Count);
				return coordPotentials[randomIndex];
			}
		}

		private Type RandomCellType(List<Type> types)
		{
			// Pick a cell type randomly, and also eliminate it as a candidate for the current cell to avoid 
			// re-testing it in the future if it is rejected. DungeonCellTypes have weights, so some are more 
			// likely to be picked than others.
			
			int total = 0;
			
			foreach (Type type in types)
			{
				total += type.Weight;
			}
			
			int threshold = this.R.Next(total);
			
			Type selected = null;

			foreach (Type type in types)
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
			Cell cell = forceGrowthCells[this.R.Next(forceGrowthCells.Count)];
			forceGrowthCells.Remove(cell);
			return cell;
		}
			
		#endregion

		#region Checks

		// With "coords" representing the new, empty dungeon location, check that each of the adjacent cells 
		// is compatible with the proposed (randomly determined) new cell type.
		private bool TypeCompatibleWithAdjacentCells(Type newCellType, Coords coords)
		{
			// This is an innocent-until-proven guilty scenario. However, if any of the cells is proven to be
			// incompatible, that's enough to eliminate it as a prospect.
			
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

		// If the dungeon level was modified on the last pass, it cannot yet be considered complete. If it 
		// was not modified on the last pass, check to see if the dungeon level is filled to completion. If 
		// it is not, modify a cell to allow the dungeon to grow some more.
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
						// involve a bunch of extra programming. Instead, simply abandon this "failed" 
						// dungeon level and start fresh...
						throw new DungeonBuildException();
					}
				}
			}

			return complete;
		}
			
		#endregion
		#region Rooms

		private void PlaceRooms()
		{	
			this.Rooms = new List<Room>();
			
			CalcRooms();
			PlaceMines();
			PlaceRegularRooms();
			MergeRooms();
			PlaceRoundRoom();
			CleanRoomScraps();
			CleanRoomsArray();
			ConnectRooms();
			ConnectMines();
			CleanRoomsArray();
			ConvertRoomsToCatacombs();
		}

		private void CalcRooms()
		{
			int rand = 0;
			this.MinesCount = 0;

			rand = this.R.Next(100);
			
			if (rand >= 85 && rand <= 99)
			{
				this.MinesCount = 1;
			}
	 
			this.CatacombsCount = 0;

			rand = this.R.Next(100);
			
			if (rand >= 70 && rand < 92)
			{
				this.CatacombsCount = 1;
			}
			else if (rand >= 92 && rand < 98)
			{	
				this.CatacombsCount = 2;
			}
			else if (rand >= 98)
			{
				this.CatacombsCount = 3;
			}

			int rooms = this.R.Next(Constants.MaxRooms - Constants.MinRooms + 1) + Constants.MinRooms;  // MinRooms ~ MaxRooms
			
			this.RoomsCount = rooms - MinesCount;
		}

		private void PlaceRegularRooms()
		{
			int attempts = 0;
			int roomsCount = this.RoomsCount;
			int maxAttempts = Constants.GridWidth * Constants.GridHeight;
			
			while (roomsCount > 0 && attempts <= maxAttempts) 
			{
				if (RandomRoom(Descriptions.Room_TBD, Constants.MaxRoomWidth, Constants.MaxRoomHeight, Constants.MinRoomWidth, Constants.MinRoomHeight))
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

		private void PlaceRoundRoom()
		{
			bool place = Constants.PlaceRoundRoom;  // This is just to trick the compiler into not issuing an unreachable code warning for the code below.

			if (place)
			{
				bool placed = false;
				int attempts = 0, maxAttempts = Constants.GridWidth * Constants.GridHeight;
				
				while (attempts <= maxAttempts) 
				{
					attempts++;
					Coords coords = RandomCell(true);

					if (!RoomFits(coords, 3, 3, true))
					{
						continue;
					}
					else
					{
						this.Rooms.Add(BuildRoom(RoomType.Round, coords, 3, 3));
						placed = true;
						break;
					}
				}
				
				if (!placed)
				{
					throw new DungeonBuildException();
				}
			}
		}

		private void PlaceMines()
		{
			int attempts = 0;
			int count = this.MinesCount;  // Preserve original count.
			int maxAttempts = Constants.GridWidth * Constants.GridHeight;
			
			while (count > 0 && attempts <= maxAttempts) 
			{
				bool horiz = RandomBool();
			
				if (horiz)
				{
					if (RandomRoom(Descriptions.Mines_Horiz, Constants.MaxMinesHeight, Constants.MaxMinesWidth, Constants.MinMinesHeight, Constants.MinMinesWidth))
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
					if (RandomRoom(Descriptions.Mines_Vert, Constants.MaxMinesWidth, Constants.MaxMinesHeight, Constants.MinMinesWidth, Constants.MinMinesHeight))
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

		private bool RandomRoom(Description descr, int maxWidth, int maxHeight, int minWidth, int minHeight)
		{
			int width = this.R.Next(maxWidth - minWidth + 1) + minWidth;
			int height = this.R.Next(maxHeight - minHeight + 1) + minHeight;
			
			Coords coords = RandomCell(true);  // Get a random empty cell as the attach point.
			
			if (!RoomFits(coords, width, height, false))
			{
				return false;
			}
			else
			{
				if (descr == Descriptions.Room_TBD)
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
				else 
				{
					BuildMines(coords, width, height, descr);
				}
			}
			return true;
		}

		// Returns true if the room fits into the dungeon entirely within allowable, currently-empty cells.
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
							(cell.Descr.IsMines) ||
							(
								y > this.StartCoords.Y - 2 && y < this.StartCoords.Y + 2 && 
								x > this.StartCoords.X - 2 && x < this.StartCoords.X + 2
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

		private Room BuildRoom(RoomType roomType, Coords coords, int width, int height)
		{
			Description descr = roomType == RoomType.Round ? Descriptions.Constructed : Descriptions.Room_TBD;
			
			Room room = new Room(coords.X, coords.Y, descr);
			
			int widthReduce = 0, heightReduce = 0;
			
			if (roomType != RoomType.Regular && roomType != RoomType.Round) 
			{
				widthReduce = this.R.Next(width - 2) + 1;
				heightReduce = this.R.Next(height - 2) + 1;
			}
			
			for (int y = coords.Y; y < coords.Y + height; y++) 
			{
				for (int x = coords.X; x < coords.X + width; x++) 
				{
					Type newType = null;
					
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
						
						case RoomType.Round:
							newType = RoundRoomType(x, y, coords, width, height);
							break;
							
						default:
							newType = RegRoomType(x, y, coords, width, height);
							break;
					}
					
					if (newType == null)
						continue;
					
					Cell currentCell = CellAt(x, y);
					
					if (IncompatibleCornerTypes(currentCell.Type, newType))
					{
						DeleteRoom(room);
						return null;
					}
					
					Cell newCell = new Cell(x, y, newType, descr);
					SetCellValue(x, y, newCell);
					
					if (roomType == RoomType.Round)
					{
						newCell.Merged = true;
						newCell.DescrWeight = 0;
					}
					
					if (newType == Types.RoomSpace || newType == Types.Fountain)
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

		private Type RegRoomType(int x, int y, Coords coords, int width, int height)
		{
			Type newType = null;
			
			if (x == coords.X && y == coords.Y)                                // Bottom-left corner.
			{
				newType = Types.RoomWallDL;
			}
			else if (x == coords.X + width - 1 && y == coords.Y)               // Bottom-right corner.
			{
				newType = Types.RoomWallDR;
			}
			else if (x == coords.X && y == coords.Y + height - 1)              // Top-left corner.
			{
				newType = Types.RoomWallUL;
			}
			else if (x == coords.X + width - 1 && y == coords.Y + height - 1)  // Top-right corner.
			{
				newType = Types.RoomWallUR;
			}
			else if (x == coords.X)                                            // Left wall.
			{
				newType = Types.RoomWallL;
			}
			else if (x == coords.X + width - 1)                                // Right wall.
			{
				newType = Types.RoomWallR;   
			}
			else if (y == coords.Y)                                            // Bottom wall.
			{
				newType = Types.RoomWallD;
			}
			else if (y == coords.Y + height - 1)                               // Top wall.
			{
				newType = Types.RoomWallU;
			}
			else
			{
				newType = Types.RoomSpace;
			}
			
			return newType;
		}

		private Type RoundRoomType(int x, int y, Coords coords, int width, int height)
		{
			Type newType = null;
			
			if (x == coords.X && y == coords.Y)                                    // Bottom-left corner.
			{
				newType = Types.RoomWallDL_Round;
			}
			else if (x == coords.X + width - 1 && y == coords.Y)                   // Bottom-right corner.
			{
				newType = Types.RoomWallDR_Round;
			}
			else if (x == coords.X && y == coords.Y + height - 1)                  // Top-left corner.
			{
				newType = Types.RoomWallUL_Round;
			}
			else if (x == coords.X + width - 1 && y == coords.Y + height - 1)      // Top-right corner.
			{
				newType = Types.RoomWallUR_Round;
			}
			else if (x == coords.X && x == 0)                                      // Left wall.
			{
				newType = Types.RoomWallL_Round;
			}
			else if (x == coords.X)                                                // Left exit.
			{
				newType = Types.RoomExitL_Round;
			}
			else if (x == coords.X + width - 1 && x == Constants.GridWidth - 1)    // Right wall.
			{
				newType = Types.RoomWallR_Round; 
			}
			else if (x == coords.X + width - 1)                                    // Right exit.
			{
				newType = Types.RoomExitR_Round;  
			}
			else if (y == coords.Y && y == 0)                                      // Bottom wall.
			{
				newType = Types.RoomWallD_Round;
			}
			else if (y == coords.Y)                                                // Bottom exit.
			{
				newType = Types.RoomExitD_Round;
			}
			else if (y == coords.Y + height - 1 && y == Constants.GridHeight - 1)  // Top wall.
			{
				newType = Types.RoomWallU_Round;
			}
			else if (y == coords.Y + height - 1)                                   // Top exit.
			{
				newType = Types.RoomExitU_Round;
			}
			else
			{
				newType = Types.Fountain;
			}
			
			return newType;
		}

		private void BuildIrregularlyShapedRoom(Coords coords, int width, int height)
		{
			int quadrant = this.R.Next(4);
			
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
		private Type IrregRoomTypeUL(int x, int y, Coords coords, int width, int height, int widthReduce, int heightReduce) 
		{
			Type newType = null;
			
			if (y >= coords.Y && y < coords.Y + height - heightReduce - 1)         // Normal room section.
			{
				if (x == coords.X && y == coords.Y)                                // Bottom-left corner.
				{
					newType = Types.RoomWallDL;
				}
				else if (x == coords.X + width - 1 && y == coords.Y)               // Bottom-right corner.
				{
					newType = Types.RoomWallDR;
				}
				else if (x == coords.X && y == coords.Y + height - 1)              // Top-left corner.
				{
					newType = Types.RoomWallUL;
				}
				else if (x == coords.X + width - 1 && y == coords.Y + height - 1)  // Top-right corner.
				{
					newType = Types.RoomWallUR;
				}
				else if (x == coords.X)                                            // Left wall.
				{
					newType = Types.RoomWallL;
				}
				else if (x == coords.X + width - 1)                                // Right wall.
				{
					newType = Types.RoomWallR;   
				}
				else if (y == coords.Y)                                            // Bottom wall.
				{
					newType = Types.RoomWallD;
				}
				else if (y == coords.Y + height - 1)                               // Top wall.
				{
					newType = Types.RoomWallU;
				}
				else
				{
					newType = Types.RoomSpace;
				}
			}
			else if (y == coords.Y + height - heightReduce - 1)  
			{
				if (x == coords.X)                                                
				{
					newType = Types.RoomWallUL;
				}
				else if (x == coords.X + width - widthReduce - 1)                
				{
					newType = Types.RoomWallDRinv;
				}
				else if (x == coords.X + width - 1)                                
				{
					newType = Types.RoomWallR;
				}
				else if (x > coords.X && x < coords.X + width - widthReduce - 1) 
				{
					newType = Types.RoomWallU;
				}
				else
				{
					newType = Types.RoomSpace;
				}
			}
			else if (y > coords.Y + height - heightReduce - 1 && y < coords.Y + height - 1) 
			{
				if (x == coords.X + width - 1)                                            
				{
					newType = Types.RoomWallR;
				}
				else if (x == coords.X + width - widthReduce - 1)                
				{
					newType = Types.RoomWallL; 
				}
				else if (x > coords.X + width - widthReduce - 1)   
				{
					newType = Types.RoomSpace;
				}
				//else Empty.
			}
			else
			{
				if (x == coords.X + width - 1)                                            
				{
					newType = Types.RoomWallUR;
				}
				else if (x == coords.X + width - widthReduce - 1)               
				{
					newType = Types.RoomWallUL;
				}
				else if (x > coords.X + width - widthReduce - 1)  
				{
					newType = Types.RoomWallU;
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
		private Type IrregRoomTypeUR(int x, int y, Coords coords, int width, int height, int widthReduce, int heightReduce)
		{
			Type newType = null;
			
			if (y >= coords.Y && y < coords.Y + height - heightReduce - 1)         // Normal room section.
			{
				if (x == coords.X && y == coords.Y)                                // Bottom-left corner.
				{
					newType = Types.RoomWallDL;
				}
				else if (x == coords.X + width - 1 && y == coords.Y)               // Bottom-right corner.
				{
					newType = Types.RoomWallDR;
				}
				else if (x == coords.X && y == coords.Y + height - 1)              // Top-left corner.
				{
					newType = Types.RoomWallUL;
				}
				else if (x == coords.X + width - 1 && y == coords.Y + height - 1)  // Top-right corner.
				{
					newType = Types.RoomWallUR;
				}
				else if (x == coords.X)                                            // Left wall.
				{
					newType = Types.RoomWallL;
				}
				else if (x == coords.X + width - 1)                                // Right wall.
				{
					newType = Types.RoomWallR;   
				}
				else if (y == coords.Y)                                            // Bottom wall.
				{
					newType = Types.RoomWallD;
				}
				else if (y == coords.Y + height - 1)                               // Top wall.
				{
					newType = Types.RoomWallU;
				}
				else
				{
					newType = Types.RoomSpace;
				}
			}
			else if (y == coords.Y + height - heightReduce - 1)  
			{
				if (x == coords.X)                                                 // Left wall.
				{
					newType = Types.RoomWallL;
				}
				else if (x == coords.X + width - widthReduce - 1)                  // Bottom-left corner inv.
				{
					newType = Types.RoomWallDLinv;
				}
				else if (x == coords.X + width - 1)                                // Top-right corner.
				{
					newType = Types.RoomWallUR;
				}
				else if (x > coords.X + width - widthReduce - 1 && x < coords.X + width - 1)   // Top wall.
				{
					newType = Types.RoomWallU;
				}
				else
				{
					newType = Types.RoomSpace;
				}
			}
			else if (y > coords.Y + height - heightReduce - 1 && y < coords.Y + height - 1) 
			{
				if (x == coords.X)                                                 // Left wall.
				{
					newType = Types.RoomWallL;
				}
				else if (x == coords.X + width - widthReduce - 1)                  // Right wall.
				{
					newType = Types.RoomWallR; 
				}
				else if (x > coords.X && x < coords.X + width - widthReduce - 1)   // Room space.
				{
					newType = Types.RoomSpace;
				}
				//else Empty.
			}
			else
			{
				if (x == coords.X)                                                 // Top-left corner.
				{
					newType = Types.RoomWallUL;
				}
				else if (x == coords.X + width - widthReduce - 1)                  // Top-right corner.
				{
					newType = Types.RoomWallUR;
				}
				else if (x > coords.X && x < coords.X + width - widthReduce - 1)   // Top wall.
				{
					newType = Types.RoomWallU;
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
		private Type IrregRoomTypeDL(int x, int y, Coords coords, int width, int height, int widthReduce, int heightReduce)
		{
			Type newType = null;
			
			if (y > coords.Y + height - heightReduce - 1)                          // Normal room section.
			{
				if (x == coords.X && y == coords.Y)                                // Bottom-left corner.
				{
					newType = Types.RoomWallDL;
				}
				else if (x == coords.X + width - 1 && y == coords.Y)               // Bottom-right corner.
				{
					newType = Types.RoomWallDR;
				}
				else if (x == coords.X && y == coords.Y + height - 1)              // Top-left corner.
				{
					newType = Types.RoomWallUL;
				}
				else if (x == coords.X + width - 1 && y == coords.Y + height - 1)  // Top-right corner.
				{
					newType = Types.RoomWallUR;
				}
				else if (x == coords.X)                                            // Left wall.
				{
					newType = Types.RoomWallL;
				}
				else if (x == coords.X + width - 1)                                // Right wall.
				{
					newType = Types.RoomWallR;   
				}
				else if (y == coords.Y)                                            // Bottom wall.
				{
					newType = Types.RoomWallD;
				}
				else if (y == coords.Y + height - 1)                               // Top wall.
				{
					newType = Types.RoomWallU;
				}
				else
				{
					newType = Types.RoomSpace;
				}
			}
			else if (y == coords.Y + height - heightReduce - 1)  
			{
				if (x == coords.X)                                                
				{
					newType = Types.RoomWallDL;
				}
				else if (x == coords.X + width - widthReduce - 1)                
				{
					newType = Types.RoomWallURinv;
				}
				else if (x == coords.X + width - 1)                              
				{
					newType = Types.RoomWallR;
				}
				else if (x > coords.X && x < coords.X + width - widthReduce - 1)   
				{
					newType = Types.RoomWallD;
				}
				else
				{
					newType = Types.RoomSpace;
				}
			}
			else if (y > coords.Y && y < coords.Y + height - heightReduce - 1) 
			{
				if (x == coords.X + width - 1)                                                
				{
					newType = Types.RoomWallR;
				}
				else if (x == coords.X + width - widthReduce - 1)               
				{
					newType = Types.RoomWallL; 
				}
				else if (x > coords.X + width - widthReduce - 1 && x < coords.X + width - 1)  
				{
					newType = Types.RoomSpace;
				}
				//else Empty.
			}
			else
			{
				if (x == coords.X + width - 1)                                                 
				{
					newType = Types.RoomWallDR;
				}
				else if (x == coords.X + width - widthReduce - 1)           
				{
					newType = Types.RoomWallDL;
				}
				else if (x > coords.X + width - widthReduce - 1 && x < coords.X + width - 1)  
				{
					newType = Types.RoomWallD;
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
		private Type IrregRoomTypeDR(int x, int y, Coords coords, int width, int height, int widthReduce, int heightReduce) 
		{
			Type newType = null;
			
			if (y > coords.Y + height - heightReduce - 1)                          // Normal room section.
			{
				if (x == coords.X && y == coords.Y)                                // Bottom-left corner.
				{
					newType = Types.RoomWallDL;
				}
				else if (x == coords.X + width - 1 && y == coords.Y)               // Bottom-right corner.
				{
					newType = Types.RoomWallDR;
				}
				else if (x == coords.X && y == coords.Y + height - 1)              // Top-left corner.
				{
					newType = Types.RoomWallUL;
				}
				else if (x == coords.X + width - 1 && y == coords.Y + height - 1)  // Top-right corner.
				{
					newType = Types.RoomWallUR;
				}
				else if (x == coords.X)                                            // Left wall.
				{
					newType = Types.RoomWallL;
				}
				else if (x == coords.X + width - 1)                                // Right wall.
				{
					newType = Types.RoomWallR;   
				}
				else if (y == coords.Y)                                            // Bottom wall.
				{
					newType = Types.RoomWallD;
				}
				else if (y == coords.Y + height - 1)                               // Top wall.
				{
					newType = Types.RoomWallU;
				}
				else
				{
					newType = Types.RoomSpace;
				}
			}
			else if (y == coords.Y + height - heightReduce - 1)  
			{
				if (x == coords.X)                                                
				{
					newType = Types.RoomWallL;
				}
				else if (x == coords.X + width - widthReduce - 1)                
				{
					newType = Types.RoomWallULinv;
				}
				else if (x == coords.X + width - 1)                              
				{
					newType = Types.RoomWallDR;
				}
				else if (x > coords.X + width - widthReduce - 1 && x < coords.X + width - 1)   
				{
					newType = Types.RoomWallD;
				}
				else
				{
					newType = Types.RoomSpace;
				}
			}
			else if (y > coords.Y && y < coords.Y + height - heightReduce - 1) 
			{
				if (x == coords.X)                                                
				{
					newType = Types.RoomWallL;
				}
				else if (x == coords.X + width - widthReduce - 1)               
				{
					newType = Types.RoomWallR; 
				}
				else if (x > coords.X && x < coords.X + width - widthReduce - 1)  
				{
					newType = Types.RoomSpace;
				}
				//else Empty.
			}
			else
			{
				if (x == coords.X)                                                 
				{
					newType = Types.RoomWallDL;
				}
				else if (x == coords.X + width - widthReduce - 1)           
				{
					newType = Types.RoomWallDR;
				}
				else if (x > coords.X && x < coords.X + width - widthReduce - 1)  
				{
					newType = Types.RoomWallD;
				}
				//else Empty.
			}
			
			return newType;
		}

		private void BuildMines(Coords coords, int width, int height, Description descr)
		{
			Type newType = null;
			
			for (int y = coords.Y; y < coords.Y + height; y++) 
			{
				for (int x = coords.X; x < coords.X + width; x++) 
				{
					if (x == coords.X && y == coords.Y)                                // Bottom-left corner.
					{
						newType = Types.ElbUR;
					}
					else if (x == coords.X + width - 1 && y == coords.Y)               // Bottom-right corner.
					{
						newType = Types.ElbUL;
					}
					else if (x == coords.X && y == coords.Y + height - 1)              // Top-left corner.
					{
						newType = Types.ElbDR;
					}
					else if (x == coords.X + width - 1 && y == coords.Y + height - 1)  // Top-right corner.
					{
						newType = Types.ElbDL;
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
					else if (descr == Descriptions.Mines_Horiz)
					{
						newType = Types.Horiz;
					}
					else if (descr == Descriptions.Mines_Vert)
					{
						newType = Types.Vert;
					}
					
					Cell currentCell = CellAt(x, y);
					
					if (currentCell.Type.IsEmpty)
					{
						Cell newCell = new Cell(x, y, newType, descr);
						SetCellValue(x, y, newCell);
					}
				}
			}
		}

		private Type MinesWall(int x, int y, Direction dir, Description desc)
		{
			Type type = null;
		
			// Room exits can only occur if the room wall in question is at least 1 cell from the dungeon 
			// edge, to allow the resulting corridors to grow or be "capped".
			
			if (dir == Direction.Up)                             
			{
				if (desc == Descriptions.Mines_Horiz)
				{
					type = Types.Horiz;    // No exit for horizontal mine.
				}
				else if (y + 1 < Constants.GridHeight)
				{
					type = Types.Inter;    // Exit for vertical mine.
				}
				else
				{
					type = Types.JuncDLR;  // No exit for vertical mine.
				}
			}
			else if (dir == Direction.Down)                                            
			{
				if (desc == Descriptions.Mines_Horiz)
				{
					type = Types.Horiz;    // No exit for horizontal mine.
				}
				else if (y - 1 >= 0)
				{
					type = Types.Inter;    // Exit for vertical mine.
				}
				else
				{
					type = Types.JuncULR;  // No exit for vertical mine.
				}
			}
			else if (dir == Direction.Left)                                              
			{        
				if (desc == Descriptions.Mines_Vert)
				{
					type = Types.Vert;    // No exit for vertical mine.
				}
				else if (x - 1 >= 0)
				{
					type = Types.Inter;    // Exit for horizontal mine.
				}
				else
				{
					type = Types.JuncUDR;  // No exit for horizontal mine.
				}
			}                                
			else if (dir == Direction.Right)                                  
			{
				if (desc == Descriptions.Mines_Vert)
				{
					type = Types.Vert;    // No exit for vertical mine.
				}
				else if (x + 1 < Constants.GridHeight)
				{
					type = Types.Inter;    // Exit for horizontal mine.
				}
				else
				{
					type = Types.JuncUDL;  // No exit for horizontal mine.
				}
			}
			
			return type;
		}

		// Kerplunk, the rooms are all superimposed. Each room needs to be outlined properly with no gaps.
		private void MergeRooms()
		{
			for (int y = 0; y < Constants.GridHeight; y++)
			{
				for (int x = 0; x < Constants.GridWidth; x++)
				{
					Cell cell = CellAt(x, y);
					
					if (cell.Type == Types.RoomWallDL && !cell.Merged)
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
							throw new DungeonBuildException();
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
			Room room = new Room(startX, startY, Descriptions.Room_TBD);
			Cell currentCell, cellUp, cellDown, cellLeft, cellRight;
			
			do
			{        
				// Since the room "plunking" process, and the outline traverse process aren't perfect 
				// systems, a protection is required for the infrequent, but typical, times when the 
				// traverse attempts to exceed the size of the dungeon level.
				if (y < 0 || y >= Constants.GridHeight || x < 0 || x >= Constants.GridWidth)
				{
					throw new DungeonBuildException();
				}
				
				currentCell = CellAt(x, y);
				Type newType = currentCell.Type;  // Default.
				
				// Arriving back at an already merged cell in the same room means that the rooms cannot be 
				// properly merged. This is usually due to a "missing" wall cells occurring due to the way 
				// the "sub-rooms" were "plunked" down on the level. Irrecoverable. Abort.
				if (currentCell.Merged && room.Walls.Contains(currentCell))
				{
					throw new DungeonBuildException();
				}
				
				cellUp = y + 1 >= 0 && y + 1 < Constants.GridHeight && x >= 0 && x < Constants.GridWidth 
						? CellAt(x, y + 1) : null;
				
				cellDown  = y - 1 >= 0 && y - 1 < Constants.GridHeight && x >= 0 && x < Constants.GridWidth             
						? CellAt(x, y - 1) : null;
				
				cellLeft  = x - 1 >= 0 && x - 1 < Constants.GridWidth && y >= 0 && y < Constants.GridWidth 
						? CellAt(x - 1, y) : null;
				
				cellRight = x + 1 >= 0 && x + 1 < Constants.GridWidth && y >= 0 && y < Constants.GridWidth 
						? CellAt(x + 1, y) : null;
				
				if (x == startX && y == startY)
				{
					newType = RandomRoomCorner(x, y, Direction.DownLeft);
					newX = x;
					newY = y + 1;
					dir = Direction.Up;
				}
				else if (dir == Direction.Up && cellLeft != null && cellLeft.Type.RoomConnectsRight)
				{
					newType = Types.RoomWallURinv;
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
				else if (dir == Direction.Up && (currentCell.Type == Types.RoomWallUL || currentCell.Type == Types.RoomWallULinv))
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
					newType = Types.RoomWallULinv;
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
				else if (dir == Direction.Left && (currentCell.Type == Types.RoomWallDL || currentCell.Type == Types.RoomWallDLinv))
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
					newType = Types.RoomWallDRinv;
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
				else if (dir == Direction.Right && (currentCell.Type == Types.RoomWallUR || currentCell.Type == Types.RoomWallURinv))
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
					newType = Types.RoomWallDLinv;
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
				else if (dir == Direction.Down && (currentCell.Type == Types.RoomWallDR || currentCell.Type == Types.RoomWallDRinv))
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
				
				Cell newCell = new Cell(x, y, newType, Descriptions.Room_TBD);
				newCell.Merged = true;
				SetCellValue(x, y, newCell);
				room.Walls.Add(newCell);
				
				x = newX;
				y = newY;
				
			} while (!(x == startX && y == startY));
			
			this.Rooms.Add(room);
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
				currentCell = y < Constants.GridHeight ? CellAt(x, y) : null;
				cellUp = y + up < Constants.GridHeight ? CellAt(x, y + up) : null;
				
				if (Types.IsCleanStartWall(currentCell.Type))
				{
					while (!cellUp.Merged)
					{
						Type newType = Types.RoomSpace;
						Cell newCell = new Cell(x, y + up, newType, Descriptions.Room_TBD);
						newCell.Merged = true;
						SetCellValue(x, y + up, newCell);
						room.Space.Add(newCell);
						up += 1;
						currentCell = CellAt(x, y + up);
						cellUp = y + up < Constants.GridHeight ? CellAt(x, y + up) : null;
					}
				}
				
				cellUp    = y + 1 < Constants.GridHeight ? CellAt(x, y + 1) : null;
				cellDown  = y - 1 >= 0                   ? CellAt(x, y - 1) : null;
				cellLeft  = x - 1 >= 0                   ? CellAt(x - 1, y) : null;
				cellRight = x + 1 < Constants.GridWidth  ? CellAt(x + 1, y) : null;
				
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
					throw new DungeonBuildException();
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
			for (int y = 0; y < Constants.GridHeight; y++) 
			{
				for (int x = 0; x < Constants.GridWidth; x++) 
				{
					Cell cell = CellAt(x, y);
					
					if (Types.IsRoomType(cell.Type) && !cell.Merged)
					{
						ReplaceDungeonCellValue(x, y, this.EmptyCell);
					}
				}
			}
		}

		// Remove any bits and pieces left over from the room placement and merge process.
		private void CleanRoomsArray()
		{
			List<Cell> junk = new List<Cell>();
			
			foreach (Room room in this.Rooms) 
			{
				foreach (Cell wall in room.Walls) 
				{
					if (!this.Grid.Contains(wall))
					{
						junk.Add(wall);
					}
				}
				
				foreach (Cell space in room.Space) 
				{
					if (!this.Grid.Contains(space))
					{
						junk.Add(space);
					}
				}
			}
			
			foreach (Room room in this.Rooms) 
			{
				foreach (Cell junkCell in junk) 
				{
					room.Walls.Remove(junkCell);
					room.Space.Remove(junkCell);
				}
			}
		}

		// Need a random piece of wall? Then you've come to the right place!
		private Type RandomRoomWall(int x, int y, Direction dir)
		{
			Type type = null;
			bool exit = this.R.Next(100) + 1 <= Constants.RoomExitProb;
			
			// Room exits can only occur if the room wall in question is at least 1 cell
			// from the dungeon edge, to allow the resulting corridors to grow or be "capped".
			
			if (dir == Direction.Up)                             
			{
				if (exit && y + 1 < Constants.GridHeight)
					type = Types.RoomExitU; 
				else
					type = Types.RoomWallU;
			}
			else if (dir == Direction.Down)                                            
			{
				if (exit && y - 1 >= 0)
					type = Types.RoomExitD;
				else
					type = Types.RoomWallD;
			}
			else if (dir == Direction.Left)                                              
			{
				if (exit && x - 1 >= 0) 
					type = Types.RoomExitL; 
				else
					type = Types.RoomWallL;
			}                                
			else if (dir == Direction.Right)                                  
			{
				if (exit && x + 1 < Constants.GridHeight) 
					type = Types.RoomExitR;
				else
					type = Types.RoomWallR;
			}
			
			return type;
		}

		private Type RandomRoomCorner(int x, int y, Direction dir)
		{
			Type type = null;
			bool exit = this.R.Next(100) + 1 <= Constants.RoomExitProb;
			List<Type> possibleExitTypes = new List<Type>();
			
			// Room exits can only occur if the room wall in question is at least 1 cell
			// from the dungeon edge, to allow the resulting corridors to grow or be "capped".
			
			if (dir == Direction.UpLeft)                             
			{
				if (!exit) 
				{
					return Types.RoomWallUL;
				}
				
				if (y + 1 < Constants.GridHeight)
				{
					possibleExitTypes.Add(Types.RoomExitUL_U);
				}
				
				if (x - 1 >= 0)
				{
					possibleExitTypes.Add(Types.RoomExitUL_L);
				}
				
				if (x - 1 >= 0 && y + 1 < Constants.GridHeight)
				{
					possibleExitTypes.Add(Types.RoomExitUL_UL);
				}
				
				if (possibleExitTypes.Count > 0)
				{
					type = possibleExitTypes[this.R.Next(possibleExitTypes.Count)];
				}
				else
				{
					type = Types.RoomWallUL;  // No exits fit.
				}
			}
			else if (dir == Direction.UpRight)                                            
			{
				if (!exit) 
				{
					return Types.RoomWallUR;
				}
				
				if (y + 1 < Constants.GridHeight)
				{
					possibleExitTypes.Add(Types.RoomExitUR_U);
				}
				
				if (x + 1 < Constants.GridHeight)
				{
					possibleExitTypes.Add(Types.RoomExitUR_R);
				}
				
				if (x + 1 < Constants.GridHeight && y + 1 < Constants.GridHeight)
				{
					possibleExitTypes.Add(Types.RoomExitUR_UR);
				}
				
				if (possibleExitTypes.Count > 0)
				{
					type = possibleExitTypes[this.R.Next(possibleExitTypes.Count)];
				}
				else
				{
					type = Types.RoomWallUR;  // No exits fit.
				}
			}
			else if (dir == Direction.DownLeft)                                              
			{
				if (!exit) 
				{
					return Types.RoomWallDL;
				}
				
				if (y - 1 >= 0)
				{
					possibleExitTypes.Add(Types.RoomExitDL_D);
				}
				
				if (x - 1 >= 0)
				{
					possibleExitTypes.Add(Types.RoomExitDL_L);
				}
				
				if (x - 1 >= 0 && y - 1 >= 0)
				{
					possibleExitTypes.Add(Types.RoomExitDL_DL);
				}
				
				if (possibleExitTypes.Count > 0)
				{
					type = possibleExitTypes[this.R.Next(possibleExitTypes.Count)];
				}
				else
				{
					type = Types.RoomWallDL;  // No exits fit.
				}
			}                                
			else if (dir == Direction.DownRight)                                  
			{
				if (!exit) 
				{
					return Types.RoomWallDR;
				}
				
				if (y - 1 >= 0)
				{
					possibleExitTypes.Add(Types.RoomExitDR_D);
				}
				
				if (x + 1 < Constants.GridHeight)
				{
					possibleExitTypes.Add(Types.RoomExitDR_R);
				}
				
				if (x + 1 < Constants.GridHeight && y - 1 >= 0)
				{
					possibleExitTypes.Add(Types.RoomExitDR_DR);
				}
				
				if (possibleExitTypes.Count > 0)
				{
					type = possibleExitTypes[this.R.Next(possibleExitTypes.Count)];
				}
				else
				{
					type = Types.RoomWallDR;  // No exits fit.
				}
			}
			
			return type;
		}

		private bool IncompatibleCornerTypes(Type currentType, Type newType)
		{
			return 
			(currentType == Types.RoomWallUL && newType == Types.RoomWallDR) ||
			(currentType == Types.RoomWallUR && newType == Types.RoomWallDL) ||
			(currentType == Types.RoomWallDL && newType == Types.RoomWallUR) ||
			(currentType == Types.RoomWallDR && newType == Types.RoomWallUL);
		}

		// Any existing room exit should be connected to any adjacent room section belonging to another room.
		// We don't need to worry about existing exits that, just by chance, happened to be adjacent to one of
		// the exits from another room, because SetCellValue() already takes care of that! 👍
		private void ConnectRooms()
		{
			foreach (Room room in this.Rooms) 
			{
				Direction dir = Direction.Up;
				int x = room.X, y = room.Y;
				int newX = x, newY = y;
				Cell cellUp, cellDown, cellLeft, cellRight;
				
				AssertExit(room);
				
				do
				{
					ConnectCells(x, y);
					
					cellUp    = y + 1 < Constants.GridHeight ? CellAt(x, y + 1) : null;
					cellDown  = y - 1 >= 0                   ? CellAt(x, y - 1) : null;
					cellLeft  = x - 1 >= 0                   ? CellAt(x - 1, y) : null;
					cellRight = x + 1 < Constants.GridWidth  ? CellAt(x + 1, y) : null;
					
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
						throw new DungeonBuildException();
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
			Cell cell = CellAt(x, y);
			
			if (!Types.IsRoomExit(cell.Type))
			{
				return;
			}
			
			// The room wall section is an unconnected exit if the cell has an available connection.
			if (cell.Type.ConnectsLeft && x - 1 >= 0 && cell.AvailableConnections > 0)                                 
			{
				ConnectRoomCells(cell, x - 1, y, Direction.Left);
			}
			
			if (cell.Type.ConnectsRight && x + 1 < Constants.GridWidth && cell.AvailableConnections > 0)
			{
				ConnectRoomCells(cell, x + 1, y, Direction.Right);
			}
			
			if (cell.Type.ConnectsDown && y - 1 >= 0 && cell.AvailableConnections > 0)
			{
				ConnectRoomCells(cell, x, y - 1, Direction.Down);
			}
			
			if (cell.Type.ConnectsUp && y + 1 < Constants.GridHeight && cell.AvailableConnections > 0)
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
				else if (this.Grid.Contains(cell))
				// Some cells are still in the room.walls array even though they are no longer in the dungeon
				// level, because they were "plunked upon" by other room cells. Do not consider those for exits.
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
					throw new DungeonBuildException();
				}
				
				Cell cell = walls[this.R.Next(walls.Count)];
				
				Direction directionOK = Direction.NoDir;
				
				if (Types.IsRoomCorner(cell.Type))
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
					Type newType = Types.ConvRoomWallToExit(cell.Type, directionOK);
					
					Cell newCell = new Cell(cell.X, cell.Y, newType, Descriptions.Room_TBD);
					
					SetDungeonCellValue(cell.X, cell.Y, newCell);
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
			int r = this.R.Next(3);
			
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
			Direction dir = Types.RoomWallDirection(cell.Type);
				
			if (dir == Direction.Up && cell.Y + 1 < Constants.GridHeight)
			{
				adjacentCell = CellAt(cell.X, cell.Y + 1);
			}
			else if (dir == Direction.Down && cell.Y - 1 >= 0)
			{
				adjacentCell = CellAt(cell.X, cell.Y - 1);
			}
			else if (dir == Direction.Left && cell.X - 1 >= 0)
			{
				adjacentCell = CellAt(cell.X - 1, cell.Y);
			}
			else if (dir == Direction.Right && cell.X + 1 < Constants.GridWidth)
			{
				adjacentCell = CellAt(cell.X + 1, cell.Y);
			}
				
			if (adjacentCell != null && (adjacentCell.Type.IsEmpty || (adjacentCell.Type.RoomExitCompatible && adjacentCell.Descr == Descriptions.Room_TBD)))
			{
				return dir;
			}
			else
			{
				return Direction.NoDir;
			}
		}

		// RP: 2017-03-22. Fixed what appeared to be a copy-paste bug in this method where the CellAt() calls 
		// didn't match coords with the conditional checks immediately above them.
		private Direction RoomCellsAdjacentOK(Cell cell)
		{
			bool okUp = false, okDown = false, okLeft = false, okRight = false;
			Cell adjCellUp, adjCellDown, adjCellLeft, adjCellRight;
			
			Direction dir = Types.RoomWallDirection(cell.Type);
				
			if (dir == Direction.UpLeft || dir == Direction.UpRight)
			{
				if (cell.Y + 1 < Constants.GridHeight)
				{
					adjCellUp = CellAt(cell.X, cell.Y + 1);

					if (adjCellUp.Type.IsEmpty || (adjCellUp.Type.RoomExitCompatible && adjCellUp.Descr == Descriptions.Room_TBD))
					{
						okUp = true;
					}
				}
			}
			
			if (dir == Direction.DownLeft || dir == Direction.DownRight)
			{
				if (cell.Y - 1 >= 0)
				{
					adjCellDown = CellAt(cell.X, cell.Y - 1);

					if (adjCellDown.Type.IsEmpty || (adjCellDown.Type.RoomExitCompatible && adjCellDown.Descr == Descriptions.Room_TBD))
					{
						okDown = true;
					}
				}
			}
			
			if (dir == Direction.UpLeft || dir == Direction.DownLeft)
			{
				if (cell.X - 1 >= 0)
				{
					adjCellLeft = CellAt(cell.X - 1, cell.Y);
					if (adjCellLeft.Type.IsEmpty || (adjCellLeft.Type.RoomExitCompatible && adjCellLeft.Descr == Descriptions.Room_TBD))
					{
						okLeft = true;
					}
				}
			}
			
			if (dir == Direction.UpRight || dir == Direction.DownRight)
			{
				if (cell.X + 1 < Constants.GridWidth)
				{
					adjCellRight = CellAt(cell.X + 1, cell.Y);
					if (adjCellRight.Type.IsEmpty || (adjCellRight.Type.RoomExitCompatible && adjCellRight.Descr == Descriptions.Room_TBD))
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
			Type newType;
			Cell adjacent = CellAt(adjX, adjY);
			
			// No point in trying to force connect two special rooms, they won't join anyways.
			if (cell.Descr.IsMines && adjacent.Descr.IsMines)
			{
				throw new DungeonBuildException();
			}
			
			if (adjacent.Type.IsEmpty || cell.Type.ConnectsTo(adjacent.Type, dir))
			{
				return;
			}
			else if (adjacent.Type.RoomExitCompatible)
			{
				newType = Types.ConvRoomWallToExit(adjacent.Type, OppositeDir(dir));
				newCell = new Cell(adjX, adjY, newType, adjacent.Descr);
				SetCellValue(adjX, adjY, newCell);
				AddNewCellToRoom(adjacent, newCell);
			}
			else
			{
				newType = Types.ConvRoomExitToWall(cell.Type, dir, cell.Descr);
				newCell = new Cell(cell.X, cell.Y, newType, cell.Descr);
				
				if (newCell.Descr.IsMines) 
				{
					ReplaceDungeonCellValue(cell.X, cell.Y, newCell);
				}
				else
				{
					SetDungeonCellValue(cell.X, cell.Y, newCell);
					AddNewCellToRoom(cell, newCell);
				}
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
			foreach (Room room in this.Rooms) 
			{
				if (room.Walls.Contains(existingCell))
				{
					room.Walls.Add(newCell);
					return;
				}
			}
		}

		private void ConnectMines()
		{
			for (int y = 0; y < Constants.GridHeight; y++)
			{
				for (int x = 0; x < Constants.GridWidth; x++)
				{
					Cell cell = CellAt(x, y);
					
					if (cell.Type == Types.Inter)
					{
						Cell cellUp, cellDown, cellLeft, cellRight;
							
						cellUp    = y + 1 < Constants.GridHeight ? CellAt(x, y + 1) : null;
						cellDown  = y - 1 >= 0                   ? CellAt(x, y - 1) : null;
						cellLeft  = x - 1 >= 0                   ? CellAt(x - 1, y) : null;
						cellRight = x + 1 < Constants.GridWidth  ? CellAt(x + 1, y) : null;
						
						if (!cellUp.Type.IsEmpty && !cell.Type.ConnectsTo(cellUp.Type, Direction.Up))
						{
							ConnectRoomCells(cellUp, x, y + 1, Direction.Up);
						}
						else if (!cellDown.Type.IsEmpty && !cell.Type.ConnectsTo(cellDown.Type, Direction.Down))
						{
							ConnectRoomCells(cellDown, x, y - 1, Direction.Down);
						}
						else if (!cellLeft.Type.IsEmpty && !cell.Type.ConnectsTo(cellLeft.Type, Direction.Left))
						{
							ConnectRoomCells(cellLeft, x - 1, y, Direction.Left);
						}
						else if (!cellRight.Type.IsEmpty && !cell.Type.ConnectsTo(cellRight.Type, Direction.Right))
						{
							ConnectRoomCells(cellRight, x + 1, y, Direction.Right);
						}
					}
				}
			}
		}

		private void ConvertRoomsToCatacombs()
		{
			if (this.CatacombsCount == 0)
			{
				return;
			}
			
			List<Room> rooms = CloneRoomsList();
			
			int added = 0;
			
			do 
			{
				if (rooms.Count > 0)
				{
					Room room = rooms[this.R.Next(rooms.Count)];
					rooms.Remove(room);
					
					int volume = room.Walls.Count + room.Space.Count;
					
					if (room.Description == Descriptions.Room_TBD && volume >= Constants.MinCatacombsVolume) 
					{
						ConvertRoomToCatacombs(room);
						added++;
					}
				}
				else
				{
					return;
				}
				
			} while (added < this.CatacombsCount);
		}

		// Make a deep copy clone of the Rooms list.
		private List<Room> CloneRoomsList()
		{
			List<Room> rooms = null;
			
			if (this.Rooms != null)
			{
				rooms = new List<Room>();

				foreach (Room room in this.Rooms)
				{
					rooms.Add(new Room(room));
				}
			}

			return rooms;
		}

		private void ConvertRoomToCatacombs(Room room)
		{
			foreach (Cell cell in room.Walls) 
			{
				Type newType = Types.ConvRoomTypeToCatacomb(cell.Type);
				Cell newCell = new Cell(cell.X, cell.Y, newType, Descriptions.Catacombs_TBD);
				newCell.IsCatacombs = true;
				ReplaceDungeonCellValue(cell.X, cell.Y, newCell);
			}
			
			foreach (Cell cell in room.Space) 
			{
				Type newType = Types.ConvRoomTypeToCatacomb(cell.Type);
				Cell newCell =  new Cell(cell.X, cell.Y, newType, Descriptions.Catacombs_TBD);
				newCell.IsCatacombs = true;
				ReplaceDungeonCellValue(cell.X, cell.Y, newCell);
			}
		}

		private void DeleteRoom(Room room)
		{
			foreach (Cell wall in room.Walls) 
			{
				int i = (Constants.GridWidth * wall.X) + wall.Y;
				this.Grid[i] = this.EmptyCell;
			}
		}

		#endregion
		#region Doors and Keys
			
		// Put in some random doors. Only certain cell descriptions "allow" doors. 
		// Also, doors should not "cluster" up too much.
		private void PlaceDoors()
		{
			Cell cell;
			
			for (int x = 0; x < Constants.GridWidth; x++)
			{
				for (int y = 0; y < Constants.GridHeight; y++) 
				{
					cell = CellAt(x, y);
					
					if (!cell.Descr.IsMines && !(cell.Descr == Descriptions.Catacombs_TBD) && !SuppressDoor(cell)) 
					{
						cell.Doors = RandomDoorSetup(cell);
					}
				}
			}
		}


		// Don't allow two doors to appear side-by-side if the door is to be placed in a corridor.
		// Also, don't allow a door to appear in the viscinity of the start cell, because it could
		// end up being a locked door with no key before it in the dungeon level...
		private bool SuppressDoor(Cell cell)
		{
			// Maintain a 2-cell margin around the start cell.
			if (cell.Y > this.StartCoords.Y - 2 && cell.Y < this.StartCoords.Y + 2 && 
				cell.X > this.StartCoords.X - 2 && cell.X < this.StartCoords.X + 2) 
			{
				return true;
			}

			// Cell above.
			if (cell.Y + 1 < Constants.GridHeight)
			{
				Cell cellAbove = CellAt(cell.X, cell.Y + 1);
				
				if (cellAbove.Doors != null && cellAbove.Doors.Count > 0)
				{
					return true;
				}
			}
			
			// Cell below.
			if (cell.Y - 1 >= 0)
			{
				Cell cellBelow = CellAt(cell.X, cell.Y - 1);
				
				if (cellBelow.Doors != null && cellBelow.Doors.Count > 0)
				{
					return true;
				}
			}
			
			// Cell left.
			if (cell.X - 1 >= 0)
			{
				Cell cellLeft = CellAt(cell.X - 1, cell.Y);
				
				if (cellLeft.Doors != null && cellLeft.Doors.Count > 0)
				{
					return true;
				}
			}
			
			// Cell right.
			if (cell.X + 1 < Constants.GridWidth)
			{
				Cell cellRight = CellAt(cell.X + 1, cell.Y);
				
				if (cellRight.Doors != null && cellRight.Doors.Count > 0)
				{
					return true;
				}
			}
			
			return false;  // Made it this far...
		}


		private List<Door> RandomDoorSetup(Cell cell)
		{    
			List<Door> doors = null;
			
			if (cell.Type.ConnectsUp)
			{
				Door door = RandomDoor(cell, Direction.Up);
				
				if (door != null)
				{
					if (doors == null)
					{
						doors = new List<Door>();
					}

					doors.Add(door);
				}
			}
			
			if (cell.Type.ConnectsDown)
			{
				Door door = RandomDoor(cell, Direction.Down);
				
				if (door != null)
				{
					if (doors == null)
					{
						doors = new List<Door>();
					}
					
					doors.Add(door);
				}
			}
			
			if (cell.Type.ConnectsLeft)
			{
				Door door = RandomDoor(cell, Direction.Left);
				
				if (door != null)
				{
					if (doors == null)
					{
						doors = new List<Door>();
					}
					
					doors.Add(door);
				}
			}
			
			if (cell.Type.ConnectsRight)
			{
				Door door = RandomDoor(cell, Direction.Right);
				
				if (door != null)
				{
					if (doors == null)
					{
						doors = new List<Door>();
					}
					
					doors.Add(door);
				}
			}
			
			return doors;
		}

		private Door RandomDoor(Cell cell, Direction dir)
		{    
			if (RandomPercent() >= cell.Type.DoorProb)
			{
				return null;
			}
			
			DoorType type;
			bool open = false, locked = false;
			
			int random = RandomPercent();
			
			if (random >= 60 && random < 75)
			{
				type = DoorType.Portcullis;
			}
			else if (random >= 75)
			{
				type = DoorType.SecretDoor;
			}
			else
			{
				type = DoorType.RegularDoor;
			}
			
			if (type == DoorType.RegularDoor || type == DoorType.Portcullis)
			{
				locked = RandomPercent() < Constants.DoorLockedProb;
				
				if (locked)
				{
					this.CellsWithLockedDoors.Add(cell);
				}
				else
				{
					open = RandomPercent() < Constants.DoorOpenProb;
				}
			}
			
			return new Door(dir, open, locked, type);
		}

		private void PlaceKeys()
		{
			Cell keyCell;
			List<Cell> potentials;

			this.CellsWithLockedDoors = this.CellsWithLockedDoors.OrderBy(cell => cell.Sequence).ToList();
			
			foreach (Cell cell in this.CellsWithLockedDoors) 
			{
				foreach (Door door in cell.Doors) 
				{
					potentials = KeyLocationPotentials(cell.Sequence);
					keyCell = potentials[this.R.Next(potentials.Count)];  // Pick a key cell at random.
					keyCell.HasKey = true;
				}
			}
		}

		private List<Cell> KeyLocationPotentials(int endSequence)
		{
			Cell cell;
			List<Cell> potentials = new List<Cell>();
			
			for (int x = 0; x < Constants.GridWidth; x++)
			{
				for (int y = 0; y < Constants.GridHeight; y++) 
				{
					cell = CellAt(x, y);
					
					if (cell.Sequence >= 0 && cell.Sequence < endSequence && cell.Doors == null)
					{
						potentials.Add(cell);
					}
				}
			}
			
			return potentials;
		}

		#endregion
		#region Accessors

		private Cell CellAt(int x, int y)
		{
			return this.Grid[Constants.GridWidth * x + y];
		}

		private void SetCellValue(int x, int y, Cell cell)
		{
			int i = Constants.GridWidth * x + y;
			this.Grid[i] = cell;
			RecordNewAttachment(cell);
		}

		private void SetDungeonCellValue(int x, int y, Cell cell)
		{
			int i = (Constants.GridWidth * x) + y;
			this.Grid[i] = cell;
			RecordNewAttachment(cell);
		}


		// This is intended to be used only for special rooms durring the rooms connect phase to 
		// avoid screwing up the available connections count of when converting room exits to walls.
		private void ReplaceDungeonCellValue(int x, int y, Cell cell)
		{
			int i = (Constants.GridWidth * x) + y;
			this.Grid[i] = cell;
		}

		// Returns a random cell from the dungeon level. If empty == true, then the random
		// cell will be empty. If empty == false, then the random cell will be occupied.
		private Coords RandomCell(bool empty)
		{
			Coords coords = null;
			
			while (coords == null) 
			{
				int x = this.R.Next(Constants.GridWidth);
				int y = this.R.Next(Constants.GridHeight);
				
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
			
			Type newType = null;
			Description descr = null;
			List<Cell> cells = ForceGrowthCells();
			
			while (!success && cells.Count > 0) 
			{
				Cell cell = RandomForceGrowthCell(cells);
				Coords coords = new Coords(cell.X, cell.Y);
				
				// Attempt to replace it from the standard types.
				List<Type> types = Types.GetTypes(coords);
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
						// It is? Cool.
						descr = cell.Descr;
						typeMatch = true;
					}
				}
				
				if (typeMatch) 
				{
					// Now set the new cell.
					Cell newCell = new Cell(coords.X, coords.Y, newType, descr);
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
			
			foreach (Cell cell in this.Grid)
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
		// placing keys in the level prior to corresponding locked doors, and positioning the down stairs at 
		// a suitable location.
		private void LevelSolve()
		{
			this.DownStairsCell = null;
			this.SequenceNumber = 0;
			this.DownStairsCellDistance = 0;
			
			Solve(CellAt(this.StartCoords.X, this.StartCoords.Y));
			
			for (int y = 0; y < Constants.GridHeight; y++) 
			{
				for (int x = 0; x < Constants.GridWidth; x++)
				{
					if (!CellAt(x, y).Visited)
					{
						throw new DungeonBuildException();
					}
				}
			}
		}

		// Recursive solve algorithm.
		private void Solve(Cell cell)
		{
			this.SequenceNumber++;

			cell.Visited = true;
			cell.Sequence = this.SequenceNumber;
			
			// Cell above.
			if (cell.Type.TraversableUp && cell.Y + 1 < Constants.GridHeight)
			{
				Cell cellAbove = CellAt(cell.X, cell.Y + 1);
				
				if (!cellAbove.Visited)
				{
					Solve(cellAbove);
				}
			}

			// Cell below.
			if (cell.Type.TraversableDown && cell.Y - 1 >= 0)
			{
				Cell cellBelow = CellAt(cell.X, cell.Y - 1);
				
				if (!cellBelow.Visited)
				{
					Solve(cellBelow);
				}
			}
			
			// Cell left.
			if (cell.Type.TraversableLeft && cell.X - 1 >= 0)
			{
				Cell cellLeft = CellAt(cell.X - 1, cell.Y);
				
				if (!cellLeft.Visited)
				{
					Solve(cellLeft);
				}
			}
			
			// Cell right.
			if (cell.Type.TraversableRight && cell.X + 1 < Constants.GridWidth)
			{
				Cell cellRight = CellAt(cell.X + 1, cell.Y);
				
				if (!cellRight.Visited)
				{
					Solve(cellRight);
				}
			}

			if (Types.IsDeadEnd(cell.Type)) 
        	{
				CheckForDownStairsPlacement(cell);
			}
		}

		private void CheckForDownStairsPlacement(Cell cell)
		{
			double distance = DistanceFromStartCell(cell);
			
			if (distance >= this.DownStairsCellDistance) 
			{
				int placementChance = (int)Math.Round((distance * 100) / this.MaxDistance);
				int random = this.R.Next(100 + 1);
				
				if (random < placementChance && this.DownStairsCell != null)
				{
					return;
				}
				
				this.DownStairsCell = cell;
				this.DownStairsCellDistance = distance;
			}
		}

		private double DistanceFromStartCell(Cell cell)
		{
			// distance = SQRT[(x2 - x1)^2 + (y2 - y1)^2]
			return Math.Abs(Math.Sqrt(Math.Pow(cell.X - this.StartCoords.X, 2) + Math.Pow(cell.Y - this.StartCoords.Y, 2)));
		}

		#endregion
		#region Descriptions

		private void AddDescriptions()
		{ 
			bool forceChange = false;
			
			do 
			{
				bool changed = false;
				
				for (int Y = 0; Y < Constants.GridHeight; Y++)
				{
					for (int X = 0; X < Constants.GridWidth; X++)
					{
						Cell cell = CellAt(X, Y);
						
						if (cell.Descr == null || cell.Descr.IsTBD)
						{
							int dominantWeight = -1;
							Description dominantDescr = Descriptions.Empty;  // Initialize to empty to satisfy compiler.
							DominantDescr(cell, ref dominantWeight, ref dominantDescr);
							
							if (dominantWeight == -1 && !forceChange)
							{
								continue;
							}
							else
							{
								int weight = 0, randomPercent = RandomPercent();
								Description descr;
								
								if (randomPercent >= dominantWeight || forceChange)
								{
									descr = RandomCellDescr(dominantDescr, cell.Type);
									weight = 100;
								}
								else
								{
									descr = dominantDescr;
									
									if (weight > 0)
									{
										weight = dominantWeight -= dominantDescr.WeightReduction;
									}
								}
								
								// Update descr for either entire room, or individual cell.
								if (Types.IsRoomType(cell.Type))
								{
									UpdateRoomDescr(cell, descr, weight);
								}
								else
								{
									cell.Descr = descr;
									cell.DescrWeight = weight;
								}
								
								changed = true;
							}
						}
					}
				}
				
				if (!changed)
				{
					forceChange = true;
				}
				else
				{
					forceChange = false;
				}
				
			} while (!DescriptionsComplete());
			
			CompleteFlooding();
		}


		private bool DescriptionsComplete()
		{
			bool complete = true;
			
			for (int y = 0; y < Constants.GridHeight; y++)
			{
				for (int x = 0; x < Constants.GridWidth; x++)
				{
					Cell cell = CellAt(x, y);
					
					if (cell.Descr.IsTBD) 
					{
						complete = false;
						break;
					}
				}
			}
			
			return complete;
		}

		private void DominantDescr(Cell cell, ref int weight, ref Description descr)
		{    
			Cell adjacentCell;
			List<Cell> cells = new List<Cell>();
			
			// Cell above.
			if (cell.Type.TraversableUp && cell.Y + 1 < Constants.GridHeight)
			{
				adjacentCell = CellAt(cell.X, cell.Y + 1);
				
				if (adjacentCell.Descr != null && !adjacentCell.Descr.IsTBD) 
				{
					cells.Add(adjacentCell);
				}
			}
			
			// Cell below.
			if (cell.Type.TraversableDown && cell.Y - 1 >= 0)
			{
				adjacentCell = CellAt(cell.X, cell.Y - 1);
				
				if (adjacentCell.Descr != null && !adjacentCell.Descr.IsTBD) 
				{
					cells.Add(adjacentCell);
				}
			}
			
			// Cell left.
			if (cell.Type.TraversableLeft && cell.X - 1 >= 0)
			{
				adjacentCell = CellAt(cell.X - 1, cell.Y);
				
				if (adjacentCell.Descr != null && !adjacentCell.Descr.IsTBD) 
				{
					cells.Add(adjacentCell);
				}
			}
			
			// Cell right.
			if (cell.Type.TraversableRight && cell.X + 1 < Constants.GridWidth)
			{
				adjacentCell = CellAt(cell.X + 1, cell.Y);
				
				if (adjacentCell.Descr != null && !adjacentCell.Descr.IsTBD) 
				{
					cells.Add(adjacentCell);
				}
			}
			
			if (cells.Count == 0)
			{
				return;
			}

			cells = cells.OrderByDescending(d => d.DescrWeight).ToList();
			
			int i = 0;
			
			if (cells[i].DescrWeight == 0)
			{
				// All weights are 0. Choose a key at random.
				i = this.R.Next(cells.Count);
			}

			weight = cells[i].DescrWeight;
			descr = cells[i].Descr;
		}

		// Incoming cell, locate room, and update all room cells with provided descr and weight.
		private void UpdateRoomDescr(Cell cell, Description descr, int weight)
		{
			Room updateRoom = null;
			
			foreach (Room room in this.Rooms) 
			{
				if (room.Walls.Contains(cell))
				{
					updateRoom = room;
					break;
				}
				else if (room.Space.Contains(cell))
				{
					updateRoom = room;
					break;
				}
			}
			
			foreach (Cell updateCell in updateRoom.Walls) 
			{
				updateCell.Descr = descr;
				updateCell.DescrWeight = weight;
			}
			
			foreach (Cell updateCell in updateRoom.Space) 
			{
				updateCell.Descr = descr;
				updateCell.DescrWeight = weight;
			}
		}

		private Description RandomCellDescr(Description previousDescr, Type cellType)
		{
			// DungeonCellDescr objects have weights, so some are more likely to be picked than others.
			
			List<Description> descrs = new List<Description>();

			foreach (Description descr in Descriptions.Descrs)
			{
				descrs.Add(descr);  // Make a shallow copy clone of the descriptions.
			}

			descrs.Remove(previousDescr);
			
			// Flooding transitions can only occur with certain cell types.
			
			if (!Types.IsFloodingTransition(cellType))
			{
				descrs.Remove(Descriptions.Constructed_Flooded);
				descrs.Remove(Descriptions.Cavern_Flooded);
			}
			
			int total = 0;
			
			foreach (Description descr in descrs) 
			{
				total += descr.Weight;
			}
			
			int threshold = this.R.Next(total);
			
			Description randomDescr = null;
			
			foreach (Description descr in descrs) 
			{
				randomDescr = descr;
				threshold -= descr.Weight;
				
				if (threshold < 0)
				{
					break;
				}
			}
			
			return randomDescr;
		}

		private void CompleteFlooding()
		{
			bool changed = false;
			
			do 
			{
				changed = false;
			
				for (int y = 0; y < Constants.GridHeight; y++)
				{
					for (int x = 0; x < Constants.GridWidth; x++)
					{
						Cell cell = CellAt(x, y);
						
						if (cell.Descr.IsFlooded && !Types.IsFloodingTransition(cell.Type))
						{
							// Cell above.
							if (cell.Type.TraversableUp && cell.Y + 1 < Constants.GridHeight)
								changed = FloodCell(x, y + 1) || changed;
							
							// Cell below.
							if (cell.Type.TraversableDown && cell.Y - 1 >= 0)
								changed = FloodCell(x, y - 1) || changed;
							
							// Cell left.
							if (cell.Type.TraversableLeft && cell.X - 1 >= 0)
								changed = FloodCell(x - 1, y) || changed;
							
							// Cell right.
							if (cell.Type.TraversableRight && cell.X + 1 < Constants.GridWidth)
								changed = FloodCell(x + 1, y) || changed;
						}
					}
				}
				
			} while (changed);
			
			RemoveMiniFloods();
		}

		private bool FloodCell(int X, int Y)
		{
			bool changed = false;
			Cell cell = CellAt(X, Y);
			
			if (!cell.Descr.IsFlooded)  // If the cell is not already flooded, then flood it.
			{
				if (Types.IsFloodingIncompatible(cell.Type) || cell.Type == Types.Entrance)
				{
					throw new DungeonBuildException();  // Except when the cell cannot be flooded.
				}
				else
				{
					Description descr = Descriptions.Empty;  // Initialize with empty to satisfy the compiler.
					
					if (cell.Descr == Descriptions.Constructed)
					{
						descr = Descriptions.Constructed_Flooded;
					}
					else if (cell.Descr == Descriptions.Cavern)
					{
						descr = Descriptions.Cavern_Flooded;
					}
					else if (cell.Descr == Descriptions.Mines_Horiz)
					{
						descr = Descriptions.Mines_Horiz_Flooded;
					}
					else if (cell.Descr == Descriptions.Mines_Vert)
					{
						descr = Descriptions.Mines_Vert_Flooded;
					}
					
					// Update descr for either entire room, or individual cell.
					if (Types.IsRoomType(cell.Type))
					{
						UpdateRoomDescr(cell, descr, 0);
					}
					else
					{
						cell.Descr = descr;
						cell.DescrWeight = 0;
					}
					
					changed = true;
				}
			}
			
			return changed;
		}

		// Floods must be at least two cells wide.
		private void RemoveMiniFloods()
		{
			for (int y = 0; y < Constants.GridHeight; y++)
			{
				for (int x = 0; x < Constants.GridWidth; x++)
				{
					Cell adjacentCell, cell = CellAt(x, y);
					
					bool adjacentFlooded = false;
					
					if (cell.Descr.IsFlooded)
					{
						// Cell above.
						if (cell.Type.TraversableUp && cell.Y + 1 < Constants.GridHeight)
						{
							adjacentCell = CellAt(x, y + 1);
							adjacentFlooded = adjacentCell.Descr.IsFlooded || adjacentFlooded;
						}
						
						// Cell below.
						if (cell.Type.TraversableDown && cell.Y - 1 >= 0)
						{
							adjacentCell = CellAt(x, y - 1);
							adjacentFlooded = adjacentCell.Descr.IsFlooded || adjacentFlooded;
						}
						
						// Cell left.
						if (cell.Type.TraversableLeft && cell.X - 1 >= 0)
						{
							adjacentCell = CellAt(x - 1, y);
							adjacentFlooded = adjacentCell.Descr.IsFlooded || adjacentFlooded;
						}
						
						// Cell right.
						if (cell.Type.TraversableRight && cell.X + 1 < Constants.GridWidth)
						{
							adjacentCell = CellAt(x + 1, y);
							adjacentFlooded = adjacentCell.Descr.IsFlooded || adjacentFlooded;
						}
						
						if (!adjacentFlooded)
						{
							Description descr = Descriptions.Empty;  // Initialize with empty to satisfy the compiler.
							
							if (cell.Descr == Descriptions.Constructed_Flooded)
							{
								descr = Descriptions.Constructed;
							}
							else if (cell.Descr == Descriptions.Cavern_Flooded)
							{
								descr = Descriptions.Cavern;
							}
							else if (cell.Descr == Descriptions.Mines_Horiz_Flooded)
							{
								descr = Descriptions.Mines_Horiz;
							}
							else if (cell.Descr == Descriptions.Mines_Vert_Flooded)
							{
								descr = Descriptions.Mines_Vert;
							}
							
							cell.Descr = descr;
						}
					}
				}
			}
		}

		#endregion
		#region Utility

		// Returns a random number 0 >= x < 100, representing percent.
		private int RandomPercent()
		{
			return this.R.Next(100);
		}

		private bool RandomBool()
		{
			return this.R.Next(2) == 0 ? false : true;
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

					if (x % 2 == 0)
					{  
						if (cell.Doors != null)
						{
							if (cell.Doors.Count == 2)
							{
								line.Append("2");
							}
							else
							{
								Door door = cell.Doors[0];
								
								if (door.Type == DoorType.RegularDoor && door.Locked == false && door.Open == false)
								{
									line.Append("d");
								}
								else if (door.Type == DoorType.RegularDoor && door.Locked == false && door.Open == true)
								{
									line.Append("o");
								}
								else if (door.Type == DoorType.RegularDoor && door.Locked == true)
								{
									line.Append("D");
								}
								else if (door.Type == DoorType.Portcullis && door.Locked == false && door.Open == false)
								{
									line.Append("p");
								}
								else if (door.Type == DoorType.Portcullis && door.Locked == false && door.Open == true)
								{
									line.Append("b");
								}
								else if (door.Type == DoorType.Portcullis && door.Locked == true)
								{
									line.Append("P");
								}
								else if (door.Type == DoorType.SecretDoor)
								{
									line.Append("s");
								}
							}
						}
						else if (cell.HasKey)
						{
								line.Append("K");
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

		public string VisualizeAsTextWithDescription()
		{
			string padding;
			StringBuilder grid = new StringBuilder();
			StringBuilder line = new StringBuilder();
			Cell cell = CellAt(0, Constants.GridHeight - 1);
			
			int x;
			
			// Because it is console printing, start with the "top" of the dungeon, and work down.
			for (int y = Constants.GridHeight - 1; y >= 0; y--) 
			{
				for (x = 0; x < Constants.GridWidth * 2; x++) 
				{
					if (x % 2 == 0)
					{
						cell = CellAt(x / 2, y);
						line.Append(cell.Descr.TextRep);
					}
					else
					{
						line.Append(cell.Descr.TextRep);
					}
				}
				
				padding = (y < 10) ? "0" : "";  // For co-ordinate printing.

				grid.AppendLine(padding + y + line.ToString());
							
				if (y >= 0)
				{
					line = new StringBuilder();
				}
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

		public string BuildStats()
		{
			return Environment.NewLine +
			       "Build Passes: " + this.BuildPasses.ToString() + Environment.NewLine +
				   "Build Time: " + this.BuildTime.ToString() + Environment.NewLine +
				   "Room Count: " + this.RoomsCount.ToString() + Environment.NewLine +
				   "Mines Count: " + this.MinesCount.ToString() + Environment.NewLine +
				   "Catacombs Count: " + this.CatacombsCount.ToString();
		}

		#endregion
	}

	public class DungeonBuildException : Exception
	{
		public DungeonBuildException() : base() {}
	}
}