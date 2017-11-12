"use strict";

let FadeTime: number = 2000;
let GridMax: number = 15;
let Level: number;
let Score: number;
let KeyCount: number;
let HeartCount: number;
let SwordCount: number;
let IsGameOver: boolean;
let PlayerCoords: Coords;
let StartCoords: Array<Coords>;
let Dungeon: Array<object> = new Array<object>();

window.onload = function(){ Reset(); }

function Reset(): void
{
	Level = 1;
	Score = 0;
	KeyCount = 0;
	HeartCount = 3;
	SwordCount = 0;
	IsGameOver = false;
	UpdateCounts();
	UpdateHighScore();
	RemoveExplosions();
	RemoveAttackSwords();
	$("#gameOver").hide();
	$("#newHighScore").hide();
	$("#level").text("1");
	StartCoords = new Array<Coords>();
	StartCoords[1] = new Coords(7, 0);
	Dungeon = new Array<object>();
	RenderLevel();
}

function LevelUp(): void
{
	Level--;  // Levels closer to the "surface" have smaller numbers.
	RenderLevel();
	$("#level").text(Level);
}

function LevelDown(): void
{
	StartCoords[Level] = new Coords(PlayerCoords.X, PlayerCoords.Y);
	Level++;
	RenderLevel();
	$("#level").text(Level);  // Add 1 for display as _level is zero-indexed.
}

function RenderLevel()
{
	let dungeon: object = Dungeon[Level];
	
	if (dungeon != null)
	{
		BuildDungeon(Level, dungeon);
	}
	else
	{
		let spinner: Spinner = SpinnerSetup();
		spinner.spin($('#grid')[0]);

		let startCoords: Coords = StartCoords[Level];
		let sX: number = Level == 1 ? 7 : Number(startCoords.X);
		let sY: number = Level == 1 ? 0 : Number(startCoords.Y);
		let sD: string = Level == 1 ? "U" : startCoords.Dir;
		
		$.ajax
		({
			type: 'GET',
			dataType: 'json',
			data: { level: Level, startX: sX, startY: sY, direction: sD },
			contentType: 'application/json',
			url: '/Index/DungeonView',
			success: function (result) 
			{
				dungeon = JSON.parse(result);  // As this is a reset, assign the JSON to element 0 of the dungeon array.
				Dungeon[Level] = dungeon;
				BuildDungeon(Level, dungeon);
				spinner.stop();
			}
		});
	}
}

function SpinnerSetup(): Spinner
{
	var opts = 
	{
		lines: 9,
		length: 0,
		width: 13,
		radius: 21,
		color: '#fff',
		opacity: 0.25,
		rotate: 33
	}

	return new Spinner(opts);
}

// This function is going to iterate through each div in the grid and determine its location. It will then
// strip it of any existing CSS classes. This will clear any dungeon level currently being presented. That
// also removes the "native" classes the div needs ("tile" and it's location class). Those classes are 
// added back on, as well as the tile name class that allows the div to display a graphical tile from the
// tile sprite sheet.
function BuildDungeon(level: number, dungeon: object): void
{
	RemoveSprites();
	
	for (var x = 0; x < GridMax; x++) 
	{
		for (let y = 0; y < GridMax; y++)
		{
			let gridReference: string = GridReference(x, y);
			let gridId: string = "#" + gridReference;       // Prepend the "#" because jQuery needs it for the id attribute.
			$(gridId).removeClass();                         // Remove all classes on the div.
			$(gridId).addClass("tile");                      // Add "native" class.
			$(gridId).addClass(gridReference);               // Add "native" class.
			
			let tileName: string = dungeon[x][y].CssName;
			$(gridId).addClass(tileName);                    // Add tile name class.

			if (tileName == "e6" || tileName == "e7" || tileName == "e8" || tileName == "e9")
			{
				RecordStart(level, tileName, new Coords(x, y));
			}
			
			PlaceContents(x, y);
		}
	}
	
	PlacePlayer();
}

// Remove any sprites that are "local" to a different dungeon level.
function RemoveSprites(): void
{
	$('.player').each
	(
		function(i, obj) 
		{
			obj.remove();
		}
	);

	$('.goblin').each
	(
		function(i, obj) 
		{
			obj.remove();
		}
	);
	
	$('.doorh').each
	(
		function(i, obj) 
		{
			obj.remove();
		}
	);

	$('.doorv').each
	(
		function(i, obj) 
		{
			obj.remove();
		}
	);

	$('.key').each
	(
		function(i, obj) 
		{
			if (obj.id != "keyCountIcon")
			{
				obj.remove();
			}
		}
	);

	$('.gem').each
	(
		function(i, obj) 
		{
			obj.remove();
		}
	);

	$('.heart').each
	(
		function(i, obj) 
		{
			if (obj.id != "heartCountIcon")
			{
				obj.remove();
			}
		}
	);

	$('.sword').each
	(
		function(i, obj) 
		{
			if (obj.id != "swordCountIcon")
			{
				obj.remove();
			}
		}
	);
}

