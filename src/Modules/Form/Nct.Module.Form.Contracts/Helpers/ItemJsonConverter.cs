

using Castle.Core.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using Nct.Module.Form.Items;

namespace Nct.Module.Form.Helpers
{
    public class ItemJsonConverter : JsonConverter<BaseItem>
    {
        public override bool CanConvert(Type typeToConvert)
        {

            return true;
        }

        public override BaseItem Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {

            var itemType = "";

            Utf8JsonReader readerClone = reader;

            while (readerClone.Read())
            {
                if (readerClone.TokenType == JsonTokenType.PropertyName)
                {
                    string propertyName = readerClone.GetString();
                    if (propertyName.IgnoreCaseEqual("Type"))
                    {
                        readerClone.Read();
                        itemType = readerClone.GetString();
                        break;

                    }


                }


            }


            Type type = null;

            if (itemType.IsNotEmpty())
            {
                var itemTypeName = $"Nct.Module.Form.Items.{itemType}";

                type = Type.GetType(itemTypeName);
            }

            if (type == null) throw new JsonException($"{type} not found");

            return (BaseItem)JsonSerializer.Deserialize(ref reader,type,options);



        }

        public override void Write(Utf8JsonWriter writer, BaseItem value, JsonSerializerOptions options)
        {
            var type = value.GetType();
            JsonSerializer.Serialize(writer, value, type, options);

        }
    }

}
