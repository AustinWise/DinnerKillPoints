namespace Austin.DkpLib
{
    public class SplitPerson : IEquatable<SplitPerson>
    {
        public required int Id { get; init; }

        public required string FullName { get; init; }

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
