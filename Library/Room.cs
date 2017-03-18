using System.Collections.Generic;

namespace DigitalWizardry.LevelGenerator
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

		public Room(int x, int y, CellDescription descr)
		{
			X = x;
			Y = y;
			Descr = descr;
			Walls = new List<Cell>();
			Space = new List<Cell>();
		}
	}
}