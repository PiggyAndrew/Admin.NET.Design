using Autodesk.Revit.DB;
using BUD.Revit.Framework;
using Newtonsoft.Json;
using System;

namespace BUD.Drainage.Revit.Addin.JsonConverters
{
    /// <summary>
    /// 用于在JSON和ElementType之间进行转换的转换器
    /// </summary>
    public class ElementTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            if (reader.TokenType == JsonToken.String)
            {
                string value = reader.Value.ToString();
                return value;
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteValue(value.ToString());
        }
    }
}