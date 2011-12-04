using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DinnerKillPoints
{
    class Person
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    class Debt
    {
        public int Amount { get; set; }
        public Person Creditor { get; set; }
        public Person Debtor { get; set; }
    }

    class Transaction : Debt
    {
        public string Desc { get; set; }
        public DateTime When { get; set; }
    }

    class Pair
    {
        public Pair(Person p1, Person p2)
        {
            if (p1 == null)
                throw new ArgumentNullException();
            if (p2 == null)
                throw new ArgumentNullException();
            P1 = p1;
            P2 = p2;
        }

        public Person P1 { get; private set; }
        public Person P2 { get; private set; }

        public override bool Equals(object obj)
        {
            Pair other = obj as Pair;
            if (other == null)
                return false;

            return (other.P1 == this.P1 && other.P2 == this.P2) || (other.P2 == this.P1 && other.P1 == this.P2);
        }

        public override int GetHashCode()
        {
            return P1.GetHashCode() ^ P2.GetHashCode();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Person austin, david, caspar, wesley, maria;
            var people = new List<Person>();
            people.Add(austin = new Person() { Name = "Austin" });
            people.Add(david = new Person() { Name = "David" });
            people.Add(caspar = new Person() { Name = "Caspar" });
            people.Add(wesley = new Person() { Name = "Wesley" });
            people.Add(maria = new Person() { Name = "Maria" });

            var register = new List<Transaction>();
            register.Add(new Transaction() { Amount = 4450, Debtor = wesley, Creditor = austin, Desc = "Limon", When = new DateTime(2011, 11, 1) });
            register.Add(new Transaction() { Amount = 550, Debtor = austin, Creditor = caspar, Desc = "Limon", When = new DateTime(2011, 11, 1) });
            register.Add(new Transaction() { Amount = 6100, Debtor = david, Creditor = austin, Desc = "Limon", When = new DateTime(2011, 11, 1) });
            register.Add(new Transaction() { Amount = 3450, Debtor = maria, Creditor = austin, Desc = "Limon", When = new DateTime(2011, 11, 1) });

            register.Add(new Transaction() { Amount = 3000, Debtor = wesley, Creditor = david, Desc = "Fiesta", When = new DateTime(2011, 12, 1) });
            register.Add(new Transaction() { Amount = 2300, Debtor = caspar, Creditor = david, Desc = "Fiesta", When = new DateTime(2011, 12, 1) });
            register.Add(new Transaction() { Amount = 2800, Debtor = austin, Creditor = david, Desc = "Fiesta", When = new DateTime(2011, 12, 1) });

            //fake data follows
            //register.Add(new Transaction() { Amount = 6666, Debtor = david, Creditor = austin, Desc = "Fiesta", When = new DateTime(2011, 12, 15) });

            //sum all debts from one person to another
            var summedDebts = new Dictionary<Tuple<Person, Person>, int>();
            foreach (var t in register)
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
            for (int i = 0; i < people.Count - 1; i++)
            {
                var p1 = people[i];
                for (int j = i + 1; j < people.Count; j++)
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
        }
    }
}
