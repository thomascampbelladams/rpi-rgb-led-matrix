using rpi_rgb_led_matrix_sharp.Converters;
using Newtonsoft.Json;

namespace rpi_rgb_led_matrix_sharp.Models
{
    /// <summary>
    /// Represents a list of scenes as deserialized from JSON
    /// </summary>
    public class SceneList
    {
        /// <summary>
        /// Scenes to show
        /// </summary>
        public JsonScene[] Scenes { get; set; }
        /// <summary>
        /// Times to loop over the scenes
        /// </summary>
        public int TimesToRepeat { get; set; }
    }

    /// <summary>
    /// Represents a scene as deserialized from JSON
    /// </summary>
    public class JsonScene
    {
        /// <summary>
        /// Type of the scene
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Content for the scene. Could be <see cref="string"/> for marquees or <see cref="uint[][][]"/> for animations.
        /// </summary>
        public object Content { get; set; }
        /// <summary>
        /// Font file name to use for the scene
        /// </summary>
        public string Font { get; set; }
        /// <summary>
        /// Color to use for the scene
        /// </summary>
        public uint Color { get; set; }
        /// <summary>
        /// Delay between frames for the scene
        /// </summary>
        public int AnimationDelay { get; set; }
        /// <summary>
        /// Number of times to repeat the animation
        /// </summary>
        public int NumberOfTimesToRepeat { get; set; }
        /// <summary>
        /// Background scene for an animation
        /// </summary>
        [JsonConverter(typeof(SceneDataConverter))]
        public JsonScene SceneToShowInBackground { get; set; }
        /// <summary>
        /// Is animation using hex color values or the 0,1,2 scheme
        /// </summary>
        public bool isTwoBitAnimation { get; set; }
        /// <summary>
        /// Height of the block increment for the rainbow transition
        /// </summary>
        public int BlockHeight { get; set; }
        /// <summary>
        /// Width of the block increment for the rainbow transition
        /// </summary>
        public int BlockWidth { get; set; }
        /// <summary>
        /// Is text centered vertically
        /// </summary>
        public bool CenteredVertically { get; set; }
        /// <summary>
        /// Is text centered horizontally
        /// </summary>
        public bool CenteredHorizontally { get; set; }
        /// <summary>
        /// Number of seconds to show the scene
        /// </summary>
        public int NumberOfSecondsToShow { get; set; }
    }
}
