using System.Collections.Generic;

namespace DigitalWizardry.LevelGenerator
{	
	public class CellDescription
	{
		public int Weight;
		public int WeightReduction;
		public string TextRep;

		public CellDescription(){}
		
		public CellDescription(string textRep)
		{
			TextRep = textRep;
		}

		// Copy constructor. Creates a deep copy clone of the source.
		public CellDescription(CellDescription source) : this()
		{
			this.Weight = source.Weight;
			this.WeightReduction = source.WeightReduction;
			this.TextRep = source.TextRep;
		}
	}

	public class CellDescriptions
	{
		public static CellDescription Empty = new CellDescription("E");
		public static CellDescription Room_TBD = new CellDescription(",");
		public static CellDescription Corridor_TBD = new CellDescription(".");
		public static CellDescription Catacombs_TBD = new CellDescription("'");
		public static CellDescription Mines_Horiz = new CellDescription("M");
		public static CellDescription Mines_Vert = new CellDescription("M");
		public static CellDescription Mines_Horiz_Flooded = new CellDescription("x");
		public static CellDescription Mines_Vert_Flooded = new CellDescription("x");
		public static CellDescription Constructed = new CellDescription("C");
		public static CellDescription Constructed_Flooded = new CellDescription("F");
		public static CellDescription Cavern = new CellDescription("c");
    	public static CellDescription Cavern_Flooded = new CellDescription("f");
    	public static List<CellDescription> Descrs = new List<CellDescription>();

		public static void Initialize()
		{
			Constructed.Weight = 100;
			Constructed.WeightReduction = 1;

			Constructed_Flooded.Weight = 20;
			Constructed_Flooded.WeightReduction = 2;

			Cavern.Weight = 100;
			Cavern.WeightReduction = 1;

			Cavern_Flooded.Weight = 20;
			Cavern_Flooded.WeightReduction = 1;

			Descrs.Add(Constructed);
			Descrs.Add(Constructed_Flooded);
			Descrs.Add(Cavern);
			Descrs.Add(Cavern_Flooded);
		}

		public static bool IsTBD(CellDescription descr)
		{
			return descr == Room_TBD || descr == Corridor_TBD || descr == Catacombs_TBD;
		}

		public static bool IsMines(CellDescription descr)
		{
			return descr == Mines_Horiz || descr == Mines_Vert || descr == Mines_Horiz_Flooded || descr == Mines_Vert_Flooded;
		}

		public static bool IsFlooded(CellDescription descr)
		{
			return descr == Constructed_Flooded || descr == Cavern_Flooded || descr == Mines_Horiz_Flooded || descr == Mines_Vert_Flooded;
		}
	}
}