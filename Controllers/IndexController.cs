using Microsoft.AspNetCore.Mvc;

namespace DigitalWizardry.Dungeon.Controllers
{
    public class IndexController : Controller
    {
        public IActionResult Index()
        {
    		return View();
        }

		public IActionResult DungeonView(int level, int startX, int startY, string direction)
		{
			Direction start = Direction.NoDir;

			switch (direction)
			{
				case "U":
					start = Direction.Up;
					break;
				
				case "D":
					start = Direction.Down;
					break;
				
				case "L":
					start = Direction.Left;
					break;
				
				case "R":
					start = Direction.Right;
					break;

				default:
					break;
			}

			Dungeon dungeon = new Dungeon(level, startX, startY, start);
    		return Json(dungeon.DungeonView);
		}
    }
}