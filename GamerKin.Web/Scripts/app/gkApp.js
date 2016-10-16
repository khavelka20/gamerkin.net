app = angular.module('gamerkinApp', ['ngRoute', 'ui.bootstrap', 'truncate'], function ($interpolateProvider, $routeProvider) {
    $interpolateProvider.startSymbol('<%');
    $interpolateProvider.endSymbol('%>');

    $routeProvider.when('/', {
        templateUrl: 'Content/views/home.html',
        controller: 'HomeController'
    }).when('/browse/:genre?/:genreId?', {
        templateUrl: 'Content/views/browse.html',
        controller: 'BrowseController'
    }).when('/details/:gameName/:gameId', {
        templateUrl: 'Content/views/details.html',
        controller: 'DetailsController'
    }).when('/recommended', {
        templateUrl: 'Content/views/recommended.html',
        controller: 'RecommendedController'
    }).otherwise({
        redirectTo: '/'
    });
});

app.controller('RecommendedController', ['$scope', 'recommendedService', function ($scope, service) {
    $scope.vm = {
        object: {
            games: []
        },
        state: {
            loading: false,
            gamesPerPage: 12,
            currentPage: 1,
            gamesToBrowse: 0
        }
    };
    $scope.globalVm = {};

    function startUp() {
        service.startUp($scope);
        service.loadRecommendedGames($scope);
    };

    $scope.showPreview = function (game) {
        service.showPreview($scope, game);
    };

    $scope.hidePreview = function (game) {
        service.hidePreview(game);
    };

    $scope.showYoutube = function (game) {
        service.showModal($scope, 'youtube', game);
    };

    startUp();
}]);

app.factory('recommendedService', ['dataService', 'stateService', '$uibModal', function (dataService, stateService, $uibModal) {

    var loadRecommendedGames = function ($scope) {
        dataService.loadRecommendedGames($scope.globalVm.gamer).then(function (data) {
            $scope.vm.object.games = data;
            $scope.vm.state.gamesToBrowse = data.length;
            orderByPredictedScore($scope);
        });
    };

    var orderByPredictedScore = function ($scope) {
        $scope.vm.object.games.sort(function (a, b) {
            if (a.predicted_score > b.predicted_score) {
                return -1;
            }
            if (a.predicted_score < b.predicted_score) {
                return 1;
            }
            return 0;
        });
    };

    var startUp = function ($scope) {
        $scope.globalVm = stateService.getGlobalVm();
    };

    var showPreview = function ($scope, game) {
        hidePreviews($scope);
        game.show_preview = true;
        game.show_header = false;
    };

    var hidePreview = function (game) {
        game.show_preview = false;
        game.show_header = true;
    };

    var hidePreviews = function ($scope) {
        angular.forEach($scope.vm.games, function (game) {
            game.show_preview = false;
            game.show_header = true;
        });
    };

    var showModal = function ($scope, modal, game) {
        $uibModal.open({
            animation: true,
            backdrop: 'static',
            templateUrl: 'views/modals/' + modal + '.html',
            controller: 'ModalInstanceController',
            resolve: {
                gameData: function () {
                    return {
                        youtubeId: game.videos[0].youtube_id,
                        name: game.name
                    };
                }
            },
            size: 'lg'
        });
    };

    return {
        startUp: startUp,
        loadRecommendedGames: loadRecommendedGames,
        showPreview: showPreview,
        hidePreview: hidePreview,
        showModal: showModal
    };

}]);

app.controller('DetailsController', ['$scope', 'detailsService', function ($scope, detailsService) {

    $scope.globalVm = {};
    $scope.vm = {
        object: {
            gameDetailsVM: {}
        },
        state: {
            loading: false,
            mediaGallery: {
                showScreenShot: false,
                showGamePlayClip: true,
                showYoutubeVideo: false,
                screenShotUrl: ""
            }
        },
        input: {
            gameId: ""
        }
    };

    function startUp() {
        detailsService.startUp($scope);
    }

    $scope.showGamePlayClip = function () {
        detailsService.showGamePlayClip($scope);
    };

    $scope.showYoutubeVideo = function () {
        detailsService.showYoutubeVideo($scope);
    };

    $scope.showScreenShot = function (screenShot) {
        detailsService.showScreenShot($scope, screenShot);
    };

    startUp();
}]);

