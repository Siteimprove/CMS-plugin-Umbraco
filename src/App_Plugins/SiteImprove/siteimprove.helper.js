var siteimprove = { log: false, recrawlIds: [], token: '' };

siteimprove.helper = {

    backofficeApiUrl: '/umbraco/backoffice/api/siteImprove',

    /**
     * Will fetch _si token and push of method to _si
     */
    pushSi: function (method, url) {
        var _si = window._si || [];

        // Get token from backoffice if not set
        if (!siteimprove.token) {
            $.get(this.backofficeApiUrl + '/getToken')
                .then(function (response) {
                    siteimprove.token = response;

                    if (siteimprove.log)
                        console.log('SiteImprove pass: ' + method + ' - ' + url);

                    // Build full URL
                    _si.push([method, url, response]);
                });
        } else {
            if (siteimprove.log)
                console.log('SiteImprove pass: ' + method + ' - ' + url);

            // Build full URL
            _si.push([method, url, siteimprove.token]);
        }
    },

    /**
     * Workaround for closing the _si window
     */
    closeSi: function () {
        (window._si || []).push(['input', '', '']);
    },

    /**
     * Wrap simple ajax call getPageUrl
     * @return {Promise}
     */
    getPageUrl: function (pageId) {
        return $.get(siteimprove.helper.backofficeApiUrl + '/getPageUrl?pageid=' + pageId);
    },

    /**
     * Requests pageUrl from backoffice and send to SI
     */
    handleFetchPushUrl: function (method, pageId, isFormPublish) {
        this.getPageUrl(pageId)
            .then(function (response) {

                if (response.success) {

                    // When recieved the url => send off to _si
                    siteimprove.helper.pushSi(method, response.url);

                }
                else {
                    if (isFormPublish) {
                        siteimprove.helper.pushSi('input', '');
                        return;
                    }

                    // If can't find page pass empty url
                    siteimprove.helper.pushSi(method, '');
                }

            })
            .fail(function (error) {
                siteimprove.helper.closeSi();
            });
    },

    /**
     * Handles the RouteChangeSuccess event. Will take care of pushing to _si and listen on publish event for the new edit scope.
     */
    on$routeChangeSuccess: function (e, next, current) {

        // Only listen when user works on the content tree
        if (next.params.tree === 'content' && !next.params.hasOwnProperty('create') && next.params.id) {

            if (siteimprove.recrawlIds.length < 1) {
                $.get(siteimprove.helper.backofficeApiUrl + '/getCrawlingIds')
                    .then(function (response) {
                        siteimprove.recrawlIds = (response || '').split(',');
                    });
            }

            var method;

            if (current)
                method = current.params.hasOwnProperty('create') ? 'recheck' : 'input';
            else
                method = "input";

            siteimprove.helper.handleFetchPushUrl(method, next.params.id);

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
                    siteimprove.helper.handleFetchPushUrl('recheck', form.targetScope.content.id, true);
                });

            }, 0);

        } else {
            if (siteimprove.log)
                console.log('SiteImprove pass: clear');

            (window._si || []).push(['clear']); // Because we listen on the routeChange, if we're no longer on the "content tree" => Clear siteimprove
        }
    }
}