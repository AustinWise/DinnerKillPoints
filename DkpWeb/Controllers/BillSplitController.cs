using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Austin.DkpLib;

namespace DkpWeb.Controllers
{
    public class BillSplitController : Controller
    {
        private DkpDataContext mData = new DkpDataContext();

        //
        // GET: /BillSplit/

        public ActionResult Index()
        {
            return View(mData.BillSplits.OrderByDescending(bs => bs.ID).ToList());
        }

        //
        // GET: /BillSplit/Details/5

        public ActionResult Details(int id)
        {
            return View(mData.BillSplits.Where(bs => bs.ID == id).Single());
        }

        //
        // POST: /BillSplit/Delete/5

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var bs = mData.BillSplits.Where(b => b.ID == id).Single();
                mData.BillSplits.DeleteOnSubmit(bs);
                mData.SubmitChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
                mData.Dispose();
        }
    }
}
