namespace DigitalWizardry.LevelGenerator
{	
	public class Coords
	{	
		public int X;
		public int Y;
		public bool AdjacentEdgeUp;
		public bool AdjacentEdgeDown;
		public bool AdjacentEdgeLeft;
		public bool AdjacentEdgeRight;

		public Direction Direction;  // Sneaky, it has a dir for utility purposes sometimes :-)

		public Coords(int x, int y)
		{
			X = x;
			Y = y;
			Direction = Direction.NoDir;

			if (X == 0) 
			{
				AdjacentEdgeLeft = true;
			} 
			
			if (Y == 0) 
			{
				AdjacentEdgeDown = true;
			}
			
			if (X + 1 == Constants.GridWidth) 
			{
				AdjacentEdgeRight = true;
			}
			
			if (Y + 1 == Constants.GridHeight) 
			{
				AdjacentEdgeUp = true;
			}
		}

		bool IsOppositeDirTo(Coords otherCoords)
		{
			return
			
			(Direction == Direction.Up && otherCoords.Direction == Direction.Down) ||
			(Direction == Direction.Down && otherCoords.Direction == Direction.Up) ||
			(Direction == Direction.Left && otherCoords.Direction == Direction.Right) ||
			(Direction == Direction.Right && otherCoords.Direction == Direction.Left) ||
			
			(Direction == Direction.UpLeft && otherCoords.Direction == Direction.DownRight) ||
			(Direction == Direction.DownLeft && otherCoords.Direction == Direction.UpRight) ||
			(Direction == Direction.UpRight && otherCoords.Direction == Direction.DownLeft) ||
			(Direction == Direction.DownRight && otherCoords.Direction == Direction.DownLeft);
		}
	}
}