using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Austin.DkpLib;
using System.IO;
using DkpWeb.Models;

namespace DkpWeb.Controllers
{
    public class AnalyseController : Controller
    {
        DkpDataContext dc;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            dc = new DkpDataContext();
            base.Initialize(requestContext);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                dc.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult Index()
        {
            return View(dc.ActivePeopleOrderedByName);
        }

        public ActionResult Display(int[] peopleIds)
        {
            var people = peopleIds
                .Select(i => dc.People.Where(p => p.ID == i).Single())
                .ToArray();
            var swLog = new StringWriter();
            var netMoney = DebtGraph.TestAlgo(dc, people, true, swLog);
            
            var swGraph = new StringWriter();
            DebtGraph.WriteGraph(netMoney, swGraph);
            var bytes = DebtGraph.RenderGraphAsPng(swGraph.ToString());

            var mod = new AnalyseModel();
            mod.LogOutput = swLog.ToString();
            mod.ImageBase64 = Convert.ToBase64String(bytes);
            mod.Debtors = DebtGraph.GreatestDebtor(netMoney);
            return View(mod);
        }
    }
}
