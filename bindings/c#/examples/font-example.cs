using rpi_rgb_led_matrix_sharp;
using rpi_rgb_led_matrix_sharp.Utils;
using System;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using rpi_rgb_led_matrix_sharp.Models;

namespace font_example
{
    class Program
    {
        private static bool debug = true;

        static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                if (debug) Console.WriteLine("font-example.exe [font_path] <text>");
                return -1;
            }
            string text = "Hello World!";
            if (args.Length > 1)            
                text = args[1];
            List<Glyph> glyphList = new List<Glyph>();
            var matrix = new RGBLedMatrix(new RGBLedMatrixOptions
            {
                Rows = 32,
                Cols = 64,
                HardwareMapping = "adafruit-hat"
            });
            var canvas = matrix.CreateOffscreenCanvas();
            var font = new RGBLedFont(args[0]);

            foreach (char c in text)
            {
                glyphList.Add(new Glyph(c, font));
            }

            List<Glyph> glyphs = LayoutUtils.LinesToMappedGlyphs(LayoutUtils.TextToLines(font, 64, text), font.Height(), 64, 32);

            foreach (Glyph glyph in glyphs)
            {
                if (debug) Console.WriteLine($"Writing {glyph.Character} at x: {glyph.X}, y: {glyph.Y}");

                canvas.DrawText(font, glyph.X, glyph.Y, new Color(0, 255, 0), $"{glyph.Character}");
            }

            matrix.SwapOnVsync(canvas);

            if (debug) Console.WriteLine($"Font Height: {font.Height()} Font Width: {font.Width(text)}");

            while (!Console.KeyAvailable)
            {
                Thread.Sleep(250);
            }

            return 0;
        }
    }
}
