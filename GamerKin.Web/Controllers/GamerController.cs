using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace GamerKin
{
    [RoutePrefix("api/Gamer")]
    public class GamerController : Controller
    {
        GamerKinService _service = new GamerKinService();

        [Route("Get/{id}")]
        [HttpGet]
        public ActionResult Get(int id)
        {
            var gamer = _service.LoadGamer(id);
            return Json(gamer, JsonRequestBehavior.AllowGet);
        }

        [Route("GetRecommendedGames/{id}")]
        [HttpGet]
        public ActionResult GetRecommendedGames(int id)
        {
            var games = _service.GetRecommendedGamesByGamerId(id);
            return Json(games, JsonRequestBehavior.AllowGet);
        }

        [Route("CreateGamer/{steamId}")]
        [HttpGet]
        public ActionResult CreateGamer(long steamId)
        {
            var result = _service.CreateGamer(steamId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}