app.factory('detailsService', ['dataService', '$routeParams', 'stateService', function (dataService, $routeParams, stateService) {

    var startUp = function ($scope) {

        $scope.globalVm = stateService.getGlobalVm();
        $scope.vm.state.loading = true;
        $scope.vm.input.gameId = $routeParams.gameId;

        dataService.loadGameDetailsViewModel($scope.vm.input.gameId, $scope.globalVm.gamer.id).then(
        function (data) {
            $scope.vm.object.gameDetailsVM = data;
            $scope.vm.state.loading = false;
        });
    };

    var setMediaGalleryType = function ($scope, media) {

        $scope.vm.state.mediaGallery.showScreenShot = false;
        $scope.vm.state.mediaGallery.showGamePlayClip = false;
        $scope.vm.state.mediaGallery.showYoutubeVideo = false;

        switch (media) {
            case "screenShot":
                $scope.vm.state.mediaGallery.showScreenShot = true;
                break;
            case "gamePlayClip":
                $scope.vm.state.mediaGallery.showGamePlayClip = true;
                break;
            case "youTubeVideo":
                $scope.vm.state.mediaGallery.showYoutubeVideo = true;
                break;
        };
    };

    var resetActiveScreenShot = function ($scope) {
        angular.forEach($scope.vm.object.gameDetailsVM.Details.screen_shots, function (screenShot) {
            screenShot.active = false;
        });
    };

    var showGamePlayClip = function ($scope) {
        resetActiveScreenShot($scope);
        setMediaGalleryType($scope, "gamePlayClip");
    };

    var showYoutubeVideo = function ($scope) {
        resetActiveScreenShot($scope);
        setMediaGalleryType($scope, "youTubeVideo");
    };

    var showScreenShot = function ($scope, screenShot) {
        setMediaGalleryType($scope, "screenShot");
        resetActiveScreenShot($scope);
        screenShot.active = true;
        $scope.vm.state.mediaGallery.screenShotUrl = screenShot.full_url;
    };

    return {
        startUp: startUp,
        showScreenShot: showScreenShot,
        showGamePlayClip: showGamePlayClip,
        showYoutubeVideo: showYoutubeVideo
    };
}]);

app.controller('SideBarController', ['$scope', 'sideBarService', '$rootScope', function ($scope, sideBarService, $rootScope) {

    $scope.globalVm = {};
    $scope.vm = {
        state: {
            pages: {
                myGames: false,
                browse: false,
                reccomendedGames: false
            },
            loading: false,
            show: true,
            filters: {
                genreId: ""
            }
        },
        object: {
            sidebarVM: {}
        }
    };


    function startUp() {
        sideBarService.startUp($scope);
    }

    $scope.toggleFilters = function (filter) {
        sideBarService.toggleFilter($scope, filter);
    };

    $scope.searchGames = function () {
        sideBarService.searchGames($scope);
    };

    $rootScope.$on('$routeChangeSuccess', function () {
        sideBarService.handleRouteChange($scope);
        //sideBarService.clearSearch($scope);
    });

    startUp();
}]);

