namespace DigitalWizardry.LevelGenerator
{	
	public class CellType
	{
		// These "connects" members only apply to corridor cells.
		public bool ConnectsUp;
		public bool ConnectsDown;
		public bool ConnectsLeft;
		public bool ConnectsRight;
		
		public bool TraversableUp;
		public bool TraversableDown;
		public bool TraversableLeft;
		public bool TraversableRight;
		
		public int Weight;  // Weights the random selection.
		
		public bool IsEmpty;
		public bool IsJunction;
		public bool ForceGrowthCompatible;  // Can be substituted for another cell to increase dungeon fill.
		
		public string TextRep;
		public string TextRep2;  // Used for expanding the text representation into the horizontal.
		
		public int InitialAvailableConnections;
		
		public string MapImageFile;
		public string MapImageFileAltUp;
		public string MapImageFileAltRight;

		public CellType()
		{
        	ForceGrowthCompatible = true;
		}

		public bool ConnectsTo(CellType otherCell, Direction direction)
		{    
			if (otherCell.IsEmpty)
			{
				return false;
			}
			else if (direction == Direction.Up && this.ConnectsUp && otherCell.ConnectsDown)
			{
				return true;
			}
			else if (direction == Direction.Down && this.ConnectsDown && otherCell.ConnectsUp)
			{
				return true;
			}
			else if (direction == Direction.Left && this.ConnectsLeft && otherCell.ConnectsRight)
			{
				return true;
			}
			else if (direction == Direction.Right && this.ConnectsRight && otherCell.ConnectsLeft)
			{
				return true;
			}
			
			return false;
		}

		public bool CompatibleWith(CellType otherCell, Direction direction)
		{
			/*
				Another cell is compatible with the current cell if
				a) it is empty, or
				b) in the same direction, either both cells connect, or both do not connect.
				   (In other words if in the same direction one connects but the other does
				   not, that's bad).
			*/
			
			// Also, two junctions cannot appear side-by-side.
			if (this.IsJunction && otherCell.IsJunction) 
			{
				return false;
			}
			
			if (otherCell.IsEmpty)
			{
				return true;
			}
			else if (direction == Direction.Up)
			{
				if ((this.ConnectsUp && otherCell.ConnectsDown) || 
					(!this.ConnectsUp && !otherCell.ConnectsDown))
				{
					return true;
				}
			}
			else if (direction == Direction.Down)
			{
				if ((this.ConnectsDown && otherCell.ConnectsUp) || 
					(!this.ConnectsDown && !otherCell.ConnectsUp))
				{
					return true;
				}
			}
			else if (direction == Direction.Left)
			{
				if ((this.ConnectsLeft && otherCell.ConnectsRight) || 
					(!this.ConnectsLeft && !otherCell.ConnectsRight))
				{
					return true;
				}
			}
			else if (direction == Direction.Right)
			{
				if ((this.ConnectsRight && otherCell.ConnectsLeft) || 
					(!this.ConnectsRight && !otherCell.ConnectsLeft))
				{
					return true;
				}
			}

			return false;
		}
	}
}