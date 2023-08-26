using System.Text.Json.Serialization;

namespace Austin.DkpLib
{
    [JsonConverter(typeof(MoneyJsonConverter))]
    public readonly struct Money : IEquatable<Money>, IComparable<Money>, IFormattable
    {
        public const string FORMAT_BARE = "bare";

        public static Money Zero => default;

        private readonly int _pennies;

        public Money(int pennies)
        {
            this._pennies = pennies;
        }

        public bool IsZero => _pennies == 0;

        public static Money operator +(Money a, Money b)
        {
            return new Money(a._pennies + b._pennies);
        }

        public static Money operator -(Money a, Money b)
        {
            return new Money(a._pennies - b._pennies);
        }

        public static bool operator >(Money a, Money b)
        {
            return a._pennies > b._pennies;
        }

        public static bool operator <(Money a, Money b)
        {
            return a._pennies < b._pennies;
        }

        public static bool operator >=(Money a, Money b)
        {
            return a._pennies >= b._pennies;
        }

        public static bool operator <=(Money a, Money b)
        {
            return a._pennies <= b._pennies;
        }

        public static bool operator ==(Money a, Money b)
        {
            return a._pennies == b._pennies;
        }

        public static bool operator !=(Money a, Money b)
        {
            return a._pennies != b._pennies;
        }

        public static Money operator -(Money a)
        {
            return new Money(-a._pennies);
        }


        public int ToPennies()
        {
            return _pennies;
        }

        public override string ToString()
        {
            return ToString(null, null);
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            int pennies = _pennies;
            bool isNeg = _pennies < 0;
            if (isNeg)
                pennies = -pennies;

            int dollars = pennies / 100;
            int cents = pennies % 100;

            if (string.IsNullOrEmpty(format))
            {
                if (isNeg)
                {
                    return $"(${dollars}.{cents:00})";
                }
                else
                {
                    return $"${dollars}.{cents:00}";
                }
            }
            else if (format == FORMAT_BARE)
            {
                if (isNeg)
                {
                    return $"-{dollars}.{cents:00}";
                }
                else
                {
                    return $"{dollars}.{cents:00}";
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(format), "Unkown format string: " + format);
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj is Money)
                return Equals((Money)obj);
            return false;
        }

        public override int GetHashCode()
        {
            return _pennies;
        }

        public bool Equals(Money other)
        {
            return this._pennies == other._pennies;
        }

        public int CompareTo(Money other)
        {
            return this._pennies.CompareTo(other._pennies);
        }
    }
}
