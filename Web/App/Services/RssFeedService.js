'use strict';
app.factory('RssFeedService', ['$http', 'ngAuthSettings', function ($http, ngAuthSettings) {

    var serviceBase = ngAuthSettings.apiServiceBaseUri;
    var FeedServiceFactory = {};

    var _submitUrl = function (RssData) {

        return $http.post(serviceBase + 'api/rssreader/addfeed', RssData)
    };

    var _getFeed = function () {

        return $http.get(serviceBase + 'api/rssreader').then(function (results) {
            return results;
        });
    };

    FeedServiceFactory.submitUrl = _submitUrl;
    FeedServiceFactory.getFeed = _getFeed;

    return FeedServiceFactory;

}]);