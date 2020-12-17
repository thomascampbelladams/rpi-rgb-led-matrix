using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;
using rpi_rgb_led_matrix_sharp.Models;

namespace rpi_rgb_led_matrix_sharp.Converters
{
    /// <summary>
    /// Converts the JSON representation of the Scene to a serializable object if its an object.
    /// Sometimes the JSON will report this as false to show there is no embedded scene.
    /// </summary>
    public class SceneDataConverter : JsonConverter
    {
        /// <summary>
        /// Determines if the type matches the JSON scene
        /// </summary>
        /// <param name="objectType"><see cref="Type"/> of the object</param>
        /// <returns><see cref="bool"/>, true if it is, false otherwise.</returns>
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(JsonScene));
        }

        /// <summary>
        /// Reads the JSON and returns a serializable object.
        /// </summary>
        /// <param name="reader"><see cref="JsonReader"/> that is reading the JSON</param>
        /// <param name="objectType"><see cref="Type"/> type of the object that should be returned.</param>
        /// <param name="existingValue"><see cref="object"/> currently being deserialized.</param>
        /// <param name="serializer"><see cref="JsonSerializer"/> the is serializer currently working on the object.</param>
        /// <returns><see cref="object"/> deserialized object</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Object)
            {
                return token.ToObject<JsonScene>();
            }
            return null;
        }

        /// <summary>
        /// Writes the object to Json. Just passes off the the <see cref="JsonSerializer"/>.
        /// </summary>
        /// <param name="writer"><see cref="JsonWriter"/> being used for the object.</param>
        /// <param name="value"><see cref="object"/> value to write.</param>
        /// <param name="serializer"><see cref="JsonSerializer"/> being used for the object.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
