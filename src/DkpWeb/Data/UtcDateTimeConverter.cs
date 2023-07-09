using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace DkpWeb.Data
{
    class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
    {
        public UtcDateTimeConverter()
            : this(null)
        {
        }

        public UtcDateTimeConverter(ConverterMappingHints mappingHints)
        : base(
            v => v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
            mappingHints)
        {
        }

        public static ValueConverterInfo DefaultInfo { get; }
            = new(typeof(DateTime), typeof(DateTime), i => new UtcDateTimeConverter(i.MappingHints));
    }
}
