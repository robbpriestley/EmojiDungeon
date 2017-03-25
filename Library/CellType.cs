using System.Collections.Generic;

namespace DigitalWizardry.Dungeon
{	
	public class CellType
	{
		// The "connects" members only apply to corridor cells.
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
				Another cell is compatible with the current cell if:
				a) it is empty, or
				b) in the same direction, either both cells connect, or both do not connect.
				   (In other words if in the same direction one connects but the other does not, that's bad).
			*/
					
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
		public static CellType EmptyCell = new CellType();	       // Empty, i.e. unused.
		public static CellType Entrance = new CellType();	       // Entrance Cell, used only once on level 0...
		public static CellType Vert = new CellType();		       // Vertical Corridor            
		public static CellType Horiz = new CellType();		       // Horizontal Corridor           
		public static CellType Inter = new CellType();		       // Intersection                 
		public static CellType JuncULR = new CellType();	       // Junction Up Left Right       
		public static CellType JuncUDR = new CellType();	       // Junction Up Down Right       
		public static CellType JuncDLR = new CellType();	       // Junction Down Left Right     
		public static CellType JuncUDL = new CellType();	       // Junction Up Down Left        
		public static CellType ElbUR = new CellType();		       // Elbow Up Right               
		public static CellType ElbDR = new CellType();		       // Elbow Down Right             
		public static CellType ElbDL = new CellType();		       // Elbow Down Left              
		public static CellType ElbUL = new CellType();		       // Elbow Up Left                
		public static CellType DeadU = new CellType();		       // Dead End Up                  
		public static CellType DeadD = new CellType();		       // Dead End Down                
		public static CellType DeadL = new CellType();		       // Dead End Left                
		public static CellType DeadR = new CellType();		       // Dead End Right 
		public static CellType DeadexU = new CellType();           // Dead End Exit Up                  
		public static CellType DeadexD = new CellType();           // Dead End Exit Down                
		public static CellType DeadexL = new CellType();           // Dead End Exit Left                
		public static CellType DeadexR = new CellType();           // Dead End Exit Right 
		public static CellType UpStairU = new CellType();          // Stairs Up Connects Up        
		public static CellType UpStairD = new CellType();          // Stairs Up Connects Down      
		public static CellType UpStairL = new CellType();          // Stairs Up Connects Left      
		public static CellType UpStairR = new CellType();          // Stairs Up Connects Right     
		public static CellType DownStairU = new CellType();        // Stairs Down Connects Up      
		public static CellType DownStairD = new CellType();        // Stairs Down Connects Down    
		public static CellType DownStairL = new CellType();        // Stairs Down Connects Left    
		public static CellType DownStairR = new CellType();        // Stairs Down Connects Right 
		public static CellType RoomSpace = new CellType();         // Room Space
		public static CellType Fountain = new CellType();          // Healing Fountain
		public static CellType RoomWallU = new CellType();         // Room Wall Up
		public static CellType RoomWallD = new CellType();         // Room Wall Down
		public static CellType RoomWallL = new CellType();         // Room Wall Left
		public static CellType RoomWallR = new CellType();         // Room Wall Right
		public static CellType RoomWallU_Round = new CellType();   // Room Wall Up Round
		public static CellType RoomWallD_Round = new CellType();   // Room Wall Down Round
		public static CellType RoomWallL_Round = new CellType();   // Room Wall Left Round
		public static CellType RoomWallR_Round = new CellType();   // Room Wall Right Round
		public static CellType RoomWallUL = new CellType();        // Room Wall Up Left
		public static CellType RoomWallUR = new CellType();        // Room Wall Up Right
		public static CellType RoomWallDL = new CellType();        // Room Wall Down Left
		public static CellType RoomWallDR = new CellType();        // Room Wall Down Right
		public static CellType RoomWallUL_Round = new CellType();  // Room Wall Up Left Round
		public static CellType RoomWallUR_Round = new CellType();  // Room Wall Up Right Round
		public static CellType RoomWallDL_Round = new CellType();  // Room Wall Down Left Round
		public static CellType RoomWallDR_Round = new CellType();  // Room Wall Down Right Round
		public static CellType RoomWallULinv = new CellType();     // Room Wall Up Left Inverted
		public static CellType RoomWallURinv = new CellType();     // Room Wall Up Right Inverted
		public static CellType RoomWallDLinv = new CellType();     // Room Wall Down Left Inverted
		public static CellType RoomWallDRinv = new CellType();     // Room Wall Down Right Inverted
		public static CellType RoomExitU = new CellType();         // Room Exit Up
		public static CellType RoomExitD = new CellType();         // Room Exit Down
		public static CellType RoomExitL = new CellType();         // Room Exit Left
		public static CellType RoomExitR = new CellType();         // Room Exit Right
		public static CellType RoomExitU_Round = new CellType();   // Room Exit Up Round
		public static CellType RoomExitD_Round = new CellType();   // Room Exit Down Round
		public static CellType RoomExitL_Round = new CellType();   // Room Exit Left Round
		public static CellType RoomExitR_Round = new CellType();   // Room Exit Right Round
		public static CellType RoomExitUL_U = new CellType();      // Room Exit Up Left Corner, Exit Up
		public static CellType RoomExitUL_L = new CellType();      // Room Exit Up Left Corner, Exit Left
		public static CellType RoomExitUL_UL = new CellType();     // Room Exit Up Left Corner, Exits Up and Left
		public static CellType RoomExitUR_U = new CellType();      // Room Exit Up Right Corner, Exit Up
		public static CellType RoomExitUR_R = new CellType();      // Room Exit Up Right Corner, Exit Right
		public static CellType RoomExitUR_UR = new CellType();     // Room Exit Up Right Corner, Exits Up and Right
		public static CellType RoomExitDL_D = new CellType();      // Room Exit Down Left Corner, Exit Down
		public static CellType RoomExitDL_L = new CellType();      // Room Exit Down Left Corner, Exit Left
		public static CellType RoomExitDL_DL = new CellType();     // Room Exit Down Left Corner, Exits Down and Left
		public static CellType RoomExitDR_D = new CellType();      // Room Exit Down Right Corner, Exit Down
		public static CellType RoomExitDR_R = new CellType();      // Room Exit Down Right Corner, Exit Right
		public static CellType RoomExitDR_DR = new CellType();     // Room Exit Down Right Corner, Exits Down and Right

		public static void Initialize()
		{
			// *** BEGIN SPECIAL CELLS ***
			// These cell are only ever placed in specific, known circumstances. They are 
			// never randomly assigned. Therefore, their Weight values are 0.
			// For simplicity, the empty cell is considered to connect in every direction.

			EmptyCell.Weight = 0;
			EmptyCell.IsEmpty = true;
			EmptyCell.ConnectsUp = true;
			EmptyCell.ConnectsDown = true;
			EmptyCell.ConnectsLeft = true;
			EmptyCell.ConnectsRight = true;
			EmptyCell.TextRep = @" ";
			EmptyCell.TextRep2 = @" ";
			EmptyCell.ForceGrowthCompatible = false;
			EmptyCell.InitialAvailableConnections = 4;
			
			Entrance.Weight = 0;
			Entrance.ConnectsUp = true;
			Entrance.TraversableUp = true;
			Entrance.TextRep = @"❍";
			Entrance.TextRep2 = @" ";
			Entrance.ForceGrowthCompatible = false;
			Entrance.InitialAvailableConnections = 1;
			
			// *** END SPECIAL CELLS ***
			
			// *** BEGIN CORRIDOR CELLS ***
			
			Vert.Weight = 100;
			Vert.ConnectsUp = true;
			Vert.ConnectsDown = true;
			Vert.TraversableUp = true;
			Vert.TraversableDown = true;
			Vert.TextRep = @"║";
			Vert.TextRep2 = @" ";
			Vert.InitialAvailableConnections = 2;
			
			Horiz.Weight = 100;
			Horiz.ConnectsLeft = true;
			Horiz.ConnectsRight = true;
			Horiz.TraversableLeft = true;
			Horiz.TraversableRight = true;
			Horiz.TextRep = @"═";
			Horiz.TextRep2 = @"═";
			Horiz.InitialAvailableConnections = 2;
			
			Inter.Weight = 20;
			Inter.ConnectsUp = true;
			Inter.ConnectsDown = true;
			Inter.ConnectsLeft = true;
			Inter.ConnectsRight = true;
			Inter.TraversableUp = true;
			Inter.TraversableDown = true;
			Inter.TraversableLeft = true;
			Inter.TraversableRight = true;
			Inter.TextRep = @"╬";
			Inter.TextRep2 = @"═";
			Inter.IsJunction = true;
			Inter.InitialAvailableConnections = 4;
			
			JuncULR.Weight = 20;
			JuncULR.ConnectsUp = true;
			JuncULR.ConnectsLeft = true;
			JuncULR.ConnectsRight = true;
			JuncULR.TraversableUp = true;
			JuncULR.TraversableLeft = true;
			JuncULR.TraversableRight = true;
			JuncULR.TextRep = @"╩";
			JuncULR.TextRep2 = @"═";
			JuncULR.IsJunction = true;
			JuncULR.InitialAvailableConnections = 3;
			
			JuncUDR.Weight = 20;
			JuncUDR.ConnectsUp = true;
			JuncUDR.ConnectsDown = true;
			JuncUDR.ConnectsRight = true;
			JuncUDR.TraversableUp = true;
			JuncUDR.TraversableDown = true;
			JuncUDR.TraversableRight = true;
			JuncUDR.TextRep = @"╠";
			JuncUDR.TextRep2 = @"═";
			JuncUDR.IsJunction = true;
			JuncUDR.InitialAvailableConnections = 3;
			
			JuncDLR.Weight = 20;
			JuncDLR.ConnectsDown = true;
			JuncDLR.ConnectsLeft = true;
			JuncDLR.ConnectsRight = true;
			JuncDLR.TraversableDown = true;
			JuncDLR.TraversableLeft = true;
			JuncDLR.TraversableRight = true;
			JuncDLR.TextRep = @"╦";
			JuncDLR.TextRep2 = @"═";
			JuncDLR.IsJunction = true;
			JuncDLR.InitialAvailableConnections = 3;
			
			JuncUDL.Weight = 20;
			JuncUDL.ConnectsUp = true;
			JuncUDL.ConnectsDown = true;
			JuncUDL.ConnectsLeft = true;
			JuncUDL.TraversableUp = true;
			JuncUDL.TraversableDown = true;
			JuncUDL.TraversableLeft = true;
			JuncUDL.TextRep = @"╣";
			JuncUDL.TextRep2 = @" ";
			JuncUDL.IsJunction = true;
			JuncUDL.InitialAvailableConnections = 3;
			
			ElbUR.Weight = 20;
			ElbUR.ConnectsUp = true;
			ElbUR.ConnectsRight = true;
			ElbUR.TraversableUp = true;
			ElbUR.TraversableRight = true;
			ElbUR.TextRep = @"╚";
			ElbUR.TextRep2 = @"═";
			ElbUR.InitialAvailableConnections = 2;
			
			ElbDR.Weight = 20;
			ElbDR.ConnectsDown = true;
			ElbDR.ConnectsRight = true;
			ElbDR.TraversableDown = true;
			ElbDR.TraversableRight = true;
			ElbDR.TextRep = @"╔";
			ElbDR.TextRep2 = @"═";
			ElbDR.InitialAvailableConnections = 2;
			
			ElbDL.Weight = 20;
			ElbDL.ConnectsDown = true;
			ElbDL.ConnectsLeft = true;
			ElbDL.TraversableDown = true;
			ElbDL.TraversableLeft = true;
			ElbDL.TextRep = @"╗";
			ElbDL.TextRep2 = @" ";
			ElbDL.InitialAvailableConnections = 2;
			
			ElbUL.Weight = 20;
			ElbUL.ConnectsUp = true;
			ElbUL.ConnectsLeft = true;
			ElbUL.TraversableUp = true;
			ElbUL.TraversableLeft = true;
			ElbUL.TextRep = @"╝";
			ElbUL.TextRep2 = @" ";
			ElbUL.InitialAvailableConnections = 2;

			// *** END CORRIDOR CELLS ***
			// *** BEGIN DEAD END CELLS ***
			
			DeadU.Weight = 1;
			DeadU.ConnectsUp = true;
			DeadU.TraversableUp = true;
			DeadU.TextRep = @"╨";
			DeadU.TextRep2 = @" ";
			DeadU.InitialAvailableConnections = 1;
			
			DeadD.Weight = 1;
			DeadD.ConnectsDown = true;
			DeadD.TraversableDown = true;
			DeadD.TextRep = @"╥";
			DeadD.TextRep2 = @" ";
			DeadD.InitialAvailableConnections = 1;
			
			DeadL.Weight = 1;
			DeadL.ConnectsLeft = true;
			DeadL.TraversableLeft = true;
			DeadL.TextRep = @"╡";
			DeadL.TextRep2 = @" ";
			DeadL.InitialAvailableConnections = 1;
			
			DeadR.Weight = 1;
			DeadR.ConnectsRight = true;
			DeadR.TraversableRight = true;
			DeadR.TextRep = @"╞";
			DeadR.TextRep2 = @"═";
			DeadR.InitialAvailableConnections = 1;
			
			DeadexU.ConnectsUp = true;
			DeadexU.TraversableUp = true;
			DeadexU.TextRep = @"╨";
			DeadexU.TextRep2 = @" ";
			
			DeadexD.ConnectsDown = true;
			DeadexD.TraversableDown = true;
			DeadexD.TextRep = @"╥";
			DeadexD.TextRep2 = @" ";
			
			DeadexL.ConnectsLeft = true;
			DeadexL.TraversableLeft = true;
			DeadexL.TextRep = @"╡";
			DeadexL.TextRep2 = @" ";

			DeadexR.ConnectsRight = true;
			DeadexR.TraversableRight = true;
			DeadexR.TextRep = @"╞";
			DeadexR.TextRep2 = @"═";

			// *** END DEAD END CELLS ***
			// *** BEGIN ROOM CELLS ***
			
			RoomSpace.TraversableUp = true;
			RoomSpace.TraversableDown = true;
			RoomSpace.TraversableLeft = true;
			RoomSpace.TraversableRight = true;
			RoomSpace.TextRep = " ";
			RoomSpace.TextRep2 = " ";
			RoomSpace.ForceGrowthCompatible = false;
			
			Fountain.TraversableUp = true;
			Fountain.TraversableDown = true;
			Fountain.TraversableLeft = true;
			Fountain.TraversableRight = true;
			Fountain.TextRep = "F";
			Fountain.TextRep2 = " ";
			Fountain.ForceGrowthCompatible = false;
			
			RoomWallU.TraversableDown = true;
			RoomWallU.TraversableLeft = true;
			RoomWallU.TraversableRight = true;
			RoomWallU.RoomExitCompatible = true;
			RoomWallU.RoomConnectsLeft = true;
			RoomWallU.RoomConnectsRight = true;
			RoomWallU.TextRep = "─";
			RoomWallU.TextRep2 = "─";
			RoomWallU.ForceGrowthCompatible = false;
			
			RoomWallD.TraversableUp = true;
			RoomWallD.TraversableLeft = true;
			RoomWallD.TraversableRight = true;
			RoomWallD.RoomExitCompatible = true;
			RoomWallD.RoomConnectsLeft = true;
			RoomWallD.RoomConnectsRight = true;
			RoomWallD.TextRep = "─";
			RoomWallD.TextRep2 = "─";
			RoomWallD.ForceGrowthCompatible = false;
			
			RoomWallL.TraversableUp = true;
			RoomWallL.TraversableDown = true;
			RoomWallL.TraversableRight = true;
			RoomWallL.RoomExitCompatible = true;
			RoomWallL.RoomConnectsUp = true;
			RoomWallL.RoomConnectsDown = true;
			RoomWallL.TextRep = "│";
			RoomWallL.TextRep2 = " ";
			RoomWallL.ForceGrowthCompatible = false;
			
			RoomWallR.TraversableUp = true;
			RoomWallR.TraversableDown = true;
			RoomWallR.TraversableLeft = true;
			RoomWallR.RoomExitCompatible = true;
			RoomWallR.RoomConnectsUp = true;
			RoomWallR.RoomConnectsDown = true;
			RoomWallR.TextRep = "│";
			RoomWallR.TextRep2 = " ";
			RoomWallR.ForceGrowthCompatible = false;
			
			RoomWallU_Round.TraversableDown = true;
			RoomWallU_Round.TraversableLeft = true;
			RoomWallU_Round.TraversableRight = true;
			RoomWallU_Round.RoomExitCompatible = false;
			RoomWallU_Round.RoomConnectsLeft = true;
			RoomWallU_Round.RoomConnectsRight = true;
			RoomWallU_Round.TextRep = "┉";
			RoomWallU_Round.TextRep2 = "┉";
			RoomWallU_Round.ForceGrowthCompatible = false;
			
			RoomWallD_Round.TraversableUp = true;
			RoomWallD_Round.TraversableLeft = true;
			RoomWallD_Round.TraversableRight = true;
			RoomWallD_Round.RoomExitCompatible = false;
			RoomWallD_Round.RoomConnectsLeft = true;
			RoomWallD_Round.RoomConnectsRight = true;
			RoomWallD_Round.TextRep = "┉";
			RoomWallD_Round.TextRep2 = "┉";
			RoomWallD_Round.ForceGrowthCompatible = false;
			
			RoomWallL_Round.TraversableUp = true;
			RoomWallL_Round.TraversableDown = true;
			RoomWallL_Round.TraversableRight = true;
			RoomWallL_Round.RoomExitCompatible = false;
			RoomWallL_Round.RoomConnectsUp = true;
			RoomWallL_Round.RoomConnectsDown = true;
			RoomWallL_Round.TextRep = "┋";
			RoomWallL_Round.TextRep2 = " ";
			RoomWallL_Round.ForceGrowthCompatible = false;
			
			RoomWallR_Round.TraversableUp = true;
			RoomWallR_Round.TraversableDown = true;
			RoomWallR_Round.TraversableLeft = true;
			RoomWallR_Round.RoomExitCompatible = false;
			RoomWallR_Round.RoomConnectsUp = true;
			RoomWallR_Round.RoomConnectsDown = true;
			RoomWallR_Round.TextRep = "┋";
			RoomWallR_Round.TextRep2 = " ";
			RoomWallR_Round.ForceGrowthCompatible = false;
			
			RoomWallUL.TraversableDown = true;
			RoomWallUL.TraversableRight = true;
			RoomWallUL.RoomExitCompatible = true;
			RoomWallUL.RoomConnectsDown = true;
			RoomWallUL.RoomConnectsRight = true;
			RoomWallUL.TextRep = "┌";
			RoomWallUL.TextRep2 = "─";
			RoomWallUL.ForceGrowthCompatible = false;
			
			RoomWallUR.TraversableDown = true;
			RoomWallUR.TraversableLeft = true;
			RoomWallUR.RoomExitCompatible = true;
			RoomWallUR.RoomConnectsDown = true;
			RoomWallUR.RoomConnectsLeft = true;
			RoomWallUR.TextRep = "┐";
			RoomWallUR.TextRep2 = " ";
			RoomWallUR.ForceGrowthCompatible = false;
			
			RoomWallDL.TraversableUp = true;
			RoomWallDL.TraversableRight = true;
			RoomWallDL.RoomExitCompatible = true;
			RoomWallDL.RoomConnectsUp = true;
			RoomWallDL.RoomConnectsRight = true;
			RoomWallDL.TextRep = "└";
			RoomWallDL.TextRep2 = "─";
			RoomWallDL.ForceGrowthCompatible = false;
			
			RoomWallDR.TraversableUp = true;
			RoomWallDR.TraversableLeft = true;
			RoomWallDR.RoomExitCompatible = true;
			RoomWallDR.RoomConnectsUp = true;
			RoomWallDR.RoomConnectsLeft = true;
			RoomWallDR.TextRep = "┘";
			RoomWallDR.TextRep2 = " ";
			RoomWallDR.ForceGrowthCompatible = false;
			
			RoomWallUL_Round.TraversableDown = true;
			RoomWallUL_Round.TraversableRight = true;
			RoomWallUL_Round.RoomExitCompatible = false;
			RoomWallUL_Round.RoomConnectsDown = true;
			RoomWallUL_Round.RoomConnectsRight = true;
			RoomWallUL_Round.TextRep = "╭";
			RoomWallUL_Round.TextRep2 = "─";
			RoomWallUL_Round.ForceGrowthCompatible = false;
			
			RoomWallUR_Round.TraversableDown = true;
			RoomWallUR_Round.TraversableLeft = true;
			RoomWallUR_Round.RoomExitCompatible = false;
			RoomWallUR_Round.RoomConnectsDown = true;
			RoomWallUR_Round.RoomConnectsLeft = true;
			RoomWallUR_Round.TextRep = "╮";
			RoomWallUR_Round.TextRep2 = " ";
			RoomWallUR_Round.ForceGrowthCompatible = false;
			
			RoomWallDL_Round.TraversableUp = true;
			RoomWallDL_Round.TraversableRight = true;
			RoomWallDL_Round.RoomExitCompatible = false;
			RoomWallDL_Round.RoomConnectsUp = true;
			RoomWallDL_Round.RoomConnectsRight = true;
			RoomWallDL_Round.TextRep = "╰";
			RoomWallDL_Round.TextRep2 = "─";
			RoomWallDL_Round.ForceGrowthCompatible = false;
			
			RoomWallDR_Round.TraversableUp = true;
			RoomWallDR_Round.TraversableLeft = true;
			RoomWallDR_Round.RoomExitCompatible = false;
			RoomWallDR_Round.RoomConnectsUp = true;
			RoomWallDR_Round.RoomConnectsLeft = true;
			RoomWallDR_Round.TextRep = "╯";
			RoomWallDR_Round.TextRep2 = " ";
			RoomWallDR_Round.ForceGrowthCompatible = false;
			
			RoomWallULinv.TraversableDown = true;
			RoomWallULinv.TraversableRight = true;
			RoomWallULinv.RoomConnectsDown = true;
			RoomWallULinv.RoomConnectsRight = true;
			RoomWallULinv.TextRep = "┌";
			RoomWallULinv.TextRep2 = "─";
			RoomWallULinv.ForceGrowthCompatible = false;
			
			RoomWallURinv.TraversableDown = true;
			RoomWallURinv.TraversableLeft = true;
			RoomWallURinv.RoomConnectsDown = true;
			RoomWallURinv.RoomConnectsLeft = true;
			RoomWallURinv.TextRep = "┐";
			RoomWallURinv.TextRep2 = " ";
			RoomWallURinv.ForceGrowthCompatible = false;
			
			RoomWallDLinv.TraversableUp = true;
			RoomWallDLinv.TraversableRight = true;
			RoomWallDLinv.RoomConnectsUp = true;
			RoomWallDLinv.RoomConnectsRight = true;
			RoomWallDLinv.TextRep = "└";
			RoomWallDLinv.TextRep2 = "─";
			RoomWallDLinv.ForceGrowthCompatible = false;
			
			RoomWallDRinv.TraversableUp = true;
			RoomWallDRinv.TraversableLeft = true;
			RoomWallDRinv.RoomConnectsUp = true;
			RoomWallDRinv.RoomConnectsLeft = true;
			RoomWallDRinv.TextRep = "┘";
			RoomWallDRinv.TextRep2 = " ";
			RoomWallDRinv.ForceGrowthCompatible = false;
			
			RoomExitU.ConnectsUp = true;
			RoomExitU.TraversableUp = true;
			RoomExitU.TraversableDown = true;
			RoomExitU.TraversableLeft = true;
			RoomExitU.TraversableRight = true;
			RoomExitU.RoomConnectsLeft = true;
			RoomExitU.RoomConnectsRight = true;
			RoomExitU.InitialAvailableConnections = 1;
			RoomExitU.TextRep = "╨";
			RoomExitU.TextRep2 = "─";
			RoomExitU.DoorProb = 50;
			RoomExitU.IsRoomExit = true;
			RoomExitU.ForceGrowthCompatible = false;
			
			RoomExitD.ConnectsDown = true;
			RoomExitD.TraversableUp = true;
			RoomExitD.TraversableDown = true;
			RoomExitD.TraversableLeft = true;
			RoomExitD.TraversableRight = true;
			RoomExitD.RoomConnectsLeft = true;
			RoomExitD.RoomConnectsRight = true;
			RoomExitD.InitialAvailableConnections = 1;
			RoomExitD.TextRep = "╥";
			RoomExitD.TextRep2 = "─";
			RoomExitD.DoorProb = 50;
			RoomExitD.IsRoomExit = true;
			RoomExitD.ForceGrowthCompatible = false;
			
			RoomExitL.ConnectsLeft = true;
			RoomExitL.TraversableUp = true;
			RoomExitL.TraversableDown = true;
			RoomExitL.TraversableLeft = true;
			RoomExitL.TraversableRight = true;
			RoomExitL.RoomConnectsUp = true;
			RoomExitL.RoomConnectsDown = true;
			RoomExitL.InitialAvailableConnections = 1;
			RoomExitL.TextRep = "╡";
			RoomExitL.TextRep2 = " ";
			RoomExitL.DoorProb = 50;
			RoomExitL.IsRoomExit = true;
			RoomExitL.ForceGrowthCompatible = false;
			
			RoomExitR.ConnectsRight = true;
			RoomExitR.TraversableUp = true;
			RoomExitR.TraversableDown = true;
			RoomExitR.TraversableLeft = true;
			RoomExitR.TraversableRight = true;
			RoomExitR.RoomConnectsUp = true;
			RoomExitR.RoomConnectsDown = true;
			RoomExitR.InitialAvailableConnections = 1;
			RoomExitR.TextRep = "╞";
			RoomExitR.TextRep2 = "═";
			RoomExitR.DoorProb = 50;
			RoomExitR.IsRoomExit = true;
			RoomExitR.ForceGrowthCompatible = false;
			
			RoomExitU_Round.ConnectsUp = true;
			RoomExitU_Round.TraversableUp = true;
			RoomExitU_Round.TraversableDown = true;
			RoomExitU_Round.TraversableLeft = true;
			RoomExitU_Round.TraversableRight = true;
			RoomExitU_Round.RoomConnectsLeft = true;
			RoomExitU_Round.RoomConnectsRight = true;
			RoomExitU_Round.InitialAvailableConnections = 1;
			RoomExitU_Round.TextRep = "╨";
			RoomExitU_Round.TextRep2 = "┉";
			RoomExitU_Round.DoorProb = 0;
			RoomExitU_Round.IsRoomExit = true;
			RoomExitU_Round.ForceGrowthCompatible = false;
			
			RoomExitD_Round.ConnectsDown = true;
			RoomExitD_Round.TraversableUp = true;
			RoomExitD_Round.TraversableDown = true;
			RoomExitD_Round.TraversableLeft = true;
			RoomExitD_Round.TraversableRight = true;
			RoomExitD_Round.RoomConnectsLeft = true;
			RoomExitD_Round.RoomConnectsRight = true;
			RoomExitD_Round.InitialAvailableConnections = 1;
			RoomExitD_Round.TextRep = "╥";
			RoomExitD_Round.TextRep2 = "┉";
			RoomExitD_Round.DoorProb = 0;
			RoomExitD_Round.IsRoomExit = true;
			RoomExitD_Round.ForceGrowthCompatible = false;
			
			RoomExitL_Round.ConnectsLeft = true;
			RoomExitL_Round.TraversableUp = true;
			RoomExitL_Round.TraversableDown = true;
			RoomExitL_Round.TraversableLeft = true;
			RoomExitL_Round.TraversableRight = true;
			RoomExitL_Round.RoomConnectsUp = true;
			RoomExitL_Round.RoomConnectsDown = true;
			RoomExitL_Round.InitialAvailableConnections = 1;
			RoomExitL_Round.TextRep = "╡";
			RoomExitL_Round.TextRep2 = " ";
			RoomExitL_Round.DoorProb = 0;
			RoomExitL_Round.IsRoomExit = true;
			RoomExitL_Round.ForceGrowthCompatible = false;
			
			RoomExitR_Round.ConnectsRight = true;
			RoomExitR_Round.TraversableUp = true;
			RoomExitR_Round.TraversableDown = true;
			RoomExitR_Round.TraversableLeft = true;
			RoomExitR_Round.TraversableRight = true;
			RoomExitR_Round.RoomConnectsUp = true;
			RoomExitR_Round.RoomConnectsDown = true;
			RoomExitR_Round.InitialAvailableConnections = 1;
			RoomExitR_Round.TextRep = "╞";
			RoomExitR_Round.TextRep2 = "═";
			RoomExitR_Round.DoorProb = 0;
			RoomExitR_Round.IsRoomExit = true;
			RoomExitR_Round.ForceGrowthCompatible = false;
			
			// *** END ROOM CELLS ***
			// *** ROOM CORNER EXITS ***
			
			RoomExitUL_U.ConnectsUp = true;
			RoomExitUL_U.TraversableUp = true;
			RoomExitUL_U.TraversableDown = true;
			RoomExitUL_U.TraversableRight = true;
			RoomExitUL_U.RoomConnectsDown = true;
			RoomExitUL_U.RoomConnectsRight = true;
			RoomExitUL_U.InitialAvailableConnections = 1;
			RoomExitUL_U.TextRep = "┞";
			RoomExitUL_U.TextRep2 = "─";
			RoomExitUL_U.DoorProb = 50;
			RoomExitUL_U.IsRoomExit = true;
			RoomExitUL_U.RoomExitCompatible = true;
			RoomExitUL_U.ForceGrowthCompatible = false;
			
			RoomExitUL_L.ConnectsLeft = true;
			RoomExitUL_L.TraversableDown = true;
			RoomExitUL_L.TraversableLeft = true;
			RoomExitUL_L.TraversableRight = true;
			RoomExitUL_L.RoomConnectsDown = true;
			RoomExitUL_L.RoomConnectsRight = true;
			RoomExitUL_L.InitialAvailableConnections = 1;
			RoomExitUL_L.TextRep = "┭";
			RoomExitUL_L.TextRep2 = "─";
			RoomExitUL_L.DoorProb = 50;
			RoomExitUL_L.IsRoomExit = true;
			RoomExitUL_L.RoomExitCompatible = true;
			RoomExitUL_L.ForceGrowthCompatible = false;
			
			RoomExitUL_UL.ConnectsUp = true;
			RoomExitUL_UL.ConnectsLeft = true;
			RoomExitUL_UL.TraversableUp = true;
			RoomExitUL_UL.TraversableDown = true;
			RoomExitUL_UL.TraversableLeft = true;
			RoomExitUL_UL.TraversableRight = true;
			RoomExitUL_UL.RoomConnectsDown = true;
			RoomExitUL_UL.RoomConnectsRight = true;
			RoomExitUL_UL.InitialAvailableConnections = 2;
			RoomExitUL_UL.TextRep = "╃";
			RoomExitUL_UL.TextRep2 = "─";
			RoomExitUL_UL.DoorProb = 50;
			RoomExitUL_UL.IsRoomExit = true;
			RoomExitUL_UL.ForceGrowthCompatible = false;
			
			RoomExitUR_U.ConnectsUp = true;
			RoomExitUR_U.TraversableUp = true;
			RoomExitUR_U.TraversableDown = true;
			RoomExitUR_U.TraversableLeft = true;
			RoomExitUR_U.RoomConnectsDown = true;
			RoomExitUR_U.RoomConnectsLeft = true;
			RoomExitUR_U.InitialAvailableConnections = 1;
			RoomExitUR_U.TextRep = "┦";
			RoomExitUR_U.TextRep2 = " ";
			RoomExitUR_U.DoorProb = 50;
			RoomExitUR_U.IsRoomExit = true;
			RoomExitUR_U.RoomExitCompatible = true;
			RoomExitUR_U.ForceGrowthCompatible = false;
			
			RoomExitUR_R.ConnectsRight = true;
			RoomExitUR_R.TraversableDown = true;
			RoomExitUR_R.TraversableLeft = true;
			RoomExitUR_R.TraversableRight = true;
			RoomExitUR_R.RoomConnectsDown = true;
			RoomExitUR_R.RoomConnectsLeft = true;
			RoomExitUR_R.InitialAvailableConnections = 1;
			RoomExitUR_R.TextRep = "┮";
			RoomExitUR_R.TextRep2 = "═";
			RoomExitUR_R.DoorProb = 50;
			RoomExitUR_R.IsRoomExit = true;
			RoomExitUR_R.RoomExitCompatible = true;
			RoomExitUR_R.ForceGrowthCompatible = false;
			
			RoomExitUR_UR.ConnectsUp = true;
			RoomExitUR_UR.ConnectsRight = true;
			RoomExitUR_UR.TraversableUp = true;
			RoomExitUR_UR.TraversableDown = true;
			RoomExitUR_UR.TraversableLeft = true;
			RoomExitUR_UR.TraversableRight = true;
			RoomExitUR_UR.RoomConnectsDown = true;
			RoomExitUR_UR.RoomConnectsLeft = true;
			RoomExitUR_UR.InitialAvailableConnections = 2;
			RoomExitUR_UR.TextRep = "╄";
			RoomExitUR_UR.TextRep2 = "═";
			RoomExitUR_UR.DoorProb = 50;
			RoomExitUR_UR.IsRoomExit = true;
			RoomExitUR_UR.ForceGrowthCompatible = false;
			
			RoomExitDL_D.ConnectsDown = true;
			RoomExitDL_D.TraversableUp = true;
			RoomExitDL_D.TraversableDown = true;
			RoomExitDL_D.TraversableRight = true;
			RoomExitDL_D.RoomConnectsUp = true;
			RoomExitDL_D.RoomConnectsRight = true;
			RoomExitDL_D.InitialAvailableConnections = 1;
			RoomExitDL_D.TextRep = "┟";
			RoomExitDL_D.TextRep2 = "─";
			RoomExitDL_D.DoorProb = 50;
			RoomExitDL_D.IsRoomExit = true;
			RoomExitDL_D.RoomExitCompatible = true;
			RoomExitDL_D.ForceGrowthCompatible = false;
			
			RoomExitDL_L.ConnectsLeft = true;
			RoomExitDL_L.TraversableUp = true;
			RoomExitDL_L.TraversableLeft = true;
			RoomExitDL_L.TraversableRight = true;
			RoomExitDL_L.RoomConnectsUp = true;
			RoomExitDL_L.RoomConnectsRight = true;
			RoomExitDL_L.InitialAvailableConnections = 1;
			RoomExitDL_L.TextRep = "┵";
			RoomExitDL_L.TextRep2 = "─";
			RoomExitDL_L.DoorProb = 50;
			RoomExitDL_L.IsRoomExit = true;
			RoomExitDL_L.RoomExitCompatible = true;
			RoomExitDL_L.ForceGrowthCompatible = false;
			
			RoomExitDL_DL.ConnectsDown = true;
			RoomExitDL_DL.ConnectsLeft = true;
			RoomExitDL_DL.TraversableUp = true;
			RoomExitDL_DL.TraversableDown = true;
			RoomExitDL_DL.TraversableLeft = true;
			RoomExitDL_DL.TraversableRight = true;
			RoomExitDL_DL.RoomConnectsUp = true;
			RoomExitDL_DL.RoomConnectsRight = true;
			RoomExitDL_DL.InitialAvailableConnections = 2;
			RoomExitDL_DL.TextRep = "╅";
			RoomExitDL_DL.TextRep2 = "─";
			RoomExitDL_DL.DoorProb = 50;
			RoomExitDL_DL.IsRoomExit = true;
			RoomExitDL_DL.ForceGrowthCompatible = false;
			
			RoomExitDR_D.ConnectsDown = true;
			RoomExitDR_D.TraversableUp = true;
			RoomExitDR_D.TraversableDown = true;
			RoomExitDR_D.TraversableLeft = true;
			RoomExitDR_D.RoomConnectsUp = true;
			RoomExitDR_D.RoomConnectsLeft = true;
			RoomExitDR_D.InitialAvailableConnections = 1;
			RoomExitDR_D.TextRep = "┧";
			RoomExitDR_D.TextRep2 = " ";
			RoomExitDR_D.DoorProb = 50;
			RoomExitDR_D.IsRoomExit = true;
			RoomExitDR_D.RoomExitCompatible = true;
			RoomExitDR_D.ForceGrowthCompatible = false;
			
			RoomExitDR_R.ConnectsRight = true;
			RoomExitDR_R.TraversableUp = true;
			RoomExitDR_R.TraversableLeft = true;
			RoomExitDR_R.TraversableRight = true;
			RoomExitDR_R.RoomConnectsUp = true;
			RoomExitDR_R.RoomConnectsLeft = true;
			RoomExitDR_R.InitialAvailableConnections = 1;
			RoomExitDR_R.TextRep = "┶";
			RoomExitDR_R.TextRep2 = "═";
			RoomExitDR_R.DoorProb = 50;
			RoomExitDR_R.IsRoomExit = true;
			RoomExitDR_R.RoomExitCompatible = true;
			RoomExitDR_R.ForceGrowthCompatible = false;
			
			RoomExitDR_DR.ConnectsDown = true;
			RoomExitDR_DR.ConnectsRight = true;
			RoomExitDR_DR.TraversableUp = true;
			RoomExitDR_DR.TraversableDown = true;
			RoomExitDR_DR.TraversableLeft = true;
			RoomExitDR_DR.TraversableRight = true;
			RoomExitDR_DR.RoomConnectsUp = true;
			RoomExitDR_DR.RoomConnectsLeft = true;
			RoomExitDR_DR.InitialAvailableConnections = 2;
			RoomExitDR_DR.TextRep = "╆";
			RoomExitDR_DR.TextRep2 = "═";
			RoomExitDR_DR.DoorProb = 50;
			RoomExitDR_DR.IsRoomExit = true;
			RoomExitDR_DR.ForceGrowthCompatible = false;

			// *** END ROOM CORNER EXITS ***
			// *** BEGIN STAIRWAY CELLS ***
			// These cells are specially placed, prior the falsermal cell placement routine.
			
			UpStairU.Weight = 1;
			UpStairU.ConnectsUp = true;
			UpStairU.TraversableUp = true;
			UpStairU.TextRep = "^";
			UpStairU.TextRep2 = " ";
			UpStairU.InitialAvailableConnections = 1;
			UpStairU.ForceGrowthCompatible = false;
			
			UpStairD.Weight = 1;
			UpStairD.ConnectsDown = true;
			UpStairD.TraversableDown = true;
			UpStairD.TextRep = "^";
			UpStairD.TextRep2 = " ";
			UpStairD.InitialAvailableConnections = 1;
			UpStairD.ForceGrowthCompatible = false;
			
			UpStairL.Weight = 1;
			UpStairL.ConnectsLeft = true;
			UpStairL.TraversableLeft = true;
			UpStairL.TextRep = "^";
			UpStairL.TextRep2 = " ";
			UpStairL.InitialAvailableConnections = 1;
			UpStairL.ForceGrowthCompatible = false;
			
			UpStairR.Weight = 1;
			UpStairR.ConnectsRight = true;
			UpStairR.TraversableRight = true;
			UpStairR.TextRep = "^";
			UpStairR.TextRep2 = " ";
			UpStairR.InitialAvailableConnections = 1;
			UpStairR.ForceGrowthCompatible = false;
			
			DownStairU.Weight = 1;
			DownStairU.ConnectsUp = true;
			DownStairU.TraversableUp = true;
			DownStairU.TextRep = "v";
			DownStairU.TextRep2 = " ";
			DownStairU.InitialAvailableConnections = 1;
			DownStairU.ForceGrowthCompatible = false;
			
			DownStairD.Weight = 1;
			DownStairD.ConnectsDown = true;
			DownStairD.TraversableDown = true;
			DownStairD.TextRep = "v";
			DownStairD.TextRep2 = " ";
			DownStairD.InitialAvailableConnections = 1;
			DownStairD.ForceGrowthCompatible = false;
			
			DownStairL.Weight = 1;
			DownStairL.ConnectsLeft = true;
			DownStairL.TraversableLeft = true;
			DownStairL.TextRep = "v";
			DownStairL.TextRep2 = " ";
			DownStairL.InitialAvailableConnections = 1;
			DownStairL.ForceGrowthCompatible = false;
			
			DownStairR.Weight = 1;
			DownStairR.ConnectsRight = true;
			DownStairR.TraversableRight = true;
			DownStairR.TextRep = "v";
			DownStairR.TextRep2 = " ";
			DownStairR.InitialAvailableConnections = 1;
			DownStairR.ForceGrowthCompatible = false;
			
			// *** END STAIRWAY CELLS ***	
		}

		// Edge: meaning the extreme edge of the dungeon's grid.
		public static List<CellType> GetTypes(Coords coords)
		{
			List<CellType> types = new List<CellType>();
			
			if (coords.AdjacentEdgeUp && !coords.AdjacentEdgeLeft && !coords.AdjacentEdgeRight) 
			{
				types.Add(Horiz);
				types.Add(JuncDLR);
				types.Add(ElbDR);
				types.Add(ElbDL);
				types.Add(DeadD);
				types.Add(DeadL);
				types.Add(DeadR);
			} 
			else if (coords.AdjacentEdgeDown && !coords.AdjacentEdgeLeft && !coords.AdjacentEdgeRight) 
			{
				types.Add(Horiz);
				types.Add(JuncULR); 
				types.Add(ElbUR);
				types.Add(ElbUL);
				types.Add(DeadU);
				types.Add(DeadL);
				types.Add(DeadR);
			}
			else if (coords.AdjacentEdgeLeft && !coords.AdjacentEdgeUp && !coords.AdjacentEdgeDown) 
			{
				types.Add(Vert);
				types.Add(JuncUDR);
				types.Add(ElbUR);
				types.Add(ElbDR);
				types.Add(DeadU);
				types.Add(DeadD);
				types.Add(DeadR);
			}
			else if (coords.AdjacentEdgeRight && !coords.AdjacentEdgeUp && !coords.AdjacentEdgeDown) 
			{
				types.Add(Vert);
				types.Add(JuncUDL);
				types.Add(ElbDL);
				types.Add(ElbUL);
				types.Add(DeadU);
				types.Add(DeadD);
				types.Add(DeadL);
			}
			else if (coords.AdjacentEdgeUp && coords.AdjacentEdgeLeft) 
			{
				types.Add(ElbDR);
				types.Add(DeadD);
				types.Add(DeadR);
			}
			else if (coords.AdjacentEdgeUp && coords.AdjacentEdgeRight) 
			{
				types.Add(ElbDL);
				types.Add(DeadD);
				types.Add(DeadL);
			} 
			else if (coords.AdjacentEdgeDown && coords.AdjacentEdgeLeft) 
			{ 
				types.Add(ElbUR);
				types.Add(DeadU);
				types.Add(DeadR);
			}
			else if (coords.AdjacentEdgeDown && coords.AdjacentEdgeRight) 
			{
				types.Add(ElbUL);
				types.Add(DeadU);
				types.Add(DeadL);
			}
			else  // Standard (non-edge) types.
			{
				types.Add(Vert);
				types.Add(Horiz);
				types.Add(Inter);
				types.Add(JuncULR); 
				types.Add(JuncUDR);
				types.Add(JuncDLR);
				types.Add(JuncUDL);
				types.Add(ElbUR);
				types.Add(ElbDR);
				types.Add(ElbDL);
				types.Add(ElbUL);
				types.Add(DeadU);
				types.Add(DeadD);
				types.Add(DeadL);
				types.Add(DeadR);
			}
			
			return types;
		}

		public static CellType ConvRoomWallToExit(CellType wall, Direction dir)
		{
			CellType exit;
			
			if (wall == RoomWallU)
				exit = RoomExitU;
			else if (wall == RoomWallD)
				exit = RoomExitD;
			else if (wall == RoomWallL)
				exit = RoomExitL; 
			else if (wall == RoomWallR)
				exit = RoomExitR;
		
			else if (wall == RoomWallUL && dir == Direction.Up)
				exit = RoomExitUL_U;
			else if (wall == RoomWallUL && dir == Direction.Left)
				exit = RoomExitUL_L;
			else if (wall == RoomWallUR && dir == Direction.Up) 
				exit = RoomExitUR_U;
			else if (wall == RoomWallUR && dir == Direction.Right)
				exit = RoomExitUR_R;
			else if (wall == RoomWallDL && dir == Direction.Down)
				exit = RoomExitDL_D;
			else if (wall == RoomWallDL && dir == Direction.Left)
				exit = RoomExitDL_L;
			else if (wall == RoomWallDR && dir == Direction.Down)
				exit = RoomExitDR_D;
			else if (wall == RoomWallDR && dir == Direction.Right)
				exit = RoomExitDR_R;
			
			else if (wall == RoomWallUL && dir == Direction.UpLeft)
				exit = RoomExitUL_UL;
			else if (wall == RoomWallUR && dir == Direction.UpRight) 
				exit = RoomExitUR_UR;
			else if (wall == RoomWallDL && dir == Direction.DownLeft)
				exit = RoomExitDL_DL;
			else if (wall == RoomWallDR && dir == Direction.DownRight)
				exit = RoomExitDR_DR;
			
			else if ((wall == RoomExitUL_U && dir == Direction.Left) || (wall == RoomExitUL_L && dir == Direction.Up))
				exit = RoomExitUL_UL;
			else if ((wall == RoomExitUR_U && dir == Direction.Right) || (wall == RoomExitUR_R && dir == Direction.Up))
				exit = RoomExitUR_UR;
			else if ((wall == RoomExitDL_D && dir == Direction.Left) || (wall == RoomExitDL_L && dir == Direction.Down))
				exit = RoomExitDL_DL;
			else if ((wall == RoomExitDR_D && dir == Direction.Right) || (wall == RoomExitDR_R && dir == Direction.Down))
				exit = RoomExitDR_DR;
			
			else
				exit = Inter;
			
			return exit;
		}

		public static CellType ConvRoomExitToWall(CellType exit, Direction dir, CellDescription descr)
		{
			CellType wall;
			
			if (exit == RoomExitU)
				wall = RoomWallU;
			else if (exit == RoomExitD)
				wall = RoomWallD;
			else if (exit == RoomExitL)
				wall = RoomWallL; 
			else if (exit == RoomExitR)
				wall = RoomWallR; 
			
			else if (dir == Direction.Up && exit == Inter && descr == CellDescriptions.Mines_Vert)
				wall = JuncDLR;
			else if (dir == Direction.Down && exit == Inter && descr == CellDescriptions.Mines_Vert)
				wall = JuncULR;
			else if (dir == Direction.Left && exit == Inter && descr == CellDescriptions.Mines_Horiz)
				wall = JuncUDR;
			else if (dir == Direction.Right && exit == Inter && descr == CellDescriptions.Mines_Horiz)
				wall = JuncUDL;
			
			else if (exit == RoomExitUL_U || exit == RoomExitUL_L)
				wall = RoomWallUL;
			else if (exit == RoomExitUR_U || exit == RoomExitUR_R)
				wall = RoomWallUR;
			else if (exit == RoomExitDL_D || exit == RoomExitDL_L)
				wall = RoomWallDL;
			else if (exit == RoomExitDR_D || exit == RoomExitDR_R)
				wall = RoomWallDR;
			
			else if (exit == RoomExitUL_UL && dir == Direction.Up)
				wall = RoomExitUL_L;
			else if (exit == RoomExitUL_UL && dir == Direction.Left)
				wall = RoomExitUL_U;
			else if (exit == RoomExitUR_UR && dir == Direction.Up)
				wall = RoomExitUR_R;
			else if (exit == RoomExitUR_UR && dir == Direction.Right)
				wall = RoomExitUR_U;
			else if (exit == RoomExitDL_DL && dir == Direction.Down)
				wall = RoomExitDL_L;
			else if (exit == RoomExitDL_DL && dir == Direction.Left)
				wall = RoomExitDL_D;
			else if (exit == RoomExitDR_DR && dir == Direction.Down)
				wall = RoomExitDR_R;
			else if (exit == RoomExitDR_DR && dir == Direction.Right)
				wall = RoomExitDR_D;
			
			else
				throw new LevelGenerateException();  // Unknown... scrap it (never happens).
			
			return wall;
		}

		public static CellType ConvDeadEndToDownStairs(CellType deadEnd)
		{    
			CellType stairs = null;
			
			if (deadEnd == DeadU)
				stairs = DownStairU;
			else if (deadEnd == DeadD)
				stairs = DownStairD;
			else if (deadEnd == DeadL)
				stairs = DownStairL; 
			else if (deadEnd == DeadR)
				stairs = DownStairR; 
			
			return stairs;
		}

		public static bool IsRoomExit(CellType type)
		{
			return type == RoomExitU || type == RoomExitD || type == RoomExitL || type == RoomExitR || 
				   type == Inter || type == RoomExitUL_U || type == RoomExitUL_L || type == RoomExitUL_UL ||
				   type == RoomExitUR_U || type == RoomExitUR_R || type == RoomExitUR_UR || type == RoomExitDL_D || 
				   type == RoomExitDL_L || type == RoomExitDL_DL || type == RoomExitDR_D || type == RoomExitDR_R || 
				   type == RoomExitDR_DR || type == RoomExitU_Round || type == RoomExitD_Round || 
				   type == RoomExitL_Round || type == RoomExitR_Round;
		}

		public static bool IsRoomType(CellType type)
		{
			return type == RoomSpace  || type == RoomWallD || type == RoomWallDL || type == RoomWallDR ||
				   type == RoomWallL  || type == RoomWallR || type == RoomWallU  || type == RoomWallUL || 
				   type == RoomWallUR || type == RoomExitD || type == RoomExitL  || type == RoomExitR  || 
				   type == RoomExitU  || type == RoomWallDLinv || type == RoomWallDRinv || type == RoomWallULinv ||
				   type == RoomWallURinv || type == RoomWallDL_Round || type == RoomWallDR_Round ||
				   type == RoomWallUL_Round || type == RoomWallUR_Round || type == Fountain || 
				   type == RoomExitU_Round || type == RoomExitD_Round || 
				   type == RoomExitL_Round || type == RoomExitR_Round ||
				   type == RoomWallU_Round || type == RoomWallD_Round || 
				   type == RoomWallL_Round || type == RoomWallR_Round;
		}

		public static bool IsRoomCorner(CellType type)
		{
			return type == RoomWallDL || type == RoomWallDR || type == RoomWallUL || type == RoomWallUR ||
				   type == RoomWallDL_Round || type == RoomWallDR_Round  || type == RoomWallUL_Round  || 
				   type == RoomWallUR_Round ;
		}

		public static bool IsDeadEnd(CellType type)
		{
			return type == DeadU || type == DeadD || type == DeadL || type == DeadR;
		}

		public static bool IsCleanStartWall(CellType type)
		{
			return type == RoomWallD || type == RoomWallURinv || type == RoomWallULinv || type == RoomExitD;
		}

		public static bool IsFloodingTransition(CellType type)
		{
			return type == Vert || type == Horiz;
		}

		public static bool IsFloodingIncompatible(CellType type)
		{
			return type == RoomExitU_Round || type == RoomExitD_Round || type == RoomExitL_Round || 
				type == RoomExitR_Round;
		}

		public static Direction RoomWallDirection(CellType type)
		{
			Direction dir = Direction.Up;
			
			if (type == RoomWallU)
				dir = Direction.Up;
			else if (type == RoomWallD)
				dir = Direction.Down;
			else if (type == RoomWallL)
				dir = Direction.Left;
			else if (type == RoomWallR)
				dir = Direction.Right;
			else if (type == RoomWallU_Round)
				dir = Direction.Up;
			else if (type == RoomWallD_Round)
				dir = Direction.Down;
			else if (type == RoomWallL_Round)
				dir = Direction.Left;
			else if (type == RoomWallR_Round)
				dir = Direction.Right;
			else if (type == RoomWallUL)
				dir = Direction.UpLeft;
			else if (type == RoomWallUR)
				dir = Direction.UpRight;
			else if (type == RoomWallDL)
				dir = Direction.DownLeft;
			else if (type == RoomWallDR)
				dir = Direction.DownRight;
			else if (type == RoomWallUL_Round)
				dir = Direction.UpLeft;
			else if (type == RoomWallUR_Round)
				dir = Direction.UpRight;
			else if (type == RoomWallDL_Round)
				dir = Direction.DownLeft;
			else if (type == RoomWallDR_Round)
				dir = Direction.DownRight;
			
			return dir;
		}

		public static CellType ConvRoomTypeToCatacomb(CellType roomType)
		{
			CellType catacomb;
			
			if (roomType == RoomWallU) 
				catacomb = JuncDLR;
			else if (roomType == RoomWallD)
				catacomb = JuncULR;
			else if (roomType == RoomWallL) 
				catacomb = JuncUDR;
			else if (roomType == RoomWallR)
				catacomb = JuncUDL;
			
			else if (roomType == RoomWallUL)
				catacomb = ElbDR;
			else if (roomType == RoomWallUR) 
				catacomb = ElbDL;
			else if (roomType == RoomWallDL)
				catacomb = ElbUR;
			else if (roomType == RoomWallDR)
				catacomb = ElbUL;
			
			else if (roomType == RoomExitUL_U)
				catacomb = JuncUDR;
			else if (roomType == RoomExitUL_L) 
				catacomb = JuncDLR;
		
			else if (roomType == RoomExitUR_U)
				catacomb = JuncUDL;
			else if (roomType == RoomExitUR_R) 
				catacomb = JuncDLR;
		
			else if (roomType == RoomExitDL_D)
				catacomb = JuncUDR;
			else if (roomType == RoomExitDL_L) 
				catacomb = JuncULR;
		
			else if (roomType == RoomExitDR_D)
				catacomb = JuncUDL;
			else if (roomType == RoomExitDR_R) 
				catacomb = JuncULR;
			
			else
				catacomb = Inter;
			
			return catacomb;
		}
	}
}