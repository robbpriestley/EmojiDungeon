namespace DigitalWizardry.LevelGenerator
{	
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

	public class Globals
	{	
		// The empty cell is re-used everywhere to save memory. It exists outside of "normal space" because its coords are -1,-1.
		public static Cell EmptyCell;

		public static void Initialize()
		{
			CellTypes.Initialize();
			CellDescriptions.Initialize();
			EmptyCell = new Cell(-1, -1, CellTypes.emptyCell, CellDescriptions.Empty);
		}
	}
}