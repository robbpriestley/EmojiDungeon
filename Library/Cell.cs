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
		public bool IsCatacombs { get; set; }
		public bool ExitImpossible { get; set; }
		public Coords SourceCoords { get; set; }       // For use when solving shortest-path.
		public bool AttachBlocked { get; set; }        // There are frequent cases where the cell has an available connection point, but nothing can be attached there. In those cases, attachBlocked is set to true.
		public int AvailableConnections { get; set; }  // Records number of available connection points;
		public int DescrWeight { get; set; }           // Essentially a percentage, used to determine how "sticky" the description is.
		public CellType Type { get; set; }
		public Description Descr { get; set; }
		public List<Door> Doors { get; set; }
		
		public Cell(){}
		
		public Cell(int x, int y, CellType type, Description descr)
		{
				X = x;
				Y = y;
				Type = type;
				Descr = descr;
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
			IsCatacombs = source.IsCatacombs;
			ExitImpossible = source.ExitImpossible;
			SourceCoords = source.SourceCoords == null ? null : new Coords(source.SourceCoords);
			AttachBlocked = source.AttachBlocked;
			AvailableConnections = source.AvailableConnections;
			DescrWeight = source.DescrWeight;
			Type = source.Type;    // This does not require deep copy.
			Descr = source.Descr;  // This does not require deep copy.
			
			if (source.Doors != null)
			{
				Doors = new List<Door>();

				foreach (Door door in source.Doors)
				{
					Doors.Add(new Door(door));
				}
			}
		}

		public string Filepath
		{
			get
			{
				if (Descr.IsCavern)
				{
					return "images/tiles/dungeon/cavern/" + Type.Name + ".png";
				}
				else
				{
					return "images/tiles/dungeon/regular/" + Type.Name + ".png";
				}
			}
		}
	}
}