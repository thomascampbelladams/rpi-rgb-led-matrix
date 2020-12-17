using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using rpi_rgb_led_matrix_sharp.Models;

namespace rpi_rgb_led_matrix_sharp
{
    /// <summary>
    /// Font representation used for describing what binary distribution format file to use for rendering font.
    /// </summary>
    public class RGBLedFont : IDisposable
    {
        [DllImport("librgbmatrix.so", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr load_font(string bdf_font_file);

        [DllImport("librgbmatrix.so", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int draw_text(IntPtr canvas, IntPtr font, int x, int y, byte r, byte g, byte b, string utf8_text, int extra_spacing);

        [DllImport("librgbmatrix.so", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int vertical_draw_text(IntPtr canvas, IntPtr font, int x, int y, byte r, byte g, byte b, string utf8_text, int kerning_offset);

        [DllImport("librgbmatrix.so", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void delete_font(IntPtr font);

        [DllImport("librgbmatrix.so", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int height_font(IntPtr font);

        [DllImport("librgbmatrix.so", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int width_font(IntPtr font, string str);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bdf_font_file_path">File path to the BDF file to use for the font.</param>
        public RGBLedFont(string bdf_font_file_path)
        {
            _font = load_font(bdf_font_file_path);
        }
        internal IntPtr _font;

        /// <summary>
        /// height of the font.
        /// </summary>
        /// <returns>Font height</returns>
        public int Height() => height_font(_font);

        /// <summary>
        /// Width of the string in this font.
        /// </summary>
        /// <param name="str">String to get the width of.</param>
        /// <returns>The width of the font in pixels when it's rendered in this font.</returns>
        public int Width(string str) => return width_font(_font, str);

        /// <summary>
        /// Draws text in this font. 
        /// </summary>
        /// <param name="canvas">Canvas to draw font on.</param>
        /// <param name="x">x point to start rendering text.</param>
        /// <param name="y">y point to start rendering text.</param>
        /// <param name="color">Color of the text</param>
        /// <param name="text">Text to render</param>
        /// <param name="spacing">Extra spacing to use in between characters</param>
        /// <param name="vertical">If true, font will be rendered vertically</param>
        /// <returns></returns>
        internal int DrawText(IntPtr canvas, int x, int y, Color color, string text, int spacing=0, bool vertical=false)
        {
            if (!vertical)
                return draw_text(canvas, _font, x, y, color.R, color.G, color.B, text, spacing);
            else
                return vertical_draw_text(canvas, _font, x, y, color.R, color.G, color.B, text, spacing);
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                delete_font(_font);
                disposedValue = true;
            }
        }
        ~RGBLedFont()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
