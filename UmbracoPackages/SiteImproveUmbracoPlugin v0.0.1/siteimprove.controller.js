
angular.module('umbraco').factory('SiteImproveMenuActions', ['$http', '$log', 'treeService', '$location', 'navigationService', 'appState', 'editorState', function ($http, $log, treeService, $location, navigationService, appState, editorState) {
    var factory = new function () {

        this.Check = function () {
            var _si = window._si || [];

            $http.get('/umbraco/backoffice/api/siteImproveToken/getToken')
            .then(function (response) {
                var url = $location.protocol() + "://" + $location.host() + editorState.current.urls[0];
                _si.push(['input', url, response.data, function () {  }]);
            });

        }
        
    };

    return factory;
}]);