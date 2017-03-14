﻿var siteimprove = { log: false, recrawlIds: [] };

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

            if (siteimprove.log)
                console.log('SiteImprove pass: ' + method + ' - ' + url);

            // Build full URL
            _si.push([method, url, response]);
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
                        siteimprove.helper.closeSi();
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

            siteimprove.helper.handleFetchPushUrl('input', next.params.id);

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
                    if (siteimprove.recrawlIds.indexOf(form.targetScope.content.id.toString()) > -1) {
                        siteimprove.helper.handleFetchPushUrl('recrawl', form.targetScope.content.id, true);
                    } else {
                        siteimprove.helper.handleFetchPushUrl('recheck', form.targetScope.content.id, true);
                    }
                });

            }, 0);

        } else {
            siteimprove.helper.closeSi(); // Because we listen on the routeChange, if we're no longer on the "content tree" => Close siteimprove window
        }

    }
}