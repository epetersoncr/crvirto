﻿angular.module('virtoCommerce.mailingModule')
.controller('mailingWidgetController', ['$scope', 'bladeNavigationService', function ($scope, bladeNavigationService) {
    var blade = $scope.widget.blade;
    $scope.showWidget = blade.currentEntity.id == 'MailChimp.Mailing';
}]);