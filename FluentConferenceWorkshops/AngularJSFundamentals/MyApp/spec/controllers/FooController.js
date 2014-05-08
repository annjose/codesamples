describe('FooController', function(){
	beforeEach(function() {module('app')});	

	beforeEach(
		inject(
			function($rootScope, $controller){
				this.scope = $rootScope.new();
				$controller('FooController', {$scope: this.scope})
			}	
		)	
	);

	it('sets the value', function(){
		expect(this.scope.foo).not.toBeDefined();
		this.scope.setFoo('bar');
		expect(this.scope.foo).toEqual('bar');
	});

});

