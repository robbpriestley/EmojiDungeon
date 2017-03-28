using System.Collections.Generic;

namespace DigitalWizardry.Dungeon
{	
	public class Type
	{
		// *** BEGIN CONNECTS MEMBERS ***
		// Connects: determines if one cell is capable of being mated to another.
		public bool ConnectsUp { get; set; }
		public bool ConnectsDown { get; set; }
		public bool ConnectsLeft { get; set; }
		public bool ConnectsRight { get; set; }
		public bool RoomConnectsUp { get; set; }
		public bool RoomConnectsDown { get; set; }
		public bool RoomConnectsLeft { get; set; }
		public bool RoomConnectsRight { get; set; }
		// *** END CONNECTS MEMBERS ***
		// *** BEGIN TRAVERSABLE MEMBERS ***
		// Traversable: determines if it would be possible to move from one cell to another.
		public bool TraversableUp { get; set; }
		public bool TraversableDown { get; set; }
		public bool TraversableLeft { get; set; }
		public bool TraversableRight { get; set; }
		// *** END TRAVERSABLE MEMBERS ***
		// *** BEGIN DESCRIPTIVE MEMBERS ***
		public bool IsEmpty { get; set; }
		public bool IsJunction { get; set; }
		public bool IsDeadEnd { get; set; }
		public bool IsRoomType { get; set; }
		public bool IsRoomExit { get; set; }
		public bool IsRoomCorner { get; set; }
		public bool IsCleanStartWall { get; set; }
		public bool IsFloodingTransition { get; set; }
		public bool IsFloodingIncompatible { get; set; }
		public Direction RoomWallDirection { get; set; }
		public bool RoomExitCompatible { get; set; }
		public bool ForceGrowthCompatible { get; set; }       // The type be substituted for another cell to increase dungeon fill.
		// *** END DESCRIPTIVE MEMBERS ***
		// *** BEGIN UTILITY MEMBERS ***
		public int Weight { get; set; }                       // Influences the selection when types are being randomly determined.
		public int DoorProbability { get; set; }              // The probability that a door can be placed in a section of room.
		public string TextRep { get; set; }                   // Primary character used to represent a type in text.
		public string TextRep2 { get; set; }                  // Used for better rendering of the text representation, which appears "squished" in the horizontal dimension.
		public int InitialAvailableConnections { get; set; }  // Used when generating the dungeon to determine if other cells can be attached to a target cell.
		// *** END UTILITY MEMBERS ***

		public Type()
		{
        	ForceGrowthCompatible = true;  // Attention! This is the only bool member which is initialized to true.
			RoomWallDirection = Direction.NoDir;
		}

		public bool ConnectsTo(Type otherCell, Direction direction)
		{    
			if (otherCell.IsEmpty)
			{
				return false;
			}
			else if (direction == Direction.Up && ConnectsUp && otherCell.ConnectsDown)
			{
				return true;
			}
			else if (direction == Direction.Down && ConnectsDown && otherCell.ConnectsUp)
			{
				return true;
			}
			else if (direction == Direction.Left && ConnectsLeft && otherCell.ConnectsRight)
			{
				return true;
			}
			else if (direction == Direction.Right && ConnectsRight && otherCell.ConnectsLeft)
			{
				return true;
			}
			
			return false;
		}

		public bool CompatibleWith(Type otherCell, Direction direction)
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
				if ((ConnectsUp && otherCell.ConnectsDown) || 
					(!ConnectsUp && !otherCell.ConnectsDown))
				{
					return true;
				}
			}
			else if (direction == Direction.Down)
			{
				if ((ConnectsDown && otherCell.ConnectsUp) || 
					(!ConnectsDown && !otherCell.ConnectsUp))
				{
					return true;
				}
			}
			else if (direction == Direction.Left)
			{
				if ((ConnectsLeft && otherCell.ConnectsRight) || 
					(!ConnectsLeft && !otherCell.ConnectsRight))
				{
					return true;
				}
			}
			else if (direction == Direction.Right)
			{
				if ((ConnectsRight && otherCell.ConnectsLeft) || 
					(!ConnectsRight && !otherCell.ConnectsLeft))
				{
					return true;
				}
			}

