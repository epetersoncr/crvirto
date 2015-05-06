﻿angular.module('platformWebApp')
.controller('installWizardController', ['$scope', 'bladeNavigationService', 'FileUploader', 'modules', function ($scope, bladeNavigationService, FileUploader, modules) {

    $scope.submit = function () {
        $scope.isInstalling = true;

        if ($scope.blade.mode === 'install') {
            modules.install({ fileName: $scope.currentEntity.fileName }, onAfterSubmitted);
        } else if ($scope.blade.mode === 'update') {
            modules.update({ id: $scope.currentEntity.id, fileName: $scope.currentEntity.fileName }, onAfterSubmitted);
        }
    };

    function onAfterSubmitted(data) {
        var newBlade = {
            id: 'moduleInstallProgress',
            currentEntityId: data.id,
            title: $scope.blade.title,
            subtitle: 'Installation progress',
            controller: 'moduleInstallProgressController',
            template: 'Scripts/app/packaging/wizards/newModule/module-wizard-progress-step.tpl.html'
        };
        bladeNavigationService.showBlade(newBlade, $scope.blade.parentBlade);
    }

    function endsWith(str, suffix) {
        return str.indexOf(suffix, str.length - suffix.length) !== -1;
    }

    function initialize() {
        if (!$scope.uploader) {
            // Creates a uploader
            var uploader = $scope.uploader = new FileUploader({
                scope: $scope,
                headers: { Accept: 'application/json' },
                url: 'api/modules',
                autoUpload: true,
                removeAfterUpload: true
            });

            // ADDING FILTERS
            // packages only
            uploader.filters.push({
                name: 'packageFilter',
                fn: function (i /*{File|FileLikeObject}*/, options) {
                    return endsWith(i.name, '.zip');
                }
            });

            uploader.onAfterAddingFile = function (item) {
                $scope.blade.isLoading = true;
            };

            uploader.onCompleteAll = function () {
                $scope.blade.isLoading = false;
            }

            uploader.onSuccessItem = function (fileItem, data, status, headers) {
                $scope.currentEntity = data;
            };
        }
    };

    initialize();
    if ($scope.blade.mode === 'install') {
        $scope.actionButtonTitle = 'Install';
    } else {
        $scope.actionButtonTitle = 'Update';
    }

    $scope.blade.isLoading = false;
}]);