// Synthesize a grid reference given X and Y coords.
function GridReference(x: number, y: number): string
{
	let xs = x < 10 ? "0" + x.toString(): x.toString();
	let ys = y < 10 ? "0" + y.toString(): y.toString();
	return "g" + xs + ys;
}

function RecordStart(level: number, tileName: string, coords: Coords): void
{
	let direction: string = null;

	switch (tileName)
	{
		case "e6":
			coords.Dir = "U";
			break;

		case "e7":
			coords.Dir = "D";
			break;

		case "e8":
			coords.Dir = "L";
			break;

		case "e9":
			coords.Dir = "R";
			break;
	
		default:
			break;
	}

	StartCoords[Level + 1] = coords;
}

function UpdateCounts(): void
{
	$("#scoreCount").html(Score.toString());
	$("#keyCount").html(KeyCount.toString());
	$("#heartCount").html(HeartCount.toString());
	$("#swordCount").html(SwordCount.toString());
}

// *** BEGIN PLAYER ***

function PlacePlayer(): void
{	
	let startCoords: Coords = StartCoords[Level];
	let x: number = Number(startCoords.X);
	let y: number = Number(startCoords.Y);
	
	PlayerCoords = new Coords(x, y);

	let xPixels: number = x * 45;
	let yPixels: number = 630 - (y * 45);
	$("#grid").append('<div id="player" class="sprite player" style="top: ' + yPixels + 'px; left: ' + xPixels + 'px;"></div>');
}

function RepositionPlayer(coords: Coords): void
{
	PlayerCoords = coords;
	let xPixels: number = coords.X * 45;
	let yPixels: number = 630 - (coords.Y * 45);
	$("#player").css("top", yPixels + "px");
	$("#player").css("left", xPixels + "px");
}

// *** END PLAYER ***
// *** BEGIN GOBLINS ***

function PlaceGoblin(x: number, y: number): void
{
	let xPixels: number = x * 45;
	let yPixels: number = 630 - (y * 45);
	let reference = "goblin" + GridReference(x, y);
	$("#grid").append('<div id="' + reference + '" class="sprite goblin" style="top: ' + yPixels + 'px; left: ' + xPixels + 'px;"></div>');
}

function RemoveGoblin(x: number, y: number): void
{
	Dungeon[Level][x][y].HasGoblin = false;
	$("#goblin" + GridReference(x, y)).remove();
}

// *** END GOBLINS ***
// *** BEGIN CONTENTS ***

function PlaceContents(x: number, y: number): void
{
	if (Dungeon[Level][x][y].DoorDirection != "")
	{
		PlaceDoor(x, y, Dungeon[Level][x][y].DoorDirection);
	}		
	else if (Dungeon[Level][x][y].HasKey)
	{
		PlaceKey(x, y);
	}
	else if (Dungeon[Level][x][y].HasGem != "")
	{
		PlaceGem(x, y);
	}
	else if (Dungeon[Level][x][y].HasHeart != "")
	{
		PlaceHeart(x, y);
	}
	else if (Dungeon[Level][x][y].HasSword != "")
	{
		PlaceSword(x, y);
	}
	else if (Dungeon[Level][x][y].HasGoblin != "")
	{
		PlaceGoblin(x, y);
	}
}

function PlaceDoor(x: number, y: number, direction: string): void
{
	let xPixels: number = x * 45 + DoorXFudge(direction);
	let yPixels: number = 630 - (y * 45) + DoorYFudge(direction);
	let doorClass: string = direction == "U" || direction == "D" ? "doorh": "doorv";
	let reference = "door" + GridReference(x, y); 	
	$("#grid").append('<div id="' + reference + '" class="sprite ' + doorClass + '" style="top: ' + yPixels + 'px; left: ' + xPixels + 'px;"></div>');
}

function RemoveDoor(x: number, y: number): void
{
	Dungeon[Level][x][y].DoorDirection = "";
	$("#door" + GridReference(x, y)).remove();
}

function DoorXFudge(direction: string): number
{
	let fudge: number = 0;

	switch (direction)
	{
		case "U":
			fudge = 4;
			break;
		
		case "D":
			fudge = 4;
			break;
		
		case "L":
			fudge = -8;
			break;
		
		case "R":
			fudge = 38;
			break;
	
		default:
			break;
	}

	return fudge;
}

function DoorYFudge(direction: string): number
{
	let fudge: number = 0;

	switch (direction)
	{
		case "U":
			fudge = -5;
			break;
		
		case "D":
			fudge = 40;
			break;
		
		case "L":
			fudge = 9;
			break;
		
		case "R":
			fudge = 9;
			break;
	
		default:
			break;
	}

	return fudge;
}

