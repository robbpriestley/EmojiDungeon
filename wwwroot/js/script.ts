"use strict";

window.onload = function(){Start();}

function Start() : void
{
	sessionStorage.clear();
	SetLevel(1);
	$("#level").text("1");
	RenderLevel(1);
}

function LevelUp() : void
{
	let level : number = Number(sessionStorage.getItem("level"));
	level--  // Levels closer to the "surface" have smaller numbers.
	SetLevel(level);
	RenderLevel(level);
	$("#level").text(level);  // Add 1 for display as _level is zero-indexed.
	sessionStorage.setItem("level", level.toString());
}

function LevelDown() : void
{
	let level : number = Number(sessionStorage.getItem("level"));
	level++  // Levels farther from the "surface" have larger numbers.
	SetLevel(level);
	RenderLevel(level);
	$("#level").text(level);  // Add 1 for display as _level is zero-indexed.
	sessionStorage.setItem("level", level.toString());
}

function RenderLevel(level : number)
{
	let dungeon : object = GetDungeon(level);
	
	if (dungeon != null)
	{
		BuildDungeon(level, dungeon);
	}
	else
	{
		let spinner : Spinner = SpinnerSetup();
		spinner.spin($('#grid')[0]);

		let startCoords : string = GetStartCoords(level);
		let sX : number = level == 1 ? 7 : Number(startCoords.substring(1, 3));
		let sY : number = level == 1 ? 0 : Number(startCoords.substring(3, 5));
		let sD : string = level == 1 ? "U" : startCoords.substring(5, 6);
		
		$.ajax
		({
			type: 'GET',
			dataType: 'json',
			data: { level: level, startX: sX, startY: sY, direction: sD },
			contentType: 'application/json',
			url: '/Index/DungeonView',
			success: function (result) 
			{
				dungeon = JSON.parse(result);  // As this is a reset, assign the JSON to element 0 of the dungeon array.
				BuildDungeon(level, dungeon);
				SetDungeon(level, dungeon);
				spinner.stop();
			}
		});
	}
}

function SpinnerSetup() : Spinner
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
function BuildDungeon(level : number, dungeon : object) : void
{
	RemoveSprites();
	
	for (var x = 0; x <= 14; x++) 
	{
		for (let y = 0; y <= 14; y++)
		{
			let gridReference : string = GridReference(x, y);
			let gridId : string = "#" + gridReference;       // Prepend the "#" because jQuery needs it for the id attribute.
			$(gridId).removeClass();                         // Remove all classes on the div.
			$(gridId).addClass("tile");                      // Add "native" class.
			$(gridId).addClass(gridReference);               // Add "native" class.
			
			let tileName : string = dungeon[x][y].N
			$(gridId).addClass(tileName);                    // Add tile name class.

			if (tileName == "e6" || tileName == "e7" || tileName == "e8" || tileName == "e9")
			{
				RecordStart(level, tileName, gridReference);
			}

			PlacePlayer();
			PlaceAnyGoblin(x, y, dungeon[x][y].X);
			PlaceAnyItem(x, y, dungeon[x][y].D, dungeon[x][y].K, dungeon[x][y].G, dungeon[x][y].H);
		}
	}
}

// Remove any sprites that are "local" to a different dungeon level.
function RemoveSprites() : void
{
	$('.player').each
	(
		function(i, obj) 
		{
			obj.remove();
		}
	);

	$('.goblingreen').each
	(
		function(i, obj) 
		{
			obj.remove();
		}
	);

	$('.goblinblack').each
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

	$('.heartr').each
	(
		function(i, obj) 
		{
			obj.remove();
		}
	);
}

// Synthesize a grid reference given X and Y coords.
function GridReference(x : number, y : number) : string
{
	let xs = x < 10 ? "0" + x.toString() : x.toString();
	let ys = y < 10 ? "0" + y.toString() : y.toString();
	return "g" + xs + ys;
}

function RecordStart(level : number, tileName : string, gridReference : string) : void
{
	let direction : string = null;

	switch (tileName)
	{
		case "e6":
			direction = "U";
			break;

		case "e7":
			direction = "D";
			break;

		case "e8":
			direction = "L";
			break;

		case "e9":
			direction = "R";
			break;
	
		default:
			break;
	}

	SetStartCoords(level + 1, gridReference + direction);
}

// *** BEGIN PLAYER ***

