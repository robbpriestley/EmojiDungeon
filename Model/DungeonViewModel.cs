namespace DigitalWizardry.Dungeon.Models
{
	public class DungeonViewModel
	{
		public string[,] CssNames { get; set; }

		public DungeonViewModelCell[,] Cells { get; set; }
	}

	public class DungeonViewModelCell
	{
		public int X { get; set; }
		public int Y { get; set; }
		public string CssName { get; set; }
		public string CssLocation { get; set; }

		public DungeonViewModelCell(int x, int y)
		{
			X = x;
			Y = y;
		}
	}
}
