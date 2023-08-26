using DkpWeb.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DkpWeb.Controllers
{
    [Authorize(Roles = "DKP")]
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
        // GET: /BillSplit/Add

        public ActionResult Add()
        {
            return View();
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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
                mData.Dispose();
        }
    }
}
