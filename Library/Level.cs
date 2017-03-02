using System;
using System.Collections.Generic;

namespace DigitalWizardry.LevelGenerator
{	
	public class Level
	{
		private List<Object> Grid;

		public Level(int width, int height)
		{
			Globals.GridWidth = width;
			Globals.GridHeight = height;
			
			// Create "2-D" grid array, it's actually just a "smart indexed" 1-D list.
			Grid = new List<Object>(width * height);
		}
	}
}