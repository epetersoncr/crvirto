﻿angular.module('virtoCommerce.contentModule')
.controller('virtoCommerce.contentModule.editPageController', ['$scope', 'dialogService', 'virtoCommerce.contentModule.stores', 'virtoCommerce.contentModule.pages', '$timeout', function ($scope, dialogService, pagesStores, pages, $timeout) {
    var blade = $scope.blade;
    var codemirrorEditor;

    blade.refresh = function () {
        pagesStores.get({ id: blade.choosenStoreId }, function (data) {
            blade.languages = data.languages;
            blade.defaultStoreLanguage = data.defaultLanguage;

            if (!blade.newPage) {
                pages.getPage({ storeId: blade.choosenStoreId, language: blade.choosenPageLanguage, pageName: blade.choosenPageName }, function (data) {
                    blade.isLoading = false;
                    blade.currentEntity = data;
                    
                    $timeout(function () {
                        if (codemirrorEditor) {
                            codemirrorEditor.refresh();
                            codemirrorEditor.focus();
                        }
                        blade.origEntity = angular.copy(blade.currentEntity);
                    }, 1);
                });

                $scope.bladeToolbarCommands = [
				{
				    name: "Save page", icon: 'fa fa-save',
				    executeMethod: function () {
				        $scope.saveChanges();
				    },
				    canExecuteMethod: function () {
				        return isDirty();
				    }
				},
				{
				    name: "Reset page", icon: 'fa fa-undo',
				    executeMethod: function () {
				        angular.copy(blade.origEntity, blade.currentEntity);
				    },
				    canExecuteMethod: function () {
				        return isDirty();
				    }
				},
				{
				    name: "Delete page", icon: 'fa fa-trash-o',
				    executeMethod: function () {
				        deleteEntry();
				    },
				    canExecuteMethod: function () {
				        return !isDirty();
				    }
				}];
            }
            else {
                blade.currentEntity = { storeId: blade.choosenStoreId, language: blade.defaultStoreLanguage };

                blade.isLoading = false;
            }
        });
    };

    function isDirty() {
        return !angular.equals(blade.currentEntity, blade.origEntity);
    };

    $scope.saveChanges = function () {
        blade.isLoading = true;

        if (blade.newPage) {
            pages.checkName({ storeId: blade.choosenStoreId, pageName: blade.currentEntity.name, language: blade.currentEntity.language }, function (data) {
                if (Boolean(data.result)) {
                    pages.update({ storeId: blade.choosenStoreId }, blade.currentEntity, function () {
                        blade.parentBlade.refresh(true);
                        blade.choosenPageName = blade.currentEntity.name;
                        blade.choosenPageLanguage = blade.currentEntity.language;
                        blade.title = blade.currentEntity.name;
                        blade.subtitle = 'Edit page';
                        blade.newPage = false;
                        blade.refresh();
                    });
                }
                else {
                    blade.isLoading = false;
                    var dialog = {
                        id: "errorInName",
                        title: "Name not unique",
                        message: "Name must be unique for this language!",
                        callback: function (remove) {

                        }
                    }
                    dialogService.showNotificationDialog(dialog);
                }
            });
        }
        else {
            pages.update({ storeId: blade.choosenStoreId }, blade.currentEntity, function () {
                blade.parentBlade.refresh(true);
                blade.choosenPageName = blade.currentEntity.name;
                blade.choosenPageLanguage = blade.currentEntity.language;
                blade.title = blade.currentEntity.name;
                blade.subtitle = 'Edit page';
                blade.newPage = false;
                blade.refresh();
            });
        }
    };

    function deleteEntry() {
        var dialog = {
            id: "confirmDelete",
            title: "Delete confirmation",
            message: "Are you sure you want to delete this page?",
            callback: function (remove) {
                if (remove) {
                    blade.isLoading = true;

                    pages.delete({ storeId: blade.choosenStoreId, pageNamesAndLanguges: blade.choosenPageLanguage + '^' + blade.choosenPageName }, function () {
                        $scope.bladeClose();
                        blade.parentBlade.refresh();
                    });
                }
            }
        }
        dialogService.showConfirmationDialog(dialog);
    }

    function isCanSave() {
        return (!(angular.isUndefined(blade.currentEntity.name) || blade.currentEntity.name === null) &&
			!(angular.isUndefined(blade.currentEntity.content) || blade.currentEntity.content === null));
    }

    blade.onClose = function (closeCallback) {
        if ((isDirty() && !blade.newPage) || (isCanSave() && blade.newPage)) {
            var dialog = {
                id: "confirmCurrentBladeClose",
                title: "Save changes",
                message: "The page has been modified. Do you want to save changes?",
                callback: function (needSave) {
                    if (needSave) {
                        $scope.saveChanges();
                    }
                    closeCallback();
                }
            }
            dialogService.showConfirmationDialog(dialog);
        }
        else {
            closeCallback();
        }
    };

    $scope.bladeHeadIco = 'fa fa-archive';

    blade.getFlag = function (lang) {
        switch (lang) {
            case 'ru-RU':
                return 'ru';

            case 'en-US':
                return 'us';

            case 'fr-FR':
                return 'fr';

            case 'zh-CN':
                return 'ch';

            case 'ru-RU':
                return 'ru';

            case 'ja-JP':
                return 'jp';

            case 'de-DE':
                return 'de';
        }
    };

    // Codemirror configuration
    $scope.editorOptions = {
        lineWrapping: true,
        lineNumbers: true,
        extraKeys: { "Ctrl-Q": function (cm) { cm.foldCode(cm.getCursor()); } },
        foldGutter: true,
        gutters: ["CodeMirror-linenumbers", "CodeMirror-foldgutter"],
        onLoad: function (_editor) {
            codemirrorEditor = _editor;
        },
        //mode: 'xml'
        mode: 'htmlmixed'
    };

    blade.refresh();
}]);