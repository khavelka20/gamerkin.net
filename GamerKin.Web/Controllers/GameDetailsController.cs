using GamerKin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GamerKin.Web.Controllers
{
    [RoutePrefix("api/GameDetails")]
    public class GameDetailsController : ApiController
    {
        GamerKinService _service = new GamerKinService();

        [Route("GetViewModel/{gameId}/{gamerId}")]
        [HttpGet]
        public GameDetailsViewModel GetViewModel(int gameId, int gamerId)
        {
            var viewModel = _service.GetGameDetailsViewModel(gameId, gamerId);
            return viewModel;
        }
    }
}
