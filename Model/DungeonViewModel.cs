namespace DigitalWizardry.Dungeon.Models
{
	// Each one of these is equivalent to a dungeon Cell.
	public class DungeonViewModelCell
	{
		public string D { get; set; }  // Door direction: "U", "D", "L", "R", or "" if no door in the cell.
		public string G { get; set; }  // Gem exists at this cell location, "1": gem, "": no gem.
		public string H { get; set; }  // Heart exists at this cell location, "1": heart, "": no heart.
		public string K { get; set; }  // Key exists at this cell location, "1": key, "": no key.
		public string L { get; set; }  // CssLocation
		public string N { get; set; }  // CssName
		public string X { get; set; }  // Goblin, "G": green, "B": black, "": none.
	}
}
