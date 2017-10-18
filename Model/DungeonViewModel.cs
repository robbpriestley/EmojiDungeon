namespace DigitalWizardry.Dungeon.Models
{
	// Each one of these is equivalent to a dungeon Cell.
	public class DungeonViewModelCell
	{
		public string DoorDirection { get; set; }  // "U", "D", "L", "R", or "" if no door in the cell.
		public bool HasGem { get; set; }
		public bool HasHeart { get; set; }
		public bool HasKey { get; set; }
		public bool HasGoblin { get; set; }
		public string CssLocation { get; set; } 
		public string CssName { get; set; }
	}
}
