using System;
using System.Linq;
using System.Collections.Generic;

namespace Austin.DkpLib
{
    partial class DkpDataContext
    {
        public IEnumerable<Person> ActivePeopleOrderedByName
        {
            get
            {
                return People
                    .Where(p => !p.IsDeleted)
                    .OrderBy(p => p.FirstName)
                    .ThenBy(p => p.LastName);
            }
        }
    }

    partial class Transaction
    {
        internal const string DebtTransferString = "Debt Transfer: ";
        public static string CreateDebtTransferString(Person debtor, Person oldCreditor, Person newCreditor)
        {
            return string.Format("{0}{1} {2} {3}", DebtTransferString, debtor.ID, oldCreditor.ID, newCreditor.ID);
        }

        string _PrettyDescription = null;
        public string PrettyDescription
        {
            get
            {
                return _PrettyDescription ?? _Description;
            }
            set
            {
                _PrettyDescription = value;
            }
        }

        public void SetPrettyDescription(Dictionary<int, Person> personMap)
        {
            _PrettyDescription = CreatePrettyDescription(Description, Amount, personMap);
        }

        public static string CreatePrettyDescription(string desc, int amount, Dictionary<int, Person> personMap)
        {
            if (!desc.StartsWith(DebtTransferString))
                return null;

            var splits = desc.Remove(0, DebtTransferString.Length).Split(' ');
            if (splits.Length != 3)
                return null;

            var debtor = personMap[int.Parse(splits[0])];
            var oldCreditor = personMap[int.Parse(splits[1])];
            var newCreditor = personMap[int.Parse(splits[2])];

            return string.Format("{0}{2}'s debt of {1:c} to {3} is now owed to {4}",
                DebtTransferString, amount / 100d, debtor.FirstName, oldCreditor.FirstName, newCreditor.FirstName);
        }
    }

    partial class Person : ICloneable, IComparable<Person>
    {
        public override string ToString()
        {
            return FirstName + " " + LastName;
        }

        public override int GetHashCode()
        {
            return this.ID;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Person;
            if (other == null)
                return false;
            return other.ID == this.ID;
        }

        //for cycle removal
        public bool Visited;
        public void PrepareForCycleTesting()
        {
            Visited = false;
        }

        public object Clone()
        {
            var copy = new Person();
            copy._ID = this._ID;
            copy._FirstName = this._FirstName;
            copy._LastName = this._LastName;
            copy._IsDeleted = this._IsDeleted;
            return copy;
        }

        public int CompareTo(Person other)
        {
            int ret = this.FirstName.CompareTo(other.FirstName);
            if (ret == 0)
                ret = this.LastName.CompareTo(other.LastName);
            return ret;
        }
    }

    partial class BillSplit
    {
        string _PrettyName = null;
        public string PrettyName
        {
            get
            {
                return _PrettyName ?? _Name;
            }
            set
            {
                _PrettyName = value;
            }
        }

        public void SetPrettyDescription(Dictionary<int, Person> personMap)
        {
            if (!Name.StartsWith(Transaction.DebtTransferString))
                return;
            var amount = Transactions.Select(t => t.Amount).Distinct().Single();
            _PrettyName = Transaction.CreatePrettyDescription(Name, amount, personMap);
        }
    }
}
