using System.Collections.Generic;

namespace DigitalWizardry.Dungeon
{	
	public class Cell
	{
		public int X { get; set; }
		public int Y { get; set; }
		public bool HasKey { get; set; }
		public int Sequence { get; set; }             // Sequence number used when solving the dungeon.
		public bool Merged { get; set; }              // Used for room merge to record cells that have already been merged.
		public bool Visited { get; set; }             // For use when traversing the dungeon.
		public bool ExitImpossible { get; set; }
		public Coords SourceCoords { get; set; }       // For use when solving shortest-path.
		public bool AttachBlocked { get; set; }        // There are frequent cases where the cell has an available connection point, but nothing can be attached there. In those cases, attachBlocked is set to true.
		public int AvailableConnections { get; set; }  // Records number of available connection points;
		public CellType Type { get; set; }
		public Door Door { get; set; }
		
		public Cell(){}
		
		public Cell(int x, int y, CellType type)
		{
				X = x;
				Y = y;
				Type = type;
				AvailableConnections = Type.InitialAvailableConnections;
		}

		// Copy constructor. Creates a deep copy clone of the source.
		public Cell(Cell source) : this()
		{
			X = source.X;
			Y = source.Y;
			HasKey = source.HasKey;
			Sequence = source.Sequence;
			Merged = source.Merged;
			Visited = source.Visited;
			ExitImpossible = source.ExitImpossible;
			SourceCoords = source.SourceCoords == null ? null : new Coords(source.SourceCoords);
			AttachBlocked = source.AttachBlocked;
			AvailableConnections = source.AvailableConnections;
			Type = source.Type;    // This does not require deep copy.
			
			if (source.Door != null)
			{
				Door = new Door(source.Door);
			}
		}

		public string CssName
		{
			get
			{
				return Type.Name;
			}
		}

		public string CssLocation
		{
			get
			{
				var xs = X < 10 ? "0" + X.ToString() : X.ToString();
				var ys = Y < 10 ? "0" + Y.ToString() : Y.ToString();
				return "g" + xs + ys;
			}
		}
	}
}