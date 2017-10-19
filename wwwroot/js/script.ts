"use strict";

let Level: number;
let PlayerCoords: Coords;
let Dungeon: Array<object> = new Array<object>();
let StartCoords: Array<Coords> = new Array<Coords>();

window.onload = function(){Start();}

function Start(): void
{
	sessionStorage.clear();
	Level = 1;
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
				BuildDungeon(Level, dungeon);
				Dungeon[Level] = dungeon;
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

			if (dungeon[x][y].HasGoblin)
			{
				PlaceGoblin(x, y);
			}
			
			PlaceItem(x, y, dungeon[x][y].DoorDirection, dungeon[x][y].HasKey, dungeon[x][y].HasGem, dungeon[x][y].HasHeart);
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
			obj.remove();
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
			obj.remove();
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

function MovePlayer(coords: Coords): void
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
	$("#grid").append('<div class="sprite goblin" style="top: ' + yPixels + 'px; left: ' + xPixels + 'px;"></div>');
}

// *** END GOBLINS ***
// *** BEGIN DOORS, KEYS, GEMS, and HEARTS ***

function PlaceItem(x: number, y: number, DoorDirection: string, Key: string, Gem: string, Heart: string): void
{
	if (DoorDirection != "")
	{
		PlaceDoor(x, y, DoorDirection);
	}		
	else if (Key != "")
	{
		PlaceKey(x, y);
	}
	else if (Gem != "")
	{
		PlaceGem(x, y);
	}
	else if (Heart != "")
	{
		PlaceHeart(x, y);
	}
}

function PlaceDoor(x: number, y: number, direction: string): void
{
	let xPixels: number = x * 45 + DoorXFudge(direction);
	let yPixels: number = 630 - (y * 45) + DoorYFudge(direction);
	let doorClass: string = direction == "U" || direction == "D" ? "doorh": "doorv";
	$("#grid").append('<div class="sprite ' + doorClass + '" style="top: ' + yPixels + 'px; left: ' + xPixels + 'px;"></div>');
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
	$("#grid").append('<div class="sprite key" style="top: ' + yPixels + 'px; left: ' + xPixels + 'px;"></div>');
}

function PlaceGem(x: number, y: number): void
{
	let xPixels: number = x * 45;
	let yPixels: number = 630 - (y * 45);
	$("#grid").append('<div class="sprite gem" style="top: ' + yPixels + 'px; left: ' + xPixels + 'px;"></div>');
}

function PlaceHeart(x: number, y: number): void
{
	let xPixels: number = x * 45;
	let yPixels: number = 630 - (y * 45);
	$("#grid").append('<div class="sprite heart" style="top: ' + yPixels + 'px; left: ' + xPixels + 'px;"></div>');
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
	
	if (dir == "U" && MoveAllowed(x, y, dir))
	{
		PlayerCoords.Y += 1;
		MovePlayer(PlayerCoords);
	}
	else if (dir == "D" && MoveAllowed(x, y, dir))
	{
		PlayerCoords.Y -= 1;
		MovePlayer(PlayerCoords);
	}
	else if (dir == "L" && MoveAllowed(x, y, dir))
	{
		PlayerCoords.X -= 1;
		MovePlayer(PlayerCoords);
	}
	else if (dir == "R" && MoveAllowed(x, y, dir))
	{
		PlayerCoords.X += 1;
		MovePlayer(PlayerCoords);
	}
	else
	{
		// No move.
	}
}

function MoveAllowed(x: number, y: number, dir: string)
{	
	let allowed: boolean = false;
	let dungeon: object = Dungeon[Level];

	if (dir == "U" && dungeon[x][y].TraversableUp && y < 14 && dungeon[x][y + 1].DoorDirection != "D")
	{
		allowed = true;
	}
	else if (dir == "D" && dungeon[x][y].TraversableDown && y > 0 && dungeon[x][y - 1].DoorDirection != "U")
	{
		allowed = true;
	}
	else if (dir == "L" && dungeon[x][y].TraversableLeft && x > 0 && dungeon[x - 1][y].DoorDirection != "R")
	{
		allowed = true;
	}
	else if (dir == "R" && dungeon[x][y].TraversableRight && x < 14 && dungeon[x + 1][y].DoorDirection != "L")
	{
		allowed = true;
	}
	
	return allowed;
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