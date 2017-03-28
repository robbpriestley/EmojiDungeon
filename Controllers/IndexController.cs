using Microsoft.AspNetCore.Mvc;

namespace DigitalWizardry.Dungeon.Controllers
{
    public class IndexController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