app.factory('sideBarService', ['stateService', '$location', '$routeParams', 'dataService', function (stateService, $location, $routeParams, dataService) {

    var startUp = function ($scope) {
        $scope.globalVm = stateService.getGlobalVm();
        dataService.loadSidebarViewModel().then(function (data) {
            $scope.vm.object.sidebarVM = data;
        });
    };

    var handleRouteChange = function ($scope) {
        resetNav($scope);
        var paths = $location.path().split("/");
        var page = paths[1];

        switch (page) {
            case "browse":
            case "details":
                $scope.vm.state.pages.browse = true;
                break;
            default:
                $scope.vm.state.pages.myGames = true
                break;
        }

        $scope.vm.state.filters.genreId = $routeParams.genreId;
    }

    function resetNav($scope) {
        $scope.vm.state.pages.browse = false;
        $scope.vm.state.pages.myGames = false;
    }

    var searchGames = function ($scope) {
        if ($scope.vm.object.sidebarVM.Search.Input.length >= 3) {
            $scope.vm.object.sidebarVM.Action = "Load Search Results";
            $scope.vm.state.loading = true;
            dataService.updateSideBarViewModel($scope.vm.object.sidebarVM).then(function (data) {
                $scope.vm.state.loading = false;
                $scope.vm.object.sidebarVM = data;
            });
        };
    };

    return {
        startUp: startUp,
        searchGames: searchGames,
        handleRouteChange: handleRouteChange
    };
}]);

app.controller('BrowseController', ['$scope', 'browseService', function ($scope, browseService) {

    $scope.globalVm = {};
    $scope.vm = {
        object: {
            browseVM: {}
        },
        state: {
            loading: false,
            currentPage: 1,
            gamesToBrowse: 0
        },
        settings: {
            gamesPerPage: 12
        },
        input: {
            filterText: ""
        }
    };

    function startUp() {
        browseService.startUp($scope);
        browseService.loadViewModel($scope, $scope.globalVm.gamer);
    }

    $scope.showPreview = function (game) {
        browseService.showPreview($scope, game);
    };

    $scope.hidePreview = function (game) {
        browseService.hidePreview(game);
    };

    $scope.showYoutube = function (game) {
        browseService.showModal($scope, 'youtube', game);
    };

    startUp();
}]);

app.factory('browseService', ['dataService', 'stateService', '$uibModal', '$routeParams', function (dataService, stateService, $uibModal, $routeParams) {

    var startUp = function ($scope) {
        $scope.globalVm = stateService.getGlobalVm();
        $scope.vm.input.filterText = $routeParams.genre;
    };

    var showPreview = function ($scope, game) {
        hidePreviews($scope);
        game.show_preview = true;
        game.show_header = false;
    };

    var hidePreview = function (game) {
        game.show_preview = false;
        game.show_header = true;
    };

    var hidePreviews = function ($scope) {
        angular.forEach($scope.vm.object.browseVM.Games, function (game) {
            game.show_preview = false;
            game.show_header = true;
        });
    };

    var loadViewModel = function ($scope, gamer) {
        var genreId = $routeParams.genreId === "" ? "All" : $routeParams.genreId;
        $scope.vm.state.loading = true;
        dataService.loadBrowseViewModel(gamer.id, genreId).then(function (data) {
            $scope.vm.state.loading = false;
            $scope.vm.object.browseVM = data;
            $scope.vm.state.gamesToBrowse = $scope.vm.object.browseVM.Games.length;
        });
    };

    var showModal = function ($scope, modal, game) {
        $uibModal.open({
            animation: true,
            backdrop: 'static',
            templateUrl: 'views/modals/' + modal + '.html',
            controller: 'ModalInstanceController',
            resolve: {
                gameData: function () {
                    return {
                        youtubeId: game.videos[0].youtube_id,
                        name: game.name
                    };
                }
            },
            size: 'lg'
        });
    };

    return ({
        startUp: startUp,
        showPreview: showPreview,
        hidePreview: hidePreview,
        loadViewModel: loadViewModel,
        showModal: showModal
    });
}]);

app.controller('HomeController', ['$scope', 'homeService', function ($scope, homeService) {

    $scope.globalVm = {};
    $scope.vm = {
        settings: {
            gamesPerPage: 12
        },
        object: {
            homeVM: {}
        },
        state: {
            loading: false,
            ratingPending: false,
            currentPage: 1,
            librarySize: 0
        }
    };

    function startUp() {
        homeService.startUp($scope);
    }

    $scope.rateGame = function (gamerGame) {
        homeService.rateGame($scope, gamerGame);
    };

    startUp();
}]);

