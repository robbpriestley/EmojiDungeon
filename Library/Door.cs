namespace DigitalWizardry.Dungeon
{	
	public class Door
	{
		public Direction Dir;
		public bool Open;
		public bool Locked;
		public DoorType Type;

		public Door(){}
		
		public Door(Direction dir, bool open, bool locked, DoorType type)
		{
			this.Dir = dir;
			this.Open = open;
			this.Locked = locked;
			this.Type = type;
		}

		// Copy constructor. Creates a deep copy clone of the source.
		public Door(Door source) : this()
		{
			this.Dir = source.Dir;
			this.Open = source.Open;
			this.Locked = source.Locked;
			this.Type = source.Type;
		}
	}
}