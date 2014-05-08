console.log('In closures, objects, arrays..');

function incrementMe(start) {
    return function () {
        start++;
        console.log('In returned function. start = ' + start);
    };
}

var incrementor = incrementMe(10);
incrementor();
incrementor();

/* Immediately Invoked Function Expression IIFE */
console.log('....Doom of closures...');
var funcArray = [];
for(var i = 0; i < 5; i++){
    funcArray.push(function () { return i; } );
}
console.log(funcArray[0]()); // will print 5 because when funcArray[0] executes, i will have the value 5 (the last incremented value). This is because of closures
console.log(funcArray[1]());
console.log(funcArray[2]());
console.log(funcArray[3]());
console.log(funcArray[4]());

console.log('....IIFE to the rescue');
var funcArrayNew = [];
for (var i = 0; i < 5; i++) {
    (function () {
        var i2 = i;
        funcArrayNew.push(function () { return i2; });
    }());
}
console.log(funcArrayNew[0]()); // will print values 0, 1, 2, 3,4 correctly
console.log(funcArrayNew[1]());
console.log(funcArrayNew[2]());
console.log(funcArrayNew[3]());
console.log(funcArrayNew[4]());
