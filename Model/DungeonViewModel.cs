namespace DigitalWizardry.Dungeon.Models
{
	public class DungeonViewModel
	{
		// public int X { get; set; }
		// public int Y { get; set; }
		public string N { get; set; }  // CssName
		public string L { get; set; }  // CssLocation

		public DungeonViewModel(int x, int y)
		{
			X = x;
			Y = y;
		}
	}
}
