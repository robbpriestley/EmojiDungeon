using Microsoft.AspNetCore.Mvc;

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
    		return Json(dungeon.DungeonView);
		}
    }
}