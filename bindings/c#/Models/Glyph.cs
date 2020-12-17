using System;

namespace rpi_rgb_led_matrix_sharp.Models
{
    /// <summary>
    /// Representation of a character within a canvas
    /// </summary>
    public class Glyph
    {
        /// <summary>
        /// Height of the Glyph
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// Width of the Glyph
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Character for the glyph
        /// </summary>
        public char Character { get; set; }
        private int _x = 0;
        /// <summary>
        /// X coordinate for the Glyph
        /// </summary>
        public int X {
            get
            {
                return this._x;
            }
            set
            {
                this._x = value;
            }
        }
        private int _y = 0;
        /// <summary>
        /// Y coordinate for the glyph
        /// </summary>
        public int Y 
        {
            get
            {
                return this._y;
            }
            set
            {
                //this is done to readjust the y value to the bottom of the glyph
                this._y = value + this.Height;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="c">Character for the glyph</param>
        /// <param name="font">Font used to render the glyph</param>
        public Glyph(char c, RGBLedFont font)
        {
            this.Height = font.Height();
            this.Width = font.Width($"{c}");
            this.Character = c;
        }
    }
}
