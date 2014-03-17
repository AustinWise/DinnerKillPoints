using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Austin.DkpLib;
using DkpWeb.Models;

namespace DkpWeb.Controllers
{
    public class MyDebtController : Controller
    {
        //
        // GET: /MyDebt/

        public ActionResult Index()
        {
            using (var dc = new DkpDataContext())
            {
                return View(dc.ActivePeopleOrderedByName.ToArray());
            }
        }

        //
        // GET: /MyDebt/Details/5

        public ActionResult Details(int id)
        {
            using (var dc = new DkpDataContext())
            {
                var person = dc.People.Where(p => p.ID == id).Single();

                var transactions = dc.Transactions
                    .Where(t => t.CreditorID != t.DebtorID
                        && (t.CreditorID == person.ID || t.DebtorID == person.ID)
                        && (!t.Creditor.IsDeleted && !t.Debtor.IsDeleted))
                    .ToList();

                var swLog = new StringWriter();
                var netMoney = DebtGraph.TestAlgo(dc, transactions, true, swLog);

                var swGraph = new StringWriter();
                DebtGraph.WriteGraph(netMoney, swGraph);
                var bytes = DebtGraph.RenderGraphAsPng(swGraph.ToString());

                var creditors = DebtGraph.GreatestDebtor(netMoney);
                var myDebt = creditors.Where(d => d.Item1.ID == person.ID).SingleOrDefault();
                if (myDebt != null)
                    creditors.Remove(myDebt);

                //change the list of debtors in to creditors
                creditors = creditors.Select(c => new Tuple<Person, int>(c.Item1, -c.Item2))
                    .Reverse()
                    .ToList();

                var mod = new MyDebtModel();
                mod.Person = person;
                mod.LogOutput = swLog.ToString();
                mod.ImageBase64 = Convert.ToBase64String(bytes);
                mod.Creditors = creditors;
                mod.OverallDebt = (myDebt == null) ? 0 : myDebt.Item2;

                return View(mod);
            }
        }

        //
        // GET: /MyDebt/DebtHistory/5/6

        public ActionResult DebtHistory(int debtorId, int creditorId)
        {
            using (var dc = new DkpDataContext())
            {
                var debtor = dc.People.Where(p => p.ID == debtorId).Single();
                var creditor = dc.People.Where(p => p.ID == creditorId).Single();

                var trans = dc.Transactions.Where(t =>
                       (t.CreditorID == debtor.ID && t.DebtorID == creditor.ID)
                    || (t.CreditorID == creditor.ID && t.DebtorID == debtor.ID))
                    .OrderBy(t => t.Created);

                var personMap = dc.People.ToDictionary(p => p.ID);
                int runningTotal = 0;
                var entries = new List<DebtLedgerEntry>();

                foreach (var t in trans)
                {
                    t.SetPrettyDescription(personMap);

                    int amount;
                    if (t.DebtorID == debtor.ID)
                        amount = t.Amount;
                    else
                        amount = -t.Amount;
                    runningTotal += amount;

                    var entry = new DebtLedgerEntry(t, amount, runningTotal);
                    entries.Add(entry);
                }

                var ret = new DebtLedger();
                ret.Debtor = debtor;
                ret.Creditor = creditor;
                ret.Entries = entries;

                return View(ret);
            }
        }
    }
}
