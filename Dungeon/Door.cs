namespace DigitalWizardry.Dungeon
{	
	public class Door
	{
		public Direction Dir { get; set; }
		public bool Open { get; set; }
		public bool Locked { get; set; }
		public DoorType Type { get; set; }

		public Door(){}
		
		public Door(Direction dir, bool open, bool locked, DoorType type)
		{
			Dir = dir;
			Open = open;
			Locked = locked;
			Type = type;
		}

		// Copy constructor. Creates a deep copy clone of the source.
		public Door(Door source) : this()
		{
			Dir = source.Dir;
			Open = source.Open;
			Locked = source.Locked;
			Type = source.Type;
		}
	}
}