function PlaceKey(x: number, y: number): void
{
	let xPixels: number = x * 45;
	let yPixels: number = 630 - (y * 45);
	let reference = "key" + GridReference(x, y);
	$("#grid").append('<div id="' + reference + '" class="sprite key" style="top: ' + yPixels + 'px; left: ' + xPixels + 'px;"></div>');
}

function RemoveKey(x: number, y: number): void
{
	Dungeon[Level][x][y].HasKey = false;
	$("#key" + GridReference(x, y)).remove();
}

function PlaceGem(x: number, y: number): void
{
	let xPixels: number = x * 45;
	let yPixels: number = 630 - (y * 45);
	let reference = "gem" + GridReference(x, y);
	$("#grid").append('<div id="' + reference + '" class="sprite gem" style="top: ' + yPixels + 'px; left: ' + xPixels + 'px;"></div>');
}

function RemoveGem(x: number, y: number): void
{
	Dungeon[Level][x][y].HasGem = false;
	$("#gem" + GridReference(x, y)).remove();
}

function PlaceHeart(x: number, y: number): void
{
	let xPixels: number = x * 45;
	let yPixels: number = 630 - (y * 45);
	let reference = "heart" + GridReference(x, y);
	$("#grid").append('<div id="' + reference + '" class="sprite heart" style="top: ' + yPixels + 'px; left: ' + xPixels + 'px;"></div>');
}

function RemoveHeart(x: number, y: number): void
{
	Dungeon[Level][x][y].HasHeart = false;
	$("#heart" + GridReference(x, y)).remove();
}

function PlaceSword(x: number, y: number): void
{
	let xPixels: number = x * 45;
	let yPixels: number = 630 - (y * 45);
	let reference = "sword" + GridReference(x, y);
	$("#grid").append('<div id="' + reference + '" class="sprite sword" style="top: ' + yPixels + 'px; left: ' + xPixels + 'px;"></div>');
}

function RemoveSword(x: number, y: number): void
{
	Dungeon[Level][x][y].HasSword = false;
	$("#sword" + GridReference(x, y)).remove();
}

function PlaceAttackSword(x: number, y: number): void
{
	let xPixels: number = x * 45;
	let yPixels: number = 630 - (y * 45);
	let reference = "attack" + GridReference(x, y);
	$("#grid").append('<div id="' + reference + '" class="sprite sword attack" style="top: ' + yPixels + 'px; left: ' + xPixels + 'px;"></div>');
	$(".attack").fadeOut();  // Keep it generic so if there are any overlapped sprites they all get faded out.
}

function RemoveAttackSwords(): void
{
	$(".attack").remove();
}

function PlaceExplosion(x: number, y: number, fade: boolean): void
{
	let xPixels: number = x * 45;
	let yPixels: number = 630 - (y * 45);
	let reference = "exp" + GridReference(x, y);
	$("#grid").append('<div id="' + reference + '" class="sprite exp" style="top: ' + yPixels + 'px; left: ' + xPixels + 'px;"></div>');
	
	if (fade)
	{
		$(".exp").fadeOut();  // Keep it generic so if there are any overlapped sprites they all get faded out.
	}
}

function RemoveExplosions(): void
{
	$(".exp").remove();
}

// *** END DOORS, KEYS, GEMS, and HEARTS ***
// *** BEGIN USER INPUT ***

document.onkeydown = KeyPress;

function KeyPress(e) 
{
    e = e || window.event;

    if (e.keyCode == "38")
	{
		PlayerMove("U");
    }
    else if (e.keyCode == "40")
	{
        PlayerMove("D");
    }
    else if (e.keyCode == "37")
	{
       PlayerMove("L");
    }
    else if (e.keyCode == "39")
	{
       PlayerMove("R");
    }
}

// *** END USER INPUT ***
// *** BEGIN PLAYER MOVEMENT ***

function PlayerMove(dir: string)
{		
	if (IsGameOver)
	{
		GameOver();
		return;
	}

	RemoveExplosions();
	RemoveAttackSwords();

	let x: number = PlayerCoords.X;
	let y: number = PlayerCoords.Y;
	let dungeon: object = Dungeon[Level];
	
	if (dir == "U" && MoveAllowed(Level, x, y, dir, KeyCount > 0, false))
	{
		MoveSequence(x, y, x, y + 1, dir);
	}
	else if (dir == "D" && MoveAllowed(Level, x, y, dir, KeyCount > 0, false))
	{
		MoveSequence(x, y, x, y - 1, dir);
	}
	else if (dir == "L" && MoveAllowed(Level, x, y, dir, KeyCount > 0, false))
	{
		MoveSequence(x, y, x - 1, y, dir);
	}
	else if (dir == "R" && MoveAllowed(Level, x, y, dir, KeyCount > 0, false))
	{
		MoveSequence(x, y, x + 1, y, dir);
	}
	else
	{
		// No move.
	}

	UpdateHighScore();
}

