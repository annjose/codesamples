//: Playground - noun: a place where people can play

import UIKit

var str = "Hello, playground"

var statusCode: Int = 404
var errorString: String = "The request failed with the error:"

switch statusCode {
case 100, 101:
    errorString += " Informational, 1xx."
case 204:
    errorString += " Successful but no content, 204."
case 300...307:
    errorString += " Redirection, 3xx."
case 400...417:
    errorString += " Client error, 4xx."
case 500...505:
    errorString += " Server error, 5xx."
default:
    errorString = "Unknown. Please review the request and try again."
}


var myFirstInt: Int = 0
for i in 1...5 {
    myFirstInt += 1
    myFirstInt
    print(myFirstInt)
    print("myFirstInt equals \(myFirstInt) at iteration \(i)")
}

var shields = 5
var blastersOverheating = false
var blasterFireCount = 0
var spaceDemonsDestroyed = 0
while shields > 0 {
    if spaceDemonsDestroyed == 500 {
        print("You beat the game!")
        break
    }
    if blastersOverheating {
        print("Blasters are overheated! Cooldown initiated.")
        sleep(5)
        print("Blasters ready to fire")
        sleep(1)
        blastersOverheating = false
        blasterFireCount = 0
        continue
    }
    if blasterFireCount > 100 {
        blastersOverheating = true
        continue
    }
    // Fire blasters!
    print("Fire blasters!")
    blasterFireCount += 1
    spaceDemonsDestroyed += 1
}



let playground = "Hello, playground"
var mutablePlayground = "Hello, mutable playground"
mutablePlayground += "!"
for c: Character in mutablePlayground.characters {
    print("'\(c)'")
}
let oneCoolDude = "\u{1F60E}"
let aAcute = "\u{0061}\u{0301}"
for scalar in playground.unicodeScalars {
    print("\(scalar.value) ")
}
