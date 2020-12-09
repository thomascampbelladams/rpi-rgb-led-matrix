namespace rpi_rgb_led_matrix_sharp.Models
{
    public class Glyph
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public char Character { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public Glyph(char c, RGBLedFont font)
        {
            this.Height = font.Height();
            this.Width = font.Width(c.ToString());
            this.Character = c;
        }
    }
}