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
		public IActionResult Level()
		{
			if (!BasicAuthentication.Authenticate(Secrets, Request))
			{
				return new UnauthorizedResult();
			}
			
			Coords startCoords = new Coords(3, 0, 7, 7);
			Level level = new Level(1, 7, 7, startCoords);
			string visualize = level.VisualizeAsText();

			return new ObjectResult(visualize);
			//return Utility.SerializedJsonObjectResult(visualize);
		}
	}
}