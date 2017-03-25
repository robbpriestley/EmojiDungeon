using System.Collections.Generic;

namespace DigitalWizardry.Dungeon
{	
	public class CellType
	{
		// These "connects" members only apply to corridor cells.
		public bool ConnectsUp;
		public bool ConnectsDown;
		public bool ConnectsLeft;
		public bool ConnectsRight;
		public bool RoomConnectsUp;
		public bool RoomConnectsDown;
		public bool RoomConnectsLeft;
		public bool RoomConnectsRight;
		public bool TraversableUp;
		public bool TraversableDown;
		public bool TraversableLeft;
		public bool TraversableRight;
		public int Weight;  // Weights the random selection.
		public int DoorProb;
		public bool IsEmpty;
		public bool IsJunction;
		public bool IsRoomExit;
		public bool RoomExitCompatible;
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
			
			// This code snippet will suppress side-by-side junctions, useful perhaps in maze-only levels,
			// but it will also cause certain levels with rooms to go into an infinite loop.
			// if (this.IsJunction && otherCell.IsJunction) { return false; }
			
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
		public static CellType emptyCell = new CellType();	       // Empty, i.e. unused.
		public static CellType entrance = new CellType();	       // Entrance Cell, used only once on level 0...
		public static CellType vert = new CellType();		       // Vertical Corridor            
		public static CellType horiz = new CellType();		       // Horizontal Corridor           
		public static CellType inter = new CellType();		       // Intersection                 
		public static CellType juncULR = new CellType();	       // Junction Up Left Right       
		public static CellType juncUDR = new CellType();	       // Junction Up Down Right       
		public static CellType juncDLR = new CellType();	       // Junction Down Left Right     
		public static CellType juncUDL = new CellType();	       // Junction Up Down Left        
		public static CellType elbUR = new CellType();		       // Elbow Up Right               
		public static CellType elbDR = new CellType();		       // Elbow Down Right             
		public static CellType elbDL = new CellType();		       // Elbow Down Left              
		public static CellType elbUL = new CellType();		       // Elbow Up Left                
		public static CellType deadU = new CellType();		       // Dead End Up                  
		public static CellType deadD = new CellType();		       // Dead End Down                
		public static CellType deadL = new CellType();		       // Dead End Left                
		public static CellType deadR = new CellType();		       // Dead End Right 
		public static CellType deadexU = new CellType();           // Dead End Exit Up                  
		public static CellType deadexD = new CellType();           // Dead End Exit Down                
		public static CellType deadexL = new CellType();           // Dead End Exit Left                
		public static CellType deadexR = new CellType();           // Dead End Exit Right 
		public static CellType upStairU = new CellType();          // Stairs Up Connects Up        
		public static CellType upStairD = new CellType();          // Stairs Up Connects Down      
		public static CellType upStairL = new CellType();          // Stairs Up Connects Left      
		public static CellType upStairR = new CellType();          // Stairs Up Connects Right     
		public static CellType downStairU = new CellType();        // Stairs Down Connects Up      
		public static CellType downStairD = new CellType();        // Stairs Down Connects Down    
		public static CellType downStairL = new CellType();        // Stairs Down Connects Left    
		public static CellType downStairR = new CellType();        // Stairs Down Connects Right 
		public static CellType roomSpace = new CellType();         // Room Space
		public static CellType fountain = new CellType();          // Healing Fountain
		public static CellType roomWallU = new CellType();         // Room Wall Up
		public static CellType roomWallD = new CellType();         // Room Wall Down
		public static CellType roomWallL = new CellType();         // Room Wall Left
		public static CellType roomWallR = new CellType();         // Room Wall Right
		public static CellType roomWallU_Round = new CellType();   // Room Wall Up Round
		public static CellType roomWallD_Round = new CellType();   // Room Wall Down Round
		public static CellType roomWallL_Round = new CellType();   // Room Wall Left Round
		public static CellType roomWallR_Round = new CellType();   // Room Wall Right Round
		public static CellType roomWallUL = new CellType();        // Room Wall Up Left
		public static CellType roomWallUR = new CellType();        // Room Wall Up Right
		public static CellType roomWallDL = new CellType();        // Room Wall Down Left
		public static CellType roomWallDR = new CellType();        // Room Wall Down Right
		public static CellType roomWallUL_Round = new CellType();  // Room Wall Up Left Round
		public static CellType roomWallUR_Round = new CellType();  // Room Wall Up Right Round
		public static CellType roomWallDL_Round = new CellType();  // Room Wall Down Left Round
		public static CellType roomWallDR_Round = new CellType();  // Room Wall Down Right Round
		public static CellType roomWallULinv = new CellType();     // Room Wall Up Left Inverted
		public static CellType roomWallURinv = new CellType();     // Room Wall Up Right Inverted
		public static CellType roomWallDLinv = new CellType();     // Room Wall Down Left Inverted
		public static CellType roomWallDRinv = new CellType();     // Room Wall Down Right Inverted
		public static CellType roomExitU = new CellType();         // Room Exit Up
		public static CellType roomExitD = new CellType();         // Room Exit Down
		public static CellType roomExitL = new CellType();         // Room Exit Left
		public static CellType roomExitR = new CellType();         // Room Exit Right
		public static CellType roomExitU_Round = new CellType();   // Room Exit Up Round
		public static CellType roomExitD_Round = new CellType();   // Room Exit Down Round
		public static CellType roomExitL_Round = new CellType();   // Room Exit Left Round
		public static CellType roomExitR_Round = new CellType();   // Room Exit Right Round
		public static CellType roomExitUL_U = new CellType();      // Room Exit Up Left Corner, Exit Up
		public static CellType roomExitUL_L = new CellType();      // Room Exit Up Left Corner, Exit Left
		public static CellType roomExitUL_UL = new CellType();     // Room Exit Up Left Corner, Exits Up and Left
		public static CellType roomExitUR_U = new CellType();      // Room Exit Up Right Corner, Exit Up
		public static CellType roomExitUR_R = new CellType();      // Room Exit Up Right Corner, Exit Right
		public static CellType roomExitUR_UR = new CellType();     // Room Exit Up Right Corner, Exits Up and Right
		public static CellType roomExitDL_D = new CellType();      // Room Exit Down Left Corner, Exit Down
		public static CellType roomExitDL_L = new CellType();      // Room Exit Down Left Corner, Exit Left
		public static CellType roomExitDL_DL = new CellType();     // Room Exit Down Left Corner, Exits Down and Left
		public static CellType roomExitDR_D = new CellType();      // Room Exit Down Right Corner, Exit Down
		public static CellType roomExitDR_R = new CellType();      // Room Exit Down Right Corner, Exit Right
		public static CellType roomExitDR_DR = new CellType();     // Room Exit Down Right Corner, Exits Down and Right

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
			
