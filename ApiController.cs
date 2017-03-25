using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DigitalWizardry.Dungeon
{
	[Route("dungeon")]
	public class DungeonApiController : Controller
	{
		public Secrets Secrets { get; set; }
		
		public DungeonApiController(IOptions<Secrets> secrets)
		{
			Secrets = secrets.Value;
		}

		[HttpGet]
		[Route("create")]
		public IActionResult Dungeon()
		{
			if (!BasicAuthentication.Authenticate(Secrets, Request))
			{
				return new UnauthorizedResult();
			}
			
			Dungeon dungeon = new Dungeon(0);
			StringBuilder output = new StringBuilder();
			output.AppendLine(dungeon.VisualizeAsText());
			output.AppendLine(dungeon.BuildStats() + Environment.NewLine);
			output.AppendLine(dungeon.VisualizeAsTextWithDescription());

			return new ObjectResult(output.ToString());
			//return Utility.SerializedJsonObjectResult(visualize);
		}

		[HttpGet]
		[Route("test")]
		public IActionResult Test()
		{
			if (!BasicAuthentication.Authenticate(Secrets, Request))
			{
				return new UnauthorizedResult();
			}
			
			StringBuilder output = new StringBuilder();

			for (int i = 0; i < 100; i++)
			{
				Dungeon dungeon = new Dungeon(0);
				output.AppendLine(dungeon.VisualizeAsText());
				output.AppendLine(dungeon.BuildStats() + Environment.NewLine);
				Console.WriteLine("Iteration: " + i.ToString());
			}

			return new ObjectResult(output.ToString());
		}
	}
}