app.factory('homeService', ['dataService', 'stateService', 'messageService', function (dataService, stateService, messageService) {

    var startUp = function ($scope) {
        $scope.globalVm = stateService.getGlobalVm();
        loadHomeViewModel($scope);
    };

    var loadHomeViewModel = function ($scope) {
        $scope.vm.state.loading = true;
        dataService.loadHomeViewModel($scope.globalVm.gamer.id).
        then(function (data) {
            $scope.vm.state.loading = false;
            $scope.vm.object.homeVM = data;
            $scope.vm.state.librarySize = $scope.vm.object.homeVM.gamer_games.length;
        });
    };

    var rateGame = function ($scope, gamerGame) {
        $scope.vm.ratingPending = true;

        dataService.rateGame($scope.globalVm.gamer, gamerGame).then(function () {
            $scope.vm.ratingPending = false;
            messageService.showTimedMessage($scope, "success", "Game Rating Saved", 2000);
        });
    };

    return ({
        startUp: startUp,
        rateGame: rateGame
    });

}]);

app.controller('ModalInstanceController', ['$scope', '$uibModalInstance', 'gameData', 'modalService', function ($scope, $uibModalInstance, gameData, modalService) {

    $scope.close = function () {
        $uibModalInstance.close();
    };

    $scope.showVideo = function (video) {
        modalService.showVideo($scope, video);
    };

    function start() {
        modalService.start($scope);
        $scope.vm.gameData = gameData;
        modalService.loadMoreGameplayVideos($scope, gameData.name, 4);
    }

    start();
}]);

app.factory('modalService', ['dataService', '$window', function (dataService, $window) {

    var showVideo = function ($scope, video) {
        $scope.vm.gameData.youtubeId = video.id;
    };

    var loadMoreGameplayVideos = function ($scope, gameTitle, number) {
        $scope.vm.loading = true;
        dataService.loadMoreGamePlayVideos(gameTitle, number).then(function (data) {
            $scope.vm.videos = data;
            $scope.vm.videos.shift();
            $scope.vm.loading = false;
        });
    };

    var start = function ($scope) {
        $scope.vm = {
            gameData: {},
            videos: {},
            loading: false,
            baseUrl: $window.baseUrl
        };
    };

    return {
        loadMoreGameplayVideos: loadMoreGameplayVideos,
        start: start,
        showVideo: showVideo
    };
}]);

app.factory('dataService', ['$http', '$window', function ($http, $window) {

    var apiBaseUrl = $window.apiBaseUrl;

    var loadSidebarViewModel = function () {
        return $http.get(apiBaseUrl + "SideBar/GetViewModel").then(function (response) {
            return response.data;
        });
    };

    var updateSideBarViewModel = function (sideBarVM) {
        return $http.post(apiBaseUrl + "SideBar/UpdateViewModel", sideBarVM).then(function (response) {
            return response.data;
        });
    };

    var loadHomeViewModel = function (gamerId) {
        return $http.get(apiBaseUrl + "Home/GetHomeViewModel/" + gamerId).then(function (response) {
            return response.data;
        });
    };

    var loadBrowseViewModel = function (gamerId, genreId) {
        return $http.get(apiBaseUrl + "Browse/GetViewModel/" + gamerId + "/" + genreId).then(function (response) {
            return response.data;
        });
    };

    var rateGame = function (gamer, gamerGame) {
        return $http.get(apiBaseUrl + "rateGamerGame/" + gamer.id + "/" + gamerGame.id + "/" + gamerGame.rating).then(function (response) {
            return response.data;
        });
    };

    var loadMoreGamePlayVideos = function (gameTitle, number) {
        return $http.get(apiBaseUrl + "loadMoreGameplayVideos/" + gameTitle + "/" + number).then(function (response) {
            return response.data;
        });
    };

    var searchGames = function (search) {
        return $http.get(apiBaseUrl + "searchGames/" + search).then(function (response) {
            return response.data;
        });
    };

    var loadRecommendedGames = function (gamer) {
        return $http.get(apiBaseUrl + "getRecommendedGames/" + gamer.id).then(function (response) {
            return response.data;
        });
    };

    var loadGameDetailsViewModel = function (gameId, gamerId) {
        return $http.get(apiBaseUrl + "GameDetails/GetViewModel/" + gameId + "/" + gamerId).then(function (response) {
            return response.data;
        });
    };

    return ({
        rateGame: rateGame,
        loadMoreGamePlayVideos: loadMoreGamePlayVideos,
        searchGames: searchGames,
        loadRecommendedGames: loadRecommendedGames,
        loadHomeViewModel: loadHomeViewModel,
        loadSidebarViewModel: loadSidebarViewModel,
        loadBrowseViewModel: loadBrowseViewModel,
        loadGameDetailsViewModel: loadGameDetailsViewModel,
        updateSideBarViewModel: updateSideBarViewModel
    });

}]);

