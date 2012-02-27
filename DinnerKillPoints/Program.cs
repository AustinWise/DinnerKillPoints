using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DinnerKillPoints
{
    //Future GUI features:
    // * Let someone pay off another person's debt.
    // * A nice way to split a bill easily.

    class Debt
    {
        public int Amount { get; set; }
        public Person Creditor { get; set; }
        public Person Debtor { get; set; }
    }

    class BillSpliter
    {
        private string mName;
        private DateTime mDate;
        private Person mPayer;

        public BillSpliter(string name, DateTime date, Person payer)
        {
            this.mName = name;
            this.mDate = date;
            this.mPayer = payer;
            this.Party = new Dictionary<Person, int>();
        }

        public Dictionary<Person, int> Party { get; private set; }
        public int Tax { get; set; }
        public int Tip { get; set; }
        public int SharedFood { get; set; }

        public void Save(DkpDataContext db)
        {
            var pool = Party.Sum(p => p.Value) + SharedFood;
            var bs = new BillSplit() { Name = mName };
            db.BillSplits.InsertOnSubmit(bs);

            foreach (var p in Party.Keys)
            {
                double personSubtotal = SharedFood;
                personSubtotal /= Party.Count;
                personSubtotal += Party[p];
                var ratio = (double)personSubtotal / pool;
                //Console.WriteLine(ratio);
                var total = personSubtotal + ratio * (Tax + Tip);
                //Console.WriteLine(p.FirstName + ": " + total + " (" + Math.Round(total) + ")");

                if (p == mPayer)
                    continue;

                var t = new Transaction()
                {
                    ID = Guid.NewGuid(),
                    Creditor = mPayer,
                    Debtor = p,
                    Amount = (int)Math.Round(total),
                    BillSplit = bs,
                    Description = mName,
                    Created = mDate,
                };
                db.Transactions.InsertOnSubmit(t);
            }

            db.SubmitChanges();
        }
    }

    class Program
    {
        static DkpDataContext db;
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

            var bs = new BillSpliter("Tied House", new DateTime(2012, 2, 19), maria);
            bs.Party[austin] = 1130 + 700 + 500;
            bs.Party[david] = 1335;
            bs.Party[maria] = 1335;
            bs.Tax = (int)Math.Round(bs.Party.Sum(p => p.Value) * 0.0875);
            bs.Tip = (int)Math.Round(0.15 * (bs.Party.Sum(d => d.Value) + bs.Tip));
            //bs.Save(db);

            //var t = new Transaction()
            //{
            //    ID = Guid.NewGuid(),
            //    Debtor = austin,
            //    Creditor = andrea,
            //    Amount = 1630,
            //    BillID = null,
            //    Description = "Repayment",
            //    Created = DateTime.Now,
            //};
            //db.Transactions.InsertOnSubmit(t);
            //db.SubmitChanges();

            var people = new Person[] { austin, caspar, wesley, david, maria };
            //var people = db.People.ToArray();

            TestAlgo(db, people);

            db.Dispose();
        }

        private static Person GetPerson(int i)
        {
            var austin = db.People.Where(p => p.ID == i).Single();
            return austin;
        }

        private static void TestAlgo(DkpDataContext db, Person[] people)
        {
            Console.WriteLine("Raw tranactions:");
            foreach (var t in db.Transactions.OrderBy(t => t.Created))
            {
                Console.WriteLine("\t{0} -> {1}: {2}", t.Debtor, t.Creditor, t.Amount);
            }
            Console.WriteLine();

            //sum all debts from one person to another
            var summedDebts = new Dictionary<Tuple<Person, Person>, int>();
            foreach (var t in db.Transactions)
            {
                var tup = new Tuple<Person, Person>(t.Debtor, t.Creditor);
                int net = 0;
                if (summedDebts.ContainsKey(tup))
                    net = summedDebts[tup];
                net += t.Amount;
                summedDebts[tup] = net;
            }

            Console.WriteLine("Combined debts:");
            foreach (var n in summedDebts)
            {
                Console.WriteLine("\t{0} -> {1}: {2}", n.Key.Item1, n.Key.Item2, n.Value);
            }
            Console.WriteLine();

            //net up all the debts
            //what's this, O(2n) ?
            var netMoney = new List<Debt>();
            for (int i = 0; i < people.Length - 1; i++)
            {
                var p1 = people[i];
                for (int j = i + 1; j < people.Length; j++)
                {
                    var p2 = people[j];
                    var net = 0;
                    int temp = 0;
                    if (summedDebts.TryGetValue(new Tuple<Person, Person>(p1, p2), out temp))
                    {
                        net = temp;
                    }
                    if (summedDebts.TryGetValue(new Tuple<Person, Person>(p2, p1), out temp))
                    {
                        net -= temp;
                    }
                    if (net > 0)
                        netMoney.Add(new Debt() { Debtor = p1, Creditor = p2, Amount = net });
                    else if (net < 0)
                        netMoney.Add(new Debt() { Debtor = p2, Creditor = p1, Amount = -net });
                }
            }

            Console.WriteLine("Net:");
            foreach (var d in netMoney.OrderBy(n => n.Debtor.FirstName))
            {
                Console.WriteLine("\t{0} -> {1}: {2:c}", d.Debtor, d.Creditor, d.Amount / 100d);
            }
            Console.WriteLine();

            Console.WriteLine("Greatest Debtors");
            foreach (var tup in people.Select(p => new { Debtor = p, Amount = netMoney.Where(d => d.Debtor == p).Sum(d => d.Amount) }).OrderByDescending(obj => obj.Amount))
            {
                Console.WriteLine("\t{0}: {1:c}", tup.Debtor, tup.Amount / 100d);
            }
            Console.WriteLine();
        }
    }
}
