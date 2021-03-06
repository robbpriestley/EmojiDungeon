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
		public bool IsStairsUp { get; set; }
		public bool IsStairsDown { get; set; }
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
		private static readonly CellType _upStairsU;
		private static readonly CellType _upStairsD;
		private static readonly CellType _upStairsL;
		private static readonly CellType _upStairsR;
		private static readonly CellType _downStairsU;
		private static readonly CellType _downStairsD;
		private static readonly CellType _downStairsL;
		private static readonly CellType _downStairsR;
		private static readonly CellType _roomSpace;
		private static readonly CellType _roomWallU;
		private static readonly CellType _roomWallD;
		private static readonly CellType _roomWallL;
		private static readonly CellType _roomWallR;
		private static readonly CellType _roomWallUL;
		private static readonly CellType _roomWallUR;
		private static readonly CellType _roomWallDL;
		private static readonly CellType _roomWallDR;
		private static readonly CellType _roomWallULinv;
		private static readonly CellType _roomWallURinv;
		private static readonly CellType _roomWallDLinv;
		private static readonly CellType _roomWallDRinv;
		private static readonly CellType _roomExitU;
		private static readonly CellType _roomExitD;
		private static readonly CellType _roomExitL;
		private static readonly CellType _roomExitR;
		private static readonly CellType _roomExitUL_U;
		private static readonly CellType _roomExitUL_L;
		private static readonly CellType _roomExitUR_U;
		private static readonly CellType _roomExitUR_R;
		private static readonly CellType _roomExitDL_D;
		private static readonly CellType _roomExitDL_L;
		private static readonly CellType _roomExitDR_D;
		private static readonly CellType _roomExitDR_R;
		
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
		public static CellType UpStairsU { get { return _upStairsU; } }                  // Stairs Up Connects Up        
		public static CellType UpStairsD { get { return _upStairsD; } }                  // Stairs Up Connects Down      
		public static CellType UpStairsL { get { return _upStairsL; } }                  // Stairs Up Connects Left      
		public static CellType UpStairsR { get { return _upStairsR; } }                  // Stairs Up Connects Right     
		public static CellType DownStairsU { get { return _downStairsU; } }              // Stairs Down Connects Up      
		public static CellType DownStairsD { get { return _downStairsD; } }              // Stairs Down Connects Down    
		public static CellType DownStairsL { get { return _downStairsL; } }              // Stairs Down Connects Left    
		public static CellType DownStairsR { get { return _downStairsR; } }              // Stairs Down Connects Right 
		public static CellType RoomSpace { get { return _roomSpace; } }                // Room Space
		public static CellType RoomWallU { get { return _roomWallU; } }                // Room Wall Up
		public static CellType RoomWallD { get { return _roomWallD; } }                // Room Wall Down
		public static CellType RoomWallL { get { return _roomWallL; } }                // Room Wall Left
		public static CellType RoomWallR { get { return _roomWallR; } }                // Room Wall Right
		public static CellType RoomWallUL { get { return _roomWallUL; } }              // Room Wall Up Left
		public static CellType RoomWallUR { get { return _roomWallUR; } }              // Room Wall Up Right
		public static CellType RoomWallDL { get { return _roomWallDL; } }              // Room Wall Down Left
		public static CellType RoomWallDR { get { return _roomWallDR; } }              // Room Wall Down Right
		public static CellType RoomWallULinv { get { return _roomWallULinv; } }        // Room Wall Up Left Inverted
		public static CellType RoomWallURinv { get { return _roomWallURinv; } }        // Room Wall Up Right Inverted
		public static CellType RoomWallDLinv { get { return _roomWallDLinv; } }        // Room Wall Down Left Inverted
		public static CellType RoomWallDRinv { get { return _roomWallDRinv; } }        // Room Wall Down Right Inverted
		public static CellType RoomExitU { get { return _roomExitU; } }                // Room Exit Up
		public static CellType RoomExitD { get { return _roomExitD; } }                // Room Exit Down
		public static CellType RoomExitL { get { return _roomExitL; } }                // Room Exit Left
		public static CellType RoomExitR { get { return _roomExitR; } }                // Room Exit Right
		public static CellType RoomExitUL_U { get { return _roomExitUL_U; } }          // Room Exit Up Left Corner, Exit Up
		public static CellType RoomExitUL_L { get { return _roomExitUL_L; } }          // Room Exit Up Left Corner, Exit Left
		public static CellType RoomExitUR_U { get { return _roomExitUR_U; } }          // Room Exit Up Right Corner, Exit Up
		public static CellType RoomExitUR_R { get { return _roomExitUR_R; } }          // Room Exit Up Right Corner, Exit Right
		public static CellType RoomExitDL_D { get { return _roomExitDL_D; } }          // Room Exit Down Left Corner, Exit Down
		public static CellType RoomExitDL_L { get { return _roomExitDL_L; } }          // Room Exit Down Left Corner, Exit Left
		public static CellType RoomExitDR_D { get { return _roomExitDR_D; } }          // Room Exit Down Right Corner, Exit Down
		public static CellType RoomExitDR_R { get { return _roomExitDR_R; } }          // Room Exit Down Right Corner, Exit Right
		
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
			_upStairsU = new CellType();
			_upStairsD = new CellType();
			_upStairsL = new CellType();
			_upStairsR = new CellType();
			_downStairsU = new CellType();
			_downStairsD = new CellType();
			_downStairsL = new CellType();
			_downStairsR = new CellType();
			_roomSpace = new CellType();
			_roomWallU = new CellType();
			_roomWallD = new CellType();
			_roomWallL = new CellType();
			_roomWallR = new CellType();
			_roomWallUL = new CellType();
			_roomWallUR = new CellType();
			_roomWallDL = new CellType();
			_roomWallDR = new CellType();
			_roomWallULinv = new CellType();
			_roomWallURinv = new CellType();
			_roomWallDLinv = new CellType();
			_roomWallDRinv = new CellType();
			_roomExitU = new CellType();
			_roomExitD = new CellType();
			_roomExitL = new CellType();
			_roomExitR = new CellType();
			_roomExitUL_U = new CellType();
			_roomExitUL_L = new CellType();
			_roomExitUR_U = new CellType();
			_roomExitUR_R = new CellType();
			_roomExitDL_D = new CellType();
			_roomExitDL_L = new CellType();
			_roomExitDR_D = new CellType();
			_roomExitDR_R = new CellType();

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
			EmptyCell.Name = "a0";
			
			Entrance.Weight = 0;
			Entrance.ConnectsUp = true;
			Entrance.TraversableUp = true;
			Entrance.TextRep = @"║";
			Entrance.TextRep2 = @" ";
			Entrance.ForceGrowthCompatible = false;
			Entrance.InitialAvailableConnections = 1;
			Entrance.Name = "a1";
			
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
			Vert.Name = "a2";
			
			Horiz.Weight = 100;
			Horiz.ConnectsLeft = true;
			Horiz.ConnectsRight = true;
			Horiz.TraversableLeft = true;
			Horiz.TraversableRight = true;
			Horiz.TextRep = @"═";
			Horiz.TextRep2 = @"═";
			Horiz.IsFloodingTransition = true;
			Horiz.InitialAvailableConnections = 2;
			Horiz.Name = "a3";
			
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
			Inter.Name = "a4";
			
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
			JuncULR.Name = "a5";
			
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
			JuncUDR.Name = "a6";
			
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
			JuncDLR.Name = "a7";
			
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
			JuncUDL.Name = "a8";
			
			ElbUR.Weight = 20;
			ElbUR.ConnectsUp = true;
			ElbUR.ConnectsRight = true;
			ElbUR.TraversableUp = true;
			ElbUR.TraversableRight = true;
			ElbUR.TextRep = @"╚";
			ElbUR.TextRep2 = @"═";
			ElbUR.InitialAvailableConnections = 2;
			ElbUR.Name = "a9";
			
			ElbDR.Weight = 20;
			ElbDR.ConnectsDown = true;
			ElbDR.ConnectsRight = true;
			ElbDR.TraversableDown = true;
			ElbDR.TraversableRight = true;
			ElbDR.TextRep = @"╔";
			ElbDR.TextRep2 = @"═";
			ElbDR.InitialAvailableConnections = 2;
			ElbDR.Name = "b0";
			
			ElbDL.Weight = 20;
			ElbDL.ConnectsDown = true;
			ElbDL.ConnectsLeft = true;
			ElbDL.TraversableDown = true;
			ElbDL.TraversableLeft = true;
			ElbDL.TextRep = @"╗";
			ElbDL.TextRep2 = @" ";
			ElbDL.InitialAvailableConnections = 2;
			ElbDL.Name = "b1";
			
			ElbUL.Weight = 20;
			ElbUL.ConnectsUp = true;
			ElbUL.ConnectsLeft = true;
			ElbUL.TraversableUp = true;
			ElbUL.TraversableLeft = true;
			ElbUL.TextRep = @"╝";
			ElbUL.TextRep2 = @" ";
			ElbUL.InitialAvailableConnections = 2;
			ElbUL.Name = "b2";

			// *** END CORRIDOR CELLS ***
			// *** BEGIN DEAD END CELLS ***
			
			DeadU.Weight = 1;
			DeadU.ConnectsUp = true;
			DeadU.TraversableUp = true;
			DeadU.TextRep = @"╨";
			DeadU.TextRep2 = @" ";
			DeadU.IsDeadEnd = true;
			DeadU.InitialAvailableConnections = 1;
			DeadU.Name = "b3";
			
			DeadD.Weight = 1;
			DeadD.ConnectsDown = true;
			DeadD.TraversableDown = true;
			DeadD.TextRep = @"╥";
			DeadD.TextRep2 = @" ";
			DeadD.IsDeadEnd = true;
			DeadD.InitialAvailableConnections = 1;
			DeadD.Name = "b4";
			
			DeadL.Weight = 1;
			DeadL.ConnectsLeft = true;
			DeadL.TraversableLeft = true;
			DeadL.TextRep = @"╡";
			DeadL.TextRep2 = @" ";
			DeadL.IsDeadEnd = true;
			DeadL.InitialAvailableConnections = 1;
			DeadL.Name = "b5";
			
			DeadR.Weight = 1;
			DeadR.ConnectsRight = true;
			DeadR.TraversableRight = true;
			DeadR.TextRep = @"╞";
			DeadR.TextRep2 = @"═";
			DeadR.IsDeadEnd = true;
			DeadR.InitialAvailableConnections = 1;
			DeadR.Name = "b6";

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
			RoomSpace.Name = "b7";
		
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
			RoomWallU.Name = "b8";
			
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
			RoomWallD.Name = "b9";
			
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
			RoomWallL.Name = "c0";
			
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
			RoomWallR.Name = "c1";
	
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
			RoomWallUL.Name = "c2";
			
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
			RoomWallUR.Name = "c3";
			
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
			RoomWallDL.Name = "c4";
			
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
			RoomWallDR.Name = "c5";
			
			RoomWallULinv.TraversableUp = true;
			RoomWallULinv.TraversableDown = true;
			RoomWallULinv.TraversableLeft = true;
			RoomWallULinv.TraversableRight = true;
			RoomWallULinv.RoomConnectsDown = true;
			RoomWallULinv.RoomConnectsRight = true;
			RoomWallULinv.TextRep = "┌";
			RoomWallULinv.TextRep2 = "─";
			RoomWallULinv.IsRoomType = true;
			RoomWallULinv.IsCleanStartWall = true;
			RoomWallULinv.ForceGrowthCompatible = false;
			RoomWallULinv.Name = "c6";
			
			RoomWallURinv.TraversableUp = true;
			RoomWallURinv.TraversableDown = true;
			RoomWallURinv.TraversableLeft = true;
			RoomWallURinv.TraversableRight = true;
			RoomWallURinv.RoomConnectsDown = true;
			RoomWallURinv.RoomConnectsLeft = true;
			RoomWallURinv.TextRep = "┐";
			RoomWallURinv.TextRep2 = " ";
			RoomWallURinv.IsRoomType = true;
			RoomWallURinv.IsCleanStartWall = true;
			RoomWallURinv.ForceGrowthCompatible = false;
			RoomWallURinv.Name = "c7";
			
			RoomWallDLinv.TraversableUp = true;
			RoomWallDLinv.TraversableDown = true;
			RoomWallDLinv.TraversableLeft = true;
			RoomWallDLinv.TraversableRight = true;
			RoomWallDLinv.RoomConnectsUp = true;
			RoomWallDLinv.RoomConnectsRight = true;
			RoomWallDLinv.TextRep = "└";
			RoomWallDLinv.TextRep2 = "─";
			RoomWallDLinv.IsRoomType = true;
			RoomWallDLinv.ForceGrowthCompatible = false;
			RoomWallDLinv.Name = "c8";
			
			RoomWallDRinv.TraversableUp = true;
			RoomWallDRinv.TraversableDown = true;
			RoomWallDRinv.TraversableLeft = true;
			RoomWallDRinv.TraversableRight = true;
			RoomWallDRinv.RoomConnectsUp = true;
			RoomWallDRinv.RoomConnectsLeft = true;
			RoomWallDRinv.TextRep = "┘";
			RoomWallDRinv.TextRep2 = " ";
			RoomWallDRinv.IsRoomType = true;
			RoomWallDRinv.ForceGrowthCompatible = false;
			RoomWallDRinv.Name = "c9";
			
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
			RoomExitU.DoorProbability = 100;
			RoomExitU.IsRoomType = true;
			RoomExitU.IsRoomExit = true;
			RoomExitU.ForceGrowthCompatible = false;
			RoomExitU.Name = "d0";
			
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
			RoomExitD.DoorProbability = 100;
			RoomExitD.IsRoomType = true;
			RoomExitD.IsRoomExit = true;
			RoomExitD.IsCleanStartWall = true;
			RoomExitD.ForceGrowthCompatible = false;
			RoomExitD.Name = "d1";
			
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
			RoomExitL.DoorProbability = 100;
			RoomExitL.IsRoomType = true;
			RoomExitL.IsRoomExit = true;
			RoomExitL.ForceGrowthCompatible = false;
			RoomExitL.Name = "d2";
			
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
			RoomExitR.DoorProbability = 100;
			RoomExitR.IsRoomType = true;
			RoomExitR.IsRoomExit = true;
			RoomExitR.ForceGrowthCompatible = false;
			RoomExitR.Name = "d3";
			
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
			RoomExitUL_U.DoorProbability = 100;
			RoomExitUL_U.IsRoomExit = true;
			RoomExitUL_U.RoomExitCompatible = true;
			RoomExitUL_U.ForceGrowthCompatible = false;
			RoomExitUL_U.Name = "d4";
			
			RoomExitUL_L.ConnectsLeft = true;
			RoomExitUL_L.TraversableDown = true;
			RoomExitUL_L.TraversableLeft = true;
			RoomExitUL_L.TraversableRight = true;
			RoomExitUL_L.RoomConnectsDown = true;
			RoomExitUL_L.RoomConnectsRight = true;
			RoomExitUL_L.InitialAvailableConnections = 1;
			RoomExitUL_L.TextRep = "┭";
			RoomExitUL_L.TextRep2 = "─";
			RoomExitUL_L.DoorProbability = 100;
			RoomExitUL_L.IsRoomExit = true;
			RoomExitUL_L.RoomExitCompatible = true;
			RoomExitUL_L.ForceGrowthCompatible = false;
			RoomExitUL_L.Name = "d5";
			
			RoomExitUR_U.ConnectsUp = true;
			RoomExitUR_U.TraversableUp = true;
			RoomExitUR_U.TraversableDown = true;
			RoomExitUR_U.TraversableLeft = true;
			RoomExitUR_U.RoomConnectsDown = true;
			RoomExitUR_U.RoomConnectsLeft = true;
			RoomExitUR_U.InitialAvailableConnections = 1;
			RoomExitUR_U.TextRep = "┦";
			RoomExitUR_U.TextRep2 = " ";
			RoomExitUR_U.DoorProbability = 100;
			RoomExitUR_U.IsRoomExit = true;
			RoomExitUR_U.RoomExitCompatible = true;
			RoomExitUR_U.ForceGrowthCompatible = false;
			RoomExitUR_U.Name = "d6";
			
			RoomExitUR_R.ConnectsRight = true;
			RoomExitUR_R.TraversableDown = true;
			RoomExitUR_R.TraversableLeft = true;
			RoomExitUR_R.TraversableRight = true;
			RoomExitUR_R.RoomConnectsDown = true;
			RoomExitUR_R.RoomConnectsLeft = true;
			RoomExitUR_R.InitialAvailableConnections = 1;
			RoomExitUR_R.TextRep = "┮";
			RoomExitUR_R.TextRep2 = "═";
			RoomExitUR_R.DoorProbability = 100;
			RoomExitUR_R.IsRoomExit = true;
			RoomExitUR_R.RoomExitCompatible = true;
			RoomExitUR_R.ForceGrowthCompatible = false;
			RoomExitUR_R.Name = "d7";
			
			RoomExitDL_D.ConnectsDown = true;
			RoomExitDL_D.TraversableUp = true;
			RoomExitDL_D.TraversableDown = true;
			RoomExitDL_D.TraversableRight = true;
			RoomExitDL_D.RoomConnectsUp = true;
			RoomExitDL_D.RoomConnectsRight = true;
			RoomExitDL_D.InitialAvailableConnections = 1;
			RoomExitDL_D.TextRep = "┟";
			RoomExitDL_D.TextRep2 = "─";
			RoomExitDL_D.DoorProbability = 100;
			RoomExitDL_D.IsRoomExit = true;
			RoomExitDL_D.RoomExitCompatible = true;
			RoomExitDL_D.ForceGrowthCompatible = false;
			RoomExitDL_D.Name = "d8";
			
			RoomExitDL_L.ConnectsLeft = true;
			RoomExitDL_L.TraversableUp = true;
			RoomExitDL_L.TraversableLeft = true;
			RoomExitDL_L.TraversableRight = true;
			RoomExitDL_L.RoomConnectsUp = true;
			RoomExitDL_L.RoomConnectsRight = true;
			RoomExitDL_L.InitialAvailableConnections = 1;
			RoomExitDL_L.TextRep = "┵";
			RoomExitDL_L.TextRep2 = "─";
			RoomExitDL_L.DoorProbability = 100;
			RoomExitDL_L.IsRoomExit = true;
			RoomExitDL_L.RoomExitCompatible = true;
			RoomExitDL_L.ForceGrowthCompatible = false;
			RoomExitDL_L.Name = "d9";
			
			RoomExitDR_D.ConnectsDown = true;
			RoomExitDR_D.TraversableUp = true;
			RoomExitDR_D.TraversableDown = true;
			RoomExitDR_D.TraversableLeft = true;
			RoomExitDR_D.RoomConnectsUp = true;
			RoomExitDR_D.RoomConnectsLeft = true;
			RoomExitDR_D.InitialAvailableConnections = 1;
			RoomExitDR_D.TextRep = "┧";
			RoomExitDR_D.TextRep2 = " ";
			RoomExitDR_D.DoorProbability = 100;
			RoomExitDR_D.IsRoomExit = true;
			RoomExitDR_D.RoomExitCompatible = true;
			RoomExitDR_D.ForceGrowthCompatible = false;
			RoomExitDR_D.Name = "e0";
			
			RoomExitDR_R.ConnectsRight = true;
			RoomExitDR_R.TraversableUp = true;
			RoomExitDR_R.TraversableLeft = true;
			RoomExitDR_R.TraversableRight = true;
			RoomExitDR_R.RoomConnectsUp = true;
			RoomExitDR_R.RoomConnectsLeft = true;
			RoomExitDR_R.InitialAvailableConnections = 1;
			RoomExitDR_R.TextRep = "┶";
			RoomExitDR_R.TextRep2 = "═";
			RoomExitDR_R.DoorProbability = 100;
			RoomExitDR_R.IsRoomExit = true;
			RoomExitDR_R.RoomExitCompatible = true;
			RoomExitDR_R.ForceGrowthCompatible = false;
			RoomExitDR_R.Name = "e1";

			// *** END ROOM CORNER EXITS ***
			// *** BEGIN STAIRWAY CELLS ***
			// These cells are specially placed, prior the falsermal cell placement routine.
			
			UpStairsU.Weight = 1;
			UpStairsU.ConnectsUp = true;
			UpStairsU.TraversableUp = true;
			UpStairsU.TextRep = "^";
			UpStairsU.TextRep2 = " ";
			UpStairsU.InitialAvailableConnections = 1;
			UpStairsU.ForceGrowthCompatible = false;
			UpStairsU.IsStairsUp = true;
			UpStairsU.Name = "e2";
			
			UpStairsD.Weight = 1;
			UpStairsD.ConnectsDown = true;
			UpStairsD.TraversableDown = true;
			UpStairsD.TextRep = "^";
			UpStairsD.TextRep2 = " ";
			UpStairsD.InitialAvailableConnections = 1;
			UpStairsD.ForceGrowthCompatible = false;
			UpStairsD.IsStairsUp = true;
			UpStairsD.Name = "e3";
			
			UpStairsL.Weight = 1;
			UpStairsL.ConnectsLeft = true;
			UpStairsL.TraversableLeft = true;
			UpStairsL.TextRep = "^";
			UpStairsL.TextRep2 = " ";
			UpStairsL.InitialAvailableConnections = 1;
			UpStairsL.ForceGrowthCompatible = false;
			UpStairsL.IsStairsUp = true;
			UpStairsL.Name = "e4";
			
			UpStairsR.Weight = 1;
			UpStairsR.ConnectsRight = true;
			UpStairsR.TraversableRight = true;
			UpStairsR.TextRep = "^";
			UpStairsR.TextRep2 = " ";
			UpStairsR.InitialAvailableConnections = 1;
			UpStairsR.ForceGrowthCompatible = false;
			UpStairsR.IsStairsUp = true;
			UpStairsR.Name = "e5";
			
			DownStairsU.Weight = 1;
			DownStairsU.ConnectsUp = true;
			DownStairsU.TraversableUp = true;
			DownStairsU.TextRep = "v";
			DownStairsU.TextRep2 = " ";
			DownStairsU.InitialAvailableConnections = 1;
			DownStairsU.ForceGrowthCompatible = false;
			DownStairsU.IsStairsDown = true;
			DownStairsU.Name = "e6";
			
			DownStairsD.Weight = 1;
			DownStairsD.ConnectsDown = true;
			DownStairsD.TraversableDown = true;
			DownStairsD.TextRep = "v";
			DownStairsD.TextRep2 = " ";
			DownStairsD.InitialAvailableConnections = 1;
			DownStairsD.ForceGrowthCompatible = false;
			DownStairsD.IsStairsDown = true;
			DownStairsD.Name = "e7";
			
			DownStairsL.Weight = 1;
			DownStairsL.ConnectsLeft = true;
			DownStairsL.TraversableLeft = true;
			DownStairsL.TextRep = "v";
			DownStairsL.TextRep2 = " ";
			DownStairsL.InitialAvailableConnections = 1;
			DownStairsL.ForceGrowthCompatible = false;
			DownStairsL.IsStairsDown = true;
			DownStairsL.Name = "e8";
			
			DownStairsR.Weight = 1;
			DownStairsR.ConnectsRight = true;
			DownStairsR.TraversableRight = true;
			DownStairsR.TextRep = "v";
			DownStairsR.TextRep2 = " ";
			DownStairsR.InitialAvailableConnections = 1;
			DownStairsR.ForceGrowthCompatible = false;
			DownStairsR.IsStairsDown = true;
			DownStairsR.Name = "e9";
			
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
			if (wall == RoomWallU)
			{
				return RoomExitU;
			}
			else if (wall == RoomWallD)
			{
				return RoomExitD;
			}
			else if (wall == RoomWallL)
			{
				return RoomExitL; 
			}
			else if (wall == RoomWallR)
			{
				return RoomExitR;
			}
			else if (wall == RoomWallUL && dir == Direction.Up)
			{
				return RoomExitUL_U;
			}
			else if (wall == RoomWallUL && dir == Direction.Left)
			{
				return RoomExitUL_L;
			}
			else if (wall == RoomWallUR && dir == Direction.Up) 
			{
				return RoomExitUR_U;
			}
			else if (wall == RoomWallUR && dir == Direction.Right)
			{
				return RoomExitUR_R;
			}
			else if (wall == RoomWallDL && dir == Direction.Down)
			{
				return RoomExitDL_D;
			}
			else if (wall == RoomWallDL && dir == Direction.Left)
			{
				return RoomExitDL_L;
			}
			else if (wall == RoomWallDR && dir == Direction.Down)
			{
				return RoomExitDR_D;
			}
			else if (wall == RoomWallDR && dir == Direction.Right)
			{
				return RoomExitDR_R;
			}
			else
			{
				throw new DungeonGenerateException();  // Unknown... scrap it (never happens).
			}
		}

		public static CellType ConvertRoomExitToWall(CellType exit, Direction dir)
		{		
			if (exit == RoomExitU)
			{
				return RoomWallU;
			}
			else if (exit == RoomExitD)
			{
				return RoomWallD;
			}
			else if (exit == RoomExitL)
			{
				return RoomWallL; 
			}
			else if (exit == RoomExitR)
			{
				return RoomWallR; 
			}
			else if (exit == RoomExitUL_U || exit == RoomExitUL_L)
			{
				return RoomWallUL;
			}
			else if (exit == RoomExitUR_U || exit == RoomExitUR_R)
			{
				return RoomWallUR;
			}
			else if (exit == RoomExitDL_D || exit == RoomExitDL_L)
			{
				return RoomWallDL;
			}
			else if (exit == RoomExitDR_D || exit == RoomExitDR_R)
			{
				return RoomWallDR;
			}
			else
			{
				throw new DungeonGenerateException();  // Unknown... scrap it (never happens).
			}
		}

		public static CellType ConvertDeadEndToDownStairs(CellType deadEnd)
		{    
			CellType stairs = null;
			
			if (deadEnd == DeadU)
			{
				stairs = DownStairsU;
			}
			else if (deadEnd == DeadD)
			{
				stairs = DownStairsD;
			}
			else if (deadEnd == DeadL)
			{
				stairs = DownStairsL; 
			}
			else if (deadEnd == DeadR)
			{
				stairs = DownStairsR; 
			}
			
			return stairs;
		}
	}
}