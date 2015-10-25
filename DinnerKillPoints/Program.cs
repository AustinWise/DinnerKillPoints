using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Austin.DkpLib;
using System.Data.SqlClient;
using System.Net;
using System.Configuration;

namespace DinnerKillPoints
{
    //Future GUI features:
    // * Let someone pay off another person's debt.
    // * A nice way to split a bill easily.


    //split app idea:
    //pass phone around, select who you are, then check all the items that are yours
    //the next person sees that much fewer items

    class Program
    {
        static DkpDataContext db;

        static void Main(string[] args)
        {
            var cs = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["Austin.DkpLib.Properties.Settings.DKPConnectionString"].ConnectionString);
            var ips = Dns.GetHostAddresses(cs.DataSource);
            Console.Write("Please enter the password: ");
            cs.Password = Console.ReadLine();
            db = new DkpDataContext(cs.ToString());


            var austin = GetPerson(1);
            var caspar = GetPerson(2);
            var wesley = GetPerson(3);
            //var maria = GetPerson(4);
            var david = GetPerson(5);
            //var seanMc = GetPerson(6);
            //var andrea = GetPerson(7);
            //var meredith = GetPerson(8);
            var seanChen = GetPerson(9);
            var arata = GetPerson(10);
            var jeff = GetPerson(11);
            var ryuho = GetPerson(12);
            //var laura = GetPerson(13);
            var minh = GetPerson(14);
            var george = GetPerson(15);
            var justine = GetPerson(16);
            var matt = GetPerson(17);
            var elaine = GetPerson(18);
            var roger = GetPerson(19);
            var eric = GetPerson(20);
            var katherine = GetPerson(21);
            //var ryanSund = GetPerson(22);
            var adrian = GetPerson(23);
            var ed = GetPerson(24);
            var randy = GetPerson(25);
            var becky = GetPerson(26);
            var andrew = GetPerson(29);
            var justinShih = GetPerson(30);
            //var davidFang = GetPerson(31);
            var derek = GetPerson(32);
            //var elaineJeu = GetPerson(33);
            var jimmy = GetPerson(34);
            var alex = GetPerson(35);
            var victor = GetPerson(36);
            var tiffany = GetPerson(37);
            var moriaki = GetPerson(38);
            var adam = GetPerson(39);
            var benKwong = GetPerson(40);
            //var jasonBanich = GetPerson(41);
            var changLiu = GetPerson(42);
            var mike = GetPerson(43);
            //var spencer = GetPerson(44);
            //var jessie = GetPerson(45);
            var pat = GetPerson(46);
            var cam = GetPerson(47);
            var zack = GetPerson(48);
            var mary = GetPerson(49);
            var monica = GetPerson(50);
            var ryanWeinstein = GetPerson(51);
            var michelle = GetPerson(52);


            //Transaction t;
            //t = new Transaction()
            //{
            //    ID = Guid.NewGuid(),
            //    Debtor = roger, //owes money
            //    Creditor = austin, //owed money
            //    Amount = 11525,
            //    BillID = null,
            //    Description = "Repayment",
            //    Created = new DateTime(2015, 10, 22, 0, 20, 0),
            //};
            //db.Transactions.InsertOnSubmit(t);
            //db.SubmitChanges();

            //CheckForDupes();


            var ran = new Random();

            WriteData(true, db.People.Where(p => !p.IsDeleted).OrderBy(p => ran.Next()).ToArray());
            WriteData(false, db.People.ToArray());

            db.Dispose();
        }

        private static void CheckForDupes()
        {
            foreach (var trans in db.Transactions
                .Where(t => t.BillID == null && !t.Description.StartsWith(Transaction.DebtTransferString))
                .Where(t => t.Description != "Repayment")
                .GroupBy(t => t.Amount)
                .Where(ts => ts.Count() > 1))
            {
                Console.WriteLine(trans.Key);
                foreach (var newGroup in trans.GroupBy(t => t.Description).Where(ts => ts.Count() > 1))
                {
                    Console.WriteLine(newGroup.Key);
                    foreach (var t in newGroup)
                    {
                        Console.WriteLine("\t{0,20}{1,20}", t.Debtor, t.Creditor);
                    }
                }
            }
        }

        private static void WriteData(bool removeCycles, Person[] people)
        {
            const string outDir = @"D:\AustinWise\Dropbox\DKP";

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

        private static void DebtTransfer(Person debtor, Person oldCreditor, Person newCreditor)
        {
            DebtTransfer(debtor, oldCreditor, newCreditor, DateTime.Now);
        }

        private static void DebtTransfer(Person debtor, Person oldCreditor, Person newCreditor, DateTime when)
        {
            var netMoney = DebtGraph.TestAlgo(db, new[] { debtor, oldCreditor }, false, null);
            if (netMoney.Count != 1)
                throw new Exception("No debt to transfer.");

            var theDebt = netMoney[0];

            if (theDebt.Debtor.ID != debtor.ID || theDebt.Creditor.ID != oldCreditor.ID)
                throw new Exception("Debt does not go in the expected direction.");

            var msg = Transaction.CreateDebtTransferString(debtor, oldCreditor, newCreditor);

            var bs = new BillSplit();
            bs.Name = msg;
            db.BillSplits.InsertOnSubmit(bs);

            var cancelTrans = new Transaction()
            {
                ID = Guid.NewGuid(),
                DebtorID = oldCreditor.ID, //owes money
                CreditorID = debtor.ID, //owed money
                Amount = theDebt.Amount,
                BillSplit = bs,
                Description = msg,
                Created = when
            };
            db.Transactions.InsertOnSubmit(cancelTrans);

            var makeCreditorWholeTransaction = new Transaction()
            {
                ID = Guid.NewGuid(),
                DebtorID = newCreditor.ID, //owes money
                CreditorID = oldCreditor.ID, //owed money
                Amount = theDebt.Amount,
                BillSplit = bs,
                Description = msg,
                Created = when
            };
            db.Transactions.InsertOnSubmit(makeCreditorWholeTransaction);

            var makeDebtorOweNewPartyTrans = new Transaction()
            {
                ID = Guid.NewGuid(),
                DebtorID = debtor.ID, //owes money
                CreditorID = newCreditor.ID, //owed money
                Amount = theDebt.Amount,
                BillSplit = bs,
                Description = msg,
                Created = when
            };
            db.Transactions.InsertOnSubmit(makeDebtorOweNewPartyTrans);

            db.SubmitChanges();
        }
    }
}