app.factory('messageService', ['$timeout', '$sce', function ($timeout, $sce) {

    var showTimedMessage = function ($scope, messageType, message, delay) {
        var messageText = createMessageText(messageType, message);

        $scope.vm.alertMessage = $sce.trustAsHtml(messageText);

        $timeout(function () {
            $scope.vm.alertMessage = "";
        }, delay);
    };

    function createMessageText(messageType, message) {
        var messageText = "";

        if (messageType === "success") {
            messageText = '<i class="fa fa-check" aria-hidden="true"></i> <strong>' + message + '</strong>';
        }

        return messageText;
    }

    return ({
        showTimedMessage: showTimedMessage
    });

}]);

app.factory('stateService', ['$window', function ($window) {

    var globalVm = {
        steamId: "76561197974127223",
        gamer: $window.gamer
    };

    var getGlobalVm = function () {
        return globalVm;
    };

    return ({
        getGlobalVm: getGlobalVm
    });
}]);
app.filter('readableTime', function () {

    var conversions = {
        'ss': angular.identity,
        'mm': function (value) {
            return value * 60;
        },
        'hh': function (value) {
            return value * 3600;
        }
    };

    var padding = function (value, length) {
        var zeroes = length - ('' + (value)).length,
            pad = '';
        while (zeroes-- > 0)
            pad += '0';
        return pad + value;
    };

    return function (value, unit, format, isPadded) {
        var totalSeconds = conversions[unit || 'ss'](value),
            hh = Math.floor(totalSeconds / 3600),
            mm = Math.floor((totalSeconds % 3600) / 60),
            ss = totalSeconds % 60;

        format = format || 'hh:mm:ss';
        isPadded = angular.isDefined(isPadded) ? isPadded : true;
        hh = isPadded ? padding(hh, 2) : hh;
        mm = isPadded ? padding(mm, 2) : mm;
        ss = isPadded ? padding(ss, 2) : ss;

        return format.replace(/hh/, hh).replace(/mm/, mm).replace(/ss/, ss);
    };
});
app.filter('escape', function () {
    return window.encodeURIComponent;
});
app.directive('youtubeEmbed', function ($sce) {
    return {
        restrict: 'EA',
        scope: {
            code: '='
        },
        replace: false,
        template: '<div style="height:400px;"><iframe style="overflow:hidden;height:100%;width:100%" width="100%" height="100%" src="{{url}}" frameborder="0" allowfullscreen></iframe></div>',
        link: function (scope) {
            scope.$watch('code', function (newVal) {
                if (newVal) {
                    scope.url = $sce.trustAsResourceUrl("http://www.youtube.com/embed/" + newVal + "?iv_load_policy=3");
                }
            });
        }
    };
});

app.directive('gamerKinVideo', function ($sce) {
    return {
        restrict: 'EA',
        scope: {
            gameId: '='
        },
        replace: false,
        template: '',
        link: function (scope, el, attrs) {
            scope.$watch('gameId', function (newVal) {
                if (newVal && newVal !== "") {
                    var display = '<video class="game-preview-video" autoplay loop controls preload="none"> <source src="' + "http://127.0.0.1/edsa-assets/mp4/" + newVal + ".mp4" + '" type="video/mp4" /></video>'
                    el.html(display);
                }
            });
        }
    };
});

