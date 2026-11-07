using Nct.Module.Flow.Engine.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nct.Module.Flow.Helpers
{
    public class NodePropsJsonConverter : JsonConverter<NodeProps>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return true;
        }

        public override NodeProps Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {

            var propType = "";

            Utf8JsonReader readerClone = reader;

            while (readerClone.Read())
            {
                if (readerClone.TokenType == JsonTokenType.PropertyName)
                {
                    string propertyName = readerClone.GetString();

                    if (propertyName == "PropType")
                    {
                        readerClone.Read();
                        propType = readerClone.GetString();

                        break;

                    }


                }


            }
            Type type = null;
            if (propType.IsNotEmpty())
            {
                var typeName = $"{typeof(NodeProps).Namespace}.{propType}NodeProps";
                type = Type.GetType(typeName);
            }

            type ??= typeof(NullNodeProps);

            return (NodeProps)JsonSerializer.Deserialize(ref reader, type, options);



        }

        public override void Write(Utf8JsonWriter writer, NodeProps value, JsonSerializerOptions options)
        {
            var type = value.GetType();
            JsonSerializer.Serialize(writer, value, type, options);

        }
    }

}
