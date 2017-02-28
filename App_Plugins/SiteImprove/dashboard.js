angular.module("umbraco").controller("SiteImprove.DashboardController", [
    '$scope', '$http',
    function ($scope, $http) {
        $scope.token = "";

        $scope.requestNewToken = function () {
            $scope.token = "Loading...";
            $http.get('/umbraco/backoffice/api/siteImprove/requestNewToken')
                .then(function (resp) {
                    $scope.token = resp.data;
                });
        }

        $scope.loadData = function () {
            $http.get('/umbraco/backoffice/api/siteImprove/getToken')
                .then(function (resp) {
                    $scope.token = resp.data;
                });
        }

        $scope.loadData();
    }
]);