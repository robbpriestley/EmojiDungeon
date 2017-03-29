using System.Collections.Generic;

namespace DigitalWizardry.Dungeon
{	
	public class CellType
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

		public int InitialAvailableConnections { get; set; }  // Used when generating the dungeon to determine if other cells can be attached to a target cell.
		public int Weight { get; set; }                       // Influences the selection when types are being randomly determined.
		public int DoorProbability { get; set; }              // The probability that a door can be placed in a section of room.
		public string TextRep { get; set; }                   // Primary character used to represent a type in text.
		public string TextRep2 { get; set; }                  // Used for better rendering of the text representation, which appears "squished" in the horizontal dimension.
		public string Name { get; set; }                     // Name of type entity.
		
		public string Filepath
		{
			get { return "images/tiles/" + Name + ".png"; }
		}

		// *** END UTILITY MEMBERS ***

		public CellType()
		{
        	ForceGrowthCompatible = true;  // Attention! This is the only bool member which is initialized to true.
			RoomWallDirection = Direction.NoDir;
		}

		public bool ConnectsTo(CellType otherCell, Direction direction)
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

	public static class CellTypes
	{
		// *** BEGIN FIELD DECLARATIONS ***
		
		private static readonly CellType _emptyCell;
		private static readonly CellType _entrance;
		private static readonly CellType _vert;
		private static readonly CellType _horiz;
		private static readonly CellType _inter;
		private static readonly CellType _juncULR;
		private static readonly CellType _juncUDR;
		private static readonly CellType _juncDLR;
		private static readonly CellType _juncUDL;
		private static readonly CellType _elbUR;
		private static readonly CellType _elbDR;
		private static readonly CellType _elbDL;
		private static readonly CellType _elbUL;
		private static readonly CellType _deadU;
		private static readonly CellType _deadD;
		private static readonly CellType _deadL;
		private static readonly CellType _deadR;
		private static readonly CellType _upStairU;
		private static readonly CellType _upStairD;
		private static readonly CellType _upStairL;
		private static readonly CellType _upStairR;
		private static readonly CellType _downStairU;
		private static readonly CellType _downStairD;
		private static readonly CellType _downStairL;
		private static readonly CellType _downStairR;
		private static readonly CellType _roomSpace;
		private static readonly CellType _fountain;
		private static readonly CellType _roomWallU;
		private static readonly CellType _roomWallD;
		private static readonly CellType _roomWallL;
		private static readonly CellType _roomWallR;
		private static readonly CellType _roomWallU_Round;
		private static readonly CellType _roomWallD_Round;
		private static readonly CellType _roomWallL_Round;
		private static readonly CellType _roomWallR_Round;
		private static readonly CellType _roomWallUL;
		private static readonly CellType _roomWallUR;
		private static readonly CellType _roomWallDL;
		private static readonly CellType _roomWallDR;
		private static readonly CellType _roomWallUL_Round;
		private static readonly CellType _roomWallUR_Round;
		private static readonly CellType _roomWallDL_Round;
		private static readonly CellType _roomWallDR_Round;
		private static readonly CellType _roomWallULinv;
		private static readonly CellType _roomWallURinv;
		private static readonly CellType _roomWallDLinv;
		private static readonly CellType _roomWallDRinv;
		private static readonly CellType _roomExitU;
		private static readonly CellType _roomExitD;
		private static readonly CellType _roomExitL;
		private static readonly CellType _roomExitR;
		private static readonly CellType _roomExitU_Round;
		private static readonly CellType _roomExitD_Round;
		private static readonly CellType _roomExitL_Round;
		private static readonly CellType _roomExitR_Round;
		private static readonly CellType _roomExitUL_U;
		private static readonly CellType _roomExitUL_L;
		private static readonly CellType _roomExitUL_UL;
		private static readonly CellType _roomExitUR_U;
		private static readonly CellType _roomExitUR_R;
		private static readonly CellType _roomExitUR_UR;
		private static readonly CellType _roomExitDL_D;
		private static readonly CellType _roomExitDL_L;
		private static readonly CellType _roomExitDL_DL;
		private static readonly CellType _roomExitDR_D;
		private static readonly CellType _roomExitDR_R;
		private static readonly CellType _roomExitDR_DR;
		
		// *** END FIELD DECLARATIONS ***
		// *** BEGIN PROPERTY DECLARATIONS ***
		
