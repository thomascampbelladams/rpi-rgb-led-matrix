using System;

namespace rpi_rgb_led_matrix_sharp.Models
{
    public class Glyph
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public char Character { get; set; }
        private int _x = 0;
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
        public int Y 
        {
            get
            {
                return this._y;
            }
            set
            {
                this._y = value + this.Height;
            }
        }
        private RGBLedFont _rgbLedFont;

        public Glyph(char c, RGBLedFont font)
        {
            this.Height = font.Height();
            this.Width = font.Width($"{c}");
            this.Character = c;
            this._rgbLedFont = font;
        }
    }
}