function MoveAllowed(level: number, x: number, y: number, dir: string, hasKey: boolean, isGoblin: boolean)
{	
	let allowed: boolean = false;
	let dungeon: object = Dungeon[level];

	if (dir == "U" && dungeon[x][y].TraversableUp && y < GridMax)
	{
		if (isGoblin && !GoblinCanMove(level, x, y + 1))
		{
			allowed = false;  // Don't mess with this boolean logic or Goblins will start moving through doors, etc. 
		}
		else if (dungeon[x][y].DoorDirection != "U" && dungeon[x][y + 1].DoorDirection != "D")
		{
			allowed = true;
		}
		else if (hasKey)
		{
			allowed = true;
		}
	}
	else if (dir == "D" && dungeon[x][y].TraversableDown && y > 0)
	{
		if (isGoblin && !GoblinCanMove(level, x, y - 1))
		{
			allowed = false;
		}
		else if (dungeon[x][y].DoorDirection != "D" && dungeon[x][y - 1].DoorDirection != "U")
		{
			allowed = true;
		}
		else if (hasKey)
		{
			allowed = true;
		}
	}
	else if (dir == "L" && dungeon[x][y].TraversableLeft && x > 0)
	{
		if (isGoblin && !GoblinCanMove(level, x - 1, y))
		{
			allowed = false;
		}
		else if (dungeon[x][y].DoorDirection != "L" && dungeon[x - 1][y].DoorDirection != "R")
		{
			allowed = true;
		}
		else if (hasKey)
		{
			allowed = true;
		}
	}
	else if (dir == "R" && dungeon[x][y].TraversableRight && x < GridMax)
	{
		if (isGoblin && !GoblinCanMove(level, x + 1, y))
		{
			allowed = false;
		}
		else if (dungeon[x][y].DoorDirection != "R" && dungeon[x + 1][y].DoorDirection != "L")
		{
			allowed = true;
		}
		else if (hasKey)
		{
			allowed = true;
		}
	}
	
	return allowed;
}

function MoveSequence(fromX: number, fromY: number, toX: number, toY: number, dir): void
{
	DoorCheck(fromX, fromY, dir);

	PlayerCoords.X = toX;
	PlayerCoords.Y = toY;
	RepositionPlayer(PlayerCoords);
	
	if (!GoblinCheck(toX, toY))
	{
		ItemCheck(toX, toY);
		MoveGoblins();

		if (!GoblinCheck(toX, toY))
		{
			StairsCheck(toX, toY);
		}
		else
		{
			GameOver();
		}
	}
	else
	{
		GameOver();
	}
}

function DoorCheck(x: number, y: number, dir: string)
{
	if (dir == "U" && Dungeon[Level][x][y].DoorDirection == "U" && KeyCount > 0)
	{
		KeyCount--;
		$("#keyMinus").show();
		$("#keyMinus").stop(true).show().css("opacity", 1).fadeOut(FadeTime);
		RemoveDoor(x, y);
		UpdateCounts();
	} 
	if (dir == "U" && Dungeon[Level][x][y + 1].DoorDirection == "D" && KeyCount > 0)
	{
		KeyCount--;
		$("#keyMinus").show();
		$("#keyMinus").stop(true).show().css("opacity", 1).fadeOut(FadeTime);
		RemoveDoor(x, y + 1);
		UpdateCounts();
	} 
	if (dir == "D" && Dungeon[Level][x][y].DoorDirection == "D" && KeyCount > 0)
	{
		KeyCount--;
		$("#keyMinus").show();
		$("#keyMinus").stop(true).show().css("opacity", 1).fadeOut(FadeTime);
		RemoveDoor(x, y);
		UpdateCounts();
	} 
	if (dir == "D" && Dungeon[Level][x][y - 1].DoorDirection == "U" && KeyCount > 0)
	{
		KeyCount--;
		$("#keyMinus").show();
		$("#keyMinus").stop(true).show().css("opacity", 1).fadeOut(FadeTime);
		RemoveDoor(x, y - 1);
		UpdateCounts();
	} 
	if (dir == "L" && Dungeon[Level][x][y].DoorDirection == "L" && KeyCount > 0)
	{
		KeyCount--;
		$("#keyMinus").show();
		$("#keyMinus").stop(true).show().css("opacity", 1).fadeOut(FadeTime);
		RemoveDoor(x, y);
		UpdateCounts();
	} 
	if (dir == "L" && Dungeon[Level][x - 1][y].DoorDirection == "R" && KeyCount > 0)
	{
		KeyCount--;
		$("#keyMinus").show();
		$("#keyMinus").stop(true).show().css("opacity", 1).fadeOut(FadeTime);
		RemoveDoor(x - 1, y);
		UpdateCounts();
	} 
	if (dir == "R" && Dungeon[Level][x][y].DoorDirection == "R" && KeyCount > 0)
	{
		KeyCount--;
		$("#keyMinus").show();
		$("#keyMinus").stop(true).show().css("opacity", 1).fadeOut(FadeTime);
		RemoveDoor(x, y);
		UpdateCounts();
	} 
	if (dir == "R" && Dungeon[Level][x + 1][y].DoorDirection == "L" && KeyCount > 0)
	{
		KeyCount--;
		$("#keyMinus").show();
		$("#keyMinus").stop(true).show().css("opacity", 1).fadeOut(FadeTime);
		RemoveDoor(x + 1, y);
		UpdateCounts();
	} 
}

