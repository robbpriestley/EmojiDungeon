namespace DigitalWizardry.LevelGenerator
{	
	public class CellDoor
	{
		public Direction Dir;
		public bool Open;
		public bool Locked;
		public DoorType Type;

		public CellDoor(Direction dir, bool open, bool locked, DoorType type)
		{
			Dir = dir;
			Open = open;
			Locked = locked;
			Type = type;
		}
	}
}