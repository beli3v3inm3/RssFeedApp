var app = angular.module('AngularAuthApp', ['ngRoute', 'LocalStorageModule', 'angular-loading-bar']);

app.config(function ($routeProvider) {

    $routeProvider.when("/home", {
        controller: "HomeController",
        templateUrl: "/App/Views/Home.html"
    });

    $routeProvider.when("/login", {
        controller: "LoginController",
        templateUrl: "/App/Views/Login.html"
    });

    $routeProvider.when("/signup", {
        controller: "SignupController",
        templateUrl: "/App/Views/Signup.html"
    });

    $routeProvider.when("/orders", {
        controller: "OrdersController",
        templateUrl: "/App/Views/Orders.html"
    });

    $routeProvider.when("/refresh", {
        controller: "RefreshController",
        templateUrl: "/App/Views/Refresh.html"
    });

    $routeProvider.when("/tokens", {
        controller: "TokensManagerController",
        templateUrl: "/App/Views/Tokens.html"
    });

    $routeProvider.when("/associate", {
        controller: "AssociateController",
        templateUrl: "/App/Views/Associate.html"
    });

    $routeProvider.when("/feed", {
        controller: "RssFeedController",
        templateUrl: "/App/Views/AddFeed.html"
    });

    $routeProvider.when("/viewfeed", {
        controller: "ViewRssFeedController",
        templateUrl: "/App/Views/ViewFeed.html"
    });
    $routeProvider.otherwise({ redirectTo: "/home" });
});

var serviceBase = 'http://localhost:63611/';
app.constant('ngAuthSettings', {
    apiServiceBaseUri: serviceBase,
    clientId: 'ngAuthApp'
});

app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('AuthInterceptorService');
});

app.run(['AuthService', function (authService) {
    authService.fillAuthData();
}]);