			return false;
		}
	}

	public static class Types
	{
		// *** BEGIN FIELD DECLARATIONS ***
		private static readonly Type _emptyCell;
		private static readonly Type _entrance;
		private static readonly Type _vert;
		private static readonly Type _horiz;
		private static readonly Type _inter;
		private static readonly Type _juncULR;
		private static readonly Type _juncUDR;
		private static readonly Type _juncDLR;
		private static readonly Type _juncUDL;
		private static readonly Type _elbUR;
		private static readonly Type _elbDR;
		private static readonly Type _elbDL;
		private static readonly Type _elbUL;
		private static readonly Type _deadU;
		private static readonly Type _deadD;
		private static readonly Type _deadL;
		private static readonly Type _deadR;
		private static readonly Type _deadexU;
		private static readonly Type _deadexD;
		private static readonly Type _deadexL;
		private static readonly Type _deadexR;
		private static readonly Type _upStairU;
		private static readonly Type _upStairD;
		private static readonly Type _upStairL;
		private static readonly Type _upStairR;
		private static readonly Type _downStairU;
		private static readonly Type _downStairD;
		private static readonly Type _downStairL;
		private static readonly Type _downStairR;
		private static readonly Type _roomSpace;
		private static readonly Type _fountain;
		private static readonly Type _roomWallU;
		private static readonly Type _roomWallD;
		private static readonly Type _roomWallL;
		private static readonly Type _roomWallR;
		private static readonly Type _roomWallU_Round;
		private static readonly Type _roomWallD_Round;
		private static readonly Type _roomWallL_Round;
		private static readonly Type _roomWallR_Round;
		private static readonly Type _roomWallUL;
		private static readonly Type _roomWallUR;
		private static readonly Type _roomWallDL;
		private static readonly Type _roomWallDR;
		private static readonly Type _roomWallUL_Round;
		private static readonly Type _roomWallUR_Round;
		private static readonly Type _roomWallDL_Round;
		private static readonly Type _roomWallDR_Round;
		private static readonly Type _roomWallULinv;
		private static readonly Type _roomWallURinv;
		private static readonly Type _roomWallDLinv;
		private static readonly Type _roomWallDRinv;
		private static readonly Type _roomExitU;
		private static readonly Type _roomExitD;
		private static readonly Type _roomExitL;
		private static readonly Type _roomExitR;
		private static readonly Type _roomExitU_Round;
		private static readonly Type _roomExitD_Round;
		private static readonly Type _roomExitL_Round;
		private static readonly Type _roomExitR_Round;
		private static readonly Type _roomExitUL_U;
		private static readonly Type _roomExitUL_L;
		private static readonly Type _roomExitUL_UL;
		private static readonly Type _roomExitUR_U;
		private static readonly Type _roomExitUR_R;
		private static readonly Type _roomExitUR_UR;
		private static readonly Type _roomExitDL_D;
		private static readonly Type _roomExitDL_L;
		private static readonly Type _roomExitDL_DL;
		private static readonly Type _roomExitDR_D;
		private static readonly Type _roomExitDR_R;
		private static readonly Type _roomExitDR_DR;
		// *** END FIELD DECLARATIONS ***
		// *** BEGIN PROPERTY DECLARATIONS ***
		public static Type EmptyCell { get { return _emptyCell; } }                // Empty, i.e. unused.
		public static Type Entrance { get { return _entrance; } }                  // Entrance Cell, used only once on level 0...
		public static Type Vert { get { return _vert; } }                          // Vertical Corridor            
		public static Type Horiz { get { return _horiz; } }                        // Horizontal Corridor           
		public static Type Inter { get { return _inter; } }                        // Intersection                 
		public static Type JuncULR { get { return _juncULR; } }                    // Junction Up Left Right       
		public static Type JuncUDR { get { return _juncUDR; } }                    // Junction Up Down Right       
		public static Type JuncDLR { get { return _juncDLR; } }                    // Junction Down Left Right     
		public static Type JuncUDL { get { return _juncUDL; } }                    // Junction Up Down Left        
		public static Type ElbUR { get { return _elbUR; } }                        // Elbow Up Right               
		public static Type ElbDR { get { return _elbDR; } }                        // Elbow Down Right             
		public static Type ElbDL { get { return _elbDL; } }                        // Elbow Down Left              
		public static Type ElbUL { get { return _elbUL; } }                        // Elbow Up Left                
		public static Type DeadU { get { return _deadU; } }                        // Dead End Up                  
		public static Type DeadD { get { return _deadD; } }                        // Dead End Down                
		public static Type DeadL { get { return _deadL; } }                        // Dead End Left                
		public static Type DeadR { get { return _deadR; } }                        // Dead End Right 
		public static Type DeadexU { get { return _deadexU; } }                    // Dead End Exit Up                  
		public static Type DeadexD { get { return _deadexD; } }                    // Dead End Exit Down                
		public static Type DeadexL { get { return _deadexL; } }                    // Dead End Exit Left                
		public static Type DeadexR { get { return _deadexR; } }                    // Dead End Exit Right 
		public static Type UpStairU { get { return _upStairU; } }                  // Stairs Up Connects Up        
		public static Type UpStairD { get { return _upStairD; } }                  // Stairs Up Connects Down      
		public static Type UpStairL { get { return _upStairL; } }                  // Stairs Up Connects Left      
		public static Type UpStairR { get { return _upStairR; } }                  // Stairs Up Connects Right     
		public static Type DownStairU { get { return _downStairU; } }              // Stairs Down Connects Up      
		public static Type DownStairD { get { return _downStairD; } }              // Stairs Down Connects Down    
		public static Type DownStairL { get { return _downStairL; } }              // Stairs Down Connects Left    
		public static Type DownStairR { get { return _downStairR; } }              // Stairs Down Connects Right 
		public static Type RoomSpace { get { return _roomSpace; } }                // Room Space
		public static Type Fountain { get { return _fountain; } }                  // Healing Fountain
		public static Type RoomWallU { get { return _roomWallU; } }                // Room Wall Up
		public static Type RoomWallD { get { return _roomWallD; } }                // Room Wall Down
		public static Type RoomWallL { get { return _roomWallL; } }                // Room Wall Left
		public static Type RoomWallR { get { return _roomWallR; } }                // Room Wall Right
		public static Type RoomWallU_Round { get { return _roomWallU_Round; } }    // Room Wall Up Round
		public static Type RoomWallD_Round { get { return _roomWallD_Round; } }    // Room Wall Down Round
		public static Type RoomWallL_Round { get { return _roomWallL_Round; } }    // Room Wall Left Round
		public static Type RoomWallR_Round { get { return _roomWallR_Round; } }    // Room Wall Right Round
		public static Type RoomWallUL { get { return _roomWallUL; } }              // Room Wall Up Left
		public static Type RoomWallUR { get { return _roomWallUR; } }              // Room Wall Up Right
		public static Type RoomWallDL { get { return _roomWallDL; } }              // Room Wall Down Left
		public static Type RoomWallDR { get { return _roomWallDR; } }              // Room Wall Down Right
		public static Type RoomWallUL_Round { get { return _roomWallUL_Round; } }  // Room Wall Up Left Round
		public static Type RoomWallUR_Round { get { return _roomWallUR_Round; } }  // Room Wall Up Right Round
		public static Type RoomWallDL_Round { get { return _roomWallDL_Round; } }  // Room Wall Down Left Round
		public static Type RoomWallDR_Round { get { return _roomWallDR_Round; } }  // Room Wall Down Right Round
		public static Type RoomWallULinv { get { return _roomWallULinv; } }        // Room Wall Up Left Inverted
		public static Type RoomWallURinv { get { return _roomWallURinv; } }        // Room Wall Up Right Inverted
		public static Type RoomWallDLinv { get { return _roomWallDLinv; } }        // Room Wall Down Left Inverted
		public static Type RoomWallDRinv { get { return _roomWallDRinv; } }        // Room Wall Down Right Inverted
		public static Type RoomExitU { get { return _roomExitU; } }                // Room Exit Up
		public static Type RoomExitD { get { return _roomExitD; } }                // Room Exit Down
		public static Type RoomExitL { get { return _roomExitL; } }                // Room Exit Left
		public static Type RoomExitR { get { return _roomExitR; } }                // Room Exit Right
		public static Type RoomExitU_Round { get { return _roomExitU_Round; } }    // Room Exit Up Round
		public static Type RoomExitD_Round { get { return _roomExitD_Round; } }    // Room Exit Down Round
		public static Type RoomExitL_Round { get { return _roomExitL_Round; } }    // Room Exit Left Round
		public static Type RoomExitR_Round { get { return _roomExitR_Round; } }    // Room Exit Right Round
		public static Type RoomExitUL_U { get { return _roomExitUL_U; } }          // Room Exit Up Left Corner, Exit Up
		public static Type RoomExitUL_L { get { return _roomExitUL_L; } }          // Room Exit Up Left Corner, Exit Left
		public static Type RoomExitUL_UL { get { return _roomExitUL_UL; } }        // Room Exit Up Left Corner, Exits Up and Left
		public static Type RoomExitUR_U { get { return _roomExitUR_U; } }          // Room Exit Up Right Corner, Exit Up
		public static Type RoomExitUR_R { get { return _roomExitUR_R; } }          // Room Exit Up Right Corner, Exit Right
		public static Type RoomExitUR_UR { get { return _roomExitUR_UR; } }        // Room Exit Up Right Corner, Exits Up and Right
		public static Type RoomExitDL_D { get { return _roomExitDL_D; } }          // Room Exit Down Left Corner, Exit Down
		public static Type RoomExitDL_L { get { return _roomExitDL_L; } }          // Room Exit Down Left Corner, Exit Left
		public static Type RoomExitDL_DL { get { return _roomExitDL_DL; } }        // Room Exit Down Left Corner, Exits Down and Left
		public static Type RoomExitDR_D { get { return _roomExitDR_D; } }          // Room Exit Down Right Corner, Exit Down
		public static Type RoomExitDR_R { get { return _roomExitDR_R; } }          // Room Exit Down Right Corner, Exit Right
		public static Type RoomExitDR_DR { get { return _roomExitDR_DR; } }        // Room Exit Down Right Corner, Exits Down and Right
		// *** END PROPERTY DECLARATIONS ***

		static Types()
		{
			_emptyCell = new Type();
			_entrance = new Type();
			_vert = new Type();
			_horiz = new Type();
			_inter = new Type();
			_juncULR = new Type();
			_juncUDR = new Type();
			_juncDLR = new Type();
			_juncUDL = new Type();
			_elbUR = new Type();
			_elbDR = new Type();
			_elbDL = new Type();
			_elbUL = new Type();
			_deadU = new Type();
			_deadD = new Type();
			_deadL = new Type();
			_deadR = new Type();
			_deadexU = new Type();
			_deadexD = new Type();
			_deadexL = new Type();
			_deadexR = new Type();
			_upStairU = new Type();
			_upStairD = new Type();
			_upStairL = new Type();
			_upStairR = new Type();
			_downStairU = new Type();
			_downStairD = new Type();
			_downStairL = new Type();
			_downStairR = new Type();
			_roomSpace = new Type();
			_fountain = new Type();
			_roomWallU = new Type();
			_roomWallD = new Type();
			_roomWallL = new Type();
			_roomWallR = new Type();
			_roomWallU_Round = new Type();
			_roomWallD_Round = new Type();
			_roomWallL_Round = new Type();
			_roomWallR_Round = new Type();
			_roomWallUL = new Type();
			_roomWallUR = new Type();
			_roomWallDL = new Type();
			_roomWallDR = new Type();
			_roomWallUL_Round = new Type();
			_roomWallUR_Round = new Type();
			_roomWallDL_Round = new Type();
			_roomWallDR_Round = new Type();
			_roomWallULinv = new Type();
			_roomWallURinv = new Type();
			_roomWallDLinv = new Type();
			_roomWallDRinv = new Type();
			_roomExitU = new Type();
			_roomExitD = new Type();
			_roomExitL = new Type();
			_roomExitR = new Type();
			_roomExitU_Round = new Type();
			_roomExitD_Round = new Type();
			_roomExitL_Round = new Type();
			_roomExitR_Round = new Type();
			_roomExitUL_U = new Type();
			_roomExitUL_L = new Type();
			_roomExitUL_UL = new Type();
			_roomExitUR_U = new Type();
			_roomExitUR_R = new Type();
			_roomExitUR_UR = new Type();
			_roomExitDL_D = new Type();
			_roomExitDL_L = new Type();
			_roomExitDL_DL = new Type();
			_roomExitDR_D = new Type();
			_roomExitDR_R = new Type();
			_roomExitDR_DR = new Type();

			Initialize();
		}

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
			Vert.IsFloodingTransition = true;
			Vert.InitialAvailableConnections = 2;
			
			Horiz.Weight = 100;
			Horiz.ConnectsLeft = true;
			Horiz.ConnectsRight = true;
			Horiz.TraversableLeft = true;
			Horiz.TraversableRight = true;
			Horiz.TextRep = @"═";
			Horiz.TextRep2 = @"═";
			Horiz.IsFloodingTransition = true;
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
			Inter.IsRoomExit = true;
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
			DeadU.IsDeadEnd = true;
			DeadU.InitialAvailableConnections = 1;
			
			DeadD.Weight = 1;
			DeadD.ConnectsDown = true;
			DeadD.TraversableDown = true;
			DeadD.TextRep = @"╥";
			DeadD.TextRep2 = @" ";
			DeadD.IsDeadEnd = true;
			DeadD.InitialAvailableConnections = 1;
			
			DeadL.Weight = 1;
			DeadL.ConnectsLeft = true;
			DeadL.TraversableLeft = true;
			DeadL.TextRep = @"╡";
			DeadL.TextRep2 = @" ";
			DeadL.IsDeadEnd = true;
			DeadL.InitialAvailableConnections = 1;
			
			DeadR.Weight = 1;
			DeadR.ConnectsRight = true;
			DeadR.TraversableRight = true;
			DeadR.TextRep = @"╞";
			DeadR.TextRep2 = @"═";
			DeadR.IsDeadEnd = true;
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
			RoomSpace.IsRoomType = true;
			RoomSpace.ForceGrowthCompatible = false;
			
			Fountain.TraversableUp = true;
			Fountain.TraversableDown = true;
			Fountain.TraversableLeft = true;
			Fountain.TraversableRight = true;
			Fountain.TextRep = "F";
			Fountain.TextRep2 = " ";
			Fountain.IsRoomType = true;
			Fountain.ForceGrowthCompatible = false;
		
			RoomWallU.TraversableDown = true;
			RoomWallU.TraversableLeft = true;
			RoomWallU.TraversableRight = true;
			RoomWallU.RoomExitCompatible = true;
			RoomWallU.RoomConnectsLeft = true;
			RoomWallU.RoomConnectsRight = true;
			RoomWallU.TextRep = "─";
			RoomWallU.TextRep2 = "─";
			RoomWallU.IsRoomType = true;
			RoomWallU.ForceGrowthCompatible = false;
			RoomWallU.RoomWallDirection = Direction.Up;
			
			RoomWallD.TraversableUp = true;
			RoomWallD.TraversableLeft = true;
			RoomWallD.TraversableRight = true;
			RoomWallD.RoomExitCompatible = true;
			RoomWallD.RoomConnectsLeft = true;
			RoomWallD.RoomConnectsRight = true;
			RoomWallD.TextRep = "─";
			RoomWallD.TextRep2 = "─";
			RoomWallD.IsRoomType = true;
			RoomWallD.IsCleanStartWall = true;
			RoomWallD.ForceGrowthCompatible = false;
			RoomWallD.RoomWallDirection = Direction.Down;
			
			RoomWallL.TraversableUp = true;
			RoomWallL.TraversableDown = true;
			RoomWallL.TraversableRight = true;
			RoomWallL.RoomExitCompatible = true;
			RoomWallL.RoomConnectsUp = true;
			RoomWallL.RoomConnectsDown = true;
			RoomWallL.TextRep = "│";
			RoomWallL.TextRep2 = " ";
			RoomWallL.IsRoomType = true;
			RoomWallL.ForceGrowthCompatible = false;
			RoomWallL.RoomWallDirection = Direction.Left;
			
			RoomWallR.TraversableUp = true;
			RoomWallR.TraversableDown = true;
			RoomWallR.TraversableLeft = true;
			RoomWallR.RoomExitCompatible = true;
			RoomWallR.RoomConnectsUp = true;
			RoomWallR.RoomConnectsDown = true;
			RoomWallR.TextRep = "│";
			RoomWallR.TextRep2 = " ";
			RoomWallR.IsRoomType = true;
			RoomWallR.ForceGrowthCompatible = false;
			RoomWallR.RoomWallDirection = Direction.Right;

			RoomWallU_Round.TraversableDown = true;
			RoomWallU_Round.TraversableLeft = true;
			RoomWallU_Round.TraversableRight = true;
			RoomWallU_Round.RoomConnectsLeft = true;
			RoomWallU_Round.RoomConnectsRight = true;
			RoomWallU_Round.TextRep = "┉";
			RoomWallU_Round.TextRep2 = "┉";
			RoomWallU_Round.IsRoomType = true;
			RoomWallU_Round.ForceGrowthCompatible = false;
			RoomWallU_Round.RoomWallDirection = Direction.Up;
			
			RoomWallD_Round.TraversableUp = true;
			RoomWallD_Round.TraversableLeft = true;
			RoomWallD_Round.TraversableRight = true;
			RoomWallD_Round.RoomConnectsLeft = true;
			RoomWallD_Round.RoomConnectsRight = true;
			RoomWallD_Round.TextRep = "┉";
			RoomWallD_Round.TextRep2 = "┉";
			RoomWallD_Round.IsRoomType = true;
			RoomWallD_Round.ForceGrowthCompatible = false;
			RoomWallD_Round.RoomWallDirection = Direction.Down;
			
			RoomWallL_Round.TraversableUp = true;
			RoomWallL_Round.TraversableDown = true;
			RoomWallL_Round.TraversableRight = true;
			RoomWallL_Round.RoomConnectsUp = true;
			RoomWallL_Round.RoomConnectsDown = true;
			RoomWallL_Round.TextRep = "┋";
			RoomWallL_Round.TextRep2 = " ";
			RoomWallL_Round.IsRoomType = true;
			RoomWallL_Round.ForceGrowthCompatible = false;
			RoomWallL_Round.RoomWallDirection = Direction.Left;
			
			RoomWallR_Round.TraversableUp = true;
			RoomWallR_Round.TraversableDown = true;
			RoomWallR_Round.TraversableLeft = true;
			RoomWallR_Round.RoomConnectsUp = true;
			RoomWallR_Round.RoomConnectsDown = true;
			RoomWallR_Round.TextRep = "┋";
			RoomWallR_Round.TextRep2 = " ";
			RoomWallR_Round.IsRoomType = true;
			RoomWallR_Round.ForceGrowthCompatible = false;
			RoomWallR_Round.RoomWallDirection = Direction.Right;
	
			RoomWallUL.TraversableDown = true;
			RoomWallUL.TraversableRight = true;
			RoomWallUL.RoomExitCompatible = true;
			RoomWallUL.RoomConnectsDown = true;
			RoomWallUL.RoomConnectsRight = true;
			RoomWallUL.TextRep = "┌";
			RoomWallUL.TextRep2 = "─";
			RoomWallUL.IsRoomType = true;
			RoomWallUL.IsRoomCorner = true;
			RoomWallUL.ForceGrowthCompatible = false;
			RoomWallUL.RoomWallDirection = Direction.UpLeft;
			
			RoomWallUR.TraversableDown = true;
			RoomWallUR.TraversableLeft = true;
			RoomWallUR.RoomExitCompatible = true;
			RoomWallUR.RoomConnectsDown = true;
			RoomWallUR.RoomConnectsLeft = true;
			RoomWallUR.TextRep = "┐";
			RoomWallUR.TextRep2 = " ";
			RoomWallUR.IsRoomType = true;
			RoomWallUR.IsRoomCorner = true;
			RoomWallUR.ForceGrowthCompatible = false;
			RoomWallUR.RoomWallDirection = Direction.UpRight;
			
			RoomWallDL.TraversableUp = true;
			RoomWallDL.TraversableRight = true;
			RoomWallDL.RoomExitCompatible = true;
			RoomWallDL.RoomConnectsUp = true;
			RoomWallDL.RoomConnectsRight = true;
			RoomWallDL.TextRep = "└";
			RoomWallDL.TextRep2 = "─";
			RoomWallDL.IsRoomType = true;
			RoomWallDL.IsRoomCorner = true;
			RoomWallDL.ForceGrowthCompatible = false;
			RoomWallDL.RoomWallDirection = Direction.DownLeft;
			
			RoomWallDR.TraversableUp = true;
			RoomWallDR.TraversableLeft = true;
			RoomWallDR.RoomExitCompatible = true;
			RoomWallDR.RoomConnectsUp = true;
			RoomWallDR.RoomConnectsLeft = true;
			RoomWallDR.TextRep = "┘";
			RoomWallDR.TextRep2 = " ";
			RoomWallDR.IsRoomType = true;
			RoomWallDR.IsRoomCorner = true;
			RoomWallDR.ForceGrowthCompatible = false;
			RoomWallDR.RoomWallDirection = Direction.DownRight;
			
			RoomWallUL_Round.TraversableDown = true;
			RoomWallUL_Round.TraversableRight = true;
			RoomWallUL_Round.RoomConnectsDown = true;
			RoomWallUL_Round.RoomConnectsRight = true;
			RoomWallUL_Round.TextRep = "╭";
			RoomWallUL_Round.TextRep2 = "─";
			RoomWallUL_Round.IsRoomType = true;
			RoomWallUL_Round.IsRoomCorner = true;
			RoomWallUL_Round.ForceGrowthCompatible = false;
			RoomWallUL_Round.RoomWallDirection = Direction.UpLeft;
			
			RoomWallUR_Round.TraversableDown = true;
			RoomWallUR_Round.TraversableLeft = true;
			RoomWallUR_Round.RoomConnectsDown = true;
			RoomWallUR_Round.RoomConnectsLeft = true;
			RoomWallUR_Round.TextRep = "╮";
			RoomWallUR_Round.TextRep2 = " ";
			RoomWallUR_Round.IsRoomType = true;
			RoomWallUR_Round.IsRoomCorner = true;
			RoomWallUR_Round.ForceGrowthCompatible = false;
			RoomWallUR_Round.RoomWallDirection = Direction.UpRight;
			
			RoomWallDL_Round.TraversableUp = true;
			RoomWallDL_Round.TraversableRight = true;
			RoomWallDL_Round.RoomConnectsUp = true;
			RoomWallDL_Round.RoomConnectsRight = true;
			RoomWallDL_Round.TextRep = "╰";
			RoomWallDL_Round.TextRep2 = "─";
			RoomWallDL_Round.IsRoomType = true;
			RoomWallDL_Round.IsRoomCorner = true;
			RoomWallDL_Round.ForceGrowthCompatible = false;
			RoomWallDL_Round.RoomWallDirection = Direction.DownLeft;
			
			RoomWallDR_Round.TraversableUp = true;
			RoomWallDR_Round.TraversableLeft = true;
			RoomWallDR_Round.RoomConnectsUp = true;
			RoomWallDR_Round.RoomConnectsLeft = true;
			RoomWallDR_Round.TextRep = "╯";
			RoomWallDR_Round.TextRep2 = " ";
			RoomWallDR_Round.IsRoomType = true;
			RoomWallDR_Round.IsRoomCorner = true;
			RoomWallDR_Round.ForceGrowthCompatible = false;
			RoomWallDR_Round.RoomWallDirection = Direction.DownRight;
			
			RoomWallULinv.TraversableDown = true;
			RoomWallULinv.TraversableRight = true;
			RoomWallULinv.RoomConnectsDown = true;
			RoomWallULinv.RoomConnectsRight = true;
			RoomWallULinv.TextRep = "┌";
			RoomWallULinv.TextRep2 = "─";
			RoomWallULinv.IsRoomType = true;
			RoomWallULinv.IsCleanStartWall = true;
			RoomWallULinv.ForceGrowthCompatible = false;
			
			RoomWallURinv.TraversableDown = true;
			RoomWallURinv.TraversableLeft = true;
			RoomWallURinv.RoomConnectsDown = true;
			RoomWallURinv.RoomConnectsLeft = true;
			RoomWallURinv.TextRep = "┐";
			RoomWallURinv.TextRep2 = " ";
			RoomWallURinv.IsRoomType = true;
			RoomWallURinv.IsCleanStartWall = true;
			RoomWallURinv.ForceGrowthCompatible = false;
			
			RoomWallDLinv.TraversableUp = true;
			RoomWallDLinv.TraversableRight = true;
			RoomWallDLinv.RoomConnectsUp = true;
			RoomWallDLinv.RoomConnectsRight = true;
			RoomWallDLinv.TextRep = "└";
			RoomWallDLinv.TextRep2 = "─";
			RoomWallDLinv.IsRoomType = true;
			RoomWallDLinv.ForceGrowthCompatible = false;
			
			RoomWallDRinv.TraversableUp = true;
			RoomWallDRinv.TraversableLeft = true;
			RoomWallDRinv.RoomConnectsUp = true;
			RoomWallDRinv.RoomConnectsLeft = true;
			RoomWallDRinv.TextRep = "┘";
			RoomWallDRinv.TextRep2 = " ";
			RoomWallDRinv.IsRoomType = true;
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
			RoomExitU.DoorProbability = 50;
			RoomExitU.IsRoomType = true;
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
			RoomExitD.DoorProbability = 50;
			RoomExitD.IsRoomType = true;
			RoomExitD.IsRoomExit = true;
			RoomExitD.IsCleanStartWall = true;
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
			RoomExitL.DoorProbability = 50;
			RoomExitL.IsRoomType = true;
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
			RoomExitR.DoorProbability = 50;
			RoomExitR.IsRoomType = true;
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
			RoomExitU_Round.DoorProbability = 0;
			RoomExitU_Round.IsRoomType = true;
			RoomExitU_Round.IsRoomExit = true;
			RoomExitU_Round.IsFloodingIncompatible = true;
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
			RoomExitD_Round.DoorProbability = 0;
			RoomExitD_Round.IsRoomType = true;
			RoomExitD_Round.IsRoomExit = true;
			RoomExitD_Round.IsFloodingIncompatible = true;
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
			RoomExitL_Round.DoorProbability = 0;
			RoomExitL_Round.IsRoomType = true;
			RoomExitL_Round.IsRoomExit = true;
			RoomExitL_Round.IsFloodingIncompatible = true;
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
			RoomExitR_Round.DoorProbability = 0;
			RoomExitR_Round.IsRoomType = true;
			RoomExitR_Round.IsRoomExit = true;
			RoomExitR_Round.IsFloodingIncompatible = true;
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
			RoomExitUL_U.DoorProbability = 50;
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
			RoomExitUL_L.DoorProbability = 50;
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
			RoomExitUL_UL.DoorProbability = 50;
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
			RoomExitUR_U.DoorProbability = 50;
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
			RoomExitUR_R.DoorProbability = 50;
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
			RoomExitUR_UR.DoorProbability = 50;
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
			RoomExitDL_D.DoorProbability = 50;
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
			RoomExitDL_L.DoorProbability = 50;
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
			RoomExitDL_DL.DoorProbability = 50;
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
			RoomExitDR_D.DoorProbability = 50;
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
			RoomExitDR_R.DoorProbability = 50;
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
			RoomExitDR_DR.DoorProbability = 50;
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
		public static List<Type> GetTypes(Coords coords)
		{
			List<Type> types = new List<Type>();
			
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

		public static Type ConvertRoomWallToExit(Type wall, Direction dir)
		{
			Type exit;
			
			if (wall == RoomWallU)
			{
				exit = RoomExitU;
			}
			else if (wall == RoomWallD)
			{
				exit = RoomExitD;
			}
			else if (wall == RoomWallL)
			{
				exit = RoomExitL; 
			}
			else if (wall == RoomWallR)
			{
				exit = RoomExitR;
			}
			else if (wall == RoomWallUL && dir == Direction.Up)
			{
				exit = RoomExitUL_U;
			}
			else if (wall == RoomWallUL && dir == Direction.Left)
			{
				exit = RoomExitUL_L;
			}
			else if (wall == RoomWallUR && dir == Direction.Up) 
			{
				exit = RoomExitUR_U;
			}
			else if (wall == RoomWallUR && dir == Direction.Right)
			{
				exit = RoomExitUR_R;
			}
			else if (wall == RoomWallDL && dir == Direction.Down)
			{
				exit = RoomExitDL_D;
			}
			else if (wall == RoomWallDL && dir == Direction.Left)
			{
				exit = RoomExitDL_L;
			}
			else if (wall == RoomWallDR && dir == Direction.Down)
			{
				exit = RoomExitDR_D;
			}
			else if (wall == RoomWallDR && dir == Direction.Right)
			{
				exit = RoomExitDR_R;
			}
			else if (wall == RoomWallUL && dir == Direction.UpLeft)
			{
				exit = RoomExitUL_UL;
			}
			else if (wall == RoomWallUR && dir == Direction.UpRight) 
			{
				exit = RoomExitUR_UR;
			}
			else if (wall == RoomWallDL && dir == Direction.DownLeft)
			{
				exit = RoomExitDL_DL;
			}
			else if (wall == RoomWallDR && dir == Direction.DownRight)
			{
				exit = RoomExitDR_DR;
			}
			else if ((wall == RoomExitUL_U && dir == Direction.Left) || (wall == RoomExitUL_L && dir == Direction.Up))
			{
				exit = RoomExitUL_UL;
			}
			else if ((wall == RoomExitUR_U && dir == Direction.Right) || (wall == RoomExitUR_R && dir == Direction.Up))
			{
				exit = RoomExitUR_UR;
			}
			else if ((wall == RoomExitDL_D && dir == Direction.Left) || (wall == RoomExitDL_L && dir == Direction.Down))
			{
				exit = RoomExitDL_DL;
			}
			else if ((wall == RoomExitDR_D && dir == Direction.Right) || (wall == RoomExitDR_R && dir == Direction.Down))
			{
				exit = RoomExitDR_DR;
			}
			else
			{
				exit = Inter;
			}
			
			return exit;
		}

		public static Type ConvertRoomExitToWall(Type exit, Direction dir, Description descr)
		{
			Type wall;
			
			if (exit == RoomExitU)
			{
				wall = RoomWallU;
			}
			else if (exit == RoomExitD)
			{
				wall = RoomWallD;
			}
			else if (exit == RoomExitL)
			{
				wall = RoomWallL; 
			}
			else if (exit == RoomExitR)
			{
				wall = RoomWallR; 
			}
			else if (dir == Direction.Up && exit == Inter && descr == Descriptions.Mines_Vert)
			{
				wall = JuncDLR;
			}
			else if (dir == Direction.Down && exit == Inter && descr == Descriptions.Mines_Vert)
			{
				wall = JuncULR;
			}
			else if (dir == Direction.Left && exit == Inter && descr == Descriptions.Mines_Horiz)
			{
				wall = JuncUDR;
			}
			else if (dir == Direction.Right && exit == Inter && descr == Descriptions.Mines_Horiz)
			{
				wall = JuncUDL;
			}
			else if (exit == RoomExitUL_U || exit == RoomExitUL_L)
			{
				wall = RoomWallUL;
			}
			else if (exit == RoomExitUR_U || exit == RoomExitUR_R)
			{
				wall = RoomWallUR;
			}
			else if (exit == RoomExitDL_D || exit == RoomExitDL_L)
			{
				wall = RoomWallDL;
			}
			else if (exit == RoomExitDR_D || exit == RoomExitDR_R)
			{
				wall = RoomWallDR;
			}
			else if (exit == RoomExitUL_UL && dir == Direction.Up)
			{
				wall = RoomExitUL_L;
			}
			else if (exit == RoomExitUL_UL && dir == Direction.Left)
			{
				wall = RoomExitUL_U;
			}
			else if (exit == RoomExitUR_UR && dir == Direction.Up)
			{
				wall = RoomExitUR_R;
			}
			else if (exit == RoomExitUR_UR && dir == Direction.Right)
			{
				wall = RoomExitUR_U;
			}
			else if (exit == RoomExitDL_DL && dir == Direction.Down)
			{
				wall = RoomExitDL_L;
			}
			else if (exit == RoomExitDL_DL && dir == Direction.Left)
			{
				wall = RoomExitDL_D;
			}
			else if (exit == RoomExitDR_DR && dir == Direction.Down)
			{
				wall = RoomExitDR_R;
			}
			else if (exit == RoomExitDR_DR && dir == Direction.Right)
			{
				wall = RoomExitDR_D;
			}
			else
			{
				throw new DungeonGenerateException();  // Unknown... scrap it (never happens).
			}
			
			return wall;
		}

		public static Type ConvertDeadEndToDownStairs(Type deadEnd)
		{    
			Type stairs = null;
			
			if (deadEnd == DeadU)
			{
				stairs = DownStairU;
			}
			else if (deadEnd == DeadD)
			{
				stairs = DownStairD;
			}
			else if (deadEnd == DeadL)
			{
				stairs = DownStairL; 
			}
			else if (deadEnd == DeadR)
			{
				stairs = DownStairR; 
			}
			
			return stairs;
		}

		public static Type ConvertRoomTypeToCatacomb(Type roomType)
		{
			Type catacomb;
			
			if (roomType == RoomWallU) 
			{
				catacomb = JuncDLR;
			}
			else if (roomType == RoomWallD)
			{
				catacomb = JuncULR;
			}
			else if (roomType == RoomWallL) 
			{
				catacomb = JuncUDR;
			}
			else if (roomType == RoomWallR)
			{
				catacomb = JuncUDL;
			}
			else if (roomType == RoomWallUL)
			{
				catacomb = ElbDR;
			}
			else if (roomType == RoomWallUR) 
			{
				catacomb = ElbDL;
			}
			else if (roomType == RoomWallDL)
			{
				catacomb = ElbUR;
			}
			else if (roomType == RoomWallDR)
			{
				catacomb = ElbUL;
			}
			else if (roomType == RoomExitUL_U)
			{
				catacomb = JuncUDR;
			}
			else if (roomType == RoomExitUL_L) 
			{
				catacomb = JuncDLR;
			}
			else if (roomType == RoomExitUR_U)
			{
				catacomb = JuncUDL;
			}
			else if (roomType == RoomExitUR_R) 
			{
				catacomb = JuncDLR;
			}
			else if (roomType == RoomExitDL_D)
			{
				catacomb = JuncUDR;
			}
			else if (roomType == RoomExitDL_L) 
			{
				catacomb = JuncULR;
			}
			else if (roomType == RoomExitDR_D)
			{
				catacomb = JuncUDL;
			}
			else if (roomType == RoomExitDR_R) 
			{
				catacomb = JuncULR;
			}
			else
			{
				catacomb = Inter;
			}
			
			return catacomb;
		}
	}
}