'use strict';
app.controller('RssFeedController', ['$scope', '$location', 'RssFeedService', function ($scope, $location, RssFeedService) {

    $scope.message = "";
    $scope.rss = [];
    $scope.rssData = {
        url: "",
        title: "",
        body: ""
    };
    GetRss();
    $scope.submitfeed = function () {

        RssFeedService.submitUrl($scope.rssData).success(function () {
            //$location.path('/viewfeed');
        });
    };

    function GetRss() {
        RssFeedService.getRss().then(function (results) {
            $scope.rss = results.data;

        }, function (error) {
            //alert(error.data.message);
        });
    }
}]);