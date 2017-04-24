namespace DigitalWizardry.Dungeon.Models
{
	// Each one of these is equivalent to a dungeon Cell.
	public class DungeonViewModelCell
	{
		public string D { get; set; }  // Door direction: "U", "D", "L", "R", or "" if no door in the cell.
		public string K { get; set; }  // Key exists at this cell location, "K": key, "": no key.
		public string N { get; set; }  // CssName
		public string L { get; set; }  // CssLocation
	}
}
