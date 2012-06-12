using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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

            TestAlgo(db, people);

            db.Dispose();
        }

        private static Person GetPerson(int i)
        {
            var austin = db.People.Where(p => p.ID == i).Single();
            return austin;
        }

        private static void replace(Dictionary<Tuple<Person, Person>, int> summedDebts, Tuple<Person, Person> oldKey, Tuple<Person, Person> newKey)
        {
            var val = summedDebts[oldKey];
            summedDebts.Remove(oldKey);
            if (summedDebts.ContainsKey(newKey))
                val += summedDebts[newKey];
            summedDebts[newKey] = val;
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

            if (RemoveCycles)
            {
                //combine debt floaters
                while (DebtFloaters.Count != 0)
                {
                    var tup = DebtFloaters[0];
                    var mainPerson = tup.Item1;
                    var removedPerson = tup.Item2;

                    if (summedDebts.ContainsKey(DebtFloaters[0]))
                        summedDebts.Remove(DebtFloaters[0]);
                    if (summedDebts.ContainsKey(DebtFloaters[1]))
                        summedDebts.Remove(DebtFloaters[1]);

                    foreach (var t in summedDebts.Keys.ToArray())
                    {
                        if (t.Item1 == removedPerson)
                        {
                            replace(summedDebts, t, new Tuple<Person, Person>(mainPerson, t.Item2));
                        }
                        else if (t.Item2 == removedPerson)
                        {
                            replace(summedDebts, t, new Tuple<Person, Person>(t.Item1, mainPerson));
                        }
                    }


                    mainPerson.FirstName = mainPerson.FirstName + " " + mainPerson.LastName + ", ";
                    mainPerson.LastName = removedPerson.FirstName + " " + removedPerson.LastName;
                    removedPerson.FirstName += "<removed>";
                    DebtFloaters.RemoveAt(0);
                    DebtFloaters.RemoveAt(0);
                }
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

            if (RemoveCycles)
            {
                Console.WriteLine("Cycles");
                bool foundCycle;
                do
                {
                    foundCycle = false;
                    foreach (var cycle in FindCycles(people, netMoney))
                    {
                        if (cycle.Count != 1)
                            foundCycle = true;
                        Console.WriteLine("->");
                        foreach (var p in cycle)
                        {
                            Console.WriteLine("\t{0}", p.FirstName);
                        }

                        var cycleTrans = new List<Debt>();
                        for (int i = 0; i < cycle.Count; i++)
                        {
                            var p1 = cycle[(i - 1 + cycle.Count) % cycle.Count];
                            var p2 = cycle[i];
                            Console.WriteLine("\t\t{0} -> {1}", p1, p2);
                            var debt = netMoney.Where(d => d.Debtor == p1 && d.Creditor == p2).Single();
                            cycleTrans.Add(debt);
                        }

                        var subAmount = cycleTrans.Select(c => c.Amount).Min();
                        foreach (var d in cycleTrans)
                        {
                            d.Amount -= subAmount;
                        }
                    }

                    //remove 0-value debts
                    foreach (var d in netMoney.Where(d => d.Amount == 0).ToList())
                    {
                        netMoney.Remove(d);
                    }
                }
                while (foundCycle);
                Console.WriteLine();
            }

            Console.WriteLine("New net:");
            foreach (var d in netMoney.OrderBy(n => n.Debtor.FirstName))
            {
                Console.WriteLine("\t{0} -> {1}: {2:c}", d.Debtor, d.Creditor, d.Amount / 100d);
            }
            Console.WriteLine();

            Console.WriteLine("Greatest Debtors");
            foreach (var tup in people.Select(p => new { Debtor = p, Amount = netMoney.Where(d => d.Debtor == p && !(DebtFloaters.Contains(new Tuple<Person, Person>(p, d.Creditor)))).Sum(d => d.Amount) }).OrderByDescending(obj => obj.Amount))
            {
                Console.WriteLine("\t{0}: {1:c}", tup.Debtor, tup.Amount / 100d);
            }
            Console.WriteLine();


            Console.WriteLine("Graph");
            using (var sw = new StreamWriter(@"C:\temp\graph\test.gv"))
            {
                sw.WriteLine("digraph Test {");
                foreach (var d in netMoney)
                {
                    sw.WriteLine("\"{0} {1}\" -> \"{2} {3}\" [label=\"{4:c}\"];", d.Debtor.FirstName, d.Debtor.LastName, d.Creditor.FirstName, d.Creditor.LastName, d.Amount / 100d);
                }
                sw.WriteLine("}");
            }
            Console.WriteLine();
        }

        private static List<List<Person>> FindCycles(Person[] people, List<Debt> debts)
        {
            foreach (var p in people)
            {
                p.PrepareForCycleTesting();
            }

            var ret = new List<List<Person>>();

            foreach (var p in people)
            {
                var S = new Stack<Person>();
                CycleDfs(p, people, debts, S, ret);
            }

            return ret;
        }

        private static void CycleDfs(Person v, Person[] verts, List<Debt> edges, Stack<Person> S, List<List<Person>> cycles)
        {
            if (v.Visited == true)
                return;

            v.Visited = true;
            S.Push(v);

            // Consider successors of v
            foreach (var e in edges.Where(e => e.Debtor == v))
            {
                if (S.Contains(e.Creditor))
                {
                    var ret = new List<Person>();
                    foreach (var p in S)
                    {
                        ret.Add(p);
                        if (p == e.Creditor)
                            break;
                    }
                    ret.Reverse();
                    cycles.Add(ret);
                }
                CycleDfs(e.Creditor, verts, edges, S, cycles);
            }

            S.Pop();
        }
    }
}
