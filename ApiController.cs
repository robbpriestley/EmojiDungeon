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
			
			return Utility.SerializedJsonObjectResult("hello");
		}
	}
}