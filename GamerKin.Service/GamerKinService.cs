using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GamerKin.Models.Steam;
using Newtonsoft.Json;
using GamerKin.ViewModels;
using AutoMapper;
using GamerKin.Service.MappingProfiles;
using GamerKin;

namespace GamerKin
{
    public class GamerKinService
    {

        gamerkinEntities _context = new gamerkinEntities();

        public GamerKinService()
        {
            Mapper.Initialize(mapperConfiguration => mapperConfiguration.AddProfile(new GamerKinProfile()));
        }

        public GameDetailsViewModel GetGameDetailsViewModel(int gameId, int gamerId)
        {
            var viewModel = new GameDetailsViewModel();
            viewModel.Details = GetGameDetails(gameId);

            return viewModel;
        }

        public Game GetGameDetails(int gameId)
        {
            var gameEntity = _context.games.Find(gameId);

            var game = new Game();

            Mapper.Map(gameEntity, game);

            return game;
        }

        public SideBarViewModel UpdateSideBarViewModel(SideBarViewModel vm)
        {
            return vm;
        }

        public SideBarViewModel GetSideBarViewModel()
        {
            var vm = new SideBarViewModel();
            vm.Genres = GetTopGenres();
            AddDefaultGenre(vm);
            return vm;
        }

        private void AddDefaultGenre(SideBarViewModel vm)
        {
            vm.Genres.Insert(0, new Genre() { id = 0, name = "All" });
        }
        public BrowseViewModel GetBrowseViewModel(int gamerId, int genreId)
        {
            var viewModel = new BrowseViewModel();
            viewModel.Games = GetGamesByGenreForGamer(gamerId, genreId);
            return viewModel;
        }

        private List<Game> GetGamesByGenreForGamer(int gamerId, int genreId)
        {
            var gamerGames = GetGamerGames(gamerId);
            var gameIds = gamerGames.Select(x => x.game_id).ToList();

            var query = _context.games
                .Where(x => x.type == "Game")
                .Where(x => !gameIds.Contains(x.id));

            if (genreId != 0)
            {
                query = query.Where(x => x.game_genre.Any(gg => gg.genre_id == genreId && gg.rank <= 3));
            }

            query = query.OrderByDescending(x => x.steam_user_rating_value);

            var gameEntities = query.Take(100).ToList();

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<game, Game>();
            });

            var games = gameEntities
                .Select(x => Mapper.Map<Game>(x))
                .ToList();

