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
                if (Math.Abs(totalSpentPart2 - totalSpentPart3) > 0.1)
                    throw new Exception("Something terrible has happened (part3).");
            }

            foreach (var p in amountSpent)
            {
                foreach (var payer in mPayer)
                {
                    var t = new Transaction()
                    {
                        ID = Guid.NewGuid(),
                        Creditor = payer,
                        Debtor = p.Item1,
                        Amount = (int)Math.Round(p.Item2 / (double)mPayer.Length),
                        BillSplit = bs,
                        Description = mName,
                        Created = mDate,
                    };
                    db.Transactions.InsertOnSubmit(t);
                }
            }

            db.SubmitChanges();
        }
    }
}
