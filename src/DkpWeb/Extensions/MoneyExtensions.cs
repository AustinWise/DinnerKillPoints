using Austin.DkpLib;
using System;
using System.Collections.Generic;

namespace DkpWeb
{
    public static class MoneyExtensions
    {
        public static Money Sum(this IEnumerable<Money> monies)
        {
            Money ret = new Money(0);
            foreach (var m in monies)
            {
                ret += m;
            }
            return ret;
        }
        public static Money Sum<T>(this IEnumerable<T> monies, Func<T, Money> func)
        {
            Money ret = new Money(0);
            foreach (var t in monies)
            {
                ret += func(t);
            }
            return ret;
        }
    }
}
