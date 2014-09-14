using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Austin.DkpLib;

namespace DkpWeb.Controllers
{
    public class PeopleController : Controller
    {
        //
        // GET: /People/

        public ActionResult Index()
        {
            using (var dc = new DkpDataContext())
            {
                return View(dc.ActivePeopleOrderedByName.ToArray());
            }
        }

        public ActionResult All()
        {
            using (var dc = new DkpDataContext())
            {
                return View("Index", dc.People
                    .OrderBy(p => p.FirstName)
                    .ThenBy(p => p.LastName)
                    .ToArray());
            }
        }

        public ActionResult Detail(int id)
        {
            Person person;
            using (var dc = new DkpDataContext())
            {
                person = dc.People.Where(p => p.ID == id).Single();
            }
            return View(person);
        }
    }
}
