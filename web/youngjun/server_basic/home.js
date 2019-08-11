var module = require('./custom_module');

// 1. formatted 특수문자 %d를 사용해서 module.sum( ) 에서 리턴된 숫자값을 출력
console.log('sum = %d' , module.sum(100));

// 2. formatted 특수문자 %s를 사용해서 module.var1의 문자값을 출력
console.log('var1 = %s' , module.var1);
