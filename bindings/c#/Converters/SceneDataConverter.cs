using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;
using rpi_rgb_led_matrix_sharp.Models;

namespace rpi_rgb_led_matrix_sharp.Converters
{
    public class SceneDataConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(JsonScene));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Object)
            {
                return token.ToObject<JsonScene>();
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
