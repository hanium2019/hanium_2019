const Router = require('koa-router');

const api = new Router();
const books = require('./books');
const auth = require('./auth');

api.use('/auth', auth.routes());
api.use('/books', books.routes());

module.exports = api;