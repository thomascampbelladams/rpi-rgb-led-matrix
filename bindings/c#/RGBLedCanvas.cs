using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using rpi_rgb_led_matrix_sharp.Models;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace rpi_rgb_led_matrix_sharp
{
    /// <summary>
    /// Represents a single canvas for the RGB Screen
    /// Think of it as a frame in a video
    /// </summary>
    public class RGBLedCanvas
    {
        #region DLLImports
        [DllImport("librgbmatrix.so")]
        internal static extern void led_canvas_get_size(IntPtr canvas, out int width, out int height);

        [DllImport("librgbmatrix.so")]
        internal static extern void led_canvas_set_pixel(IntPtr canvas, int x, int y, byte r, byte g, byte b);

        [DllImport("librgbmatrix.so")]
        internal static extern void led_canvas_clear(IntPtr canvas);

        [DllImport("librgbmatrix.so")]
        internal static extern void led_canvas_fill(IntPtr canvas, byte r, byte g, byte b);

        [DllImport("librgbmatrix.so")]
        internal static extern void draw_circle(IntPtr canvas, int xx, int y, int radius, byte r, byte g, byte b);

        [DllImport("librgbmatrix.so")]
        internal static extern void draw_line(IntPtr canvas, int x0, int y0, int x1, int y1, byte r, byte g, byte b);
        #endregion

        // This is a wrapper for canvas no need to implement IDisposable here 
        // because RGBLedMatrix has ownership and takes care of disposing canvases
        internal IntPtr _canvas;

        // this is not called directly by the consumer code,
        // consumer uses factory methods in RGBLedMatrix
        internal RGBLedCanvas(IntPtr canvas)
        {
            _canvas = canvas;
            int width;
            int height;
            led_canvas_get_size(_canvas, out width, out height);
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Width of the canvas
        /// </summary>
        public int Width {get; private set; }

        /// <summary>
        /// Height of the canvas
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Returns the pointer for the C++ canvas object.
        /// This is needed to enable memory copies of canvases to allow for faster rendering of background canvases.
        /// </summary>
        /// <returns><see cref="IntPtr"/> reference to the C++ canvas object.</returns>
        public IntPtr GetCanvasPtr()
        {
            return _canvas;
        }

        /// <summary>
        /// Sets a pixel in the canvas
        /// </summary>
        /// <param name="x">X coordinate for the pixel</param>
        /// <param name="y">Y coordinate for the pixel</param>
        /// <param name="color">Color to set the pixel</param>
        public void SetPixel(int x, int y, Color color)
        {
            led_canvas_set_pixel(_canvas, x, y, color.R, color.G, color.B);
        }

        /// <summary>
        /// Fills the entire canvas with the supplied color.
        /// </summary>
        /// <param name="color">Color to fill the canvas with.</param>
        public void Fill(Color color)
        {
            led_canvas_fill(_canvas, color.R, color.G, color.B);
        }

        /// <summary>
        /// Clears this canvas
        /// </summary>
        public void Clear()
        {
            led_canvas_clear(_canvas);
        }

        /// <summary>
        /// Draws a circle centered at the coordinates supplies
        /// </summary>
        /// <param name="x0">x coordinate for the center of the circle.</param>
        /// <param name="y0">y coordinate for the center of the cirecle.</param>
        /// <param name="radius">Radius of the circle</param>
        /// <param name="color">Color of the circle</param>
        public void DrawCircle(int x0, int y0, int radius, Color color)
        {
            draw_circle(_canvas, x0, y0, radius, color.R, color.G, color.B);
        }

        /// <summary>
        /// Draws a line on the canvas from one coordinate to another.
        /// </summary>
        /// <param name="x0">Starting x point for the line</param>
        /// <param name="y0">Starting y point for the line</param>
        /// <param name="x1">Ending x point for the line</param>
        /// <param name="y1">Ending y point for the line</param>
        /// <param name="color">Color the line should be drawn in</param>
        public void DrawLine (int x0, int y0, int x1, int y1, Color color)
        {
            draw_line(_canvas, x0, y0, x1, y1, color.R, color.G, color.B);
        }

        /// <summary>
        /// Draws test on the canvas starting at the supplied coordinate
        /// </summary>
        /// <param name="font">Font to use for rendering the text.</param>
        /// <param name="x">x point to start the text.</param>
        /// <param name="y">y point to start the text.</param>
        /// <param name="color">Color to use for the text.</param>
        /// <param name="text">Text to render.</param>
        /// <param name="spacing">Extra spacing to use between characters</param>
        /// <param name="vertical">If true, font will render vertically instead of horizontally.</param>
        /// <returns></returns>
        public int DrawText(RGBLedFont font, int x, int y, Color color, string text, int spacing=0, bool vertical=false)
        {
            return font.DrawText(_canvas, x, y, color, text, spacing, vertical);
        }
    }
}
