namespace DigitalWizardry.LevelGenerator
{	
	public class CellDoor
	{
		public Direction Dir;
		public bool Open;
		public bool Locked;
		public DoorType Type;

		public CellDoor(){}
		
		public CellDoor(Direction dir, bool open, bool locked, DoorType type)
		{
			Dir = dir;
			Open = open;
			Locked = locked;
			Type = type;
		}

		// Copy constructor. Creates a deep copy clone of the source.
		public CellDoor(CellDoor source) : this()
		{
			this.Dir = source.Dir;
			this.Open = source.Open;
			this.Locked = source.Locked;
			this.Type = source.Type;
		}
	}
}