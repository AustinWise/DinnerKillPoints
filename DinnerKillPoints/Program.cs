using System;
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
        static bool RemoveCycles = false;
        static DkpDataContext db;

        static List<Tuple<Person, Person>> DebtFloaters = new List<Tuple<Person, Person>>();
        private static void AddDebtFloater(Person p1, Person p2)
        {
            DebtFloaters.Add(new Tuple<Person, Person>(p1, p2));
            DebtFloaters.Add(new Tuple<Person, Person>(p2, p1));
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

            AddDebtFloater(wesley, maria);
            AddDebtFloater(seanMc, meredith);

            var bs = new BillSpliter("Parking", DateTime.Now, austin);
            bs.SharedFood = 1050;
            bs.Party[austin] = 0;
            bs.Party[arata] = 0;
            bs.Party[wesley] = 0;
            bs.Party[maria] = 0;
            //bs.Tax = 1086;
            //bs.Tip = 2100;
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
            //var people = new Person[] { austin, caspar, wesley, david, maria };
            var people = db.People.ToArray();
            people = people.OrderBy(p => ran.Next()).ToArray();

            DebtGraph.TestAlgo(db, people, DebtFloaters, RemoveCycles, @"C:\temp\graph\test.gv");

            db.Dispose();
        }

        private static Person GetPerson(int i)
        {
            var austin = db.People.Where(p => p.ID == i).Single();
            return austin;
        }
    }
}
