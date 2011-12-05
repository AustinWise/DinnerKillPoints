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

    class Program
    {
        static void Main(string[] args)
        {
            var db = new DkpDataContext();
            var people = db.People.ToArray();

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
            foreach (var d in netMoney)
            {
                Console.WriteLine("\t{0} -> {1}: {2}", d.Debtor, d.Creditor, d.Amount);
            }
            Console.WriteLine();

            Console.WriteLine("Greatest Debtors");
            foreach (var tup in people.Select(p => new { Debtor = p, Amount = netMoney.Where(d => d.Debtor == p).Sum(d => d.Amount) }).OrderByDescending(obj => obj.Amount))
            {
                Console.WriteLine("\t{0}: {1}", tup.Debtor, tup.Amount);
            }
            Console.WriteLine();


            db.Dispose();
        }
    }
}
