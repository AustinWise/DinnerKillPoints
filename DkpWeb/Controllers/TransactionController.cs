using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Austin.DkpLib;
using DkpWeb.Models;

namespace DkpWeb.Controllers
{
    public class TransactionController : Controller
    {
        //
        // GET: /Transaction/

        private DkpDataContext mData = new DkpDataContext();

        public ActionResult Index()
        {
            return View(mData.Transactions.OrderByDescending(t => t.Created));
        }

        public ActionResult View(Guid id)
        {
            return View(mData.Transactions.Where(t => t.ID == id).Single());
        }

        [Authorize(Roles = "DKP")]
        public ActionResult Add()
        {
            ViewBag.People = mData.ActivePeopleOrderedByName.ToList();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(Guid id)
        {
            var trans = mData.Transactions.Where(t => t.ID == id).Single();
            mData.Transactions.DeleteOnSubmit(trans);
            mData.SubmitChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "DKP")]
        public ActionResult Add(NewTransactionModel model)
        {
            if (ModelState.IsValid)
            {
                Person debtor = null, creditor = null;
                if (model.Creditor == model.Debtor)
                {
                    ModelState.AddModelError("", "The debtor and creditor must not be the same person.");
                }
                else
                {
                    debtor = mData.People.Where(p => p.ID == model.Debtor).SingleOrDefault();
                    creditor = mData.People.Where(p => p.ID == model.Creditor).SingleOrDefault();
                    if (debtor == null)
                    {
                        ModelState.AddModelError("Debtor", "Invalid debtor.");
                    }
                    if (creditor == null)
                    {
                        ModelState.AddModelError("Creditor", "Invalid creditor.");
                    }
                }


                if (creditor != null && debtor != null)
                {
                    var trans = new Transaction();
                    trans.CreditorID = creditor.ID;
                    trans.DebtorID = debtor.ID;
                    trans.Description = model.Description;
                    trans.Amount = (int)Math.Round(model.Amount * 100d);
                    trans.Created = DateTime.Now;
                    trans.ID = Guid.NewGuid();
                    mData.Transactions.InsertOnSubmit(trans);
                    mData.SubmitChanges();
                    return RedirectToAction("Index", "Transaction");
                }
            }

            ViewBag.People = mData.ActivePeopleOrderedByName.ToList();
            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
                mData.Dispose();
        }
    }
}
