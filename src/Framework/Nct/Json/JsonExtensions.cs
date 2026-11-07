using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Nct.Json
{


    public class DynamicJsonObject : DynamicObject
    {
        private readonly JsonElement _content;
        public DynamicJsonObject(JsonElement content)
        {
            _content = content;
        }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;

            if (_content.TryGetProperty(binder.Name, out JsonElement value))
            {
                result = ObtainValue(value);
            }
            else
            {
                return false;
            }

            return true;
        }
        private object ObtainValue(in JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.String: return element.GetString();
                case JsonValueKind.Null: return null;
                case JsonValueKind.False: return false;
                case JsonValueKind.True: return true;
                case JsonValueKind.Number: return element.GetDouble();
            }
            if (element.ValueKind == JsonValueKind.Array)
            {
                return element.EnumerateArray().Select(item => ObtainValue(item));
            }
            return new DynamicJsonObject(element);
        }
    }


    public static class JsonExtensions
    {

        public static string ToJson(this object obj, bool indent = false, bool camelCase = false)
        {

            var options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = indent,

            };
            if (camelCase)
            {
                options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            }
            options.Converters.Add(new DatetimeJsonConverter());
            return JsonSerializer.Serialize(obj, options);

        }


        public static T ToObject<T>(this string jsonStr)
        {

            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            };

            options.Converters.Add(new DatetimeJsonConverter());
            options.Converters.Add(new StringToBoolConverter());

            if (typeof(T) == typeof(object))
            {
                dynamic data = new DynamicJsonObject(JsonSerializer.Deserialize<JsonElement>(jsonStr, options));
                return data;
            }

            return JsonSerializer.Deserialize<T>(jsonStr, options);

        }

        public static object ToObject(this string jsonStr,Type type)
        {
            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            };

            options.Converters.Add(new DatetimeJsonConverter());
            options.Converters.Add(new StringToBoolConverter());
            return JsonSerializer.Deserialize(jsonStr,type,options);

        }






    }
}
