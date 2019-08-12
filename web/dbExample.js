const mongoose = require('mongoose');

var Schema = mongoose.Schema;

var chatSchema = new Schema({
    user: String,
    body: String
})

var Chat = mongoose.model('Chat', chatSchema);

function connect() {
  mongoose.connect('mongodb://220.67.124.122:13222/test', { useNewUrlParser: true })
  .then(() => console.log('Sucessfully connected to mongodb'))
  .catch(e => console.error(e));
}
connect();
mongoose.connection.on('disconnected', connect);

