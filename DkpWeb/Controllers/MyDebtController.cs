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
        private DkpDataContext mData = new DkpDataContext();

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
                mData.Dispose();
        }

        //
        // GET: /MyDebt/

        public ActionResult Index()
        {
            return View(mData.ActivePeopleOrderedByName.ToArray());
        }

        //
        // GET: /MyDebt/Details/5

        public ActionResult Details(int id)
        {
            var person = mData.People.Where(p => p.ID == id).Single();

            var transactions = mData.Transactions
                .Where(t => t.CreditorID != t.DebtorID
                    && (t.CreditorID == person.ID || t.DebtorID == person.ID)
                    && (!t.Creditor.IsDeleted && !t.Debtor.IsDeleted))
                .ToList();

            var netMoney = DebtGraph.TestAlgo(mData, transactions, true, TextWriter.Null);

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
            mod.ImageBase64 = Convert.ToBase64String(bytes);
            mod.Creditors = creditors;
            mod.OverallDebt = (myDebt == null) ? 0 : myDebt.Item2;

            return View(mod);
        }

        //
        // GET: /MyDebt/DebtHistory/5/6

        public ActionResult DebtHistory(int debtorId, int creditorId)
        {
            var debtor = mData.People.Where(p => p.ID == debtorId).Single();
            var creditor = mData.People.Where(p => p.ID == creditorId).Single();

            var trans = mData.Transactions.Where(t =>
                   (t.CreditorID == debtor.ID && t.DebtorID == creditor.ID)
                || (t.CreditorID == creditor.ID && t.DebtorID == debtor.ID))
                .OrderBy(t => t.Created);

            var personMap = mData.People.ToDictionary(p => p.ID);
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
            ret.AmountCents = runningTotal;
            ret.AmountDollars = runningTotal / 100d;

            return View(ret);
        }
    }
}
