using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DigitalWizardry.Dungeon
{
	[Route("levelgenerator")]
	public class LevelGeneratorApiController : Controller
	{
		public Secrets Secrets { get; set; }
		
		public LevelGeneratorApiController(IOptions<Secrets> secrets)
		{
			Secrets = secrets.Value;
		}

		[HttpGet]
		[Route("dungeon")]
		public IActionResult Dungeon()
		{
			if (!BasicAuthentication.Authenticate(Secrets, Request))
			{
				return new UnauthorizedResult();
			}
			
			Coords startCoords = new Coords(1, 1);
			Level level = new Level(0, startCoords);
			StringBuilder output = new StringBuilder();
			output.AppendLine(level.VisualizeAsText());
			output.AppendLine(level.Stats() + Environment.NewLine);
			//output.AppendLine(level.VisualizeAsTextWithDescription());

			return new ObjectResult(output.ToString());
			//return Utility.SerializedJsonObjectResult(visualize);
		}
	}
}