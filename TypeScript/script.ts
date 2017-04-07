"use strict";

let dungeon : object = null;

function Reset() : void
{
	var spinner = SpinnerSetup();
	spinner.spin($('#grid')[0]);
	
	$.ajax
	({
		type: 'GET',
		dataType: 'json',
		contentType: 'application/json',
		url: '/Index/DungeonView',
		success: function (result) 
		{
			dungeon = [JSON.parse(result)];  // As this is a reset, assign the JSON to element 0 of the dungeon array.
			PresentDungeonLevel(dungeon[0]);
		}
	});
}

function PresentDungeonLevel(dungeonLevel : object) : void
{
	for (var x = 0; x <= 14; x++) 
	{
		for (let y = 0; y <= 14; y++)
		{
			let gridLocation : string = GridLocation(x, y);
			console.log(gridLocation + ' ' + dungeonLevel[x][y].CssName);
			// Bug: need to remove old ones first. How?
			$(gridLocation).addClass(dungeonLevel[x][y].CssName);
		}
	}
}

function GridLocation(x : number, y : number) : string
{
	let xs = x < 10 ? "0" + x.toString() : x.toString();
	let ys = y < 10 ? "0" + y.toString() : y.toString();
	return "#g" + xs + "-" + ys;  // Prepend the "#" because jQuery needs it for the id attribute.
}

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