"use strict";

let Level: number;
let Score: number = 0;
let KeyCount: number = 0;
let HeartCount: number = 3;
let SwordCount: number = 0;
let PlayerCoords: Coords;
let Dungeon: Array<object> = new Array<object>();
let StartCoords: Array<Coords> = new Array<Coords>();

window.onload = function(){Start();}

function Start(): void
{
	Level = 1;
	UpdateCounts();
	$("#level").text("1");
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
		let sX: number = Level == 1 ? 7: Number(startCoords.X);
		let sY: number = Level == 1 ? 0: Number(startCoords.Y);
		let sD: string = Level == 1 ? "U": startCoords.Dir;
		
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
	
	for (var x = 0; x <= 14; x++) 
	{
		for (let y = 0; y <= 14; y++)
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
	let x: number = 7;
	let y: number = 0;

	if (Level != 1)
	{
		let startCoords: Coords = StartCoords[Level];
		x = Number(startCoords.X);
		y = Number(startCoords.Y);
	}
	
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

// *** END DOORS, KEYS, GEMS, and HEARTS ***
// *** BEGIN USER INPUT ***

document.onkeydown = KeyPress;

function KeyPress(e) 
{
    e = e || window.event;

    if (e.keyCode == '38')
	{
		PlayerMove("U");
    }
    else if (e.keyCode == '40')
	{
        PlayerMove("D");
    }
    else if (e.keyCode == '37')
	{
       PlayerMove("L");
    }
    else if (e.keyCode == '39')
	{
       PlayerMove("R");
    }
}

// *** END USER INPUT ***
// *** BEGIN MOVEMENT ***

function PlayerMove(dir: string)
{	
	let x: number = PlayerCoords.X;
	let y: number = PlayerCoords.Y;
	let dungeon: object = Dungeon[Level];
	
	if (dir == "U" && MoveAllowed(x, y, dir))
	{
		MoveGoblins();
		DoorCheck(x, y, dir);
		GoblinCheck(x, y + 1);
		ItemCheck(x, y + 1);
		PlayerCoords.Y += 1;
		RepositionPlayer(PlayerCoords);
	}
	else if (dir == "D" && MoveAllowed(x, y, dir))
	{
		MoveGoblins();
		DoorCheck(x, y, dir);
		GoblinCheck(x, y - 1);
		ItemCheck(x, y - 1);
		PlayerCoords.Y -= 1;
		RepositionPlayer(PlayerCoords);
	}
	else if (dir == "L" && MoveAllowed(x, y, dir))
	{
		MoveGoblins();
		DoorCheck(x, y, dir);
		GoblinCheck(x - 1, y);
		ItemCheck(x - 1, y);
		PlayerCoords.X -= 1;
		RepositionPlayer(PlayerCoords);
	}
	else if (dir == "R" && MoveAllowed(x, y, dir))
	{
		MoveGoblins();
		DoorCheck(x, y, dir);
		GoblinCheck(x + 1, y);
		ItemCheck(x + 1, y);
		PlayerCoords.X += 1;
		RepositionPlayer(PlayerCoords);
	}
	else
	{
		// No move.
	}
}

function MoveGoblins(): void
{
	for (var i = 1; i <= Level; i++)
	{
		for (var j = 0; j < 14; j++)
		{
			for (var k = 0; k < 14; k++)
			{
				if (Dungeon[i][j][k].HasGoblin && !Dungeon[i][j][k].GoblinMoved)
				{
					if (i == Level)
					{
						RemoveGoblin(j, k);
						PlaceGoblin(j + 1, k);
						Dungeon[i][j + 1][k].HasGoblin = true;
						Dungeon[i][j + 1][k].GoblinMoved = true;  // Otherwise, if it moves up or right, it could get caught in a seuence of moves.
					}
				}
			}
		}
	}

	for (var i = 1; i <= Level; i++)
	{
		for (var j = 0; j < 14; j++)
		{
			for (var k = 0; k < 14; k++)
			{
				if (Dungeon[i][j][k].HasGoblin)
				{
					Dungeon[i][j][k].GoblinMoved = false;  // Reset all.
				}
			}
		}
	}
}

function MoveAllowed(x: number, y: number, dir: string)
{	
	let allowed: boolean = false;
	let dungeon: object = Dungeon[Level];

	if (dir == "U" && dungeon[x][y].TraversableUp && y < 14)
	{
		if (dungeon[x][y].DoorDirection != "U" && dungeon[x][y + 1].DoorDirection != "D")
		{
			allowed = true;
		}
		else if (KeyCount > 0)
		{
			allowed = true;
		}
	}
	else if (dir == "D" && dungeon[x][y].TraversableDown && y > 0)
	{
		if (dungeon[x][y].DoorDirection != "D" && dungeon[x][y - 1].DoorDirection != "U")
		{
			allowed = true;
		}
		else if (KeyCount > 0)
		{
			allowed = true;
		}
	}
	else if (dir == "L" && dungeon[x][y].TraversableLeft && x > 0)
	{
		if (dungeon[x][y].DoorDirection != "L" && dungeon[x - 1][y].DoorDirection != "R")
		{
			allowed = true;
		}
		else if (KeyCount > 0)
		{
			allowed = true;
		}
	}
	else if (dir == "R" && dungeon[x][y].TraversableRight && x < 14)
	{
		if (dungeon[x][y].DoorDirection != "R" && dungeon[x + 1][y].DoorDirection != "L")
		{
			allowed = true;
		}
		else if (KeyCount > 0)
		{
			allowed = true;
		}
	}
	
	return allowed;
}

function DoorCheck(x: number, y: number, dir: string)
{
	if (dir == "U" && Dungeon[Level][x][y].DoorDirection == "U" && KeyCount > 0)
	{
		KeyCount--;
		RemoveDoor(x, y);
		UpdateCounts();
	} 
	if (dir == "U" && Dungeon[Level][x][y + 1].DoorDirection == "D" && KeyCount > 0)
	{
		KeyCount--;
		RemoveDoor(x, y + 1);
		UpdateCounts();
	} 
	if (dir == "D" && Dungeon[Level][x][y].DoorDirection == "D" && KeyCount > 0)
	{
		KeyCount--;
		RemoveDoor(x, y);
		UpdateCounts();
	} 
	if (dir == "D" && Dungeon[Level][x][y - 1].DoorDirection == "U" && KeyCount > 0)
	{
		KeyCount--;
		RemoveDoor(x, y - 1);
		UpdateCounts();
	} 
	if (dir == "L" && Dungeon[Level][x][y].DoorDirection == "L" && KeyCount > 0)
	{
		KeyCount--;
		RemoveDoor(x, y);
		UpdateCounts();
	} 
	if (dir == "L" && Dungeon[Level][x - 1][y].DoorDirection == "R" && KeyCount > 0)
	{
		KeyCount--;
		RemoveDoor(x - 1, y);
		UpdateCounts();
	} 
	if (dir == "R" && Dungeon[Level][x][y].DoorDirection == "R" && KeyCount > 0)
	{
		KeyCount--;
		RemoveDoor(x, y);
		UpdateCounts();
	} 
	if (dir == "R" && Dungeon[Level][x + 1][y].DoorDirection == "L" && KeyCount > 0)
	{
		KeyCount--;
		RemoveDoor(x + 1, y);
		UpdateCounts();
	} 
}

function ItemCheck(x: number, y: number): void
{
	if (Dungeon[Level][x][y].HasKey)
	{
		KeyCount++;
		RemoveKey(x, y);
		UpdateCounts();                
	}
	else if (Dungeon[Level][x][y].HasGem)
	{
		Score++;
		RemoveGem(x, y);
		UpdateCounts();                  
	}
	else if (Dungeon[Level][x][y].HasHeart)
	{
		HeartCount++;
		RemoveHeart(x, y);
		UpdateCounts();                  
	}
	else if (Dungeon[Level][x][y].HasSword)
	{
		SwordCount++;
		RemoveSword(x, y);
		UpdateCounts();                  
	}
}

function GoblinCheck(x: number, y: number): void
{
	if (Dungeon[Level][x][y].HasGoblin)
	{
		if (SwordCount > 0)
		{
			SwordCount--;
			Score += 10;
			RemoveGoblin(x, y);
			UpdateCounts();
		}
		else
		{
			HeartCount--;
			RemoveGoblin(x, y);
			UpdateCounts();

			if (HeartCount <= 0)
			{
				alert("Game Over!");
			}
		}
	}
}

// *** END MOVEMENT ***
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