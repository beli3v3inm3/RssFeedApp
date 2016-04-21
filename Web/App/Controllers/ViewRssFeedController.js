'use strict';
app.controller('ViewRssFeedController', ['$scope', '$location', 'RssFeedService', function ($scope, $location, rssFeedService) {

    $scope.feed = [];
    $scope.currentItem = false;
    $scope.showItem = {};
    $scope.SetItemVisible = function (index) {
        if (!$scope.showItem[index]) {
            $scope.showItem[index] = true;
        } else {
            $scope.showItem[index] = false;
        }
    }
    getRssItems();
    //rssFeedService.getFeed().then(function (results) {
    //    $scope.feed = results.data;

    //    //$scope.feed.forEach(function (checkitem){
    //    //    if (!checkitem.isRead) {
    //    //        $scope.unRead = {
    //    //            "color": "white",
    //    //            "background-color": "coral"
    //    //        }
    //    //    } else {
    //    //        $scope.read = {
    //    //            "color": "white",
    //    //            "background-color": "green"
    //    //        }
    //    //    }
    //    //});

    //}, function (error) {
    //    //alert(error.data.message);
    //});

    $scope.isReadClick = function (feedId) {
        $scope.rssData = {
            id: feedId
        };

        rssFeedService.setReadItem($scope.rssData).success(function () {
            getRssItems();
        });

    };

    $scope.deleteItem = function(feedId) {
        $scope.feedData = {
            id: feedId
        };

        rssFeedService.removeItem($scope.feedData).success(function () {
            getRssItems();
        });
    };

    function getRssItems() {
        rssFeedService.getFeed().then(function (results) {
            $scope.feed = results.data;

            if ($scope.feed.length === 0) {
                $location.path('/feed');
            }

        }, function (error) {
            //alert(error.data.message);
        });
    }

}]);