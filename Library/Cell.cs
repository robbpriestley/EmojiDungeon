namespace DigitalWizardry.LevelGenerator
{	
	public class Cell
	{
		public int X;
		public int Y;
		public int Sequence;  // Sequence number used when solving the dungeon.

		public bool Visited;  // For use when traversing the dungeon.
		public Coords SourceCoords;  // For use when solving shortest-path.
		
		public CellType Type;
		
		// There are frequent cases where the cell has an available connection point, but  
		// nothing can be attached there. In those cases, attachBlocked is set to YES.
		public bool AttachBlocked;
		
		public int AvailableConnections;  // Records number of available connection points;

		public Cell(int Xin, int Yin, CellType TypeIn)
		{
				X = Xin;
				Y = Yin;
				Type = TypeIn;
				AvailableConnections = Type.InitialAvailableConnections;
		}
	}
}