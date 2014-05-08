console.log('Hello World!');

var arr = [1, 2, 3];
var i = 0;
while(i < arr.length){
    console.log(arr[i]);
    i++;
}
console.log('......................');
function add(a, b) {
    return a + b;
}

var f = function (a, b) {
    return a + b;
}

console.log(add(10, 11));
console.log(f(10, 11));
doWork(function (a, b) {
    return a + b;
});
doWork(f);
doWork(add);

function doWork(callback) {
    console.log(callback);
    console.log(callback(2, 3));
}

g(); // OK; function declarations are hoisted to the topmost scope
function g() {
    console.log('in g()');
}

//hRef(); // no OK; var assignments are no hoisted (unlike var declarations and function declarations)
var hRef = function h() {
    console.log('in h()');
}
hRef();

function fArg(a, b) {
    console.log('Args.length: ' + arguments.length);
    console.log('args[0]: ' + arguments[0]);
    console.log('args[1]: ' + arguments[1]);
    console.log('args[2]: ' + arguments[2]);
}
fArg(1, 2, 3);
fArg(1);
fArg();

//var tmp; // console.log on this gives 'undefined'
var tmp = 10; // OK
console.log('tmp: ' + tmp);
