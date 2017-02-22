
angular.module('umbraco').factory('SiteImproveMenuActions', ['$http', '$rootScope', '$location', '$routeParams', '$log', 'appState', 'editorState', function ($http, $rootScope, $location, $routeParams, $log, appState, editorState) {
    var factory = new function () {
        var _si = window._si || [];

        $rootScope.$on('$routeChangeSuccess', function (e, next, current) {

            // Only listen when user works on the content tree
            if (next.params.tree === 'content') {

                // Wait one js tick to see if editorState is set
                setTimeout(() => {

                    if (!editorState || !editorState.current) {
                        // editorState is null, get the url from backoffice
                        $http.get('/umbraco/backoffice/api/siteImprove/getPageUrl?pageid=' + $routeParams.id)
                        .then(function (response) {
                            factory.PushSi('input', response.data.replace(/"/g, ""));
                        });
                        return;
                    }

                    // editorState contains to the url to the current page the user edits
                    factory.PushSi('input', editorState.current.urls[0])
                }, 0);

            } else {
                // TODO: Close the siteimprove window
                $log.info('TODO: Implement a remove of the SiteImprove plugin');
            }
        });

        this.PushSi = function (method, url) {
            // Get token from backoffice
            $http.get('/umbraco/backoffice/api/siteImprove/getToken')
            .then(function (response) {

                // Build full URL
                var fullUrl = $location.protocol() + '://' + $location.host() + url;
                _si.push([method, fullUrl, response.data, function () { $log.info('SiteImprove pass: ' + method + ' - ' + fullUrl) }]);
            });
        };

        this.Start = function () {
            factory.PushSi('input', editorState.current.urls[0]);
        };

        this.Recheck = function () {
            factory.PushSi('recheck', editorState.current.urls[0]);
        };

    };

    return factory;
}]);