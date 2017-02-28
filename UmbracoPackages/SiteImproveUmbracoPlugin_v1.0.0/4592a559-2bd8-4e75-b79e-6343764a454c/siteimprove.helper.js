var siteimprove = { log: false, initiated: false };

siteimprove.helper = {

    backofficeApiUrl: '/umbraco/backoffice/api/siteImprove',

    pushSi: function (method, url) {
        var _si = window._si || [];

        // Get token from backoffice
        $.get(this.backofficeApiUrl + '/getToken')
        .then(function (response) {

            // Build full URL
            var fullUrl = window.location.protocol + '://' + window.location.host + url;
            _si.push([method, fullUrl, response, function () {
                if (siteimprove.log)
                    console.log('SiteImprove pass: ' + method + ' - ' + fullUrl);
            }]);
        });
    },

    closeSi: function () {
        if (siteimprove.initiated) {
            (window._si || []).push(['input', '', '']);
        }
    },

    // Method for the routeChangeSuccess event, initated either by siteimprove.main.js or siteimprove.controller.js
    on$routeChangeSuccess: function (e, next, current) {

        // Only listen when user works on the content tree
        if (next.params.tree === 'content' && next.params.id) {

            // Get page url from the backoffice api
            $.get(this.backofficeApiUrl + '/getPageUrl?pageid=' + next.params.id)
            .then(function (response) {

                // When recieved the url => send off to _si
                this.pushSi('input', response);
            }.bind(this));

            // Wait one js tick to hook on the publish button. The controller does not exist until after the route change
            setTimeout(function () {
                var uEditController = angular.element($("body div [ng-controller='Umbraco.Editors.Content.EditController']"));
                if (uEditController.length < 1) {
                    return;
                }

                var $scope = uEditController.scope();

                // Hook on save and publish event
                $scope.$on('formSubmitted', function (form) {
                    this.pushSi('recheck', form.targetScope.content.urls[0]);
                }.bind(this));

            }.bind(this), 0);

        } else {
            this.closeSi(); // Close siteimprove window
        }

    }
}