app.directive('steamRating', function () {
    var directive = {};

    directive.restrict = 'EA';
    directive.scope = {
        rating: '='
    };
    directive.replace = true;

    directive.link = function (scope, el, attrs) {
        scope.$watch('rating', function (newValue) {
            if (newValue !== "" && newValue) {
                var labelClass = 'success';

                switch (newValue) {
                    case 'Mixed':
                        labelClass = 'default';
                        break;
                    case 'Negative':
                    case 'Mostly Negative':
                    case 'Very Negative':
                    case 'Overwhelmingly Negative':
                        labelClass = 'danger';
                        break;
                }

                var display = '<label class="label label-' + labelClass + ' game-sentiment">' + newValue + '</label>';

                el.html(display);
            }
        });
    };

    return directive;
});

app.directive('displayPlatforms', function () {
    var directive = {};

    directive.restrict = 'EA';
    directive.scope = {
        platforms: '=',
        verbose: '@'
    };

    directive.link = function ($scope, el, attrs) {

        $scope.$watch('platforms', function (newValue) {
            if (newValue && newValue !== "") {
                var display = "";
                var displayText = "";

                for (i = 0; i < $scope.platforms.length; i++) {
                    if ($scope.verbose === "true") {
                        displayText = $scope.platforms[i].name;
                    }
                    switch ($scope.platforms[i].name) {
                        case "Windows":

                            display += '<span style="margin-right:10px;"><i class="fa fa-windows"></i> ' + displayText + '</span>';
                            break;
                        case "Linux":
                            display += '<span style="margin-right:10px;"><i class="fa fa-linux"></i> ' + displayText + '</span>';
                            break;
                        case "Mac":
                            display += '<span style="margin-right:10px;"><i class="fa fa-apple"></i> ' + displayText + '</span>';
                            break;
                    }
                }

                el.html(display);
            }
        });
    };

    return directive;
});

app.directive('escKey', function () {
    return function (scope, element, attrs) {
        element.bind('keydown keypress', function (event) {
            if (event.which === 27) { // 27 = esc key
                scope.$apply(function () {
                    scope.$eval(attrs.escKey);
                });

                event.preventDefault();
            }
        });
    };
});
app.filter('htmlToPlaintext', function () {
    return function (text) {
        return text ? String(text).replace(/<[^>]+>/gm, '') : '';
    };
});
app.filter('toTrusted', ['$sce', function ($sce) {
    return function (text) {
        return $sce.trustAsHtml(text);
    };
}]);
angular.module('truncate', []).filter('characters', function () {
    return function (input, chars, breakOnWord) {
        if (isNaN(chars)) return input;
        if (chars <= 0) return '';
        if (input && input.length > chars) {
            input = input.substring(0, chars);

            if (!breakOnWord) {
                var lastspace = input.lastIndexOf(' ');
                //get last space
                if (lastspace !== -1) {
                    input = input.substr(0, lastspace);
                }
            } else {
                while (input.charAt(input.length - 1) === ' ') {
                    input = input.substr(0, input.length - 1);
                }
            }
            return input + '…';
        }
        return input;
    };
}).filter('splitcharacters', function () {
    return function (input, chars) {
        if (isNaN(chars)) return input;
        if (chars <= 0) return '';
        if (input && input.length > chars) {
            var prefix = input.substring(0, chars / 2);
            var postfix = input.substring(input.length - chars / 2, input.length);
            return prefix + '...' + postfix;
        }
        return input;
    };
}).filter('words', function () {
    return function (input, words) {
        if (isNaN(words)) return input;
        if (words <= 0) return '';
        if (input) {
            var inputWords = input.split(/\s+/);
            if (inputWords.length > words) {
                input = inputWords.slice(0, words).join(' ') + '…';
            }
        }
        return input;
    };
});