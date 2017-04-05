"use strict";

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