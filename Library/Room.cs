using System.Collections.Generic;

namespace DigitalWizardry.Dungeon
{	
	public class Room
	{
		// Coordinates record the bottom-left corner of the room.
		public int X;  
		public int Y;
		public bool Round;
		public bool ExitImpossible;
		public CellDescription Descr;
		public List<Cell> Walls;
		public List<Cell> Space; 

		public Room(){}
		
		public Room(int x, int y, CellDescription descr)
		{
			X = x;
			Y = y;
			Descr = descr;
			Walls = new List<Cell>();
			Space = new List<Cell>();
		}

		// Copy constructor. Creates a deep copy clone of the source.
		public Room(Room source) : this()
		{
			this.X = source.X;
			this.Y = source.Y;
			this.Descr = source.Descr;
			
			if (source.Walls != null)
			{
				this.Walls = new List<Cell>();

				foreach (Cell cell in source.Walls)
				{
					this.Walls.Add(new Cell(cell));
				}
			}

			if (source.Space != null)
			{
				this.Space = new List<Cell>();

				foreach (Cell cell in source.Space)
				{
					this.Space.Add(new Cell(cell));
				}
			}
		}
	}
}