function ItemCheck(x: number, y: number): void
{
	if (Dungeon[Level][x][y].HasKey)
	{
		KeyCount++;
		$("#keyPlus").stop(true).show().css("opacity", 1).fadeOut(FadeTime);
		RemoveKey(x, y);
		UpdateCounts();                
	}
	else if (Dungeon[Level][x][y].HasGem)
	{
		Score++;
		$("#scorePlusOne").show();
		$("#scorePlusOne").stop(true).show().css("opacity", 1).fadeOut(FadeTime);
		RemoveGem(x, y);
		UpdateCounts();                  
	}
	else if (Dungeon[Level][x][y].HasHeart)
	{
		HeartCount++;
		$("#heartPlus").show();
		$("#heartPlus").stop(true).show().css("opacity", 1).fadeOut(FadeTime);
		RemoveHeart(x, y);
		UpdateCounts();                  
	}
	else if (Dungeon[Level][x][y].HasSword)
	{
		SwordCount++;
		$("#swordPlus").show();
		$("#swordPlus").stop(true).show().css("opacity", 1).fadeOut(FadeTime);
		RemoveSword(x, y);
		UpdateCounts();                  
	}
}

function GoblinCheck(x: number, y: number): boolean
{
	let gameOver: boolean = false;
	
	if (Dungeon[Level][x][y].HasGoblin)
	{
		if (SwordCount > 0)
		{
			SwordCount--;
			$("#swordMinus").show();
			$("#swordMinus").stop(true).show().css("opacity", 1).fadeOut(FadeTime);
			Score += 10;
			$("#scorePlusTen").show();
			$("#scorePlusTen").stop(true).show().css("opacity", 1).fadeOut(FadeTime);
			RemoveGoblin(x, y);
			PlaceAttackSword(x, y);
			UpdateCounts();
		}
		else
		{
			HeartCount--;
			$("#heartMinus").show();
			$("#heartMinus").stop(true).show().css("opacity", 1).fadeOut(FadeTime);
			RemoveGoblin(x, y);
			UpdateCounts();

			if (HeartCount <= 0)
			{
				PlaceExplosion(x, y, false);
				gameOver = true;
			}
			else
			{
				PlaceExplosion(x, y, true);
			}
		}
	}

	return gameOver;
}

function StairsCheck(x: number, y: number)
{
	if (Dungeon[Level][x][y].IsStairsDown)
	{
		LevelDown();
	}
	else if  (Dungeon[Level][x][y].IsStairsUp)
	{
		LevelUp();
	}
}

// *** END PLAYER MOVEMENT ***
// *** BEGIN GOBLIN MOVEMENT ***

function MoveGoblins(): void
{
	ResetGoblinMovement();
	
	for (var level = 1; level <= Dungeon.length - 1; level++)
	{
		for (var x = 0; x < GridMax; x++)
		{
			for (var y = 0; y < GridMax; y++)
			{
				if (Dungeon[level][x][y].HasGoblin && !Dungeon[level][x][y].GoblinMoved)
				{
					MoveGoblin(level, x, y);
				}
			}
		}
	}
}

function MoveGoblin(level: number, x: number, y: number): void
{
	let coords: Coords = GoblinMoveLocation(level, x, y);
	
	let dir: string;

	if (coords.X > x) dir = "R";
	else if (coords.X < x) dir = "L";
	else if (coords.Y > y) dir = "U";
	else dir = "D";

	if (!MoveAllowed(level, x, y, dir, false, true))
	{
		return;
	}

	if (level == Level)
	{
		RemoveGoblin(x, y);
		PlaceGoblin(coords.X, coords.Y);
	}
	else
	{
		Dungeon[level][x][y].HasGoblin = false;
	}

	Dungeon[level][coords.X][coords.Y].HasGoblin = true;
	Dungeon[level][coords.X][coords.Y].GoblinMoved = true;  // Otherwise, if it moves up or right, it could get caught in a sequence of moves.
}

