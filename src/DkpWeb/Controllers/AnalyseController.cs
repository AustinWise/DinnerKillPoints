using Austin.DkpLib;
using DkpWeb.Data;
using DkpWeb.Models.AnalyzeViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.IO;
using System.Linq;

namespace DkpWeb.Controllers
{
    [Authorize(Roles = "DKP")]
    public class AnalyseController : Controller
    {
		ApplicationDbContext dc;

		public AnalyseController(ApplicationDbContext db)
		{
			this.dc = db;
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
                .Select(i => dc.Person.Where(p => p.Id == i).Single())
                .ToArray();
            var swLog = new StringWriter();
            var netMoney = DebtGraph.CalculateDebts(dc, people, true, swLog);
            
            var swGraph = new StringWriter();
            DebtGraph.WriteGraphviz(netMoney, swGraph);
            var svg = DebtGraph.RenderGraphAsSvg(swGraph.ToString());

            var mod = new AnalyseModel();
            mod.LogOutput = swLog.ToString();
            mod.ImageSvg = svg;
            mod.Debtors = DebtGraph.GreatestDebtor(netMoney);
            return View(mod);
        }
    }
}
