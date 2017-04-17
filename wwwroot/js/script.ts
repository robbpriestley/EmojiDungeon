"use strict";

window.onload = function(){Start();}

function Start() : void
{
	sessionStorage.clear();
	SetLevel(0);            // Start at level 0...
	$("#level").text("1");  // ...presented to user as level 1.
	RenderLevel(0);
}

function LevelUp() : void
{
	let level : number = Number(sessionStorage.getItem("level"));
	level--  // Levels closer to the "surface" have smaller numbers.
	SetLevel(level);
	RenderLevel(level);
	$("#level").text(level + 1);  // Add 1 for display as _level is zero-indexed.
	sessionStorage.setItem("level", level.toString());
}

function LevelDown() : void
{
	let level : number = Number(sessionStorage.getItem("level"));
	level++  // Levels farther from the "surface" have larger numbers.
	SetLevel(level);
	RenderLevel(level);
	$("#level").text(level + 1);  // Add 1 for display as _level is zero-indexed.
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
		
		$.ajax
		({
			type: 'GET',
			dataType: 'json',
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
			$(gridId).addClass(dungeon[x][y].N);  // Add tile name class.
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

// *** BEGIN ACCESSORS ***

function GetLevel() : number
{
	return Number(sessionStorage.getItem("level"));
}

function SetLevel(level : number) : void
{
	sessionStorage.setItem("level", level.toString());
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