namespace DigitalWizardry.Dungeon
{	
	public class Constants
	{
		public const int GridWidth = 15;
		public const int GridHeight = 15;
		public const int MinRooms = 10;
		public const int MaxRooms = 15;
		public const int MinRoomWidth = 2;
		public const int MinRoomHeight = 2;
		public const int MaxRoomWidth = 4;
		public const int MaxRoomHeight = 4;
		public const int MinMinesWidth = 3;
		public const int MinMinesHeight = 6;
		public const int MaxMinesWidth = 4;
		public const int MaxMinesHeight = 8;
		public const int MinCatacombsVolume = 12;
		public const bool PlaceRoundRoom = false;
		public const int RoomExitProb = 20;
		public const int DoorLockedProb = 35;
		public const int DoorOpenProb = 20;
	}

	public enum Direction
	{
		Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight, NoDir
	}
	
	public enum DoorType
	{
    	RegularDoor, Portcullis, SecretDoor
	}

	public enum RoomType
	{
    	Round, Regular, IrregularUL, IrregularUR, IrregularDL, IrregularDR
	}
}