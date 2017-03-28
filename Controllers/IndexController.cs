using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using DigitalWizardry.Dungeon.Models;

namespace DigitalWizardry.Dungeon.Controllers
{
    public class IndexController : Controller
    {
        public IActionResult Index()
        {
			Dungeon dungeon = new Dungeon(0);
			StringBuilder output = new StringBuilder();
			output.AppendLine(dungeon.VisualizeAsText());
			output.AppendLine(dungeon.BuildStats() + Environment.NewLine);
			output.AppendLine(dungeon.VisualizeAsTextWithDescription());

			DungeonViewModel model = new DungeonViewModel();
			model.TextVisualization = output.ToString();
    		return View(model);
        }
    }
}