console.log('this is my one javascript');

var a = 100;
console.log('a = ' + a);
// if (typeof a === "undefined") {
if (a === undefined) {
    console.log('a is undefined');
} else {
    console.log('a is NOT undefined');
}

function f(a) {
    console.log('in f(): a = ' + a);
}

function func(a) {
    var ret = {};
    var x = 100;
    var b = 20;
    
    return { b : b};
}

var val = func(100);
console.log('b = ' + val.b + ' x = ' + val.x
);

f();    // call f() without params; a will be undefined inside the function

// closure

function g() {
    var gOut = {};
    var g1 = 100;
    gOut.h = function() {
        return g1;
    };

    return gOut;
}

var gRet = g();
console.log('h = ' + gRet.h());