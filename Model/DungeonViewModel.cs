namespace DigitalWizardry.Dungeon.Models
{
	// Each one of these is equivalent to a dungeon Cell.
	public class DungeonViewModelCell
	{
		public string DoorDirection { get; set; }  // "U", "D", "L", "R", or "" if no door in the cell.
		public bool Gem { get; set; }     // Gem exists at this cell location.
		public bool Heart { get; set; }   // Heart exists at this cell location.
		public bool Key { get; set; }     // Key exists at this cell location.
		public bool Goblin { get; set; }  // Goblin exists at this cell location.
		public string CssLocation { get; set; } 
		public string CssName { get; set; }
	}
}
