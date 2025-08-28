using Autodesk.Revit.DB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace BUD.Drainage.Revit.Addin.JsonConverters
{
    /// <summary>
    /// 用于在JSON和XYZ坐标点之间进行转换的转换器
    /// </summary>
    public class XYZConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(XYZ);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            // 读取JSON对象
            JObject jObject = JObject.Load(reader);

            // 提取x, y, z坐标值
            double x = jObject["x"]?.Value<double>() ?? 0;
            double y = jObject["y"]?.Value<double>() ?? 0;
            double z = jObject["z"]?.Value<double>() ?? 0;

            // 创建XYZ对象
            return new XYZ(x, y, z);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            XYZ point = (XYZ)value;

            // 创建包含x, y, z属性的JSON对象
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(point.X);
            writer.WritePropertyName("y");
            writer.WriteValue(point.Y);
            writer.WritePropertyName("z");
            writer.WriteValue(point.Z);
            writer.WriteEndObject();
        }
    }

    /// <summary>
    /// 用于在JSON和XYZ点集合之间进行转换的转换器
    /// </summary>
    public class XYZListConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<XYZ>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            // 读取JSON数组
            JArray jArray = JArray.Load(reader);
            List<XYZ> points = new List<XYZ>();

            // 遍历数组中的每个对象
            foreach (JObject jObject in jArray)
            {
                // 提取x, y, z坐标值
                double x = jObject["x"]?.Value<double>() ?? 0;
                double y = jObject["y"]?.Value<double>() ?? 0;
                double z = jObject["z"]?.Value<double>() ?? 0;

                // 创建XYZ对象并添加到列表
                points.Add(new XYZ(x, y, z));
            }

            return points;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            List<XYZ> points = (List<XYZ>)value;

            writer.WriteStartArray();

            foreach (XYZ point in points)
            {
                // 为每个点创建包含x, y, z属性的JSON对象
                writer.WriteStartObject();
                writer.WritePropertyName("x");
                writer.WriteValue(point.X);
                writer.WritePropertyName("y");
                writer.WriteValue(point.Y);
                writer.WritePropertyName("z");
                writer.WriteValue(point.Z);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }
    }
}