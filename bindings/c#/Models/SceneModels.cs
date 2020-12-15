using rpi_rgb_led_matrix_sharp.Converters;
using Newtonsoft.Json;

namespace rpi_rgb_led_matrix_sharp.Models
{
    public class SceneList
    {
        public JsonScene[] Scenes { get; set; }
        public int TimesToRepeat { get; set; }
    }

    public class JsonScene
    {
        public string Type { get; set; }
        public object Content { get; set; }
        public string Font { get; set; }
        public uint Color { get; set; }
        public int AnimationDelay { get; set; }
        public int NumberOfTimesToRepeat { get; set; }

        [JsonConverter(typeof(SceneDataConverter))]
        public JsonScene SceneToShowInBackground { get; set; }
        public bool isTwoBitAnimation { get; set; }
        public int BlockHeight { get; set; }
        public int BlockWidth { get; set; }
        public bool CenteredVertically { get; set; }
        public bool CenteredHorizontally { get; set; }
        public int NumberOfSecondsToShow { get; set; }
    }
}
