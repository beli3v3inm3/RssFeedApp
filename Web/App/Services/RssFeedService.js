'use strict';
app.factory('RssFeedService', ['$http', 'ngAuthSettings', function ($http, ngAuthSettings) {

    var serviceBase = ngAuthSettings.apiServiceBaseUri;
    var FeedServiceFactory = {};

    var _submitUrl = function (RssData) {

        return $http.post(serviceBase + 'api/rssreader/addfeed', RssData);
    };

    var _getFeed = function () {

        return $http.get(serviceBase + 'api/rssreader').then(function (results) {
            return results;
        });
    };

    var _getRss = function () {

        return $http.get(serviceBase + 'api/rssreader/getrss').then(function (results) {
            return results;
        });
    };

    var _setReadItem = function(rssData) {

        return $http.post(serviceBase + 'api/rssreader', rssData);
    };

    FeedServiceFactory.submitUrl = _submitUrl;
    FeedServiceFactory.getFeed = _getFeed;
    FeedServiceFactory.getRss = _getRss;
    FeedServiceFactory.setReadItem = _setReadItem;

    return FeedServiceFactory;

}]);