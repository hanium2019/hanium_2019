require('dotenv').config();
const PORT = process.env.PORT || 3000;
const HOST = process.env.HOST || 'localhost'
const MONGODB_URI = process.env.MONGODB_URI || 'mongodb://localhost:27001/hanium2019';

console.log(process.env.PORT);
console.log(process.env.MONGODB_URI);

const Koa = require('koa');
const Router = require('koa-router');
const crypto = require('crypto');
const jwt = require('jsonwebtoken');
const { jwtMiddleware } = require('./lib/token');
// Koa doesn't parse the request body by default!
// It bring about ctx.request.body == undefined
const KoaBody = require('koa-body')();
const serve = require('koa-static');
const path = require('path');

const app = new Koa();
const router = new Router();
const api = require('./api');

//mongoose
const mongoose = require('mongoose');

mongoose.Promise = global.Promise;

mongoose.connect(MONGODB_URI, { useNewUrlParser: true });

app.use(KoaBody);
app.use(jwtMiddleware);
router.use('/api', api.routes());
app.use(router.routes()).use(router.allowedMethods({throw: true}));

app.use(serve(path.resolve(__dirname, '../front/build/')));
app.use(ctx => {
    ctx.body = 'NO React Build files';
});
app.listen(PORT, HOST, () => {
    console.log('server is listening to port '+PORT)
})