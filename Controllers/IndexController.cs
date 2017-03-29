using Microsoft.AspNetCore.Mvc;
using DigitalWizardry.Dungeon.Models;

namespace DigitalWizardry.Dungeon.Controllers
{
    public class IndexController : Controller
    {
        public IActionResult Index()
        {
			Dungeon dungeon = new Dungeon(0);
			DungeonViewModel model = new DungeonViewModel();
			model.Filepaths = dungeon.Filepaths();
    		return View(model);
        }
    }
}