            return games;
        }

        private List<Genre> GetTopGenres()
        {
            var topGenreIds = _context.game_genre
                .GroupBy(x => x.genre_id)
                .Select(g => new GameGenre()
                {
                    id = g.Key,
                    gameCount = g.Count()
                })
                .ToList().OrderByDescending(x => x.gameCount)
                .Take(20).Select(s => s.id).ToList();

            var genreEntities = _context.genres
                .Where(x => topGenreIds.Contains(x.id))
                .ToList();

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<genre, Genre>();
            });

            var genres = genreEntities.Select(x =>
                Mapper.Map<Genre>(x)
            ).ToList();

            return genres;
        }

        private Gamer GetGamerDto(int id)
        {
            var entity = _context.gamers
                .Where(x => x.id == id)
                .FirstOrDefault();

            var gamer = Mapper.Map<Gamer>(entity);

            return gamer;
        }

        private gamer GetGamerById(int id)
        {
            var gamer = _context.gamers
                .Where(x => x.id == id)
                .FirstOrDefault();

            return gamer;
        }

        public gamer LoadGamer(int id)
        {
            var gamer = GetGamerById(id);
            //Check to see if the gamer's library needs refreshed
            if (GamerGamesNeedRefresh(gamer))
            {
                RefreshGamerGames(gamer);
            }

            return gamer;
        }

        private bool RefreshGamerGames(gamer gamer)
        {
            var gamesFromSteam = GetGamerSteamLibrary(gamer);
            var gamerGames = GetGamerGameList(gamer.id);

            AddNewGamerGames(gamer, gamesFromSteam, gamerGames);
            UpdateExistingGamerGames(gamer, gamesFromSteam, gamerGames);
            UpdateGamerUpdatedAt(gamer);

            return true;
        }

        private void UpdateGamerUpdatedAt(gamer gamer)
        {
            gamer.updated_at = DateTime.Now;
            _context.SaveChanges();
        }

        private void UpdateExistingGamerGames(gamer gamer, PlayerLibraryResponse steamGames, List<gamer_games> gamerGames)
        {
            foreach (var gamerGame in gamerGames)
            {
                gamerGame.time_played = steamGames.games.Where(x => x.appid == gamerGame.game.steam_id).FirstOrDefault().playtime_forever;
            }

            _context.SaveChanges();
        }

        private void AddNewGamerGames(gamer gamer, PlayerLibraryResponse steamGames, List<gamer_games> gamerGames)
        {
            var newGames = steamGames.games.Where(x => !gamerGames.Any(y => y.game.steam_id == x.appid))
                .ToList();

            var newGameAppIds = newGames.Select(x => x.appid).ToList();

            var games = _context.games.Where(x => newGameAppIds.Contains(x.steam_id))
                .ToList();

            foreach (var game in games)
            {
                _context.gamer_games.Add(new gamer_games
                {
                    gamer_id = gamer.id,
                    game_id = game.id,
                    time_played = newGames.Where(x => x.appid == game.steam_id).FirstOrDefault().playtime_forever,
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                });
            }

            _context.SaveChanges();
        }

        private bool GamerGamesNeedRefresh(gamer gamer)
        {
            bool requiresRefresh = DateTime.Today - gamer.updated_at.Date >= TimeSpan.FromHours(8);
            return requiresRefresh;
        }

        public List<gamer_games> GetGamerGameList(int gamerId)
        {
            var gamerGames = _context.gamer_games
                .Where(x => x.gamer_id == gamerId)
                .Include(x => x.game)
                .OrderByDescending(x => x.time_played)
                .ToList();

            return gamerGames;
        }

        public List<GamerGame> GetGamerGames(int gamerId)
        {

            var entities = _context.gamer_games
                .Where(x => x.gamer_id == gamerId)
                .Include(x => x.game)
                .OrderByDescending(x => x.time_played)
                .ToList();

            var gamerGames = Mapper.Map<List<GamerGame>>(entities);

            return gamerGames;
        }

        public HomeViewModel GetHomeViewModel(int gamerId)
        {
            HomeViewModel model = new HomeViewModel();

            model.gamer = GetGamerDto(gamerId);
            model.gamer_games = GetGamerGames(gamerId);

            return model;
        }

        public List<game> GetRecommendedGamesByGamerId(int gamerId)
        {
            var games = _context.sp_get_recommended_games(gamerId, 7, 75)
                .ToList();

            return games;
        }

        public bool RateGamerGame(int gamerId, int gamerGameId, int rating)
        {
            var game = _context.gamer_games
                .Where(x => x.gamer_id == gamerId && x.game_id == gamerGameId)
                .FirstOrDefault();

            if (game != null)
            {
                game.rating = rating;
                _context.SaveChanges();
            }
            else
            {
                return false;
            }
            return true;
        }

        public bool CreateGamer(long steamId)
        {
            var created = false;

            if (!GamerExists(steamId))
            {
                var summary = GetSteamPlayerSummary(steamId);
                if (summary.players != null)
                {
                    var gamer = SummaryToGamer(summary);
                    _context.gamers.Add(gamer);
                    _context.SaveChanges();
                    created = true;
                }
            }

            return created;
        }

        private gamer SummaryToGamer(PlayerSummaryResponse summary)
        {
            var gamer = new gamer();
            gamer.steam_id = summary.players[0].steamid;
            gamer.avatar = summary.players[0].avatar;
            gamer.avatar_medium = summary.players[0].avatarmedium;
            gamer.name = summary.players[0].personaname;
            gamer.created_at = DateTime.Now;
            gamer.updated_at = DateTime.Now;
            return gamer;
        }

        public bool GamerExists(long steamId)
        {
            var gamerCount = _context.gamers
                .Count(x => x.steam_id == steamId);

            if (gamerCount >= 1)
            {
                return true;
            }

            return false;
        }

        public bool RefreshGamerSteamLibrary(gamer gamer)
        {
            GetGamerSteamLibrary(gamer);
            return true;
        }

        #region STEAM_FUNCTIONS

        string _steamApiKey = "8F451AE3DC1F264AF6BADC449E9D1419";
        string _steamApiBaseUrl = "http://api.steampowered.com/";

        private PlayerSummaryResponse GetSteamPlayerSummary(long steamId)
        {
            var url = GetSteamPlayerSummaryUrl(steamId);
            var client = new WebClient();
            var data = client.DownloadString(url);
            var apiResponse = JsonConvert.DeserializeObject<ApiPlayerSummaryResponse>(data);

            return apiResponse.response;
        }

        private string GetSteamPlayerSummaryUrl(long steamId)
        {
            var url = _steamApiBaseUrl + "/ISteamUser" + "/GetPlayerSummaries" + "/v0002" + "/?key=" + _steamApiKey + "&steamids=" + steamId;
            return url;
        }


        private PlayerLibraryResponse GetGamerSteamLibrary(gamer gamer)
        {
            var url = GetSteamLibaryUrl(gamer.steam_id);
            var client = new WebClient();
            var data = client.DownloadString(url);
            var apiResponse = JsonConvert.DeserializeObject<ApiPlayerLibraryResponse>(data);

            return apiResponse.response;
        }

        private string GetSteamLibaryUrl(long steamId)
        {
            var url = _steamApiBaseUrl + "/IPlayerService" + "/GetOwnedGames" + "/v0001" + "/?key="
                + _steamApiKey + "&steamid=" + steamId + "&format=json&include_played_free_games=1";

            return url;
        }
        #endregion
    }
}
