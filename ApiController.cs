using Microsoft.AspNetCore.Mvc;

namespace DigitalWizardry.LevelGenerator
{
	[Route("levelgenerator")]
	public class LevelGeneratorApiController : Controller
	{
		public LevelGeneratorApiController(){}

		[HttpGet]
		[Route("level")]
		public IActionResult Level()
		{
			return Utility.SerializedJsonObjectResult("hello");
		}
	}
}