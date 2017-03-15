angular.module("umbraco").controller("SiteImprove.DashboardController", [
    '$scope', '$http',
    function ($scope, $http) {
        $scope.token = "";
        $scope.crawlingIds = "";
        $scope.output = "";

        $scope.requestNewToken = function () {
            $scope.token = "Loading...";
            $http.get('/umbraco/backoffice/api/siteImprove/requestNewToken')
                .then(function (resp) {
                    $scope.token = resp.data;
                });
        };

        $scope.saveIds = function () {
            $http.post('/umbraco/backoffice/api/siteImprove/setCrawlingIds?ids=' + $scope.crawlingIds)
                .then(function () {
                    $scope.output = "Saved!";
                    
                    if (window.siteimprove) {
                        window.siteimprove.recrawlIds = ($scope.crawlingIds || '').split(',');
                    }

                    setTimeout(function () {
                        $scope.output = "";
                    }, 2000);
                });
        };

        $scope.loadData = function () {
            $http.get('/umbraco/backoffice/api/siteImprove/getSettings')
                .then(function (resp) {
                    $scope.token = resp.data.token;
                    $scope.crawlingIds = resp.data.crawlingIds;
                });
        };

        $scope.loadData();
    }
]);