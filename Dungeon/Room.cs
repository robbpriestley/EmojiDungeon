using System.Collections.Generic;

namespace DigitalWizardry.Dungeon
{	
	public class Room
	{
		// Coordinates record the bottom-left corner of the room.
		public int X { get; set; } 
		public int Y { get; set; }
		public List<Cell> Walls { get; set; }
		public List<Cell> Space { get; set; }

		public Room(){}
		
		public Room(int x, int y)
		{
			X = x;
			Y = y;
			Walls = new List<Cell>();
			Space = new List<Cell>();
		}

		// Copy constructor. Creates a deep copy clone of the source.
		public Room(Room source) : this()
		{
			X = source.X;
			Y = source.Y;
			
			if (source.Walls != null)
			{
				Walls = new List<Cell>();

				foreach (Cell cell in source.Walls)
				{
					Walls.Add(new Cell(cell));
				}
			}

			if (source.Space != null)
			{
				Space = new List<Cell>();

				foreach (Cell cell in source.Space)
				{
					Space.Add(new Cell(cell));
				}
			}
		}
	}
}