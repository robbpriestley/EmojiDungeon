using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DigitalWizardry.Dungeon.Controllers
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
		public IActionResult Dungeon(int level, int startX, int startY, string direction)
		{
			if (!BasicAuthentication.Authenticate(Secrets, Request))
			{
				return new UnauthorizedResult();
			}

			Direction start = Direction.NoDir;

			switch (direction)
			{
				case "U":
					start = Direction.Up;
					break;
				
				case "D":
					start = Direction.Down;
					break;
				
				case "L":
					start = Direction.Left;
					break;
				
				case "R":
					start = Direction.Right;
					break;

				default:
					break;
			}
			
			Dungeon dungeon = new Dungeon(level, startX, startY, start);
			StringBuilder output = new StringBuilder();
			output.AppendLine(dungeon.VisualizeAsText(false, false));
			output.AppendLine(dungeon.BuildStats() + Environment.NewLine);

			return new ObjectResult(output.ToString());
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
				Dungeon dungeon = new Dungeon(0, 7, 0, Direction.Up);
				output.AppendLine(dungeon.VisualizeAsText(true, true));
				output.AppendLine(dungeon.BuildStats() + Environment.NewLine);
				Console.WriteLine("Iteration: " + i.ToString());
			}

			return new ObjectResult(output.ToString());
		}
	}
}