/**
 * @ngdoc controller
 * @name Umbraco.DashboardController
 * @function
 * 
 * @description
 * Controls the dashboards of the application
 * 
 */

function DashboardController($scope, $routeParams, dashboardResource, localizationService) {

    $scope.page = {};
    $scope.page.nameLocked = true;
    $scope.page.loading = true;

    $scope.dashboard = {};
    localizationService.localize("sections_" + $routeParams.section).then(function (name) {
        $scope.dashboard.name = name;
    });

    dashboardResource.getDashboard($routeParams.section).then(function (tabs) {
        $scope.dashboard.tabs = tabs;

        // Show header on multiple tabs or a single one that requires it
        $scope.dashboard.showHeader = $scope.dashboard.tabs.length > 1 || ($scope.dashboard.tabs.length == 1 && $scope.dashboard.tabs[0].requireHeader);

        // Set first tab to active
        if ($scope.dashboard.tabs && $scope.dashboard.tabs.length > 0) {
            $scope.dashboard.tabs[0].active = true;
        }

        $scope.page.loading = false;
    });

    $scope.changeTab = function (tab) {
        $scope.dashboard.tabs.forEach(function (tab) {
            tab.active = false;
        });
        tab.active = true;
    };

}

// Register it
angular.module('umbraco').controller("Umbraco.DashboardController", DashboardController);
