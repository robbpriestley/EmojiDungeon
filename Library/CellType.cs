using System.Collections.Generic;

namespace DigitalWizardry.LevelGenerator
{	
	public class CellType
	{
		// These "connects" members only apply to corridor cells.
		public bool ConnectsUp;
		public bool ConnectsDown;
		public bool ConnectsLeft;
		public bool ConnectsRight;
		
		public bool TraversableUp;
		public bool TraversableDown;
		public bool TraversableLeft;
		public bool TraversableRight;
		
		public int Weight;  // Weights the random selection.
		
		public bool IsEmpty;
		public bool IsJunction;
		public bool ForceGrowthCompatible;  // Can be substituted for another cell to increase dungeon fill.
		
		public string TextRep;
		public string TextRep2;  // Used for expanding the text representation into the horizontal.
		
		public int InitialAvailableConnections;
		
		public string MapImageFile;
		public string MapImageFileAltUp;
		public string MapImageFileAltRight;

		public CellType()
		{
        	ForceGrowthCompatible = true;
		}

		public bool ConnectsTo(CellType otherCell, Direction direction)
		{    
			if (otherCell.IsEmpty)
			{
				return false;
			}
			else if (direction == Direction.Up && this.ConnectsUp && otherCell.ConnectsDown)
			{
				return true;
			}
			else if (direction == Direction.Down && this.ConnectsDown && otherCell.ConnectsUp)
			{
				return true;
			}
			else if (direction == Direction.Left && this.ConnectsLeft && otherCell.ConnectsRight)
			{
				return true;
			}
			else if (direction == Direction.Right && this.ConnectsRight && otherCell.ConnectsLeft)
			{
				return true;
			}
			
			return false;
		}

		public bool CompatibleWith(CellType otherCell, Direction direction)
		{
			/*
				Another cell is compatible with the current cell if
				a) it is empty, or
				b) in the same direction, either both cells connect, or both do not connect.
				   (In other words if in the same direction one connects but the other does
				   not, that's bad).
			*/
			
			// Also, two junctions cannot appear side-by-side.
			if (this.IsJunction && otherCell.IsJunction) 
			{
				return false;
			}
			
			if (otherCell.IsEmpty)
			{
				return true;
			}
			else if (direction == Direction.Up)
			{
				if ((this.ConnectsUp && otherCell.ConnectsDown) || 
					(!this.ConnectsUp && !otherCell.ConnectsDown))
				{
					return true;
				}
			}
			else if (direction == Direction.Down)
			{
				if ((this.ConnectsDown && otherCell.ConnectsUp) || 
					(!this.ConnectsDown && !otherCell.ConnectsUp))
				{
					return true;
				}
			}
			else if (direction == Direction.Left)
			{
				if ((this.ConnectsLeft && otherCell.ConnectsRight) || 
					(!this.ConnectsLeft && !otherCell.ConnectsRight))
				{
					return true;
				}
			}
			else if (direction == Direction.Right)
			{
				if ((this.ConnectsRight && otherCell.ConnectsLeft) || 
					(!this.ConnectsRight && !otherCell.ConnectsLeft))
				{
					return true;
				}
			}

			return false;
		}
	}

	public class CellTypes
	{
		public static CellType emptyCell = new CellType();	// Empty, i.e. unused.
		public static CellType vert = new CellType();		// Vertical Corridor            
		public static CellType horiz = new CellType();		// Horizontal Corridor           
		public static CellType inter = new CellType();		// Intersection                 
		public static CellType juncULR = new CellType();	// Junction Up Left Right       
		public static CellType juncUDR = new CellType();	// Junction Up Down Right       
		public static CellType juncDLR = new CellType();	// Junction Down Left Right     
		public static CellType juncUDL = new CellType();	// Junction Up Down Left        
		public static CellType elbUR = new CellType();		// Elbow Up Right               
		public static CellType elbDR = new CellType();		// Elbow Down Right             
		public static CellType elbDL = new CellType();		// Elbow Down Left              
		public static CellType elbUL = new CellType();		// Elbow Up Left                
		public static CellType deadU = new CellType();		// Dead End Up                  
		public static CellType deadD = new CellType();		// Dead End Down                
		public static CellType deadL = new CellType();		// Dead End Left                
		public static CellType deadR = new CellType();		// Dead End Right 
		public static CellType deadexU = new CellType();  // Dead End Exit Up                  
		public static CellType deadexD = new CellType();  // Dead End Exit Down                
		public static CellType deadexL = new CellType();  // Dead End Exit Left                
		public static CellType deadexR = new CellType();  // Dead End Exit Right 

