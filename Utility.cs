using System;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

namespace DigitalWizardry.Dungeon
{	
	public class Utility
	{
		public static ObjectResult SerializedJsonObjectResult(Object obj)
		{
			string json = 
			
			JsonConvert.SerializeObject
			(
				obj, 
				Formatting.Indented, 
				new JsonSerializerSettings 
				{ 
					ReferenceLoopHandling = ReferenceLoopHandling.Ignore
				}
			);
			
			return new ObjectResult(json);
		}
	}
}