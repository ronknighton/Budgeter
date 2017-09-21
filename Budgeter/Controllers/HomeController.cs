using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Budgeter.Controllers
{
    [RequireHttps]

    public class HomeController : Controller
    {
      
        public ActionResult Index()
        {
            return View();
        }

        [NoDirectAccess]
        public ActionResult ErrorPage()
        {
            return View();
        }

        [NoDirectAccess]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [NoDirectAccess]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}