			entrance.Weight = 0;
			entrance.ConnectsUp = true;
			entrance.TraversableUp = true;
			entrance.TextRep = @"❍";
			entrance.TextRep2 = @" ";
			entrance.ForceGrowthCompatible = false;
			entrance.InitialAvailableConnections = 1;
			
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

			// *** BEGIN ROOM CELLS ***
			
			roomSpace.TraversableUp = true;
			roomSpace.TraversableDown = true;
			roomSpace.TraversableLeft = true;
			roomSpace.TraversableRight = true;
			roomSpace.TextRep = " ";
			roomSpace.TextRep2 = " ";
			roomSpace.ForceGrowthCompatible = false;
			
			fountain.TraversableUp = true;
			fountain.TraversableDown = true;
			fountain.TraversableLeft = true;
			fountain.TraversableRight = true;
			fountain.TextRep = "F";
			fountain.TextRep2 = " ";
			fountain.ForceGrowthCompatible = false;
			
			roomWallU.TraversableDown = true;
			roomWallU.TraversableLeft = true;
			roomWallU.TraversableRight = true;
			roomWallU.RoomExitCompatible = true;
			roomWallU.RoomConnectsLeft = true;
			roomWallU.RoomConnectsRight = true;
			roomWallU.TextRep = "─";
			roomWallU.TextRep2 = "─";
			roomWallU.ForceGrowthCompatible = false;
			
			roomWallD.TraversableUp = true;
			roomWallD.TraversableLeft = true;
			roomWallD.TraversableRight = true;
			roomWallD.RoomExitCompatible = true;
			roomWallD.RoomConnectsLeft = true;
			roomWallD.RoomConnectsRight = true;
			roomWallD.TextRep = "─";
			roomWallD.TextRep2 = "─";
			roomWallD.ForceGrowthCompatible = false;
			
			roomWallL.TraversableUp = true;
			roomWallL.TraversableDown = true;
			roomWallL.TraversableRight = true;
			roomWallL.RoomExitCompatible = true;
			roomWallL.RoomConnectsUp = true;
			roomWallL.RoomConnectsDown = true;
			roomWallL.TextRep = "│";
			roomWallL.TextRep2 = " ";
			roomWallL.ForceGrowthCompatible = false;
			
			roomWallR.TraversableUp = true;
			roomWallR.TraversableDown = true;
			roomWallR.TraversableLeft = true;
			roomWallR.RoomExitCompatible = true;
			roomWallR.RoomConnectsUp = true;
			roomWallR.RoomConnectsDown = true;
			roomWallR.TextRep = "│";
			roomWallR.TextRep2 = " ";
			roomWallR.ForceGrowthCompatible = false;
			
			roomWallU_Round.TraversableDown = true;
			roomWallU_Round.TraversableLeft = true;
			roomWallU_Round.TraversableRight = true;
			roomWallU_Round.RoomExitCompatible = false;
			roomWallU_Round.RoomConnectsLeft = true;
			roomWallU_Round.RoomConnectsRight = true;
			roomWallU_Round.TextRep = "┉";
			roomWallU_Round.TextRep2 = "┉";
			roomWallU_Round.ForceGrowthCompatible = false;
			
			roomWallD_Round.TraversableUp = true;
			roomWallD_Round.TraversableLeft = true;
			roomWallD_Round.TraversableRight = true;
			roomWallD_Round.RoomExitCompatible = false;
			roomWallD_Round.RoomConnectsLeft = true;
			roomWallD_Round.RoomConnectsRight = true;
			roomWallD_Round.TextRep = "┉";
			roomWallD_Round.TextRep2 = "┉";
			roomWallD_Round.ForceGrowthCompatible = false;
			
			roomWallL_Round.TraversableUp = true;
			roomWallL_Round.TraversableDown = true;
			roomWallL_Round.TraversableRight = true;
			roomWallL_Round.RoomExitCompatible = false;
			roomWallL_Round.RoomConnectsUp = true;
			roomWallL_Round.RoomConnectsDown = true;
			roomWallL_Round.TextRep = "┋";
			roomWallL_Round.TextRep2 = " ";
			roomWallL_Round.ForceGrowthCompatible = false;
			