		public static void Initialize()
		{
			// *** BEGIN SPECIAL CELLS ***
			// These cell are only ever placed in specific, known circumstances. They are 
			// never randomly assigned. Therefore, their Weight values are 0.
			
			// For simplicity, the empty cell is considered to connect in every direction.
			emptyCell.Weight = 0;
			emptyCell.IsEmpty = true;
			emptyCell.ConnectsUp = true;
			emptyCell.ConnectsDown = true;
			emptyCell.ConnectsLeft = true;
			emptyCell.ConnectsRight = true;
			emptyCell.TextRep = @" ";
			emptyCell.TextRep2 = @" ";
			emptyCell.ForceGrowthCompatible = false;
			emptyCell.InitialAvailableConnections = 4;
			
			// *** END SPECIAL CELLS ***
			
			// *** BEGIN CORRIDOR CELLS ***
			
			vert.Weight = 100;
			vert.ConnectsUp = true;
			vert.ConnectsDown = true;
			vert.TraversableUp = true;
			vert.TraversableDown = true;
			vert.TextRep = @"║";
			vert.TextRep2 = @" ";
			vert.MapImageFile = @"000.png";
			vert.MapImageFileAltUp = @"001.png";
			vert.InitialAvailableConnections = 2;
			
			horiz.Weight = 100;
			horiz.ConnectsLeft = true;
			horiz.ConnectsRight = true;
			horiz.TraversableLeft = true;
			horiz.TraversableRight = true;
			horiz.TextRep = @"═";
			horiz.TextRep2 = @"═";
			horiz.MapImageFile = @"002.png";
			horiz.MapImageFileAltRight = @"003.png";
			horiz.InitialAvailableConnections = 2;
			
			inter.Weight = 20;
			inter.ConnectsUp = true;
			inter.ConnectsDown = true;
			inter.ConnectsLeft = true;
			inter.ConnectsRight = true;
			inter.TraversableUp = true;
			inter.TraversableDown = true;
			inter.TraversableLeft = true;
			inter.TraversableRight = true;
			inter.TextRep = @"╬";
			inter.TextRep2 = @"═";
			inter.IsJunction = true;
			inter.MapImageFile = @"004.png";
			inter.MapImageFileAltUp = @"005.png";
			inter.MapImageFileAltRight = @"006.png";
			inter.InitialAvailableConnections = 4;
			
			juncULR.Weight = 20;
			juncULR.ConnectsUp = true;
			juncULR.ConnectsLeft = true;
			juncULR.ConnectsRight = true;
			juncULR.TraversableUp = true;
			juncULR.TraversableLeft = true;
			juncULR.TraversableRight = true;
			juncULR.TextRep = @"╩";
			juncULR.TextRep2 = @"═";
			juncULR.IsJunction = true;
			juncULR.MapImageFile = @"007.png";
			juncULR.MapImageFileAltUp = @"008.png";
			juncULR.MapImageFileAltRight = @"009.png";
			juncULR.InitialAvailableConnections = 3;
			
			juncUDR.Weight = 20;
			juncUDR.ConnectsUp = true;
			juncUDR.ConnectsDown = true;
			juncUDR.ConnectsRight = true;
			juncUDR.TraversableUp = true;
			juncUDR.TraversableDown = true;
			juncUDR.TraversableRight = true;
			juncUDR.TextRep = @"╠";
			juncUDR.TextRep2 = @"═";
			juncUDR.IsJunction = true;
			juncUDR.MapImageFile = @"010.png";
			juncUDR.MapImageFileAltUp = @"011.png";
			juncUDR.MapImageFileAltRight = @"012.png";
			juncUDR.InitialAvailableConnections = 3;
			
			juncDLR.Weight = 20;
			juncDLR.ConnectsDown = true;
			juncDLR.ConnectsLeft = true;
			juncDLR.ConnectsRight = true;
			juncDLR.TraversableDown = true;
			juncDLR.TraversableLeft = true;
			juncDLR.TraversableRight = true;
			juncDLR.TextRep = @"╦";
			juncDLR.TextRep2 = @"═";
			juncDLR.IsJunction = true;
			juncDLR.MapImageFile = @"013.png";
			juncDLR.MapImageFileAltRight = @"014.png";
			juncDLR.InitialAvailableConnections = 3;
			
			juncUDL.Weight = 20;
			juncUDL.ConnectsUp = true;
			juncUDL.ConnectsDown = true;
			juncUDL.ConnectsLeft = true;
			juncUDL.TraversableUp = true;
			juncUDL.TraversableDown = true;
			juncUDL.TraversableLeft = true;
			juncUDL.TextRep = @"╣";
			juncUDL.TextRep2 = @" ";
			juncUDL.IsJunction = true;
			juncUDL.MapImageFile = @"015.png";
			juncUDL.MapImageFileAltUp = @"016.png";
			juncUDL.InitialAvailableConnections = 3;
			
			elbUR.Weight = 20;
			elbUR.ConnectsUp = true;
			elbUR.ConnectsRight = true;
			elbUR.TraversableUp = true;
			elbUR.TraversableRight = true;
			elbUR.TextRep = @"╚";
			elbUR.TextRep2 = @"═";
			elbUR.MapImageFile = @"017.png";
			elbUR.MapImageFileAltUp = @"018.png";
			elbUR.MapImageFileAltRight = @"019.png";
			elbUR.InitialAvailableConnections = 2;
			
			elbDR.Weight = 20;
			elbDR.ConnectsDown = true;
			elbDR.ConnectsRight = true;
			elbDR.TraversableDown = true;
			elbDR.TraversableRight = true;
			elbDR.TextRep = @"╔";
			elbDR.TextRep2 = @"═";
			elbDR.MapImageFile = @"020.png";
			elbDR.MapImageFileAltRight = @"021.png";
			elbDR.InitialAvailableConnections = 2;
			
			elbDL.Weight = 20;
			elbDL.ConnectsDown = true;
			elbDL.ConnectsLeft = true;
			elbDL.TraversableDown = true;
			elbDL.TraversableLeft = true;
			elbDL.TextRep = @"╗";
			elbDL.TextRep2 = @" ";
			elbDL.MapImageFile = @"022.png";
			elbDL.InitialAvailableConnections = 2;
			
			elbUL.Weight = 20;
			elbUL.ConnectsUp = true;
			elbUL.ConnectsLeft = true;
			elbUL.TraversableUp = true;
			elbUL.TraversableLeft = true;
			elbUL.TextRep = @"╝";
			elbUL.TextRep2 = @" ";
			elbUL.MapImageFile = @"023.png";
			elbUL.MapImageFileAltUp = @"024.png";
			elbUL.InitialAvailableConnections = 2;

			// *** END CORRIDOR CELLS ***
			// *** BEGIN DEAD END CELLS ***
			
			deadU.Weight = 1;
			deadU.ConnectsUp = true;
			deadU.TraversableUp = true;
			deadU.TextRep = @"╨";
			deadU.TextRep2 = @" ";
			deadU.MapImageFile = @"025.png";
			deadU.MapImageFileAltUp = @"026.png";
			deadU.InitialAvailableConnections = 1;
			
			deadD.Weight = 1;
			deadD.ConnectsDown = true;
			deadD.TraversableDown = true;
			deadD.TextRep = @"╥";
			deadD.TextRep2 = @" ";
			deadD.MapImageFile = @"027.png";
			deadD.InitialAvailableConnections = 1;
			
			deadL.Weight = 1;
			deadL.ConnectsLeft = true;
			deadL.TraversableLeft = true;
			deadL.TextRep = @"╡";
			deadL.TextRep2 = @" ";
			deadL.MapImageFile = @"028.png";
			deadL.InitialAvailableConnections = 1;
			
			deadR.Weight = 1;
			deadR.ConnectsRight = true;
			deadR.TraversableRight = true;
			deadR.TextRep = @"╞";
			deadR.TextRep2 = @"═";
			deadR.MapImageFile = @"029.png";
			deadR.MapImageFileAltRight = @"030.png";
			deadR.InitialAvailableConnections = 1;
			
			deadexU.ConnectsUp = true;
			deadexU.TraversableUp = true;
			deadexU.TextRep = @"╨";
			deadexU.TextRep2 = @" ";
			deadexU.MapImageFile = @"031.png";
			deadexU.MapImageFileAltUp = @"032.png";
			
			deadexD.ConnectsDown = true;
			deadexD.TraversableDown = true;
			deadexD.TextRep = @"╥";
			deadexD.TextRep2 = @" ";
			deadexD.MapImageFile = @"033.png";
			
			deadexL.ConnectsLeft = true;
			deadexL.TraversableLeft = true;
			deadexL.TextRep = @"╡";
			deadexL.TextRep2 = @" ";
			deadexL.MapImageFile = @"034.png";

			deadexR.ConnectsRight = true;
			deadexR.TraversableRight = true;
			deadexR.TextRep = @"╞";
			deadexR.TextRep2 = @"═";
			deadexR.MapImageFile = @"035.png";
			deadexR.MapImageFileAltRight = @"036.png";
		}

