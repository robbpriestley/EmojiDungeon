using Microsoft.AspNetCore.Mvc;
using DigitalWizardry.Dungeon.Models;

namespace DigitalWizardry.Dungeon.Controllers
{
    public class IndexController : Controller
    {
        public IActionResult Index()
        {
    		return View();
        }

		public IActionResult DungeonView()
		{
			Dungeon dungeon = new Dungeon(0);
			DungeonViewModel model = new DungeonViewModel();
			model.CssNames = dungeon.CssNames;
    		return View(model);
		}

		public IActionResult DungeonViewJson()
		{
			Dungeon dungeon = new Dungeon(0);
			DungeonViewModel model = new DungeonViewModel();
    		return Json(dungeon.DungeonViewModel);
		}
    }
}