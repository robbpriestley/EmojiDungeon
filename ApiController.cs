using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DigitalWizardry.LevelGenerator
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
		[Route("level")]
		public IActionResult Level(int width, int height)
		{
			if (!BasicAuthentication.Authenticate(Secrets, Request))
			{
				return new UnauthorizedResult();
			}
			
			Coords startCoords = new Coords(1, 1);
			Level level = new Level(width, height, startCoords);
			string output = level.VisualizeAsText();
			output += level.Stats();

			return new ObjectResult(output);
			//return Utility.SerializedJsonObjectResult(visualize);
		}
	}
}