		// Edge: meaning the extreme edge of the dungeon level.
		public static List<CellType> GetTypes(Coords coords, int gridWidth, int gridHeight)
		{
			coords.SetEdges(gridWidth, gridHeight);
			
			List<CellType> types = new List<CellType>();
			
			if (coords.AdjacentEdgeUp && !coords.AdjacentEdgeLeft && !coords.AdjacentEdgeRight) 
			{
				types.Add(horiz);
				types.Add(juncDLR);
				types.Add(elbDR);
				types.Add(elbDL);
				types.Add(deadD);
				types.Add(deadL);
				types.Add(deadR);
			} 
			else if (coords.AdjacentEdgeDown && !coords.AdjacentEdgeLeft && !coords.AdjacentEdgeRight) 
			{
				types.Add(horiz);
				types.Add(juncULR); 
				types.Add(elbUR);
				types.Add(elbUL);
				types.Add(deadU);
				types.Add(deadL);
				types.Add(deadR);
			}
			else if (coords.AdjacentEdgeLeft && !coords.AdjacentEdgeUp && !coords.AdjacentEdgeDown) 
			{
				types.Add(vert);
				types.Add(juncUDR);
				types.Add(elbUR);
				types.Add(elbDR);
				types.Add(deadU);
				types.Add(deadD);
				types.Add(deadR);
			}
			else if (coords.AdjacentEdgeRight && !coords.AdjacentEdgeUp && !coords.AdjacentEdgeDown) 
			{
				types.Add(vert);
				types.Add(juncUDL);
				types.Add(elbDL);
				types.Add(elbUL);
				types.Add(deadU);
				types.Add(deadD);
				types.Add(deadL);
			}
			else if (coords.AdjacentEdgeUp && coords.AdjacentEdgeLeft) 
			{
				types.Add(elbDR);
				types.Add(deadD);
				types.Add(deadR);
			}
			else if (coords.AdjacentEdgeUp && coords.AdjacentEdgeRight) 
			{
				types.Add(elbDL);
				types.Add(deadD);
				types.Add(deadL);
			} 
			else if (coords.AdjacentEdgeDown && coords.AdjacentEdgeLeft) 
			{ 
				types.Add(elbUR);
				types.Add(deadU);
				types.Add(deadR);
			}
			else if (coords.AdjacentEdgeDown && coords.AdjacentEdgeRight) 
			{
				types.Add(elbUL);
				types.Add(deadU);
				types.Add(deadL);
			}
			else  // Standard (non-edge) types.
			{
				types.Add(vert);
				types.Add(horiz);
				types.Add(inter);
				types.Add(juncULR); 
				types.Add(juncUDR);
				types.Add(juncDLR);
				types.Add(juncUDL);
				types.Add(elbUR);
				types.Add(elbDR);
				types.Add(elbDL);
				types.Add(elbUL);
				types.Add(deadU);
				types.Add(deadD);
				types.Add(deadL);
				types.Add(deadR);
			}
			
			return types;
		}

		public static bool IsDeadEnd(CellType type)
		{
			return type == deadU || type == deadD || type == deadL || type == deadR;
		}
	}
}