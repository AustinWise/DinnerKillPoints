using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Austin.DkpLib
{
    public class BillSpliter
    {
        private string mName;
        private DateTime mDate;
        private Person[] mPayer;
        private List<Tuple<Person, double>> mParty;
        private List<Person> mFreeLoaders;

        public BillSpliter(string name, DateTime date, params Person[] payer)
        {
            if (payer == null)
                throw new ArgumentNullException("payer");
            if (payer.Length < 1)
                throw new ArgumentOutOfRangeException("payer", "Need at least one payer.");

            this.mName = name;
            this.mDate = date;
            this.mPayer = payer;
            mParty = new List<Tuple<Person, double>>();
            mFreeLoaders = new List<Person>();
        }

        public double this[Person person]
        {
            get
            {
                return mParty.Where(kvp => kvp.Item1.Equals(person)).Sum(p => p.Item2);
            }
            set
            {
                mParty.Add(new Tuple<Person, double>(person, value));
            }
        }

        public int Tax { get; set; }
        public int Tip { get; set; }
        public int SharedFood { get; set; }
        public int SubTotal
        {
            get
            {
                return Convert.ToInt32(mParty.Sum(p => p.Item2)) + SharedFood;
            }
        }

        public void AddFreeLoader(Person p)
        {
            if (mFreeLoaders.Contains(p))
                throw new ArgumentException("Person is already a free loader.");
            mFreeLoaders.Add(p);
        }

        public void Save(DkpDataContext db)
        {
            if (mParty.Count == 0)
                throw new Exception("Must have one or more people in that party.");

            var pool = mParty.Sum(p => p.Item2) + SharedFood;
            var bs = new BillSplit() { Name = mName };
            db.BillSplits.InsertOnSubmit(bs);

            var amountSpent = new List<Tuple<Person, double>>();

            foreach (var p in mParty)
            {
                double personSubtotal = SharedFood;
                personSubtotal /= mParty.Count;
                personSubtotal += p.Item2;
                var ratio = (double)personSubtotal / pool;
                Console.WriteLine(ratio);
                var total = personSubtotal + ratio * (Tax + Tip);
                Console.WriteLine(p.Item1.FirstName + ": " + total + " (" + Math.Round(total) + ")");

                amountSpent.Add(new Tuple<Person, double>(p.Item1, total));
            }

            var totalSpentPart1 = pool + Tip + Tax;
            var totalSpentPart2 = amountSpent.Select(tup => tup.Item2).Sum();

            if (Math.Abs(totalSpentPart1 - totalSpentPart2) > 0.1)
                throw new Exception("Something terrible has happened.");

            var freeloadersFound = amountSpent.Where(p => mFreeLoaders.Contains(p.Item1)).ToList();
            var nonFreeLoaderCount = amountSpent.Count - freeloadersFound.Count;

            if (freeloadersFound.Count != 0)
            {
                var freeloaderSum = freeloadersFound.Select(p => p.Item2).Sum();
                amountSpent = amountSpent
                    .Where(p => !mFreeLoaders.Contains(p.Item1))
                    .Select(p => new Tuple<Person, double>(p.Item1, p.Item2 + freeloaderSum / nonFreeLoaderCount))
                    .ToList();
                foreach (var p in freeloadersFound)
                {
                    amountSpent.Add(new Tuple<Person, double>(p.Item1, 0));
                }

                var totalSpentPart3 = amountSpent.Sum(p => p.Item2);
                if (Math.Abs(totalSpentPart1 - totalSpentPart3) > 0.1)
                    throw new Exception("Something terrible has happened (part3).");
            }

            var debtsToPayers = SplitDebtsBetweenPayers(amountSpent);
            var totalSpentPart4 = debtsToPayers.SelectMany(p => p.Item2).Sum(p => p.Item2);
            if (Math.Abs(totalSpentPart1 - totalSpentPart4) > 0.1)
                throw new Exception("Something terrible has happened (part4).");

            var pennySplits = SplitPennies(debtsToPayers);
            var totalSpentPart5 = pennySplits.Sum(p => p.Item3);
            if (Math.Abs(totalSpentPart1 - (double)totalSpentPart5) > 0.1)
                throw new Exception("Something terrible has happened (part5).");

            foreach (var p in pennySplits)
            {
                var t = new Transaction()
                {
                    ID = Guid.NewGuid(),
                    Creditor = p.Item2,
                    Debtor = p.Item1,
                    Amount = p.Item3,
                    BillSplit = bs,
                    Description = mName,
                    Created = mDate,
                };
                db.Transactions.InsertOnSubmit(t);
            }

            db.SubmitChanges();
        }

        List<Tuple<Person, List<Tuple<Person, double>>>> SplitDebtsBetweenPayers(List<Tuple<Person, double>> amountSpent)
        {
            var ret = new List<Tuple<Person, List<Tuple<Person, double>>>>();
            foreach (var p in amountSpent)
            {
                var creditors = new List<Tuple<Person, double>>();
                foreach (var payer in mPayer)
                {
                    creditors.Add(new Tuple<Person, double>(payer, p.Item2 / (double)mPayer.Length));
                }
                ret.Add(new Tuple<Person, List<Tuple<Person, double>>>(p.Item1, creditors));
            }
            return ret;
        }

        /// <summary>
        /// Distributes pennies.
        /// </summary>
        /// <param name="amountSpent"></param>
        /// <returns>Item1 owes Item2 Item3 pennies</returns>
        List<Tuple<Person, Person, int>> SplitPennies(List<Tuple<Person, List<Tuple<Person, double>>>> amountSpent)
        {
            var pennies = amountSpent
                .Select(tup => new
                {
                    Debtor = tup.Item1,
                    Debts = tup.Item2.Select(debt => new Tuple<Person, int>(debt.Item1, (int)Math.Floor(debt.Item2))).ToList(),
                    PennyFraction = tup.Item2.Select(debt => debt.Item2 - Math.Floor(debt.Item2)).Sum()
                })
                .OrderByDescending(p => p.PennyFraction)
                .ToList();
            var tempDoublePennyCount = pennies.Sum(p => p.PennyFraction);

            if (Math.Abs(tempDoublePennyCount - Math.Round(tempDoublePennyCount)) > 0.01)
                throw new Exception("Non-int number of pennies.");

            var totalPennies = (int)Math.Round(tempDoublePennyCount);

            if (totalPennies < 0)
                throw new Exception("Negitive number of pennies.");


            int payerNdx = 0;
            while (totalPennies != 0)
            {
                foreach (var d in pennies)
                {
                    if (totalPennies == 0)
                        break;
                    if (mFreeLoaders.Contains(d.Debtor))
                        continue;
                    var ndx = payerNdx++ % d.Debts.Count;
                    var tup = d.Debts[ndx];
                    d.Debts[ndx] = new Tuple<Person, int>(tup.Item1, tup.Item2 + 1);
                    totalPennies--;
                }
            }

            return pennies.SelectMany(p => p.Debts.Select(d => new Tuple<Person, Person, int>(p.Debtor, d.Item1, d.Item2))).ToList();
        }
    }
}