		public static CellType EmptyCell { get { return _emptyCell; } }                // Empty, i.e. unused.
		public static CellType Entrance { get { return _entrance; } }                  // Entrance Cell, used only once on level 0...
		public static CellType Vert { get { return _vert; } }                          // Vertical Corridor            
		public static CellType Horiz { get { return _horiz; } }                        // Horizontal Corridor           
		public static CellType Inter { get { return _inter; } }                        // Intersection                 
		public static CellType JuncULR { get { return _juncULR; } }                    // Junction Up Left Right       
		public static CellType JuncUDR { get { return _juncUDR; } }                    // Junction Up Down Right       
		public static CellType JuncDLR { get { return _juncDLR; } }                    // Junction Down Left Right     
		public static CellType JuncUDL { get { return _juncUDL; } }                    // Junction Up Down Left        
		public static CellType ElbUR { get { return _elbUR; } }                        // Elbow Up Right               
		public static CellType ElbDR { get { return _elbDR; } }                        // Elbow Down Right             
		public static CellType ElbDL { get { return _elbDL; } }                        // Elbow Down Left              
		public static CellType ElbUL { get { return _elbUL; } }                        // Elbow Up Left                
		public static CellType DeadU { get { return _deadU; } }                        // Dead End Up                  
		public static CellType DeadD { get { return _deadD; } }                        // Dead End Down                
		public static CellType DeadL { get { return _deadL; } }                        // Dead End Left                
		public static CellType DeadR { get { return _deadR; } }                        // Dead End Right 
		public static CellType UpStairU { get { return _upStairU; } }                  // Stairs Up Connects Up        
		public static CellType UpStairD { get { return _upStairD; } }                  // Stairs Up Connects Down      
		public static CellType UpStairL { get { return _upStairL; } }                  // Stairs Up Connects Left      
		public static CellType UpStairR { get { return _upStairR; } }                  // Stairs Up Connects Right     
		public static CellType DownStairU { get { return _downStairU; } }              // Stairs Down Connects Up      
		public static CellType DownStairD { get { return _downStairD; } }              // Stairs Down Connects Down    
		public static CellType DownStairL { get { return _downStairL; } }              // Stairs Down Connects Left    
		public static CellType DownStairR { get { return _downStairR; } }              // Stairs Down Connects Right 
		public static CellType RoomSpace { get { return _roomSpace; } }                // Room Space
		public static CellType Fountain { get { return _fountain; } }                  // Healing Fountain
		public static CellType RoomWallU { get { return _roomWallU; } }                // Room Wall Up
		public static CellType RoomWallD { get { return _roomWallD; } }                // Room Wall Down
		public static CellType RoomWallL { get { return _roomWallL; } }                // Room Wall Left
		public static CellType RoomWallR { get { return _roomWallR; } }                // Room Wall Right
		public static CellType RoomWallU_Round { get { return _roomWallU_Round; } }    // Room Wall Up Round
		public static CellType RoomWallD_Round { get { return _roomWallD_Round; } }    // Room Wall Down Round
		public static CellType RoomWallL_Round { get { return _roomWallL_Round; } }    // Room Wall Left Round
		public static CellType RoomWallR_Round { get { return _roomWallR_Round; } }    // Room Wall Right Round
		public static CellType RoomWallUL { get { return _roomWallUL; } }              // Room Wall Up Left
		public static CellType RoomWallUR { get { return _roomWallUR; } }              // Room Wall Up Right
		public static CellType RoomWallDL { get { return _roomWallDL; } }              // Room Wall Down Left
		public static CellType RoomWallDR { get { return _roomWallDR; } }              // Room Wall Down Right
		public static CellType RoomWallUL_Round { get { return _roomWallUL_Round; } }  // Room Wall Up Left Round
		public static CellType RoomWallUR_Round { get { return _roomWallUR_Round; } }  // Room Wall Up Right Round
		public static CellType RoomWallDL_Round { get { return _roomWallDL_Round; } }  // Room Wall Down Left Round
		public static CellType RoomWallDR_Round { get { return _roomWallDR_Round; } }  // Room Wall Down Right Round
		public static CellType RoomWallULinv { get { return _roomWallULinv; } }        // Room Wall Up Left Inverted
		public static CellType RoomWallURinv { get { return _roomWallURinv; } }        // Room Wall Up Right Inverted
		public static CellType RoomWallDLinv { get { return _roomWallDLinv; } }        // Room Wall Down Left Inverted
		public static CellType RoomWallDRinv { get { return _roomWallDRinv; } }        // Room Wall Down Right Inverted
		public static CellType RoomExitU { get { return _roomExitU; } }                // Room Exit Up
		public static CellType RoomExitD { get { return _roomExitD; } }                // Room Exit Down
		public static CellType RoomExitL { get { return _roomExitL; } }                // Room Exit Left
		public static CellType RoomExitR { get { return _roomExitR; } }                // Room Exit Right
		public static CellType RoomExitU_Round { get { return _roomExitU_Round; } }    // Room Exit Up Round
		public static CellType RoomExitD_Round { get { return _roomExitD_Round; } }    // Room Exit Down Round
		public static CellType RoomExitL_Round { get { return _roomExitL_Round; } }    // Room Exit Left Round
		public static CellType RoomExitR_Round { get { return _roomExitR_Round; } }    // Room Exit Right Round
		public static CellType RoomExitUL_U { get { return _roomExitUL_U; } }          // Room Exit Up Left Corner, Exit Up
		public static CellType RoomExitUL_L { get { return _roomExitUL_L; } }          // Room Exit Up Left Corner, Exit Left
		public static CellType RoomExitUL_UL { get { return _roomExitUL_UL; } }        // Room Exit Up Left Corner, Exits Up and Left
		public static CellType RoomExitUR_U { get { return _roomExitUR_U; } }          // Room Exit Up Right Corner, Exit Up
		public static CellType RoomExitUR_R { get { return _roomExitUR_R; } }          // Room Exit Up Right Corner, Exit Right
		public static CellType RoomExitUR_UR { get { return _roomExitUR_UR; } }        // Room Exit Up Right Corner, Exits Up and Right
		public static CellType RoomExitDL_D { get { return _roomExitDL_D; } }          // Room Exit Down Left Corner, Exit Down
		public static CellType RoomExitDL_L { get { return _roomExitDL_L; } }          // Room Exit Down Left Corner, Exit Left
		public static CellType RoomExitDL_DL { get { return _roomExitDL_DL; } }        // Room Exit Down Left Corner, Exits Down and Left
		public static CellType RoomExitDR_D { get { return _roomExitDR_D; } }          // Room Exit Down Right Corner, Exit Down
		public static CellType RoomExitDR_R { get { return _roomExitDR_R; } }          // Room Exit Down Right Corner, Exit Right
		public static CellType RoomExitDR_DR { get { return _roomExitDR_DR; } }        // Room Exit Down Right Corner, Exits Down and Right
		
