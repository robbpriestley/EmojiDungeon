namespace DigitalWizardry.Dungeon
{	
	public class Reference
	{
		public static readonly int GridWidth = 15;
		public static readonly int GridHeight = 15;
		// *** PROBS ***
		public static readonly int GemProb = 4;
		public static readonly int HeartProb = 1;
		// *** ROOMS ***
		public static readonly int MinRooms = 10;
		public static readonly int MaxRooms = 15;
		public static readonly int MinRoomWidth = 2;
		public static readonly int MinRoomHeight = 2;
		public static readonly int MaxRoomWidth = 4;
		public static readonly int MaxRoomHeight = 4;
		public static readonly int RoomExitProb = 20;
	}

	public enum Direction
	{
		Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight, NoDir
	}
	
	public enum RoomType
	{
    	Regular, IrregularUL, IrregularUR, IrregularDL, IrregularDR
	}
}