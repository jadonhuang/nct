using Nct.Extentions;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Nct.DataAccess.DynamicQuery
{
    public class FieldValueJsonConverter : JsonConverter<DynamicFieldValue>
    {


        public override DynamicFieldValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {

            var tokenType = reader.TokenType;
            if (tokenType == JsonTokenType.String)
            {
                var str = reader.GetString();
                if (str.IsNotEmpty() && (str.Contains('/') || str.Contains('-')) && DateTime.TryParse(str, out DateTime dateTime))
                {
                    return dateTime;
                }
                return str;
            }
            if (tokenType == JsonTokenType.Number)
            {

                if (reader.TryGetInt32(out int intValue))
                {
                    return intValue;
                }

                if (reader.TryGetDecimal(out decimal deValue))
                {
                    return deValue;
                }
                if (reader.TryGetDouble(out double dbValue))
                {
                    return dbValue;

                }
            }
            if (tokenType == JsonTokenType.True)
            {
                return true;
            }

            if (tokenType == JsonTokenType.False)
            {
                return false;
            }


            return (string)null;
        }

        public override void Write(Utf8JsonWriter writer, DynamicFieldValue value, JsonSerializerOptions options)
        {


            if (value.IsNull)
            {
                writer.WriteNullValue();
            }
            else
            {
                var typeCode = value.GetTypeCode();
                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        writer.WriteBooleanValue((bool)value);
                        break;
                    case TypeCode.Double:
                        writer.WriteNumberValue((double)value);
                        break;

                    case TypeCode.Int64:
                        writer.WriteNumberValue((long)value);
                        break;
                    default:
                        writer.WriteStringValue(value.ToString());
                        break;
                }


            }


        }
    }
}
