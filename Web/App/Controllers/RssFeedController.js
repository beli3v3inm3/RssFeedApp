'use strict';
app.controller('RssFeedController', ['$scope', '$location', 'RssFeedService', function ($scope, $location, RssFeedService) {

    $scope.message = "";

    $scope.rssData = {
        url: "",
        title: "",
        body: ""
    };

    $scope.submitfeed = function () {

        RssFeedService.submitUrl($scope.rssData).success(function () {
            $location.path('/viewfeed');
        });
    };
}]);