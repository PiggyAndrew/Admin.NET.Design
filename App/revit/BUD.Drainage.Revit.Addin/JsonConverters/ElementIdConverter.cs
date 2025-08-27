using Autodesk.Revit.DB;
using Newtonsoft.Json;
using System;

namespace BUD.Drainage.Revit.Addin.JsonConverters
{
    /// <summary>
    /// 用于在JSON和ElementId之间进行转换的转换器
    /// </summary>
    public class ElementIdConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ElementId);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return ElementId.InvalidElementId;

            if (reader.TokenType == JsonToken.String || reader.TokenType == JsonToken.Integer)
            {
                string value = reader.Value.ToString();
                if (!int.TryParse(value, out int id))
                    return ElementId.InvalidElementId;
                return new ElementId(id);

            }

            return ElementId.InvalidElementId;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            ElementId elementId = (ElementId)value;
            writer.WriteValue(elementId.IntegerValue.ToString());
        }
    }
}