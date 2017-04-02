//
//  TryReactive.swift
//  HelloRxSwift
//
//  Created by Jose, Ann Catherine on 4/2/17.
//  Copyright © 2017 Ann. All rights reserved.
//

import Foundation
import RxSwift

func tryRx() {
    let stringSequence = Observable.just("Hello RxSwift")
    let numSequence = Observable.from([1, 2, 3])
    let dictionarySequence = Observable.from([1: "One", 2: "Two"])
    
    let subscription = stringSequence.subscribe { (event) in
        print("event: \(event)")
    }
}
