namespace DigitalWizardry.Dungeon.Models
{
	// Each one of these is equivalent to a dungeon Cell.
	public class DungeonViewModelCell
	{
		public string DoorDirection { get; set; }  // "U", "D", "L", "R", or "" if no door in the cell.
		public bool HasGem { get; set; }
		public bool HasHeart { get; set; }
		public bool HasKey { get; set; }
		public bool HasSword { get; set; }
		public bool HasGoblin { get; set; }
		public string CssLocation { get; set; } 
		public string CssName { get; set; }
		public bool TraversableUp { get; set; }
		public bool TraversableDown { get; set; }
		public bool TraversableLeft { get; set; }
		public bool TraversableRight { get; set; }
		public bool IsStairsUp { get; set; }
		public bool IsStairsDown { get; set; }
	}
}