		// *** END PROPERTY DECLARATIONS ***

		static CellTypes()
		{
			_emptyCell = new CellType();
			_entrance = new CellType();
			_vert = new CellType();
			_horiz = new CellType();
			_inter = new CellType();
			_juncULR = new CellType();
			_juncUDR = new CellType();
			_juncDLR = new CellType();
			_juncUDL = new CellType();
			_elbUR = new CellType();
			_elbDR = new CellType();
			_elbDL = new CellType();
			_elbUL = new CellType();
			_deadU = new CellType();
			_deadD = new CellType();
			_deadL = new CellType();
			_deadR = new CellType();
			_upStairU = new CellType();
			_upStairD = new CellType();
			_upStairL = new CellType();
			_upStairR = new CellType();
			_downStairU = new CellType();
			_downStairD = new CellType();
			_downStairL = new CellType();
			_downStairR = new CellType();
			_roomSpace = new CellType();
			_fountain = new CellType();
			_roomWallU = new CellType();
			_roomWallD = new CellType();
			_roomWallL = new CellType();
			_roomWallR = new CellType();
			_roomWallU_Round = new CellType();
			_roomWallD_Round = new CellType();
			_roomWallL_Round = new CellType();
			_roomWallR_Round = new CellType();
			_roomWallUL = new CellType();
			_roomWallUR = new CellType();
			_roomWallDL = new CellType();
			_roomWallDR = new CellType();
			_roomWallUL_Round = new CellType();
			_roomWallUR_Round = new CellType();
			_roomWallDL_Round = new CellType();
			_roomWallDR_Round = new CellType();
			_roomWallULinv = new CellType();
			_roomWallURinv = new CellType();
			_roomWallDLinv = new CellType();
			_roomWallDRinv = new CellType();
			_roomExitU = new CellType();
			_roomExitD = new CellType();
			_roomExitL = new CellType();
			_roomExitR = new CellType();
			_roomExitU_Round = new CellType();
			_roomExitD_Round = new CellType();
			_roomExitL_Round = new CellType();
			_roomExitR_Round = new CellType();
			_roomExitUL_U = new CellType();
			_roomExitUL_L = new CellType();
			_roomExitUL_UL = new CellType();
			_roomExitUR_U = new CellType();
			_roomExitUR_R = new CellType();
			_roomExitUR_UR = new CellType();
			_roomExitDL_D = new CellType();
			_roomExitDL_L = new CellType();
			_roomExitDL_DL = new CellType();
			_roomExitDR_D = new CellType();
			_roomExitDR_R = new CellType();
			_roomExitDR_DR = new CellType();

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
			EmptyCell.Name = "EmptyCell";
			
			Entrance.Weight = 0;
			Entrance.ConnectsUp = true;
			Entrance.TraversableUp = true;
			Entrance.TextRep = @"❍";
			Entrance.TextRep2 = @" ";
			Entrance.ForceGrowthCompatible = false;
			Entrance.InitialAvailableConnections = 1;
			Entrance.Name = "Entrance";
			
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
			Vert.Name = "Vert";
			
			Horiz.Weight = 100;
			Horiz.ConnectsLeft = true;
			Horiz.ConnectsRight = true;
			Horiz.TraversableLeft = true;
			Horiz.TraversableRight = true;
			Horiz.TextRep = @"═";
			Horiz.TextRep2 = @"═";
			Horiz.IsFloodingTransition = true;
			Horiz.InitialAvailableConnections = 2;
			Horiz.Name = "Horiz";
			
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
			Inter.Name = "Inter";
			
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
			JuncULR.Name = "JuncULR";
			
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
			JuncUDR.Name = "JuncUDR";
			
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
			JuncDLR.Name = "JuncDLR";
			
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
			JuncUDL.Name = "JuncUDL";
			
			ElbUR.Weight = 20;
			ElbUR.ConnectsUp = true;
			ElbUR.ConnectsRight = true;
			ElbUR.TraversableUp = true;
			ElbUR.TraversableRight = true;
			ElbUR.TextRep = @"╚";
			ElbUR.TextRep2 = @"═";
			ElbUR.InitialAvailableConnections = 2;
			ElbUR.Name = "ElbUR";
			
			ElbDR.Weight = 20;
			ElbDR.ConnectsDown = true;
			ElbDR.ConnectsRight = true;
			ElbDR.TraversableDown = true;
			ElbDR.TraversableRight = true;
			ElbDR.TextRep = @"╔";
			ElbDR.TextRep2 = @"═";
			ElbDR.InitialAvailableConnections = 2;
			ElbDR.Name = "ElbDR";
			
			ElbDL.Weight = 20;
			ElbDL.ConnectsDown = true;
			ElbDL.ConnectsLeft = true;
			ElbDL.TraversableDown = true;
			ElbDL.TraversableLeft = true;
			ElbDL.TextRep = @"╗";
			ElbDL.TextRep2 = @" ";
			ElbDL.InitialAvailableConnections = 2;
			ElbDL.Name = "ElbDL";
			
			ElbUL.Weight = 20;
			ElbUL.ConnectsUp = true;
			ElbUL.ConnectsLeft = true;
			ElbUL.TraversableUp = true;
			ElbUL.TraversableLeft = true;
			ElbUL.TextRep = @"╝";
			ElbUL.TextRep2 = @" ";
			ElbUL.InitialAvailableConnections = 2;
			ElbUL.Name = "ElbUL";

			// *** END CORRIDOR CELLS ***
			// *** BEGIN DEAD END CELLS ***
			
			DeadU.Weight = 1;
			DeadU.ConnectsUp = true;
			DeadU.TraversableUp = true;
			DeadU.TextRep = @"╨";
			DeadU.TextRep2 = @" ";
			DeadU.IsDeadEnd = true;
			DeadU.InitialAvailableConnections = 1;
			DeadU.Name = "DeadU";
			
			DeadD.Weight = 1;
			DeadD.ConnectsDown = true;
			DeadD.TraversableDown = true;
			DeadD.TextRep = @"╥";
			DeadD.TextRep2 = @" ";
			DeadD.IsDeadEnd = true;
			DeadD.InitialAvailableConnections = 1;
			DeadD.Name = "DeadD";
			
			DeadL.Weight = 1;
			DeadL.ConnectsLeft = true;
			DeadL.TraversableLeft = true;
			DeadL.TextRep = @"╡";
			DeadL.TextRep2 = @" ";
			DeadL.IsDeadEnd = true;
			DeadL.InitialAvailableConnections = 1;
			DeadL.Name = "DeadL";
			
			DeadR.Weight = 1;
			DeadR.ConnectsRight = true;
			DeadR.TraversableRight = true;
			DeadR.TextRep = @"╞";
			DeadR.TextRep2 = @"═";
			DeadR.IsDeadEnd = true;
			DeadR.InitialAvailableConnections = 1;
			DeadR.Name = "DeadR";

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
			RoomSpace.Name = "RoomSpace";
			
			Fountain.TraversableUp = true;
			Fountain.TraversableDown = true;
			Fountain.TraversableLeft = true;
			Fountain.TraversableRight = true;
			Fountain.TextRep = "F";
			Fountain.TextRep2 = " ";
			Fountain.IsRoomType = true;
			Fountain.ForceGrowthCompatible = false;
			Fountain.Name = "Fountain";
		
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
			RoomWallU.Name = "RoomWallU";
			
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
			RoomWallD.Name = "RoomWallD";
			
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
			RoomWallL.Name = "RoomWallL";
			
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
			RoomWallR.Name = "RoomWallR";

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
			RoomWallU_Round.Name = "RoomWallU_Round";
			
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
			RoomWallD_Round.Name = "RoomWallD_Round";
			
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
			RoomWallL_Round.Name = "RoomWallL_Round";
			
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
			RoomWallR_Round.Name = "RoomWallR_Round";
	
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
			RoomWallUL.Name = "RoomWallUL";
			
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
			RoomWallUR.Name = "RoomWallUR";
			
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
			RoomWallDL.Name = "RoomWallDL";
			
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
			RoomWallDR.Name = "RoomWallDR";
			
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
			RoomWallUL_Round.Name = "RoomWallUL_Round";
			
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
			RoomWallUR_Round.Name = "RoomWallUR_Round";
			
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
			RoomWallDL_Round.Name = "RoomWallDL_Round";
			
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
			RoomWallDR_Round.Name = "RoomWallDR_Round";
			
			RoomWallULinv.TraversableDown = true;
			RoomWallULinv.TraversableRight = true;
			RoomWallULinv.RoomConnectsDown = true;
			RoomWallULinv.RoomConnectsRight = true;
			RoomWallULinv.TextRep = "┌";
			RoomWallULinv.TextRep2 = "─";
			RoomWallULinv.IsRoomType = true;
			RoomWallULinv.IsCleanStartWall = true;
			RoomWallULinv.ForceGrowthCompatible = false;
			RoomWallULinv.Name = "RoomWallULinv";
			
			RoomWallURinv.TraversableDown = true;
			RoomWallURinv.TraversableLeft = true;
			RoomWallURinv.RoomConnectsDown = true;
			RoomWallURinv.RoomConnectsLeft = true;
			RoomWallURinv.TextRep = "┐";
			RoomWallURinv.TextRep2 = " ";
			RoomWallURinv.IsRoomType = true;
			RoomWallURinv.IsCleanStartWall = true;
			RoomWallURinv.ForceGrowthCompatible = false;
			RoomWallURinv.Name = "RoomWallURinv";
			
			RoomWallDLinv.TraversableUp = true;
			RoomWallDLinv.TraversableRight = true;
			RoomWallDLinv.RoomConnectsUp = true;
			RoomWallDLinv.RoomConnectsRight = true;
			RoomWallDLinv.TextRep = "└";
			RoomWallDLinv.TextRep2 = "─";
			RoomWallDLinv.IsRoomType = true;
			RoomWallDLinv.ForceGrowthCompatible = false;
			RoomWallDLinv.Name = "RoomWallDLinv";
			
			RoomWallDRinv.TraversableUp = true;
			RoomWallDRinv.TraversableLeft = true;
			RoomWallDRinv.RoomConnectsUp = true;
			RoomWallDRinv.RoomConnectsLeft = true;
			RoomWallDRinv.TextRep = "┘";
			RoomWallDRinv.TextRep2 = " ";
			RoomWallDRinv.IsRoomType = true;
			RoomWallDRinv.ForceGrowthCompatible = false;
			RoomWallDRinv.Name = "RoomWallDRinv";
			
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
			RoomExitU.Name = "RoomExitU";
			
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
			RoomExitD.Name = "RoomExitD";
			
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
			RoomExitL.Name = "RoomExitL";
			
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
			RoomExitR.Name = "RoomExitR";
			
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
			RoomExitU_Round.Name = "RoomExitU_Round";
			
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
			RoomExitD_Round.Name = "RoomExitD_Round";
			
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
			RoomExitL_Round.Name = "RoomExitL_Round";
			
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
			RoomExitR_Round.Name = "RoomExitR_Round";
			
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
			RoomExitUL_U.Name = "RoomExitUL_U";
			
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
			RoomExitUL_L.Name = "RoomExitUL_L";
			
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
			RoomExitUL_UL.Name = "RoomExitUL_UL";
			
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
			RoomExitUR_U.Name = "RoomExitUR_U";
			
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
			RoomExitUR_R.Name = "RoomExitUR_R";
			
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
			RoomExitUR_UR.Name = "RoomExitUR_UR";
			
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
			RoomExitDL_D.Name = "RoomExitDL_D";
			
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
			RoomExitDL_L.Name = "RoomExitDL_L";
			
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
			RoomExitDL_DL.Name = "RoomExitDL_DL";
			
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
			RoomExitDR_D.Name = "RoomExitDR_D";
			
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
			RoomExitDR_R.Name = "RoomExitDR_R";
			
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
			RoomExitDR_DR.Name = "RoomExitDR_DR";

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
			UpStairU.Name = "UpStairU";
			
			UpStairD.Weight = 1;
			UpStairD.ConnectsDown = true;
			UpStairD.TraversableDown = true;
			UpStairD.TextRep = "^";
			UpStairD.TextRep2 = " ";
			UpStairD.InitialAvailableConnections = 1;
			UpStairD.ForceGrowthCompatible = false;
			UpStairD.Name = "UpStairD";
			
			UpStairL.Weight = 1;
			UpStairL.ConnectsLeft = true;
			UpStairL.TraversableLeft = true;
			UpStairL.TextRep = "^";
			UpStairL.TextRep2 = " ";
			UpStairL.InitialAvailableConnections = 1;
			UpStairL.ForceGrowthCompatible = false;
			UpStairL.Name = "UpStairL";
			
			UpStairR.Weight = 1;
			UpStairR.ConnectsRight = true;
			UpStairR.TraversableRight = true;
			UpStairR.TextRep = "^";
			UpStairR.TextRep2 = " ";
			UpStairR.InitialAvailableConnections = 1;
			UpStairR.ForceGrowthCompatible = false;
			UpStairR.Name = "UpStairR";
			
			DownStairU.Weight = 1;
			DownStairU.ConnectsUp = true;
			DownStairU.TraversableUp = true;
			DownStairU.TextRep = "v";
			DownStairU.TextRep2 = " ";
			DownStairU.InitialAvailableConnections = 1;
			DownStairU.ForceGrowthCompatible = false;
			DownStairU.Name = "DownStairU";
			
			DownStairD.Weight = 1;
			DownStairD.ConnectsDown = true;
			DownStairD.TraversableDown = true;
			DownStairD.TextRep = "v";
			DownStairD.TextRep2 = " ";
			DownStairD.InitialAvailableConnections = 1;
			DownStairD.ForceGrowthCompatible = false;
			DownStairD.Name = "DownStairD";
			
			DownStairL.Weight = 1;
			DownStairL.ConnectsLeft = true;
			DownStairL.TraversableLeft = true;
			DownStairL.TextRep = "v";
			DownStairL.TextRep2 = " ";
			DownStairL.InitialAvailableConnections = 1;
			DownStairL.ForceGrowthCompatible = false;
			DownStairL.Name = "DownStairL";
			
			DownStairR.Weight = 1;
			DownStairR.ConnectsRight = true;
			DownStairR.TraversableRight = true;
			DownStairR.TextRep = "v";
			DownStairR.TextRep2 = " ";
			DownStairR.InitialAvailableConnections = 1;
			DownStairR.ForceGrowthCompatible = false;
			DownStairR.Name = "DownStairR";
			
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

		public static CellType ConvertRoomWallToExit(CellType wall, Direction dir)
		{
			CellType exit;
			
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

		public static CellType ConvertRoomExitToWall(CellType exit, Direction dir, Description descr)
		{
			CellType wall;
			
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

		public static CellType ConvertDeadEndToDownStairs(CellType deadEnd)
		{    
			CellType stairs = null;
			
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

		public static CellType ConvertRoomTypeToCatacomb(CellType roomType)
		{
			CellType catacomb;
			
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