function GoblinMoveLocation(level: number, x: number, y: number): Coords
{
	ResetPath(level);
	
	let destination: Coords;
	
	if (level == Level)
	{
		destination = PlayerCoords;
	}
	else if (level > Level)
	{
		destination = UpStairsLocation(level);
	}
	else
	{
		destination = DownStairsLocation(level);
	}
	
	let queue: Array<Coords> = new Array<Coords>();
	queue.push(new Coords(x, y));
	let arrived: boolean = false;
	Dungeon[level][x][y].Visited = true;

    do 
    {
		let coords: Coords = queue.shift();  // shift() is a queue operation. pop() doesn't work here!
		
        if (coords.X == destination.X && coords.Y == destination.Y)
        {
            arrived = true; 
        }
        else 
        {
			// Visit all the adjacent cells to the current one, if possible, and in a random sequence. Randomness is 
			// required or else for paths of equal lengths, certain favoured directions and patterns will become evident 
			// during game play.
			VisitAdjacents(level, coords, queue);
        }
        
    } while (!arrived);
    
    // Search complete. Now decode and return results.
	let path: Array<Coords> = PathSearchDecode(level, destination);
	
	// DebugGoblinMovementSearch(level, x, y, destination.X, destination.Y);
	// DebugGoblinMovementPath(level, x, y, path);

	return path.pop();
}

function DebugGoblinMovementSearch(level: number, startX: number, startY: number, destX: number, destY: number): void
{
	let line: string = "";
	
	for (var y = GridMax - 1; y >= 0; y--)
	{
		for (var x = 0; x < GridMax; x++)
		{
			let xs: string = "..";
			let ys: string = "..";
			
			if (x == startX && y == startY)
			{
				xs = "SS";
				ys = "SS";
			}
			else if (x == destX && y == destY)
			{
				xs = "DD";
				ys = "DD";
			}
			else if (Dungeon[level][x][y].SourceCoords)
			{
				if (Dungeon[level][x][y].SourceCoords.X < 10)
				{
					xs = "0" + Dungeon[level][x][y].SourceCoords.X.toString();
				}
				else
				{
					xs = Dungeon[level][x][y].SourceCoords.X.toString();
				}

				if (Dungeon[level][x][y].SourceCoords.Y < 10)
				{
					ys = "0" + Dungeon[level][x][y].SourceCoords.Y.toString();
				}
				else
				{
					ys = Dungeon[level][x][y].SourceCoords.Y.toString();
				}
			}

			line = line + xs + "," + ys + " ";
		}
		console.log(line + "\n");
		line = "";
	}
}

function DebugGoblinMovementPath(level: number, startX: number, startY: number, path: Array<Coords>)
{
	let xs: string;
	let ys: string;
	let line: string = "Goblin L: " + level + " [" + startX + "," + startY + "] Path: ";

	for (let i = 0; i < path.length; i++)
	{
		if (path[i].X < 10)
		{
			xs = "0" + path[i].X.toString();
		}
		else
		{
			xs = path[i].X.toString();
		}

		if (path[i].Y < 10)
		{
			ys = "0" + path[i].Y.toString();
		}
		else
		{
			ys = path[i].Y.toString();
		}
		
		line = line + "[" + xs + "," + ys + "] ";
	}

	console.log(line);
}

function DebugGoblinLocations(level: number)
{
	for (var x = 0; x < GridMax; x++)
	{
		for (var y = 0; y < GridMax; y++)
		{
			if (Dungeon[level][x][y].HasGoblin)
			{
				console.log("GOBLIN: " + GridReference(x, y));
			}
		}
	}
}

