using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Austin.DkpLib;

namespace DkpWeb.Controllers
{
    public class AnalyseController : Controller
    {
        //
        // GET: /Analyse/

        public ActionResult Index()
        {
            Person[] people;
            using (var dc = new DkpDataContext())
            {
                people = dc.People.ToArray();
            }
            return View(people);
        }

        public ActionResult Display()
        {
            return View();
        }
    }
}