			roomWallR_Round.TraversableUp = true;
			roomWallR_Round.TraversableDown = true;
			roomWallR_Round.TraversableLeft = true;
			roomWallR_Round.RoomExitCompatible = false;
			roomWallR_Round.RoomConnectsUp = true;
			roomWallR_Round.RoomConnectsDown = true;
			roomWallR_Round.TextRep = "┋";
			roomWallR_Round.TextRep2 = " ";
			roomWallR_Round.ForceGrowthCompatible = false;
			
			roomWallUL.TraversableDown = true;
			roomWallUL.TraversableRight = true;
			roomWallUL.RoomExitCompatible = true;
			roomWallUL.RoomConnectsDown = true;
			roomWallUL.RoomConnectsRight = true;
			roomWallUL.TextRep = "┌";
			roomWallUL.TextRep2 = "─";
			roomWallUL.ForceGrowthCompatible = false;
			
			roomWallUR.TraversableDown = true;
			roomWallUR.TraversableLeft = true;
			roomWallUR.RoomExitCompatible = true;
			roomWallUR.RoomConnectsDown = true;
			roomWallUR.RoomConnectsLeft = true;
			roomWallUR.TextRep = "┐";
			roomWallUR.TextRep2 = " ";
			roomWallUR.ForceGrowthCompatible = false;
			
			roomWallDL.TraversableUp = true;
			roomWallDL.TraversableRight = true;
			roomWallDL.RoomExitCompatible = true;
			roomWallDL.RoomConnectsUp = true;
			roomWallDL.RoomConnectsRight = true;
			roomWallDL.TextRep = "└";
			roomWallDL.TextRep2 = "─";
			roomWallDL.ForceGrowthCompatible = false;
			
			roomWallDR.TraversableUp = true;
			roomWallDR.TraversableLeft = true;
			roomWallDR.RoomExitCompatible = true;
			roomWallDR.RoomConnectsUp = true;
			roomWallDR.RoomConnectsLeft = true;
			roomWallDR.TextRep = "┘";
			roomWallDR.TextRep2 = " ";
			roomWallDR.ForceGrowthCompatible = false;
			
			roomWallUL_Round.TraversableDown = true;
			roomWallUL_Round.TraversableRight = true;
			roomWallUL_Round.RoomExitCompatible = false;
			roomWallUL_Round.RoomConnectsDown = true;
			roomWallUL_Round.RoomConnectsRight = true;
			roomWallUL_Round.TextRep = "╭";
			roomWallUL_Round.TextRep2 = "─";
			roomWallUL_Round.ForceGrowthCompatible = false;
			
			roomWallUR_Round.TraversableDown = true;
			roomWallUR_Round.TraversableLeft = true;
			roomWallUR_Round.RoomExitCompatible = false;
			roomWallUR_Round.RoomConnectsDown = true;
			roomWallUR_Round.RoomConnectsLeft = true;
			roomWallUR_Round.TextRep = "╮";
			roomWallUR_Round.TextRep2 = " ";
			roomWallUR_Round.ForceGrowthCompatible = false;
			
			roomWallDL_Round.TraversableUp = true;
			roomWallDL_Round.TraversableRight = true;
			roomWallDL_Round.RoomExitCompatible = false;
			roomWallDL_Round.RoomConnectsUp = true;
			roomWallDL_Round.RoomConnectsRight = true;
			roomWallDL_Round.TextRep = "╰";
			roomWallDL_Round.TextRep2 = "─";
			roomWallDL_Round.ForceGrowthCompatible = false;
			
			roomWallDR_Round.TraversableUp = true;
			roomWallDR_Round.TraversableLeft = true;
			roomWallDR_Round.RoomExitCompatible = false;
			roomWallDR_Round.RoomConnectsUp = true;
			roomWallDR_Round.RoomConnectsLeft = true;
			roomWallDR_Round.TextRep = "╯";
			roomWallDR_Round.TextRep2 = " ";
			roomWallDR_Round.ForceGrowthCompatible = false;
			
			roomWallULinv.TraversableDown = true;
			roomWallULinv.TraversableRight = true;
			roomWallULinv.RoomConnectsDown = true;
			roomWallULinv.RoomConnectsRight = true;
			roomWallULinv.TextRep = "┌";
			roomWallULinv.TextRep2 = "─";
			roomWallULinv.ForceGrowthCompatible = false;
			
			roomWallURinv.TraversableDown = true;
			roomWallURinv.TraversableLeft = true;
			roomWallURinv.RoomConnectsDown = true;
			roomWallURinv.RoomConnectsLeft = true;
			roomWallURinv.TextRep = "┐";
			roomWallURinv.TextRep2 = " ";
			roomWallURinv.ForceGrowthCompatible = false;
			
			roomWallDLinv.TraversableUp = true;
			roomWallDLinv.TraversableRight = true;
			roomWallDLinv.RoomConnectsUp = true;
			roomWallDLinv.RoomConnectsRight = true;
			roomWallDLinv.TextRep = "└";
			roomWallDLinv.TextRep2 = "─";
			roomWallDLinv.ForceGrowthCompatible = false;
			
			roomWallDRinv.TraversableUp = true;
			roomWallDRinv.TraversableLeft = true;
			roomWallDRinv.RoomConnectsUp = true;
			roomWallDRinv.RoomConnectsLeft = true;
			roomWallDRinv.TextRep = "┘";
			roomWallDRinv.TextRep2 = " ";
			roomWallDRinv.ForceGrowthCompatible = false;
			