function VisitAdjacents(level: number, coords: Coords, queue: Array<Coords>): void
{
	let visitationProfile = Math.floor(Math.random() * 24);  // Random number between 0 and 23
    
    switch (visitationProfile) 
    {
        case 0:
		// UDLR
		PathVisitUp(level, coords, queue);
		PathVisitDown(level, coords, queue);
		PathVisitLeft(level, coords, queue);
		PathVisitRight(level, coords, queue);
		break;
		
	case 1:
		// UDRL
		PathVisitUp(level, coords, queue);
		PathVisitDown(level, coords, queue);
		PathVisitRight(level, coords, queue);
		PathVisitLeft(level, coords, queue);
		break;
		
	case 2:
		// ULDR
		PathVisitUp(level, coords, queue);
		PathVisitLeft(level, coords, queue);
		PathVisitDown(level, coords, queue);
		PathVisitRight(level, coords, queue);
		break;
		
	case 3:
		// ULRD
		PathVisitUp(level, coords, queue);
		PathVisitLeft(level, coords, queue);
		PathVisitRight(level, coords, queue);
		PathVisitDown(level, coords, queue);
		break;
		
	case 4:
		// URDL
		PathVisitUp(level, coords, queue);
		PathVisitRight(level, coords, queue);
		PathVisitDown(level, coords, queue);
		PathVisitLeft(level, coords, queue);
		break;
		
	case 5:
		// URLD
		PathVisitUp(level, coords, queue);
		PathVisitRight(level, coords, queue);
		PathVisitLeft(level, coords, queue);
		PathVisitDown(level, coords, queue);
		break;
		
	case 6:
		// DURL
		PathVisitDown(level, coords, queue);
		PathVisitUp(level, coords, queue);
		PathVisitRight(level, coords, queue);
		PathVisitLeft(level, coords, queue);
		break;
		
	case 7:
		// DULR
		PathVisitDown(level, coords, queue);
		PathVisitUp(level, coords, queue);
		PathVisitLeft(level, coords, queue);
		PathVisitRight(level, coords, queue);
		break;
		
	case 8:
		// DLRU
		PathVisitDown(level, coords, queue);
		PathVisitLeft(level, coords, queue);
		PathVisitRight(level, coords, queue);
		PathVisitUp(level, coords, queue);
		break;
		
	case 9:
		// DLUR
		PathVisitDown(level, coords, queue);
		PathVisitLeft(level, coords, queue);
		PathVisitUp(level, coords, queue);
		PathVisitRight(level, coords, queue);
		break;
		
	case 10:
		// DRLU
		PathVisitDown(level, coords, queue);
		PathVisitRight(level, coords, queue);
		PathVisitLeft(level, coords, queue);
		PathVisitUp(level, coords, queue);
		break;
		
	case 11:
		// DRUL
		PathVisitDown(level, coords, queue);
		PathVisitRight(level, coords, queue);
		PathVisitUp(level, coords, queue);
		PathVisitLeft(level, coords, queue);
		break;
		
	case 12:
		// LUDR
		PathVisitLeft(level, coords, queue);
		PathVisitUp(level, coords, queue);
		PathVisitDown(level, coords, queue);
		PathVisitRight(level, coords, queue);
		break;
		
	case 13:
		// LURD
		PathVisitLeft(level, coords, queue);
		PathVisitUp(level, coords, queue);
		PathVisitRight(level, coords, queue);
		PathVisitDown(level, coords, queue);
		break;
		
	case 14:
		// LDUR
		PathVisitLeft(level, coords, queue);
		PathVisitDown(level, coords, queue);
		PathVisitUp(level, coords, queue);
		PathVisitRight(level, coords, queue);
		break;
		
	case 15:
		// LDRU
		PathVisitLeft(level, coords, queue);
		PathVisitDown(level, coords, queue);
		PathVisitRight(level, coords, queue);
		PathVisitUp(level, coords, queue);
		break;
		
	case 16:
		// LRUD
		PathVisitLeft(level, coords, queue);
		PathVisitRight(level, coords, queue);
		PathVisitUp(level, coords, queue);
		PathVisitDown(level, coords, queue);
		break;
		
	case 17:
		// LRDU
		PathVisitLeft(level, coords, queue);
		PathVisitRight(level, coords, queue);
		PathVisitDown(level, coords, queue);
		PathVisitUp(level, coords, queue);
		break;
		
	case 18:
		// RULD
		PathVisitRight(level, coords, queue);
		PathVisitUp(level, coords, queue);
		PathVisitLeft(level, coords, queue);
		PathVisitDown(level, coords, queue);
		break;
		
	case 19:
		// RUDL
		PathVisitRight(level, coords, queue);
		PathVisitUp(level, coords, queue);
		PathVisitDown(level, coords, queue);
		PathVisitLeft(level, coords, queue);
		break;
		
	case 20:
		// RDLU
		PathVisitRight(level, coords, queue);
		PathVisitDown(level, coords, queue);
		PathVisitLeft(level, coords, queue);
		PathVisitUp(level, coords, queue);
		break;
		
	case 21:
		// RDUL
		PathVisitRight(level, coords, queue);
		PathVisitDown(level, coords, queue);
		PathVisitUp(level, coords, queue);
		PathVisitLeft(level, coords, queue);
		break;
		
	case 22:
		// RLDU
		PathVisitRight(level, coords, queue);
		PathVisitLeft(level, coords, queue);
		PathVisitDown(level, coords, queue);
		PathVisitUp(level, coords, queue);
		break;
		
	case 23:
		// RLUD
		PathVisitRight(level, coords, queue);
		PathVisitLeft(level, coords, queue);
		PathVisitUp(level, coords, queue);
		PathVisitDown(level, coords, queue);
		break;
    }
}

function PathVisitUp(level: number, coords: Coords, queue: Array<Coords>): void
{
    if (Dungeon[level][coords.X][coords.Y].TraversableUp && coords.Y + 1 < GridMax)
    {
        if (!Dungeon[level][coords.X][coords.Y + 1].Visited)
        {
			Dungeon[level][coords.X][coords.Y + 1].Visited = true;
			Dungeon[level][coords.X][coords.Y + 1].SourceCoords = coords;
			queue.push(new Coords(coords.X, coords.Y + 1));
        }
    }
}

function PathVisitDown(level: number, coords: Coords, queue: Array<Coords>): void
{
    if (Dungeon[level][coords.X][coords.Y].TraversableDown && coords.Y - 1 >= 0)
    {
        if (!Dungeon[level][coords.X][coords.Y - 1].Visited)
        {
			Dungeon[level][coords.X][coords.Y - 1].Visited = true;
			Dungeon[level][coords.X][coords.Y - 1].SourceCoords = coords;
			queue.push(new Coords(coords.X, coords.Y - 1));
        }
    }
}

