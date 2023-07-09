using DkpWeb.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace DkpWeb.Data
{
    public class MoneyConverter : ValueConverter<Money, int>
    {
        public MoneyConverter()
            : this(null)
        {
        }

        public MoneyConverter(ConverterMappingHints mappingHints)
        : base(
            v => v.ToPennies(),
            v => new Money(v),
            mappingHints)
        {
        }

        public static ValueConverterInfo DefaultInfo { get; }
            = new(typeof(DateTime), typeof(int), i => new MoneyConverter(i.MappingHints));
    }
}
