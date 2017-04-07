namespace DigitalWizardry.Dungeon.Models
{
	public class DungeonViewModel
	{
		public int X { get; set; }
		public int Y { get; set; }
		public string CssName { get; set; }
		public string CssLocation { get; set; }

		public DungeonViewModel(int x, int y)
		{
			X = x;
			Y = y;
		}
	}
}