function PathVisitLeft(level: number, coords: Coords, queue: Array<Coords>): void
{
    if (Dungeon[level][coords.X][coords.Y].TraversableLeft && coords.X - 1 >= 0)
    {
        if (!Dungeon[level][coords.X - 1][coords.Y].Visited)
        {
			Dungeon[level][coords.X - 1][coords.Y].Visited = true;
			Dungeon[level][coords.X - 1][coords.Y].SourceCoords = coords;
			queue.push(new Coords(coords.X - 1, coords.Y));
        }
    }
}

function PathVisitRight(level: number, coords: Coords, queue: Array<Coords>): void
{
    if (Dungeon[level][coords.X][coords.Y].TraversableRight && coords.X + 1 < GridMax)
    {
        if (!Dungeon[level][coords.X + 1][coords.Y].Visited)
        {
			Dungeon[level][coords.X + 1][coords.Y].Visited = true;
			Dungeon[level][coords.X + 1][coords.Y].SourceCoords = coords;
			queue.push(new Coords(coords.X + 1, coords.Y));
        }
    }
}

function PathSearchDecode(level: number, destination: Coords): Array<Coords>
{
    let pathComplete: boolean = false;
	let path: Array<Coords> = new Array<Coords>();
	
	path.push(destination);
	
	if (Dungeon[level][destination.X][destination.Y].SourceCoords == null)
	{
		return path;  // It's just a shorty.
	}
	
	let coords: Coords = Dungeon[level][destination.X][destination.Y].SourceCoords;

    do 
    {
        if (Dungeon[level][coords.X][coords.Y].SourceCoords)
        {
			path.push(coords);
        }
        else 
        {
			//path.pop();  // Don't actually need the source location in the path.
            pathComplete = true;
		}
		
		coords = Dungeon[level][coords.X][coords.Y].SourceCoords;
        
    } while (!pathComplete);
    
    return path;
}

function UpStairsLocation(level: number): Coords
{
	for (var x = 0; x < GridMax; x++)
	{
		for (var y = 0; y < GridMax; y++)
		{
			if (Dungeon[level][x][y].IsStairsUp)
			{
				return new Coords(x, y);
			}
		}
	}
}

function DownStairsLocation(level: number): Coords
{
	for (var x = 0; x < GridMax; x++)
	{
		for (var y = 0; y < GridMax; y++)
		{
			if (Dungeon[level][x][y].IsStairsDown)
			{
				return new Coords(x, y);
			}
		}
	}
}

// Goblins cannot move into locations where another Goblin already is, where there are stairs, or where combat just occurred.
function GoblinCanMove(level: number, x: number, y: number)
{
	return 	!Dungeon[level][x][y].IsStairsDown && 
			!Dungeon[level][x][y].IsStairsUp && 
			!Dungeon[level][x][y].HasGoblin && 
			!(level == Level && $("#exp" + GridReference(x, y)).length > 0) &&
			!(level == Level && $("#attack" + GridReference(x, y)).length > 0)
}

function ResetPath(level)
{
	for (var x = 0; x < GridMax; x++)
	{
		for (var y = 0; y < GridMax; y++)
		{
			Dungeon[level][x][y].Visited = false;
			Dungeon[level][x][y].SourceCoords = null;
		}
	}
}

function ResetGoblinMovement()
{
	for (var level = 1; level <= Level; level++)
	{
		for (var x = 0; x < GridMax; x++)
		{
			for (var y = 0; y < GridMax; y++)
			{
				if (Dungeon[level][x][y].HasGoblin)
				{
					Dungeon[level][x][y].GoblinMoved = false;  // Reset all goblin movements.
				}
			}
		}
	}
}

// *** END GOBLIN MOVEMENT ***
// *** BEGIN MISC ***

function UpdateHighScore()
{
	let highScore: string = localStorage.getItem("highScore");
	let highScoreNum: number = Number(highScore);

	if (isNaN(highScoreNum) || Score > highScoreNum)
	{
		$("#highScore").html(Score.toString());
		localStorage.setItem("highScore", Score.toString());
	}
	else
	{
		$("#highScore").html(highScore);
	}
}

function GameOver()
{
	let highScore: string = localStorage.getItem("highScore");
	let highScoreNum: number = Number(highScore);

	$("#gameOver").show();

	if (!isNaN(highScoreNum) && Score >= highScoreNum)
	{
		$("#newHighScore").show();
	}

	IsGameOver = true;
}

// *** END MISC ***
// *** BEGIN CLASS DEFINITIONS ***

class Coords
{
    X: number;
	Y: number;
	Dir: string;

	constructor(x: number, y: number) 
	{
        this.X = x;
		this.Y = y;
    }
}

// *** END CLASS DEFINITIONS ***