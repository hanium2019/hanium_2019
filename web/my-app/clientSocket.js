const io = require('socket.io-client');

var socket = io.connect('http://localhost:80');  //localhost로 연결합니다.
socket.on('news', function (data) {  // 서버에서 news 이벤트가 일어날 때 데이터를 받습니다.
    console.log(data);
    socket.emit('my other event', { my: 'data' });   //서버에 my other event 이벤트를 보냅니다.
});