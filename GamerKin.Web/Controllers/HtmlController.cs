using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GamerKin.Web.Controllers
{
    public class HtmlController : Controller
    {
        public ActionResult Home()
        {
            return View();
        }
    }
}