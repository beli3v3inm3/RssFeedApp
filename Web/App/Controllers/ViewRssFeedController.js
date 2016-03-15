'use strict';
app.controller('ViewRssFeedController', ['$scope', 'RssFeedService', function ($scope, rssFeedService) {

    $scope.feed = [];

    rssFeedService.getFeed().then(function (results) {

        $scope.feed = results.data;

    }, function (error) {
        //alert(error.data.message);
    });
}]);