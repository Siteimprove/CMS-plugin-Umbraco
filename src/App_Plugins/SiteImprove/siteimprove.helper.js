var siteimprove = { log: false };

siteimprove.helper = {

    backofficeApiUrl: '/umbraco/backoffice/api/siteImprove',

    /**
     * Will fetch _si token and push of method to _si
     */
    pushSi: function (method, url) {
        var _si = window._si || [];

        // Get token from backoffice
        $.get(this.backofficeApiUrl + '/getToken')
        .then(function (response) {

            // Build full URL
            _si.push([method, url, response, function () {
                if (siteimprove.log)
                    console.log('SiteImprove pass: ' + method + ' - ' + url);
            }]);
        });
    },

    /**
     * Workaround for closing the _si window
     */
    closeSi: function () {
        (window._si || []).push(['input', '', '']);
    },

    /**
     * Wrap simple ajax call getPageUrl
     */
    getPageUrl: function (pageId) {
        return $.get(this.backofficeApiUrl + '/getPageUrl?pageid=' + pageId);
    },

    /**
     * Handles the RouteChangeSuccess event. Will take care of pushing to _si and listen on publish event for the new edit scope.
     */
    on$routeChangeSuccess: function (e, next, current) {

        // Only listen when user works on the content tree
        if (next.params.tree === 'content' && next.params.id) {

            // Get page url from the backoffice api
            this.getPageUrl(next.params.id)
                .then(function (response) {

                    // When recieved the url => send off to _si
                    this.pushSi('input', response);
                }.bind(this));

            // Wait one js tick to hook on the publish button. 
            // The controller does not exist until after the route change
            setTimeout(function () {
                var uEditController = angular.element($("body div [ng-controller='Umbraco.Editors.Content.EditController']"));
                if (uEditController.length < 1) {
                    return;
                }

                var $scope = uEditController.scope();

                // Hook on save and publish event
                $scope.$on('formSubmitted', function (form) {

                    this.getPageUrl(form.targetScope.content.id)
                    .then(function (response) {
                        this.pushSi('recheck', response);
                    }.bind(this));
                }.bind(this));

            }.bind(this), 0);

        } else {
            this.closeSi(); // Because we listen on the routeChange, if we're no longer on the "content tree" => Close siteimprove window
        }

    }
}