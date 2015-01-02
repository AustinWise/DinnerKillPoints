using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Austin.DkpLib;
using DkpWeb.Models;

namespace DkpWeb.Controllers
{
    public class PeopleController : Controller
    {
        private DkpDataContext mData = new DkpDataContext();

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
                mData.Dispose();
        }

        //
        // GET: /People/

        public ActionResult Index()
        {
            return View(mData.ActivePeopleOrderedByName.ToArray());
        }

        public ActionResult All()
        {
            return View("Index", mData.People
                .OrderBy(p => p.FirstName)
                .ThenBy(p => p.LastName)
                .ToArray());
        }

        public ActionResult Detail(int id)
        {
            return View(mData.People.Where(p => p.ID == id).Single());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult DeletePaymentIdentity(int id)
        {
            var ident = mData.PaymentIdentities.Single(i => i.Id == id);
            mData.PaymentIdentities.DeleteOnSubmit(ident);
            mData.SubmitChanges();
            return RedirectToAction("Detail", new { id = ident.PersonID });
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AddPaymentIdentity(int id)
        {
            ViewBag.Methods = mData.PaymentMethods;
            ViewBag.Person = mData.People.Single(p => p.ID == id);
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddPaymentIdentity(int id, AddPaymentMethodModel model)
        {
            var person = mData.People.Single(p => p.ID == id);

            if (ModelState.IsValid)
            {
                var ident = new PaymentIdentity()
                {
                    PaymentMethod = mData.PaymentMethods.Single(m => m.Id == model.PaymentMethodId),
                    Person = person,
                    UserName = model.UserName.Trim(),
                };
                mData.PaymentIdentities.InsertOnSubmit(ident);
                mData.SubmitChanges();
                return RedirectToAction("Detail", new { id = id });
            }

            ViewBag.Methods = mData.PaymentMethods;
            ViewBag.Person = person;
            return View();
        }
    }
}
