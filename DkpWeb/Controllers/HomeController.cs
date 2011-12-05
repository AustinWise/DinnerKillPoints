using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DkpWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to Dinner Kill Points";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
