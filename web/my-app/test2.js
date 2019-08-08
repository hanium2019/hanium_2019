const io = require('socket.io-client');
var socket = io('http://220.67.124.122:13222');

// var socket = io('http://127.0.0.1:13222');
socket.on('news', function(data){
    console.log(data);
    socket.emit('my other event', {my: 'data'});
});