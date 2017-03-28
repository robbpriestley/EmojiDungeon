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
		// *** BEGIN FIELD DECLARATIONS ***
		private static readonly Description _empty;
		private static readonly Description _room_TBD;
		private static readonly Description _corridor_TBD;
		private static readonly Description _catacombs_TBD;
		private static readonly Description _mines_Horiz;
		private static readonly Description _mines_Vert;
		private static readonly Description _mines_Horiz_Flooded;
		private static readonly Description _mines_Vert_Flooded;
		private static readonly Description _constructed;
		private static readonly Description _constructed_Flooded;
		private static readonly Description _cavern;
		private static readonly Description _cavern_Flooded;
		// *** END FIELD DECLARATIONS ***
		// *** BEGIN PROPERTY DECLARATIONS ***
		public static Description Empty { get { return _empty; } }
		public static Description Room_TBD { get { return _room_TBD; } }
		public static Description Corridor_TBD { get { return _corridor_TBD; } }
		public static Description Catacombs_TBD { get { return _catacombs_TBD; } }
		public static Description Mines_Horiz { get { return _mines_Horiz; } }
		public static Description Mines_Vert { get { return _mines_Vert; } }
		public static Description Mines_Horiz_Flooded { get { return _mines_Horiz_Flooded; } }
		public static Description Mines_Vert_Flooded { get { return _mines_Vert_Flooded; } }
		public static Description Constructed { get { return _constructed; } }
		public static Description Constructed_Flooded { get { return _constructed_Flooded; } }
		public static Description Cavern { get { return _cavern; } }
		public static Description Cavern_Flooded { get { return _cavern_Flooded; } }
		// *** END PROPERTY DECLARATIONS ***

    	public static List<Description> Descrs = new List<Description>();

		static Descriptions()
		{
			_empty = new Description("E");
			_room_TBD = new Description(",");
			_corridor_TBD = new Description(".");
			_catacombs_TBD = new Description("'");
			_mines_Horiz = new Description("M");
			_mines_Vert = new Description("M");
			_mines_Horiz_Flooded = new Description("x");
			_mines_Vert_Flooded = new Description("x");
			_constructed = new Description("C");
			_constructed_Flooded = new Description("F");
			_cavern = new Description("c");
			_cavern_Flooded = new Description("f");

			Initialize();
		}
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