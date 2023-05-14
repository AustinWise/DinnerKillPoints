using DkpWeb.Data;
using DkpWeb.Models;
using DkpWeb.Models.PeopleViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DkpWeb.Controllers
{
    [Authorize(Roles = "DKP")]
    public class PeopleController : Controller
    {
        private ApplicationDbContext mData;

        public PeopleController(ApplicationDbContext data)
        {
            this.mData = data;
        }

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
            return View("Index", mData.Person
                .OrderBy(p => p.FirstName)
                .ThenBy(p => p.LastName)
                .ToArray());
        }

        public ActionResult Detail(int id)
        {
            return View(mData.Person.Where(p => p.Id == id).Include(p => p.PaymentIdentity).ThenInclude(p => p.PaymentMeth).Single());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult DeletePaymentIdentity(int id)
        {
            var ident = mData.PaymentIdentity.Single(i => i.Id == id);
            mData.PaymentIdentity.Remove(ident);
            mData.SaveChanges();
            return RedirectToAction("Detail", new { id = ident.PersonId });
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AddPaymentIdentity(int id)
        {
            ViewBag.Methods = mData.PaymentMethod;
            ViewBag.Person = mData.Person.Single(p => p.Id == id);
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddPaymentIdentity(int id, AddPaymentMethodModel model)
        {
            var person = mData.Person.Single(p => p.Id == id);

            if (ModelState.IsValid)
            {
                var ident = new PaymentIdentity()
                {
                    PaymentMeth = mData.PaymentMethod.Single(m => m.Id == model.PaymentMethodId),
                    Person = person,
                    UserName = model.UserName.Trim(),
                };
                mData.PaymentIdentity.Add(ident);
                mData.SaveChanges();
                return RedirectToAction("Detail", new { id = id });
            }

            ViewBag.Methods = mData.PaymentMethod;
            ViewBag.Person = person;
            return View();
        }
    }
}
