/// <reference path="SystemUnderTest.js" />
module('Calc Tests');

test('my first test', function () {
    ok(true, 'my first test in first module');
});
test('adding two numbers should return sum', function () {
    var sum = add(1, 2);
    strictEqual(3, sum, 'Sum is 5');
});

module('module 2');

test('my second test', function () {
    ok(true, 'my first test in second module');
});