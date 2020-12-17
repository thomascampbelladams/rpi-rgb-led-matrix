namespace rpi_rgb_led_matrix_sharp.Models
{
    /// <summary>
    /// Represents a color in RGB format
    /// </summary>
    public struct Color
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="r">Red value</param>
        /// <param name="g">Green value</param>
        /// <param name="b">Blue value</param>
        public Color(int r, int g, int b)
        {
            R = (byte)r;
            G = (byte)g;
            B = (byte)b;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="r">Red value</param>
        /// <param name="g">Green value</param>
        /// <param name="b">Blue value</param>
        public Color(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="hexValue">Hex value for the color</param>
        public Color(uint hexValue)
        {
            R = (byte)((hexValue & 0xff0000) >> 0x10);
            G = (byte)((hexValue & 0xff00) >> 8);
            B = (byte)(hexValue & 0xff);
        }

        /// <summary>
        /// Red value
        /// </summary>
        public byte R;

        /// <summary>
        /// Green value
        /// </summary>
        public byte G;

        /// <summary>
        /// Blue value
        /// </summary>
        public byte B;
    }
}
