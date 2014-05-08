/* Using plain NodeJS */

/*var rawHttp = require('http');

var rawServer = rawHttp.createServer(function(request, response){
	response.writeHead(200, {"Content-Type" : "text/plain"});
	response.end(" app");
});

rawServer.listen(3000);
console.log("Raw node Server running...");
*/

/* Using ExpressJS */
var express = require('express');
var app = express();

app.get('/', function(req, res){
	res.send('My Express');
});

app.get('/me', function(req, res){
	res.send('Me - Ann Catherine Jose');
});

app.listen(3000, function(){
	console.log('Express node server listening on port 3000')
});