function PlacePlayer() : void
{
	let level : number = GetLevel();
	
	let x : number = 7;
	let y : number = 0;

	if (level != 1)
	{
		let startCoords : string = GetStartCoords(level);
		x = Number(startCoords.substring(1, 3));
		y = Number(startCoords.substring(3, 5));
	}
	
	let xPixels : number = x * 45;
	let yPixels : number = 630 - (y * 45);
	$("#grid").append('<div class="sprite player" style="top: ' + yPixels + 'px; left: ' + xPixels + 'px;"></div>');
}

// *** END PLAYER ***
// *** BEGIN GOBLINS ***

function PlaceAnyGoblin(x : number, y : number, X : string) : void
{
	if (X == "G")
	{
		PlaceGoblin(x, y);
	}
	else if (X == "B")
	{
		PlaceBlackGoblin(x, y);
	}
}

function PlaceGoblin(x : number, y : number) : void
{
	let xPixels : number = x * 45;
	let yPixels : number = 630 - (y * 45);
	$("#grid").append('<div class="sprite goblingreen" style="top: ' + yPixels + 'px; left: ' + xPixels + 'px;"></div>');
}

function PlaceBlackGoblin(x : number, y : number) : void
{
	let xPixels : number = x * 45;
	let yPixels : number = 630 - (y * 45);
	$("#grid").append('<div class="sprite goblinblack" style="top: ' + yPixels + 'px; left: ' + xPixels + 'px;"></div>');
}

// *** END GOBLINS ***
// *** BEGIN DOORS, KEYS, GEMS, and HEARTS ***

function PlaceAnyItem(x : number, y : number, D : string, K : string, G : string, H : string) : void
{
	if (D != "")
	{
		PlaceDoor(x, y, D);
	}		
	else if (K != "")
	{
		PlaceKey(x, y);
	}
	else if (G != "")
	{
		PlaceGem(x, y);
	}
	else if (H != "")
	{
		PlaceHeart(x, y);
	}
}

function PlaceDoor(x : number, y : number, direction : string) : void
{
	let xPixels : number = x * 45 + DoorXFudge(direction);
	let yPixels : number = 630 - (y * 45) + DoorYFudge(direction);
	let doorClass : string = direction == "U" || direction == "D" ? "doorh" : "doorv";
	$("#grid").append('<div class="sprite ' + doorClass + '" style="top: ' + yPixels + 'px; left: ' + xPixels + 'px;"></div>');
}

function DoorXFudge(direction : string) : number
{
	let fudge : number = 0;

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

function DoorYFudge(direction : string) : number
{
	let fudge : number = 0;

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

function PlaceKey(x : number, y : number) : void
{
	let xPixels : number = x * 45;
	let yPixels : number = 630 - (y * 45);
	$("#grid").append('<div class="sprite key" style="top: ' + yPixels + 'px; left: ' + xPixels + 'px;"></div>');
}

function PlaceGem(x : number, y : number) : void
{
	let xPixels : number = x * 45;
	let yPixels : number = 630 - (y * 45);
	$("#grid").append('<div class="sprite gem" style="top: ' + yPixels + 'px; left: ' + xPixels + 'px;"></div>');
}

function PlaceHeart(x : number, y : number) : void
{
	let xPixels : number = x * 45;
	let yPixels : number = 630 - (y * 45);
	$("#grid").append('<div class="sprite heartr" style="top: ' + yPixels + 'px; left: ' + xPixels + 'px;"></div>');
}

// *** END DOORS, KEYS, GEMS, and HEARTS ***
// *** BEGIN ACCESSORS ***

function GetLevel() : number
{
	return Number(sessionStorage.getItem("level"));
}

function SetLevel(level : number) : void
{
	sessionStorage.setItem("level", level.toString());
}

function GetStartCoords(level : number) : string
{
	return sessionStorage.getItem("start" + level.toString());
}

function SetStartCoords(level : number, coords : string) : void
{
	sessionStorage.setItem("start" + level.toString(), coords);
}

function GetDungeon(level : number) : object
{
	if (sessionStorage.getItem("dungeon" + level.toString()) === null)
	{
		return null;
	}
	else
	{
		// Leave the cast to Array<object> alone. It seems to be important in the LevelUp() scenario.
		let dungeon : object = Array<object>(JSON.parse(sessionStorage.getItem("dungeon" + level.toString())));
		return dungeon[0];
	}
}

function SetDungeon(level : number, dungeon: object) : void
{
	sessionStorage.setItem("dungeon" + level.toString(), JSON.stringify(dungeon));
}

// *** END ACCESSORS ***