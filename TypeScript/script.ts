"use strict";

let dungeon = null;

function Reset()
{
	var spinner = SpinnerSetup();
	spinner.spin($('#grid')[0]);
	
	$.ajax({
		type: 'GET',
		dataType: 'html',
		url: '/Index/DungeonView',
		success: function (result) {
			$('#grid').html(result);
		}
	});

	$.ajax
	({
		type: 'GET',
		dataType: 'json',
		contentType: 'application/json',
		url: '/Index/DungeonViewJson',
		success: function (result) 
		{
			dungeon = [JSON.parse(result)];  // As this is a reset, assign the JSON to element 0 of the dungeon array.
		}
	});
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