
// Try to find an event as late as possible so angular has been loaded
$(window).load(function () {

    // Wait until next js tick, just in case...
    setTimeout(function () {
        var siHelper = window.siteimprove.helper,
            $scope = angular.element('body').scope(); // Get $rootSope

        $.get(siHelper.backofficeApiUrl + '/getCrawlingIds')
            .then(function (response) {
                window.siteimprove.recrawlIds = (response || '').split(',');
            });

        $.get(siHelper.backofficeApiUrl + '/getToken')
            .then(function (response) {
                window.siteimprove.token = response;
            });

        if ($scope) {
            $scope.$on('$routeChangeSuccess', siHelper.on$routeChangeSuccess.bind(siHelper));
        }
        
    }, 0)
});
