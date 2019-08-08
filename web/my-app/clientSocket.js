// const io = require('socket.io-client');

// // var socket = io.connect('localhost:13000');  //localhost로 연결합니다.http://ar-conference.na.to/
// var socket = io.connect('192.168.0.76:13222');
// console.log('start client')

// socket.on('news', function (data) {  // 서버에서 news 이벤트가 일어날 때 데이터를 받습니다.
//     console.log(data);
//     socket.emit('my other event', { my: 'data' });   //서버에 my other event 이벤트를 보냅니다.
// });

const io = require('socket.io-client');
var socket = io('http://220.67.124.122:13222');

// var socket = io('http://127.0.0.1:13222');
socket.on('news', function(data){
    console.log(data);
    socket.emit('my other event', {my: 'data'});
});