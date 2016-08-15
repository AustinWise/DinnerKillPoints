using Austin.DkpLib;
using DkpWeb.Data;
using DkpWeb.Models;
using DkpWeb.Models.MyDebtViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DkpWeb.Controllers
{
    public class MyDebtController : Controller
    {
        readonly ApplicationDbContext mData;

        public MyDebtController(ApplicationDbContext data)
        {
            this.mData = data;
        }

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
            var person = mData.Person.Where(p => p.Id == id).Single();

            var transactions = mData.Transaction.Include(t => t.Creditor).Include(t => t.Debtor)
                .Where(t => t.CreditorId != t.DebtorId
                    && (t.CreditorId == person.Id || t.DebtorId == person.Id)
                    && (!t.Creditor.IsDeleted && !t.Debtor.IsDeleted))
                .ToList();

            var netMoney = DebtGraph.TestAlgo(mData, transactions, true, TextWriter.Null);

            var swGraph = new StringWriter();
            DebtGraph.WriteGraph(netMoney, swGraph);
            var svg = DebtGraph.RenderGraphAsSvg(swGraph.ToString());

            var creditors = DebtGraph.GreatestDebtor(netMoney);
            var myDebt = creditors.Where(d => d.Item1.Id == person.Id).SingleOrDefault();
            if (myDebt != null)
                creditors.Remove(myDebt);

            //change the list of debtors in to creditors
            creditors = creditors.Select(c => new Tuple<Person, int>(c.Item1, -c.Item2))
                .Reverse()
                .ToList();

            var mod = new MyDebtModel();
            mod.Person = person;
            mod.ImageSvg = svg;
            mod.Creditors = creditors;
            mod.OverallDebt = (myDebt == null) ? 0 : myDebt.Item2;

            return View(mod);
        }

        //
        // GET: /MyDebt/DebtHistory/5/6

        public ActionResult DebtHistory(int debtorId, int creditorId)
        {
            var debtor = mData.Person.Where(p => p.Id == debtorId).Include(p => p.PaymentIdentity).ThenInclude(i => i.PaymentMeth).Single();
            var creditor = mData.Person.Where(p => p.Id == creditorId).Include(p => p.PaymentIdentity).ThenInclude(i => i.PaymentMeth).Single();

            var trans = mData.Transaction.Where(t =>
                   (t.CreditorId == debtor.Id && t.DebtorId == creditor.Id)
                || (t.CreditorId == creditor.Id && t.DebtorId == debtor.Id))
                .OrderBy(t => t.Created);

            var personMap = mData.Person.ToDictionary(p => p.Id);
            int runningTotal = 0;
            var entries = new List<DebtLedgerEntry>();

            foreach (var t in trans)
            {
                t.SetPrettyDescription(personMap);

                int amount;
                if (t.DebtorId == debtor.Id)
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
            ret.AmountDollars = Math.Abs(runningTotal / 100d).ToString("C");

            return View(ret);
        }
    }
}