			roomExitU.ConnectsUp = true;
			roomExitU.TraversableUp = true;
			roomExitU.TraversableDown = true;
			roomExitU.TraversableLeft = true;
			roomExitU.TraversableRight = true;
			roomExitU.RoomConnectsLeft = true;
			roomExitU.RoomConnectsRight = true;
			roomExitU.InitialAvailableConnections = 1;
			roomExitU.TextRep = "╨";
			roomExitU.TextRep2 = "─";
			roomExitU.DoorProb = 50;
			roomExitU.IsRoomExit = true;
			roomExitU.ForceGrowthCompatible = false;
			
			roomExitD.ConnectsDown = true;
			roomExitD.TraversableUp = true;
			roomExitD.TraversableDown = true;
			roomExitD.TraversableLeft = true;
			roomExitD.TraversableRight = true;
			roomExitD.RoomConnectsLeft = true;
			roomExitD.RoomConnectsRight = true;
			roomExitD.InitialAvailableConnections = 1;
			roomExitD.TextRep = "╥";
			roomExitD.TextRep2 = "─";
			roomExitD.DoorProb = 50;
			roomExitD.IsRoomExit = true;
			roomExitD.ForceGrowthCompatible = false;
			
			roomExitL.ConnectsLeft = true;
			roomExitL.TraversableUp = true;
			roomExitL.TraversableDown = true;
			roomExitL.TraversableLeft = true;
			roomExitL.TraversableRight = true;
			roomExitL.RoomConnectsUp = true;
			roomExitL.RoomConnectsDown = true;
			roomExitL.InitialAvailableConnections = 1;
			roomExitL.TextRep = "╡";
			roomExitL.TextRep2 = " ";
			roomExitL.DoorProb = 50;
			roomExitL.IsRoomExit = true;
			roomExitL.ForceGrowthCompatible = false;
			
			roomExitR.ConnectsRight = true;
			roomExitR.TraversableUp = true;
			roomExitR.TraversableDown = true;
			roomExitR.TraversableLeft = true;
			roomExitR.TraversableRight = true;
			roomExitR.RoomConnectsUp = true;
			roomExitR.RoomConnectsDown = true;
			roomExitR.InitialAvailableConnections = 1;
			roomExitR.TextRep = "╞";
			roomExitR.TextRep2 = "═";
			roomExitR.DoorProb = 50;
			roomExitR.IsRoomExit = true;
			roomExitR.ForceGrowthCompatible = false;
			
			roomExitU_Round.ConnectsUp = true;
			roomExitU_Round.TraversableUp = true;
			roomExitU_Round.TraversableDown = true;
			roomExitU_Round.TraversableLeft = true;
			roomExitU_Round.TraversableRight = true;
			roomExitU_Round.RoomConnectsLeft = true;
			roomExitU_Round.RoomConnectsRight = true;
			roomExitU_Round.InitialAvailableConnections = 1;
			roomExitU_Round.TextRep = "╨";
			roomExitU_Round.TextRep2 = "┉";
			roomExitU_Round.DoorProb = 0;
			roomExitU_Round.IsRoomExit = true;
			roomExitU_Round.ForceGrowthCompatible = false;
			
			roomExitD_Round.ConnectsDown = true;
			roomExitD_Round.TraversableUp = true;
			roomExitD_Round.TraversableDown = true;
			roomExitD_Round.TraversableLeft = true;
			roomExitD_Round.TraversableRight = true;
			roomExitD_Round.RoomConnectsLeft = true;
			roomExitD_Round.RoomConnectsRight = true;
			roomExitD_Round.InitialAvailableConnections = 1;
			roomExitD_Round.TextRep = "╥";
			roomExitD_Round.TextRep2 = "┉";
			roomExitD_Round.DoorProb = 0;
			roomExitD_Round.IsRoomExit = true;
			roomExitD_Round.ForceGrowthCompatible = false;
			
			roomExitL_Round.ConnectsLeft = true;
			roomExitL_Round.TraversableUp = true;
			roomExitL_Round.TraversableDown = true;
			roomExitL_Round.TraversableLeft = true;
			roomExitL_Round.TraversableRight = true;
			roomExitL_Round.RoomConnectsUp = true;
			roomExitL_Round.RoomConnectsDown = true;
			roomExitL_Round.InitialAvailableConnections = 1;
			roomExitL_Round.TextRep = "╡";
			roomExitL_Round.TextRep2 = " ";
			roomExitL_Round.DoorProb = 0;
			roomExitL_Round.IsRoomExit = true;
			roomExitL_Round.ForceGrowthCompatible = false;
			
			roomExitR_Round.ConnectsRight = true;
			roomExitR_Round.TraversableUp = true;
			roomExitR_Round.TraversableDown = true;
			roomExitR_Round.TraversableLeft = true;
			roomExitR_Round.TraversableRight = true;
			roomExitR_Round.RoomConnectsUp = true;
			roomExitR_Round.RoomConnectsDown = true;
			roomExitR_Round.InitialAvailableConnections = 1;
			roomExitR_Round.TextRep = "╞";
			roomExitR_Round.TextRep2 = "═";
			roomExitR_Round.DoorProb = 0;
			roomExitR_Round.IsRoomExit = true;
			roomExitR_Round.ForceGrowthCompatible = false;
			
			// *** ROOM CORNER EXITS ***
			
