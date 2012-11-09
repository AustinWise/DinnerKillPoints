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
            Person[] people;
            using (var dc = new DkpDataContext())
            {
                people = dc.People.Where(p => !p.IsDeleted).ToArray();
            }
            return View(people);
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
