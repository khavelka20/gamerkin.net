using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GamerKin.Web.Controllers
{
    [RoutePrefix("api/Home")]
    public class HomeController : Controller
    {
        GamerKinService _service = new GamerKinService();

        [Route("GetHomeViewModel/{gamerId}")]
        public ActionResult GetHomeViewModel(int gamerId)
        {
            var result = _service.GetHomeViewModel(gamerId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}