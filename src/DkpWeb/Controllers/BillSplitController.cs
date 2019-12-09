using System;
using System.Collections.Generic;
using System.Linq;
using Austin.DkpLib;
using DkpWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace DkpWeb.Controllers
{
    public class BillSplitController : Controller
    {
        readonly ApplicationDbContext mData;

        public BillSplitController(ApplicationDbContext data)
        {
            this.mData = data;
        }

        //
        // GET: /BillSplit/

        public ActionResult Index()
        {
            var bs = mData.BillSplit.Include(b => b.Transaction).OrderByDescending(b => b.Transaction.Select(t => t.Created).FirstOrDefault()).ToList();
            var pMap = mData.Person.ToDictionary(p => p.Id);
            foreach (var b in bs)
            {
                b.SetPrettyDescription(pMap);
            }
            return View(bs);
        }

        //
        // GET: /BillSplit/Details/5

        public ActionResult Details(int id)
        {
            var bs = mData.BillSplit.Where(b => b.Id == id).Include(b => b.Transaction).Single();
            var pMap = mData.Person.ToDictionary(p => p.Id);
            bs.SetPrettyDescription(pMap);
            return View(bs);
        }

        //
        // POST: /BillSplit/Delete/5

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var bs = mData.BillSplit.Where(b => b.Id == id).Single();
            mData.BillSplit.Remove(bs);
            mData.SaveChanges();

            return RedirectToAction("Index");
        }

        //
        // GET: /BillSpit/Add
        // Just a shell to contain Blazor pages

        [HttpGet]
        [Authorize(Roles = "DKP")]
        public ActionResult Add()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
                mData.Dispose();
        }
    }
}
