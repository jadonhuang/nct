using Nct.Extentions;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nct.Json
{
    public class DatetimeJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {

            if (reader.TokenType == JsonTokenType.String)
            {
                if (DateTime.TryParse(reader.GetString(), out DateTime date))
                    return date;
            }


            if (reader.TryGetDateTime(out var dateTime))
            {
                return dateTime;
            }

            var dateText = reader.GetString();
            if (dateText.IsNotEmpty())
            {
                if (DateTime.TryParse(reader.GetString(), out DateTime date))
                    return date;
            }

            return DateTime.MinValue;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {

            writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss"));


        }
    }

}
