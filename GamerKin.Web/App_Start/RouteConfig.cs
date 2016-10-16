using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GamerKin.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "Default",
                url: "",
                defaults: new { controller = "Html", action = "Home", id = UrlParameter.Optional }
            );
        }
    }
}