			roomExitUL_U.ConnectsUp = true;
			roomExitUL_U.TraversableUp = true;
			roomExitUL_U.TraversableDown = true;
			roomExitUL_U.TraversableRight = true;
			roomExitUL_U.RoomConnectsDown = true;
			roomExitUL_U.RoomConnectsRight = true;
			roomExitUL_U.InitialAvailableConnections = 1;
			roomExitUL_U.TextRep = "┞";
			roomExitUL_U.TextRep2 = "─";
			roomExitUL_U.DoorProb = 50;
			roomExitUL_U.IsRoomExit = true;
			roomExitUL_U.RoomExitCompatible = true;
			roomExitUL_U.ForceGrowthCompatible = false;
			
			roomExitUL_L.ConnectsLeft = true;
			roomExitUL_L.TraversableDown = true;
			roomExitUL_L.TraversableLeft = true;
			roomExitUL_L.TraversableRight = true;
			roomExitUL_L.RoomConnectsDown = true;
			roomExitUL_L.RoomConnectsRight = true;
			roomExitUL_L.InitialAvailableConnections = 1;
			roomExitUL_L.TextRep = "┭";
			roomExitUL_L.TextRep2 = "─";
			roomExitUL_L.DoorProb = 50;
			roomExitUL_L.IsRoomExit = true;
			roomExitUL_L.RoomExitCompatible = true;
			roomExitUL_L.ForceGrowthCompatible = false;
			
			roomExitUL_UL.ConnectsUp = true;
			roomExitUL_UL.ConnectsLeft = true;
			roomExitUL_UL.TraversableUp = true;
			roomExitUL_UL.TraversableDown = true;
			roomExitUL_UL.TraversableLeft = true;
			roomExitUL_UL.TraversableRight = true;
			roomExitUL_UL.RoomConnectsDown = true;
			roomExitUL_UL.RoomConnectsRight = true;
			roomExitUL_UL.InitialAvailableConnections = 2;
			roomExitUL_UL.TextRep = "╃";
			roomExitUL_UL.TextRep2 = "─";
			roomExitUL_UL.DoorProb = 50;
			roomExitUL_UL.IsRoomExit = true;
			roomExitUL_UL.ForceGrowthCompatible = false;
			
			roomExitUR_U.ConnectsUp = true;
			roomExitUR_U.TraversableUp = true;
			roomExitUR_U.TraversableDown = true;
			roomExitUR_U.TraversableLeft = true;
			roomExitUR_U.RoomConnectsDown = true;
			roomExitUR_U.RoomConnectsLeft = true;
			roomExitUR_U.InitialAvailableConnections = 1;
			roomExitUR_U.TextRep = "┦";
			roomExitUR_U.TextRep2 = " ";
			roomExitUR_U.DoorProb = 50;
			roomExitUR_U.IsRoomExit = true;
			roomExitUR_U.RoomExitCompatible = true;
			roomExitUR_U.ForceGrowthCompatible = false;
			
			roomExitUR_R.ConnectsRight = true;
			roomExitUR_R.TraversableDown = true;
			roomExitUR_R.TraversableLeft = true;
			roomExitUR_R.TraversableRight = true;
			roomExitUR_R.RoomConnectsDown = true;
			roomExitUR_R.RoomConnectsLeft = true;
			roomExitUR_R.InitialAvailableConnections = 1;
			roomExitUR_R.TextRep = "┮";
			roomExitUR_R.TextRep2 = "═";
			roomExitUR_R.DoorProb = 50;
			roomExitUR_R.IsRoomExit = true;
			roomExitUR_R.RoomExitCompatible = true;
			roomExitUR_R.ForceGrowthCompatible = false;
			
			roomExitUR_UR.ConnectsUp = true;
			roomExitUR_UR.ConnectsRight = true;
			roomExitUR_UR.TraversableUp = true;
			roomExitUR_UR.TraversableDown = true;
			roomExitUR_UR.TraversableLeft = true;
			roomExitUR_UR.TraversableRight = true;
			roomExitUR_UR.RoomConnectsDown = true;
			roomExitUR_UR.RoomConnectsLeft = true;
			roomExitUR_UR.InitialAvailableConnections = 2;
			roomExitUR_UR.TextRep = "╄";
			roomExitUR_UR.TextRep2 = "═";
			roomExitUR_UR.DoorProb = 50;
			roomExitUR_UR.IsRoomExit = true;
			roomExitUR_UR.ForceGrowthCompatible = false;
			
			roomExitDL_D.ConnectsDown = true;
			roomExitDL_D.TraversableUp = true;
			roomExitDL_D.TraversableDown = true;
			roomExitDL_D.TraversableRight = true;
			roomExitDL_D.RoomConnectsUp = true;
			roomExitDL_D.RoomConnectsRight = true;
			roomExitDL_D.InitialAvailableConnections = 1;
			roomExitDL_D.TextRep = "┟";
			roomExitDL_D.TextRep2 = "─";
			roomExitDL_D.DoorProb = 50;
			roomExitDL_D.IsRoomExit = true;
			roomExitDL_D.RoomExitCompatible = true;
			roomExitDL_D.ForceGrowthCompatible = false;
			
