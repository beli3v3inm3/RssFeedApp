'use strict';
app.controller('ViewRssFeedController', ['$scope', 'RssFeedService', function ($scope, rssFeedService) {

    $scope.feed = [];
    $scope.currentItem = false;


    rssFeedService.getFeed().then(function (results) {

        $scope.feed = results.data;

    }, function (error) {
        //alert(error.data.message);
    });

    $scope.isReadClick = function (feedId) {
        $scope.rssData = {
            id: feedId,
            url: "",
            title: "",
            body: ""
        };
        
        rssFeedService.setReadItem(rssData).success(function () {
            
        });

    };

    $scope.feed.forEach(function(checkItem) {
        if (COND) {
            
        }
    });
}]);