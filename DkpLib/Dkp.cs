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
}
