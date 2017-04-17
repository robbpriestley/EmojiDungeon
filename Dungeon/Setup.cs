namespace DigitalWizardry.Dungeon
{	
	public class Reference
	{
		public static readonly int GridWidth = 15;
		public static readonly int GridHeight = 15;
		public static readonly int MinRooms = 10;
		public static readonly int MaxRooms = 15;
		public static readonly int MinRoomWidth = 2;
		public static readonly int MinRoomHeight = 2;
		public static readonly int MaxRoomWidth = 4;
		public static readonly int MaxRoomHeight = 4;
		public static readonly int MinMinesWidth = 3;
		public static readonly int MinMinesHeight = 6;
		public static readonly int MaxMinesWidth = 4;
		public static readonly int MaxMinesHeight = 8;
		public static readonly int MinCatacombsVolume = 12;
		public static readonly int RoomExitProb = 20;
		public static readonly int DoorLockedProb = 35;
		public static readonly int DoorOpenProb = 20;
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
    	Regular, IrregularUL, IrregularUR, IrregularDL, IrregularDR
	}
}