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
		BuildGrid(level, dungeon);
	}
	else
	{
		let spinner : Spinner = SpinnerSetup();
		spinner.spin($('#grid')[0]);

		let sX : number = level == 1 ? 7 : Number(GetStartCoords(level).substring(1, 3));
		let sY : number = level == 1 ? 0 : Number(GetStartCoords(level).substring(3, 6));
		let sD : string = level == 1 ? "U" : GetStartCoords(level).substring(5, 6);
		
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
				BuildGrid(level, dungeon);
				SetDungeon(level, dungeon);
				spinner.stop();
			}
		});
	}
}

// This function is going to iterate through each div in the grid and determine its location. It will then
// strip it of any existing CSS classes. This will clear any dungeon level currently being presented. That
// also removes the "native" classes the div needs ("tile" and it's location class). Those classes are 
// added back on, as well as the tile name class that allows the div to display a graphical tile from the
// tile sprite sheet.
function BuildGrid(level : number, dungeon : object) : void
{
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

			if (tileName == "f0" || tileName == "f1" || tileName == "f2" || tileName == "f3")
			{
				RecordStart(level, tileName, gridReference);
			}
		}
	}
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
		case "f0":
			direction = "U";
			break;

		case "f1":
			direction = "D";
			break;

		case "f2":
			direction = "L";
			break;

		case "f3":
			direction = "R";
			break;
	
		default:
			break;
	}

	SetStartCoords(level + 1, gridReference + direction);
}

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
// *** BEGIN UTILITY ***

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

// *** END UTILITY ***