﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Austin.DkpLib;

namespace DinnerKillPoints
{
    //Future GUI features:
    // * Let someone pay off another person's debt.
    // * A nice way to split a bill easily.




    class Program
    {
        static DkpDataContext db;

        static List<Tuple<int, int>> DebtFloaters = new List<Tuple<int, int>>();
        private static void AddDebtFloater(Person p1, Person p2)
        {
            DebtFloaters.Add(new Tuple<int, int>(p1.ID, p2.ID));
            DebtFloaters.Add(new Tuple<int, int>(p2.ID, p1.ID));
        }

        static void Main(string[] args)
        {
            db = new DkpDataContext();

            var austin = GetPerson(1);
            var caspar = GetPerson(2);
            var wesley = GetPerson(3);
            var maria = GetPerson(4);
            var david = GetPerson(5);
            var seanMc = GetPerson(6);
            var andrea = GetPerson(7);
            var meredith = GetPerson(8);
            var seanChen = GetPerson(9);
            var arata = GetPerson(10);
            var jeff = GetPerson(11);
            var ryuho = GetPerson(12);
            var laura = GetPerson(13);
            var minh = GetPerson(14);
            var george = GetPerson(15);
            var justine = GetPerson(16);
            var matt = GetPerson(17);
            var elaine = GetPerson(18);
            var roger = GetPerson(19);
            var eric = GetPerson(20);
            var katherine = GetPerson(21);
            var ryanSund = GetPerson(22);
            var adrian = GetPerson(23);
            var ed = GetPerson(24);
            var randy = GetPerson(25);

            //AddDebtFloater(wesley, maria);
            AddDebtFloater(seanMc, meredith);
            AddDebtFloater(caspar, justine);

            var bs = new BillSpliter("Cascal", new DateTime(2012, 12, 29, 21, 6, 53), caspar);
            bs.SharedFood = 1200 + 1200 + 850 + 900 + 1200 + 2600;
            bs[wesley] = 750 + 850;
            bs[austin] = 900 + 850;
            bs[david] = 950;
            bs[caspar] = 900;
            bs[wesley] = 595;
            bs.Tax = 1152;
            bs.Tip = 2253;
            //bs.Save(db);

            var t = new Transaction()
            {
                ID = Guid.NewGuid(),
                Debtor = caspar, //owes money
                Creditor = arata, //owed money
                Amount = 250,
                BillID = null,
                Description = "Repayment",
                Created = DateTime.Now
            };
            //db.Transactions.InsertOnSubmit(t);
            //db.SubmitChanges();

            var ran = new Random();

            WriteData(true, db.People.Where(p => !p.IsDeleted).OrderBy(p => ran.Next()).Where(p => p != laura).ToArray());
            WriteData(false, db.People.Where(p => !p.IsDeleted).ToArray());

            db.Dispose();
        }

        private static void WriteData(bool removeCycles, Person[] people)
        {
            const string outDir = @"C:\Users\AustinWise\Dropbox\DKP\";

            List<Debt> netMoney = null;
            netMoney = DebtGraph.TestAlgo(db, people, removeCycles, removeCycles ? new StreamWriter(Path.Combine(outDir, "Info.txt")) : Console.Out);
            Console.WriteLine("{0:c}", netMoney.Sum(m => m.Amount) / 100d);

            const string gvPath = @"c:\temp\graph\test.gv";
            using (var sw = new StreamWriter(gvPath))
            {
                DebtGraph.WriteGraph(netMoney, sw);
            }

            DebtGraph.RenderGraphAsPng(gvPath, Path.Combine(outDir, removeCycles ? "current.png" : "nocycles.png"));
            if (removeCycles)
                DebtGraph.RenderGraphAsPng(gvPath, Path.Combine(outDir, DateTime.Now.ToString("yyyy-MM-dd") + ".png"));
        }

        private static Person GetPerson(int i)
        {
            var austin = db.People.Where(p => p.ID == i).Single();
            return austin;
        }
    }
}