			roomExitDL_L.ConnectsLeft = true;
			roomExitDL_L.TraversableUp = true;
			roomExitDL_L.TraversableLeft = true;
			roomExitDL_L.TraversableRight = true;
			roomExitDL_L.RoomConnectsUp = true;
			roomExitDL_L.RoomConnectsRight = true;
			roomExitDL_L.InitialAvailableConnections = 1;
			roomExitDL_L.TextRep = "┵";
			roomExitDL_L.TextRep2 = "─";
			roomExitDL_L.DoorProb = 50;
			roomExitDL_L.IsRoomExit = true;
			roomExitDL_L.RoomExitCompatible = true;
			roomExitDL_L.ForceGrowthCompatible = false;
			
			roomExitDL_DL.ConnectsDown = true;
			roomExitDL_DL.ConnectsLeft = true;
			roomExitDL_DL.TraversableUp = true;
			roomExitDL_DL.TraversableDown = true;
			roomExitDL_DL.TraversableLeft = true;
			roomExitDL_DL.TraversableRight = true;
			roomExitDL_DL.RoomConnectsUp = true;
			roomExitDL_DL.RoomConnectsRight = true;
			roomExitDL_DL.InitialAvailableConnections = 2;
			roomExitDL_DL.TextRep = "╅";
			roomExitDL_DL.TextRep2 = "─";
			roomExitDL_DL.DoorProb = 50;
			roomExitDL_DL.IsRoomExit = true;
			roomExitDL_DL.ForceGrowthCompatible = false;
			
			roomExitDR_D.ConnectsDown = true;
			roomExitDR_D.TraversableUp = true;
			roomExitDR_D.TraversableDown = true;
			roomExitDR_D.TraversableLeft = true;
			roomExitDR_D.RoomConnectsUp = true;
			roomExitDR_D.RoomConnectsLeft = true;
			roomExitDR_D.InitialAvailableConnections = 1;
			roomExitDR_D.TextRep = "┧";
			roomExitDR_D.TextRep2 = " ";
			roomExitDR_D.DoorProb = 50;
			roomExitDR_D.IsRoomExit = true;
			roomExitDR_D.RoomExitCompatible = true;
			roomExitDR_D.ForceGrowthCompatible = false;
			
			roomExitDR_R.ConnectsRight = true;
			roomExitDR_R.TraversableUp = true;
			roomExitDR_R.TraversableLeft = true;
			roomExitDR_R.TraversableRight = true;
			roomExitDR_R.RoomConnectsUp = true;
			roomExitDR_R.RoomConnectsLeft = true;
			roomExitDR_R.InitialAvailableConnections = 1;
			roomExitDR_R.TextRep = "┶";
			roomExitDR_R.TextRep2 = "═";
			roomExitDR_R.DoorProb = 50;
			roomExitDR_R.IsRoomExit = true;
			roomExitDR_R.RoomExitCompatible = true;
			roomExitDR_R.ForceGrowthCompatible = false;
			
			roomExitDR_DR.ConnectsDown = true;
			roomExitDR_DR.ConnectsRight = true;
			roomExitDR_DR.TraversableUp = true;
			roomExitDR_DR.TraversableDown = true;
			roomExitDR_DR.TraversableLeft = true;
			roomExitDR_DR.TraversableRight = true;
			roomExitDR_DR.RoomConnectsUp = true;
			roomExitDR_DR.RoomConnectsLeft = true;
			roomExitDR_DR.InitialAvailableConnections = 2;
			roomExitDR_DR.TextRep = "╆";
			roomExitDR_DR.TextRep2 = "═";
			roomExitDR_DR.DoorProb = 50;
			roomExitDR_DR.IsRoomExit = true;
			roomExitDR_DR.ForceGrowthCompatible = false;

			// *** END ROOM CELLS ***
			
			// *** BEGIN STAIRWAY CELLS ***
			// These cells are specially placed, prior the falsermal cell placement routine.
			
			upStairU.Weight = 1;
			upStairU.ConnectsUp = true;
			upStairU.TraversableUp = true;
			upStairU.TextRep = "^";
			upStairU.TextRep2 = " ";
			upStairU.InitialAvailableConnections = 1;
			upStairU.ForceGrowthCompatible = false;
			
			upStairD.Weight = 1;
			upStairD.ConnectsDown = true;
			upStairD.TraversableDown = true;
			upStairD.TextRep = "^";
			upStairD.TextRep2 = " ";
			upStairD.InitialAvailableConnections = 1;
			upStairD.ForceGrowthCompatible = false;
			
			upStairL.Weight = 1;
			upStairL.ConnectsLeft = true;
			upStairL.TraversableLeft = true;
			upStairL.TextRep = "^";
			upStairL.TextRep2 = " ";
			upStairL.InitialAvailableConnections = 1;
			upStairL.ForceGrowthCompatible = false;
			
			upStairR.Weight = 1;
			upStairR.ConnectsRight = true;
			upStairR.TraversableRight = true;
			upStairR.TextRep = "^";
			upStairR.TextRep2 = " ";
			upStairR.InitialAvailableConnections = 1;
			upStairR.ForceGrowthCompatible = false;
			
			downStairU.Weight = 1;
			downStairU.ConnectsUp = true;
			downStairU.TraversableUp = true;
			downStairU.TextRep = "v";
			downStairU.TextRep2 = " ";
			downStairU.InitialAvailableConnections = 1;
			downStairU.ForceGrowthCompatible = false;
			
			downStairD.Weight = 1;
			downStairD.ConnectsDown = true;
			downStairD.TraversableDown = true;
			downStairD.TextRep = "v";
			downStairD.TextRep2 = " ";
			downStairD.InitialAvailableConnections = 1;
			downStairD.ForceGrowthCompatible = false;
			
