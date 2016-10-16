using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamerKin.ViewModels;

namespace GamerKin.Service.MappingProfiles
{
    public class GamerKinProfile : Profile
    {
        public GamerKinProfile()
        {
            CreateMap<gamer, Gamer>();

            CreateMap<gamer_games, GamerGame>()
                .ForMember(
                d => d.game,
                opt => opt.MapFrom(src => src.game)
            );

            CreateMap<game_assets, GameScreenShot>();

            CreateMap<game_assets, GameVideo>();

            CreateMap<game_genre, GameGenre>();

            CreateMap<genre, Genre>();

            CreateMap<game_platforms, GamePlatform>();

            CreateMap<game_prices, GamePrices>();

            CreateMap<game, Game>()
            .ForMember(
                d => d.screen_shots,
                opt => opt.MapFrom(src => src.game_assets.Where(x => x.type == "Image").ToList())
            )
            .ForMember(
                d => d.videos,
                opt => opt.MapFrom(src => src.game_assets.Where(x => x.type == "Video").ToList())
            )
            .ForMember(
                d => d.genres,
                opt => opt
                .MapFrom(src => src.game_genre
                .Where(x => x.rank <= 10)
                .OrderByDescending(x => x.rank)
                .Select(x => x.genre))
            )
            .ForMember(
                d => d.platforms,
                opt => opt.MapFrom(src => src.game_platforms)
            )
            .ForMember(
                d => d.prices,
                opt => opt.MapFrom(src => src.game_prices)
            );
        }
    }
}