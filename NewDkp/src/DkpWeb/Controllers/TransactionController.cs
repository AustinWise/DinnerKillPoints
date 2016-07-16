using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DkpWeb.Data;
using DkpWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Sakura.AspNetCore;
using Microsoft.EntityFrameworkCore;

namespace DkpWeb.Controllers
{
    public class TransactionController : Controller
    {
        //
        // GET: /Transaction/
        public TransactionController(ApplicationDbContext data)
        {
            this.mData = data;
        }

        private ApplicationDbContext mData;

        public ActionResult Index(int? page)
        {
            var totalSize = mData.Transaction.Count();
            var currentPage = page ?? 1;
            var pageSize = TransactionList.PageSize;

            if (currentPage < 1)
                return this.NotFound();

            var trans = mData.Transaction.OrderByDescending(t => t.Created).ToPagedList(pageSize, currentPage);
            var personMap = mData.Person.ToDictionary(p => p.Id);
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
            var trans = mData.Transaction.Include(t => t.Creditor).Include(t => t.Debtor).Where(t => t.Id == id).Single();
            return View(trans);
        }

        public ActionResult TopScore()
        {
            var peopleMap = mData.Person.ToDictionary(p => p.Id);

            var q = mData.Transaction
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
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Guid id)
        {
            var trans = mData.Transaction.Where(t => t.Id == id).Single();
            mData.Transaction.Remove(trans);
            await mData.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "DKP")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(NewTransactionModel model)
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
                    debtor = mData.Person.Where(p => p.Id == model.Debtor).SingleOrDefault();
                    creditor = mData.Person.Where(p => p.Id == model.Creditor).SingleOrDefault();
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
                    trans.CreditorId = creditor.Id;
                    trans.DebtorId = debtor.Id;
                    trans.Description = model.Description;
                    trans.Amount = (int)Math.Round(model.Amount * 100d);
                    trans.Created = DateTime.UtcNow;
                    trans.Id = Guid.NewGuid();
                    mData.Transaction.Add(trans);
                    await mData.SaveChangesAsync();
                    return RedirectToAction("Index", "Transaction");
                }
            }

            ViewBag.People = mData.ActivePeopleOrderedByName.ToList();
            return View(model);
        }
    }
}