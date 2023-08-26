namespace Austin.DkpLib
{
    public class SplitPerson : IEquatable<SplitPerson>
    {
        public int Id { get; set; }

        public string? FullName { get; set; }

        public override int GetHashCode()
        {
            return Id;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as SplitPerson);
        }

        public bool Equals(SplitPerson? other)
        {
            return other is not null && other.Id == this.Id;
        }
    }
}
