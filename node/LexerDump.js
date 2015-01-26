"use strict";

var jade = require('./jade');
var path = process.argv[2];
var str = require('fs').readFileSync(path, 'utf8');

str = str.replace(/^\uFEFF/, '');
var lexer = new jade.Lexer(str, path);

var tokens = [];
var t;
var i = 0;
while (true)
{
	t = lexer.advance();
	if (t.type == 'eos')
		break;
	
	tokens.push(t);
	console.log(i + ": " + t.type);
	
	if (t.type === 'indent')
	{
		console.log('    Indents: ' + t.val);
	}
	else
	{
		if (t.val)
		{
			console.log('    Value: ' + JSON.stringify(t.val));
		}
		
		if (t.type == 'attrs')
		{
			console.log('    Attributes: ' + t.attrs.length);
			for (var x = 0; x < t.attrs.length; x++)
			{
				var val = t.attrs[x].val === true ? "true" : t.attrs[x].val;
				console.log('        Name: ' + JSON.stringify(t.attrs[x].name) +
					', Value: ' + JSON.stringify(val) + ', Escaped: ' + t.attrs[x].escaped);
			}
		}
	}
	i++;
}
