using Monitoreo.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Monitoreo
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            //routes.MapRoute(
            //    name: "Mine",
            //    url: "{culture}/{controller}/{id}/{action}",
            //    defaults: new {culture = CultureHelper.GetDefaultCulture(), controller = "Home", action = "Index", id = UrlParameter.Optional },
            //    constraints: new RouteValueDictionary(new { action = "Index|Details|Edit|Insert" })
            //);

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new {controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
