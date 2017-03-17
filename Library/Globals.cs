using System.Collections.Generic;

namespace DigitalWizardry.LevelGenerator
{	
	public enum Direction
	{
		Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight, NoDir
	}
	public class Globals
	{	
		// The empty cell is re-used everywhere to save memory. It exists outside of "normal space" because its coords are -1,-1.
		public static Cell EmptyCell;

		public static void Initialize()
		{
			CellTypes.Initialize();
			EmptyCell = new Cell(-1, -1, CellTypes.emptyCell);
		}
	}
}