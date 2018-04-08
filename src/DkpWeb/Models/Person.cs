using System;
using System.Collections.Generic;

namespace DkpWeb.Models
{
    public partial class Person : IComparable<Person>, IEquatable<Person>
    {
        public Person()
        {
            PaymentIdentity = new HashSet<PaymentIdentity>();
            TransactionCreditor = new HashSet<Transaction>();
            TransactionDebtor = new HashSet<Transaction>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsDeleted { get; set; }
        public string Email { get; set; }

        public virtual ICollection<PaymentIdentity> PaymentIdentity { get; set; }
        public virtual ICollection<Transaction> TransactionCreditor { get; set; }
        public virtual ICollection<Transaction> TransactionDebtor { get; set; }


        public string FullName => ToString();

        public override string ToString()
        {
            return FirstName + " " + LastName;
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Person;
            if (other == null)
                return false;
            return Equals(other);
        }

        public bool Equals(Person other)
        {
            return other.Id == Id;
        }

        //for cycle removal
        public bool Visited;
        public void PrepareForCycleTesting()
        {
            Visited = false;
        }

        public Person Clone()
        {
            var copy = new Person();
            copy.Id = this.Id;
            copy.FirstName = this.FirstName;
            copy.LastName = this.LastName;
            copy.IsDeleted = this.IsDeleted;
            copy.Email = this.Email;
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
