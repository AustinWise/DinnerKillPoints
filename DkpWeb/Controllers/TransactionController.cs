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

        public ActionResult Index(int? page)
        {
            var totalSize = mData.Transactions.Count();
            var currentPage = (page ?? 1) - 1; //convert to 0-based
            var pageSize = TransactionList.PageSize;

            if (currentPage < 0 || currentPage * TransactionList.PageSize > totalSize)
                return this.HttpNotFound();

            var q = mData.Transactions.OrderByDescending(t => t.Created).Skip(pageSize * currentPage).Take(pageSize);
            var trans = q.ToList();
            var personMap = mData.People.ToDictionary(p => p.ID);
            foreach (var t in trans)
            {
                t.SetPrettyDescription(personMap);
            }

            //convert back to 1-based indexing for view
            var model = new TransactionList(currentPage + 1, totalSize, trans);

            return View(model);
        }

        public ActionResult View(Guid id)
        {
            return View(mData.Transactions.Where(t => t.ID == id).Single());
        }

        public ActionResult TopScore()
        {
            var peopleMap = mData.People.ToDictionary(p => p.ID);

            var q = mData.Transactions
                .Where(t => !t.Description.StartsWith(Transaction.DebtTransferString))
                .GroupBy(t => t.Description)
                .Select(g => new TopScoreEntry { Name = g.Key, Amount = g.Sum(t => t.Amount) })
                .OrderByDescending(g => g.Amount)
                .ToList();

            foreach (var t in q)
            {
                t.Name = Transaction.CreatePrettyDescription(t.Name, t.Amount, peopleMap) ?? t.Name;
            }

            return View(q);
        }

        [Authorize(Roles = "DKP")]
        public ActionResult Add()
        {
            ViewBag.People = mData.ActivePeopleOrderedByName.ToList();
            return View(new NewTransactionModel());
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
                    trans.Created = DateTime.UtcNow;
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