			downStairL.Weight = 1;
			downStairL.ConnectsLeft = true;
			downStairL.TraversableLeft = true;
			downStairL.TextRep = "v";
			downStairL.TextRep2 = " ";
			downStairL.InitialAvailableConnections = 1;
			downStairL.ForceGrowthCompatible = false;
			
			downStairR.Weight = 1;
			downStairR.ConnectsRight = true;
			downStairR.TraversableRight = true;
			downStairR.TextRep = "v";
			downStairR.TextRep2 = " ";
			downStairR.InitialAvailableConnections = 1;
			downStairR.ForceGrowthCompatible = false;
			
			// *** END STAIRWAY CELLS ***	
		}

		// Edge: meaning the extreme edge of the level's grid.
		public static List<CellType> GetTypes(Coords coords)
		{
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

		public static CellType ConvRoomWallToExit(CellType wall, Direction dir)
		{
			CellType exit;
			
			if (wall == roomWallU)
				exit = roomExitU;
			else if (wall == roomWallD)
				exit = roomExitD;
			else if (wall == roomWallL)
				exit = roomExitL; 
			else if (wall == roomWallR)
				exit = roomExitR;
		
			else if (wall == roomWallUL && dir == Direction.Up)
				exit = roomExitUL_U;
			else if (wall == roomWallUL && dir == Direction.Left)
				exit = roomExitUL_L;
			else if (wall == roomWallUR && dir == Direction.Up) 
				exit = roomExitUR_U;
			else if (wall == roomWallUR && dir == Direction.Right)
				exit = roomExitUR_R;
			else if (wall == roomWallDL && dir == Direction.Down)
				exit = roomExitDL_D;
			else if (wall == roomWallDL && dir == Direction.Left)
				exit = roomExitDL_L;
			else if (wall == roomWallDR && dir == Direction.Down)
				exit = roomExitDR_D;
			else if (wall == roomWallDR && dir == Direction.Right)
				exit = roomExitDR_R;
			
			else if (wall == roomWallUL && dir == Direction.UpLeft)
				exit = roomExitUL_UL;
			else if (wall == roomWallUR && dir == Direction.UpRight) 
				exit = roomExitUR_UR;
			else if (wall == roomWallDL && dir == Direction.DownLeft)
				exit = roomExitDL_DL;
			else if (wall == roomWallDR && dir == Direction.DownRight)
				exit = roomExitDR_DR;
			
			else if ((wall == roomExitUL_U && dir == Direction.Left) || (wall == roomExitUL_L && dir == Direction.Up))
				exit = roomExitUL_UL;
			else if ((wall == roomExitUR_U && dir == Direction.Right) || (wall == roomExitUR_R && dir == Direction.Up))
				exit = roomExitUR_UR;
			else if ((wall == roomExitDL_D && dir == Direction.Left) || (wall == roomExitDL_L && dir == Direction.Down))
				exit = roomExitDL_DL;
			else if ((wall == roomExitDR_D && dir == Direction.Right) || (wall == roomExitDR_R && dir == Direction.Down))
				exit = roomExitDR_DR;
			
			else
				exit = inter;
			
			return exit;
		}

		public static CellType ConvRoomExitToWall(CellType exit, Direction dir, CellDescription descr)
		{
			CellType wall;
			
			if (exit == roomExitU)
				wall = roomWallU;
			else if (exit == roomExitD)
				wall = roomWallD;
			else if (exit == roomExitL)
				wall = roomWallL; 
			else if (exit == roomExitR)
				wall = roomWallR; 
			
			else if (dir == Direction.Up && exit == inter && descr == CellDescriptions.Mines_Vert)
				wall = juncDLR;
			else if (dir == Direction.Down && exit == inter && descr == CellDescriptions.Mines_Vert)
				wall = juncULR;
			else if (dir == Direction.Left && exit == inter && descr == CellDescriptions.Mines_Horiz)
				wall = juncUDR;
			else if (dir == Direction.Right && exit == inter && descr == CellDescriptions.Mines_Horiz)
				wall = juncUDL;
			
			else if (exit == roomExitUL_U || exit == roomExitUL_L)
				wall = roomWallUL;
			else if (exit == roomExitUR_U || exit == roomExitUR_R)
				wall = roomWallUR;
			else if (exit == roomExitDL_D || exit == roomExitDL_L)
				wall = roomWallDL;
			else if (exit == roomExitDR_D || exit == roomExitDR_R)
				wall = roomWallDR;
			
			else if (exit == roomExitUL_UL && dir == Direction.Up)
				wall = roomExitUL_L;
			else if (exit == roomExitUL_UL && dir == Direction.Left)
				wall = roomExitUL_U;
			else if (exit == roomExitUR_UR && dir == Direction.Up)
				wall = roomExitUR_R;
			else if (exit == roomExitUR_UR && dir == Direction.Right)
				wall = roomExitUR_U;
			else if (exit == roomExitDL_DL && dir == Direction.Down)
				wall = roomExitDL_L;
			else if (exit == roomExitDL_DL && dir == Direction.Left)
				wall = roomExitDL_D;
			else if (exit == roomExitDR_DR && dir == Direction.Down)
				wall = roomExitDR_R;
			else if (exit == roomExitDR_DR && dir == Direction.Right)
				wall = roomExitDR_D;
			
			else
				throw new LevelGenerateException();  // Unknown... scrap it (never happens).
			
			return wall;
		}

		public static CellType ConvDeadEndToDownStairs(CellType deadEnd)
		{    
			CellType stairs = null;
			
			if (deadEnd == deadU)
				stairs = downStairU;
			else if (deadEnd == deadD)
				stairs = downStairD;
			else if (deadEnd == deadL)
				stairs = downStairL; 
			else if (deadEnd == deadR)
				stairs = downStairR; 
			
			return stairs;
		}

		public static bool IsRoomExit(CellType type)
		{
			return type == roomExitU || type == roomExitD || type == roomExitL || type == roomExitR || 
				   type == inter || type == roomExitUL_U || type == roomExitUL_L || type == roomExitUL_UL ||
				   type == roomExitUR_U || type == roomExitUR_R || type == roomExitUR_UR || type == roomExitDL_D || 
				   type == roomExitDL_L || type == roomExitDL_DL || type == roomExitDR_D || type == roomExitDR_R || 
				   type == roomExitDR_DR || type == roomExitU_Round || type == roomExitD_Round || 
				   type == roomExitL_Round || type == roomExitR_Round;
		}

		public static bool IsRoomType(CellType type)
		{
			return type == roomSpace  || type == roomWallD || type == roomWallDL || type == roomWallDR ||
				   type == roomWallL  || type == roomWallR || type == roomWallU  || type == roomWallUL || 
				   type == roomWallUR || type == roomExitD || type == roomExitL  || type == roomExitR  || 
				   type == roomExitU  || type == roomWallDLinv || type == roomWallDRinv || type == roomWallULinv ||
				   type == roomWallURinv || type == roomWallDL_Round || type == roomWallDR_Round ||
				   type == roomWallUL_Round || type == roomWallUR_Round || type == fountain || 
				   type == roomExitU_Round || type == roomExitD_Round || 
				   type == roomExitL_Round || type == roomExitR_Round ||
				   type == roomWallU_Round || type == roomWallD_Round || 
				   type == roomWallL_Round || type == roomWallR_Round;
		}

		public static bool IsRoomCorner(CellType type)
		{
			return type == roomWallDL || type == roomWallDR || type == roomWallUL || type == roomWallUR ||
				   type == roomWallDL_Round || type == roomWallDR_Round  || type == roomWallUL_Round  || 
				   type == roomWallUR_Round ;
		}

		public static bool IsDeadEnd(CellType type)
		{
			return type == deadU || type == deadD || type == deadL || type == deadR;
		}

		public static bool IsCleanStartWall(CellType type)
		{
			return type == roomWallD || type == roomWallURinv || type == roomWallULinv || type == roomExitD;
		}

		public static bool IsFloodingTransition(CellType type)
		{
			return type == vert || type == horiz;
		}

		public static bool IsFloodingIncompatible(CellType type)
		{
			return type == roomExitU_Round || type == roomExitD_Round || type == roomExitL_Round || 
				type == roomExitR_Round;
		}

		public static Direction RoomWallDirection(CellType type)
		{
			Direction dir = Direction.Up;
			
			if (type == roomWallU)
				dir = Direction.Up;
			else if (type == roomWallD)
				dir = Direction.Down;
			else if (type == roomWallL)
				dir = Direction.Left;
			else if (type == roomWallR)
				dir = Direction.Right;
			else if (type == roomWallU_Round)
				dir = Direction.Up;
			else if (type == roomWallD_Round)
				dir = Direction.Down;
			else if (type == roomWallL_Round)
				dir = Direction.Left;
			else if (type == roomWallR_Round)
				dir = Direction.Right;
			else if (type == roomWallUL)
				dir = Direction.UpLeft;
			else if (type == roomWallUR)
				dir = Direction.UpRight;
			else if (type == roomWallDL)
				dir = Direction.DownLeft;
			else if (type == roomWallDR)
				dir = Direction.DownRight;
			else if (type == roomWallUL_Round)
				dir = Direction.UpLeft;
			else if (type == roomWallUR_Round)
				dir = Direction.UpRight;
			else if (type == roomWallDL_Round)
				dir = Direction.DownLeft;
			else if (type == roomWallDR_Round)
				dir = Direction.DownRight;
			
			return dir;
		}

		public static CellType ConvRoomTypeToCatacomb(CellType roomType)
		{
			CellType catacomb;
			
			if (roomType == roomWallU) 
				catacomb = juncDLR;
			else if (roomType == roomWallD)
				catacomb = juncULR;
			else if (roomType == roomWallL) 
				catacomb = juncUDR;
			else if (roomType == roomWallR)
				catacomb = juncUDL;
			
			else if (roomType == roomWallUL)
				catacomb = elbDR;
			else if (roomType == roomWallUR) 
				catacomb = elbDL;
			else if (roomType == roomWallDL)
				catacomb = elbUR;
			else if (roomType == roomWallDR)
				catacomb = elbUL;
			
			else if (roomType == roomExitUL_U)
				catacomb = juncUDR;
			else if (roomType == roomExitUL_L) 
				catacomb = juncDLR;
		
			else if (roomType == roomExitUR_U)
				catacomb = juncUDL;
			else if (roomType == roomExitUR_R) 
				catacomb = juncDLR;
		
			else if (roomType == roomExitDL_D)
				catacomb = juncUDR;
			else if (roomType == roomExitDL_L) 
				catacomb = juncULR;
		
			else if (roomType == roomExitDR_D)
				catacomb = juncUDL;
			else if (roomType == roomExitDR_R) 
				catacomb = juncULR;
			
			else
				catacomb = inter;
			
			return catacomb;
		}
	}
}