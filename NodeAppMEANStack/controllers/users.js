var express = require('express');
var router = express.Router();

var db = require('../services/dbservice');

exports.index = function(req, res){
    res.send(db.getUsers());
}

// module.exports = router;
