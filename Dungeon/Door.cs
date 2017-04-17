namespace DigitalWizardry.Dungeon
{	
	public class Door
	{
		public Direction Dir { get; set; }

		public Door(){}
		
		public Door(Direction dir)
		{
			Dir = dir;
		}

		// Copy constructor. Creates a deep copy clone of the source.
		public Door(Door source) : this()
		{
			Dir = source.Dir;
		}
	}
}