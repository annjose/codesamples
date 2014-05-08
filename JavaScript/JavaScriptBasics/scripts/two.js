var f1 = function(a) {
    console.log('in f1. a: ' + a);
};
var f2 = function () {
    console.log('in f2');
};

var intuit = intuit || {};

(function (namespace) {
    console.log('inside self-invoking function');
    var init = function () {
        console.log('inside init function');
        f1();
        f1(100)
        f2();
        console.log('exiting init function');
    };
    console.log('exiting self-invoking function');
    namespace.init = init;
})(intuit);


intuit.init();

//(function (a, b) {
//    arguments[0]();
//    a(arguments.length);
//    b();
//})(f1, f2);