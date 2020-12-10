using rpi_rgb_led_matrix_sharp;
using rpi_rgb_led_matrix_sharp.Utils;
using rpi_rgb_led_matrix_sharp.Models;
using rpi_rgb_led_matrix_sharp.Enums;
using System.Collections.Generic;
using System;
using System.Threading;

namespace rpi_rgb_led_matrix_sharp.Helpers
{
    public class ScreenHelper
    {
        private RGBLedMatrix _matrix;
        private RGBLedCanvas _canvas;
        private RGBLedFont _font;
        private int _backgroundColor;
        private int _matrixWidth = 64;
        private int _matrixHeight = 32;
        private string _hardwareMapping = "adafruit-hat";
        private string _currentFontName;

        public ScreenHelper()
        {
            this._matrix = new RGBLedMatrix(new RGBLedMatrixOptions
            {
                Rows = this._matrixHeight,
                Cols = this._matrixWidth,
                HardwareMapping = this._hardwareMapping
            });

            this._canvas = this._matrix.CreateOffscreenCanvas();
        }

        public void DrawVerticallyCenteredText(string lineText, Color color, bool doASync)
        {
            List<Glyph> glyphs = this.GetLines(lineText, VerticleAlignment.Middle, HorizontalAlignment.Left);

            foreach (Glyph glyph in glyphs)
            {
                this._canvas.DrawText(this._font, glyph.X, glyph.Y, color, $"{glyph.Character}");
            }

            this._matrix.SwapOnVsync(this._canvas);
        }

        public void DrawHorizontallyCenteredText(string lineText, Color color, bool doASync)
        {
            List<Glyph> glyphs = this.GetLines(lineText, VerticleAlignment.Top, HorizontalAlignment.Center);

            foreach (Glyph glyph in glyphs)
            {
                Console.WriteLine($"Writing character {glyph.Character} at x: {glyph.X}, y: {glyph.Y}");
                this._canvas.DrawText(this._font, glyph.X, glyph.Y, color, $"{glyph.Character}");
            }

            this._matrix.SwapOnVsync(this._canvas);
        }

        public void DrawCenteredText(string lineText, Color color, bool doASync)
        {
            List<Glyph> glyphs = this.GetLines(lineText, VerticleAlignment.Middle, HorizontalAlignment.Center);

            foreach (Glyph glyph in glyphs)
            {
                this._canvas.DrawText(this._font, glyph.X, glyph.Y, color, $"{glyph.Character}");
            }

            this._matrix.SwapOnVsync(this._canvas);
        }

        private List<Glyph> GetLines(string lineText, VerticleAlignment alignV, HorizontalAlignment alignh)
        {
            return LayoutUtils.LinesToMappedGlyphs(
                LayoutUtils.TextToLines(this._font, this._matrixWidth, lineText),
                this._font.Height(), this._matrixWidth, this._matrixHeight, alignh, alignV);
        }

        public void VerticalMarqueeText(string lineText, Color color, int animationDelay, bool runIndefinitely)
        {
            int yOffset = this._matrixHeight;
            int maxHeight = 0;
            List<Glyph> lines = this.GetLines(lineText, VerticleAlignment.Middle, HorizontalAlignment.Center);

            foreach (Glyph glyph in lines)
            {
                maxHeight = glyph.Y + this._font.Height();
            }

            if (runIndefinitely)
            {
                while (true)
                {
                    this.IncrementMarquee(yOffset, runIndefinitely, maxHeight, lines, animationDelay, false, color);
                    yOffset--;
                }
            }
            else
            {
                while(yOffset > -maxHeight)
                {
                    this.IncrementMarquee(yOffset, runIndefinitely, maxHeight, lines, animationDelay, false, color);
                    yOffset--;
                }
            }
        }

        private void IncrementMarquee(int offset, bool isRunningIndefinitely, int maxNumber, List<Glyph> glyphs, 
            int animationDelay, bool isHorizontal, Color color)
        {
            if (isRunningIndefinitely)
            {
                if (offset < - maxNumber)
                {
                    offset = maxNumber;
                }
            }

            this.Clear();

            foreach (Glyph glyph in glyphs)
            {
                if (isHorizontal)
                {
                    this._canvas.DrawText(this._font, glyph.X + offset, glyph.Y, color, $"{glyph.Character}");
                } 
                else
                {
                    this._canvas.DrawText(this._font, glyph.X, glyph.Y + offset, color, $"{glyph.Character}");
                }
            }

            this._matrix.SwapOnVsync(this._canvas);
            Thread.Sleep(animationDelay);
        }

        public void SetFont(string fontPath)
        {
            this._font = new RGBLedFont(fontPath);
        }

        public void Clear()
        {
            //this._matrix.Dispose();
            //this._matrix = new RGBLedMatrix(new RGBLedMatrixOptions
            //{
            //    Rows = this._matrixHeight,
            //    Cols = this._matrixWidth,
            //    HardwareMapping = this._hardwareMapping
            //});

            //this._canvas = this._matrix.CreateOffscreenCanvas();
            this._canvas.Clear();

            //this._matrix.SwapOnVsync(this._canvas);
        }
    }
}
