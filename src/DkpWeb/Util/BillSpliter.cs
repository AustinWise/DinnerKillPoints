using DkpWeb.Data;
using DkpWeb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Austin.DkpLib
{
    public class BillSpliter
    {
        /// <summary>
        /// The maximum deviation from a rounded penny amount a sum is allowed to be.
        /// </summary>
        /// <remarks>
        /// The database stores all debts as an integer number of pennies. However during splitting a bill
        /// fractional penny debts can occure, such as when splitting an item worth an odd number of pennies between
        /// two people. Durring bill splits the algorithm uses this value as the maximum deviation a summ of all
        /// debts can be from an integer value, as a check some fractions of a penny have not been lost during rounding.
        /// </remarks>
        const double PENNY_THRESHOLD = 0.01;

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
            this.mDate = date.ToUniversalTime();
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
        public int Discount { get; set; }

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

        public void Save(ApplicationDbContext db)
        {
            Save(db, TextWriter.Null);
        }

        public void Save(ApplicationDbContext db, TextWriter log)
        {
            var debts = SplitBill(log);

            var bs = new BillSplit() { Name = mName };
            db.BillSplit.Add(bs);
            foreach (var p in debts)
            {
                var t = new Transaction()
                {
                    Id = Guid.NewGuid(),
                    Creditor = p.Item2,
                    Debtor = p.Item1,
                    Amount = p.Item3,
                    Bill = bs,
                    Description = mName,
                    Created = mDate,
                };
                db.Transaction.Add(t);
            }

            db.SaveChanges();
        }

        private void ValidateBill()
        {
            if (mParty.Count == 0)
                throw new Exception("Must have one or more people in that party.");
            foreach (var kvp in mParty)
            {
                if (kvp.Item2 < 0)
                    throw new NotSupportedException(kvp.Item1.ToString() + " spent a negitive amount of money.");
            }
            if (Tax < 0 || Tip < 0 || SharedFood < 0 || Discount < 0)
                throw new NotSupportedException("Negative bill attribute.");
        }

        public List<Tuple<Person, Person, int>> SplitBill(TextWriter log)
        {
            ValidateBill();

            var pool = mParty.Sum(p => p.Item2) + SharedFood;
            var totalBillValue = pool + Tip + Tax - Discount;

            if (totalBillValue <= 0)
                throw new Exception("Zero or negative total bill value.");
            if (Math.Abs(totalBillValue - Math.Round(totalBillValue)) > PENNY_THRESHOLD)
                throw new Exception("Non-int number of pennies.");

            log.WriteLine($"pool: {pool / 100d:c}");
            log.WriteLine($"totalBillValue: {totalBillValue / 100d:c}");
            log.WriteLine();

            var amountSpent = new List<Tuple<Person, double>>();

            log.WriteLine("First add up each person's share of the bill, splitting shared food items equally:");
            foreach (var p in mParty)
            {
                log.WriteLine($"\t{p.Item1.FullName}:");

                double personSubtotal = SharedFood;
                personSubtotal /= mParty.Count;
                log.WriteLine($"\t\tpersonal food: {p.Item2 / 100d:c}");
                log.WriteLine($"\t\tshared food share: {personSubtotal / 100d:c}");
                personSubtotal += p.Item2;
                var ratio = (double)personSubtotal / pool;
                log.WriteLine($"\t\ttax, tip, and discounts share: %{ratio * 100d}");
                var taxTipShare = ratio * (Tax + Tip - Discount);
                log.WriteLine($"\t\ttax, tip, and discounts share: {taxTipShare / 100d:c}");
                var total = personSubtotal + taxTipShare;

                amountSpent.Add(new Tuple<Person, double>(p.Item1, total));
                log.WriteLine($"\t\ttotal raw share {total / 100d:c}");
            }
            checkTotal(totalBillValue, amountSpent.Select(tup => tup.Item2).Sum());
            log.WriteLine();

            //Take each freeloader and evenly split their meal across all the non-freeloaders
            var freeloadersFound = amountSpent.Where(p => mFreeLoaders.Contains(p.Item1)).ToList();
            var nonFreeLoaderCount = amountSpent.Count - freeloadersFound.Count;
            if (freeloadersFound.Count != 0)
            {
                var freeloaderSum = freeloadersFound.Select(p => p.Item2).Sum();
                log.WriteLine($"{freeloadersFound.Count} freeloaders found, owing a total of {freeloaderSum / 100d:c}:");
                foreach (var freeloader in freeloadersFound)
                {
                    log.WriteLine($"\t{freeloader.Item1.FullName}: {freeloader.Item2 / 100d:c}");
                }
                var extraPerPerson = freeloaderSum / nonFreeLoaderCount;
                log.WriteLine($"adding {extraPerPerson / 100d:c} to each non-freeloader's debts");
                amountSpent = amountSpent
                    .Where(p => !mFreeLoaders.Contains(p.Item1))
                    .Select(p => new Tuple<Person, double>(p.Item1, p.Item2 + freeloaderSum / nonFreeLoaderCount))
                    .ToList();
                foreach (var p in freeloadersFound)
                {
                    amountSpent.Add(new Tuple<Person, double>(p.Item1, 0));
                }

                checkTotal(totalBillValue, amountSpent.Sum(p => p.Item2));
                log.WriteLine();
            }

            //Evenly split each person's debt to each payer.
            var debtsToPayers = SplitDebtsBetweenPayers(amountSpent);
            checkTotal(totalBillValue, debtsToPayers.SelectMany(p => p.Item2).Sum(p => p.Item2));

            //Take all the fractional pennies and distrubte them to each debtor, round robin to each payer.
            var pennySplits = SplitPennies(debtsToPayers);
            checkTotal(totalBillValue, pennySplits.Sum(p => p.Item3));

            foreach (var p in pennySplits)
            {
                if (p.Item3 < 0)
                    throw new Exception("Negative debt.");
            }

            return pennySplits;
        }

        static void checkTotal(double initalSum, double currentSum)
        {
            if (Math.Abs(currentSum - Math.Round(currentSum)) > PENNY_THRESHOLD)
                throw new Exception("Non-int number of pennies.");
            if (Math.Abs(initalSum - currentSum) > 0.1)
                throw new Exception("Something terrible has happened.");
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
        /// Round splits to the nearest penny.
        /// </summary>
        /// <param name="amountSpent"></param>
        /// <returns>Item1 owes Item2 Item3 pennies</returns>
        /// <remarks>
        /// Over the course of splitting a bill people can end up owing a fractional number of pennies.
        /// Thie method rounds peoples debts to the nearest penny, while preserving the invarient that
        /// the total amount owed in this group of debts does not change.
        /// </remarks>
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

            if (Math.Abs(tempDoublePennyCount - Math.Round(tempDoublePennyCount)) > PENNY_THRESHOLD)
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
