//
//  ViewController.swift
//  HelloFastlane
//
//  Created by Jose, Ann Catherine on 4/1/17.
//  Copyright © 2017 Ann. All rights reserved.
//

import UIKit
import Alamofire

struct HttpBin {
    let origin: String
    let url: String
    
    init?(json: Any) {
        guard let jsonDict = json as? NSDictionary else {
            return nil
        }
        guard let origin = jsonDict["origin"] as? String else {
            return nil
        }
        guard let url = jsonDict["url"] as? String else {
            return nil
        }
        
        self.origin = origin
        self.url = url
    }
}

class ViewController: UIViewController {

    override func viewDidLoad() {
        super.viewDidLoad()
        // Do any additional setup after loading the view, typically from a nib.
        tryAlamofire()
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }
    func tryAlamofire() {
        Alamofire.request("https://httpbin.org/get").response { (response) in
            print("Error = \(response.error)")
            print("Response = \(response.response)")
            print("Data = \(response.data)")
            print("Timeline = \(response.timeline)")
            print("Metrics = \(response.metrics)")
        }
        
        Alamofire.request("https://httpbin.org/get").responseJSON { (response) in
            print("Response value= \(response.value.debugDescription)")
            
            switch response.result {
            case .success(let value):
                if let httpBin = HttpBin(json: value) {
                    print("HttpBin. origin = \(httpBin.origin) Url = \(httpBin.url)")
                }
            case .failure(let error):
                print("Error: \(error)")
            }
        }
    }


}

