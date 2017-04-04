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
			model.CssNames = dungeon.CssNames();
    		return View(model);
        }

		public IActionResult Sum(int firstNumber, int secondNumber)
		{
			return Content((firstNumber + secondNumber).ToString(), "text/plain");
		}

		public IActionResult DisplayObject()
		{
			Destination destination = new Destination("Tokyo", "Japan", 1);
			return Json(destination);
		}

		public IActionResult DungeonView()
		{
			return View();
		}
    }

	public class Destination
    {
        public string City { get; set; }
        public string Country { get; set; }
        public int Id { get; set; }

        public Destination(string city, string country, int id = 0)
        {
            City = city;
            Country = country;
            Id = id;
        }
    }
}