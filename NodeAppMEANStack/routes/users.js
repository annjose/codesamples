var express = require('express');
var router = express.Router();

/* GET users listing. */
/*
router.get('/', function(req, res) {
    res.send('{name: value}');

});
*/

var usersController = require('../controllers/users');
router.get('/', usersController.index);

module.exports = router;
