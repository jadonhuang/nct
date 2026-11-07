using System;
using System.Text.Json;

namespace Nct.Extentions
{
    public static class JsonElementExtentions
    {
        public static JsonElement SafeGetProperty(this JsonElement element, string propertyName)
        {
            element.TryGetProperty(propertyName, out var prop);
            return prop;

        }

        public static bool IsNull(this JsonElement element)
        {
            return element.ValueKind == JsonValueKind.Null || element.ValueKind == JsonValueKind.Undefined;

        }

        public static string SafeGetString(this JsonElement element)
        {
            if (element.ValueKind != JsonValueKind.String)
                return null;
            return element.GetString();

        }

        public static string SafeGetString(this JsonElement element, string propertyName)
        {
            return element.SafeGetProperty(propertyName).SafeGetString();

        }

        public static object GetValue(this JsonElement element)
        {
            if (element.IsNull())
                return null;

            if (element.ValueKind == JsonValueKind.String)
            {
                var str = element.GetString();
                if (DateTime.TryParse(str, out DateTime dateTime))
                {
                    return dateTime;
                }
                return str;
            }

            if (element.ValueKind == JsonValueKind.False)
            {
                return false;
            }

            if (element.ValueKind == JsonValueKind.True)
            {
                return true;
            }

            if (element.ValueKind == JsonValueKind.Number)
            {
                if (element.TryGetInt32(out int intValue))
                {
                    return intValue;
                }

                if (element.TryGetDecimal(out decimal deValue))
                {
                    return deValue;
                }
                if (element.TryGetDouble(out double dbValue))
                {
                    return dbValue;

                }

            }

            return null;
        }

    }
}
