using System;
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nct.Json
{
    public class DataTableConverter : JsonConverter<DataTable>
    {
        public override DataTable Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, DataTable value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (DataRow row in value.Rows)
            {
                writer.WriteStartObject();
                foreach (DataColumn column in row.Table.Columns)
                {
                    object columnValue = row[column];
                    var propName = column.ColumnName;
                  
                    if (columnValue is DBNull)
                    {
                        writer.WriteNull(propName);
                        continue;
                    }

                    writer.WritePropertyName(propName);
                    JsonSerializer.Serialize(writer, columnValue, options);
                }
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }
    }
}
