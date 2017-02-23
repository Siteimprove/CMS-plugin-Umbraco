
angular.module('umbraco').factory('SiteImproveMenuActions', ['$http', '$rootScope', '$location', '$routeParams', '$log', 'appState', 'editorState', function ($http, $rootScope, $location, $routeParams, $log, appState, editorState) {
    var factory = new function () {
        var _si = window._si || [],
            siHelper = window.siteimprove.helper;

        if (window.siteimprove.initiated) {
            console.log('SiteImprove is already initiated');
        } else {
            $rootScope.$on('$routeChangeSuccess', siHelper.on$routeChangeSuccess.bind(siHelper));
        }

        this.Start = function () {
            siHelper.pushSi('input', editorState.current.urls[0]);
        };

        this.Recheck = function () {
            siHelper.pushSi('recheck', editorState.current.urls[0]);
        };

    };

    return factory;
}]);