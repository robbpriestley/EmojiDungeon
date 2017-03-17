using System.Collections.Generic;

namespace DigitalWizardry.LevelGenerator
{	
	public class Cell
	{
		public int X;
		public int Y;
		public bool HasKey;
		public int Sequence;         // Sequence number used when solving the dungeon.
		public bool Merged;          // Used for room merge to record cells that have already been merged.
		public bool Visited;         // For use when traversing the dungeon.
		public bool IsCatacombs;
		public bool ExitImpossible;
		public Coords SourceCoords;  // For use when solving shortest-path.
		public CellType Type;
		public CellDescription Descr;
		public List<CellDoor> Doors;
		
		// There are frequent cases where the cell has an available connection point, but  
		// nothing can be attached there. In those cases, attachBlocked is set to YES.
		public bool AttachBlocked;
		public int AvailableConnections;  // Records number of available connection points;
		public int DescrWeight;           // Essentially a percentage, used to determine how "sticky" the description is.

		public Cell(int x, int y, CellType type, CellDescription descr)
		{
				X = x;
				Y = y;
				Type = type;
				Descr = descr;
				AvailableConnections = Type.InitialAvailableConnections;
		}
	}
}