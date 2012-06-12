namespace Austin.DkpLib
{
    partial class DkpDataContext
    {
    }

    partial class Transaction
    {
    }

    partial class Person
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
    }
}
