using Monitoreo.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Monitoreo.Controllers
{
    public class HomeController : BaseController
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

         [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        [Authorize]
        public ActionResult SetCulture(string culture)
        {
            // Validate input
            //culture = CultureHelper.GetImplementedCulture(culture);

            // Save culture in a cookie
            HttpCookie cookie = Request.Cookies["_culture"];
            if (cookie != null)
                cookie.Value = culture;   // update cookie value
            else
            {
                cookie = new HttpCookie("_culture");
                cookie.Value = culture;
                cookie.Expires = DateTime.Now.AddYears(1);
            }

            Response.Cookies.Add(cookie);

            // Limpiando los SiteMaps localizados
            MvcSiteMapProvider.SiteMaps.ReleaseSiteMap();

            //return RedirectToAction("Index");
            return Redirect(HttpContext.Request.UrlReferrer.AbsoluteUri);
        }  
    }
}