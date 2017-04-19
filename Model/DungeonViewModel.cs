namespace DigitalWizardry.Dungeon.Models
{
	// Each one of these is equivalent to a dungeon Cell.
	public class DungeonViewModelCell
	{
		public string D { get; set; }  // Door direction list, ex: U: door up, UR: doors up and right. Or, null if no doors.
		public string K { get; set; }  // Key exists at this cell location, 1: true, 0: false.
		public string N { get; set; }  // CssName
		public string L { get; set; }  // CssLocation
	}
}
