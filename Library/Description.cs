using System.Collections.Generic;

namespace DigitalWizardry.Dungeon
{	
	public class Description
	{
		public int Weight { get; set; }
		public int WeightReduction { get; set; }
		public string TextRep { get; set; }
		public bool IsTBD { get; set; }
		public bool IsMines { get; set; }
		public bool IsFlooded { get; set; }

		public Description(){}
		
		public Description(string textRep)
		{
			TextRep = textRep;
		}

		// Copy constructor. Creates a deep copy clone of the source.
		public Description(Description source) : this()
		{
			Weight = source.Weight;
			WeightReduction = source.WeightReduction;
			TextRep = source.TextRep;
			IsTBD = source.IsTBD;
			IsMines = source.IsMines;
			IsFlooded = source.IsFlooded;
		}
	}

	public static class Descriptions
	{
		public static Description Empty = new Description("E");
		public static Description Room_TBD = new Description(",");
		public static Description Corridor_TBD = new Description(".");
		public static Description Catacombs_TBD = new Description("'");
		public static Description Mines_Horiz = new Description("M");
		public static Description Mines_Vert = new Description("M");
		public static Description Mines_Horiz_Flooded = new Description("x");
		public static Description Mines_Vert_Flooded = new Description("x");
		public static Description Constructed = new Description("C");
		public static Description Constructed_Flooded = new Description("F");
		public static Description Cavern = new Description("c");
    	public static Description Cavern_Flooded = new Description("f");
    	public static List<Description> Descrs = new List<Description>();

		public static void Initialize()
		{
			Room_TBD.IsTBD = true;
			Corridor_TBD.IsTBD = true;
			Catacombs_TBD.IsTBD = true;

			Mines_Horiz.IsMines = true;
			Mines_Vert.IsMines = true;
			Mines_Horiz_Flooded.IsMines = true;
			Mines_Horiz_Flooded.IsFlooded = true;
			Mines_Vert_Flooded.IsMines = true;
			Mines_Vert_Flooded.IsFlooded = true;
			
			Constructed.Weight = 100;
			Constructed.WeightReduction = 1;

			Constructed_Flooded.IsFlooded = true;
			Constructed_Flooded.Weight = 20;
			Constructed_Flooded.WeightReduction = 2;

			Cavern.Weight = 100;
			Cavern.WeightReduction = 1;
			Cavern_Flooded.IsFlooded = true;
			Cavern_Flooded.Weight = 20;
			Cavern_Flooded.WeightReduction = 1;

			Descrs.Add(Constructed);
			Descrs.Add(Constructed_Flooded);
			Descrs.Add(Cavern);
			Descrs.Add(Cavern_Flooded);
		}
	}
}