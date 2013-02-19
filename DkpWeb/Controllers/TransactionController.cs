using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Austin.DkpLib;

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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            mData.Dispose();
        }
    }
}
