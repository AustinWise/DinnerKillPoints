using DkpWeb.Models;
using System;
using System.Buffers.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DkpWeb.Data
{
    public class MoneyJsonConverter : JsonConverter<Money>
    {
        public override Money Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.Number)
                throw new Exception();
            if (!Utf8Parser.TryParse(reader.ValueSpan, out int pennies, out int bytesConsumed))
                throw new Exception();
            if (bytesConsumed != reader.ValueSpan.Length)
                throw new Exception();
            return new Money(pennies);
        }

        public override void Write(Utf8JsonWriter writer, Money value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.ToPennies());
        }
    }
}
