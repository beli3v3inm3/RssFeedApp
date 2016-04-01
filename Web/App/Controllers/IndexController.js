'use strict';
app.controller('IndexController', ['$scope', '$location', 'AuthService', function ($scope, $location, authService) {
    $scope.logOut = function () {
        authService.logOut();
        $location.path('/login');
    }

    $scope.authentication = authService.authentication;

}]);