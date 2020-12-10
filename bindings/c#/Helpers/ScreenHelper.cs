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
                while (yOffset > -maxHeight)
                {
                    this.IncrementMarquee(yOffset, runIndefinitely, maxHeight, lines, animationDelay, false, color);
                    yOffset--;
                }
            }
        }

        public void HorizontalMarqueeText(string lineText, Color color, int animationDelay, bool runIndefinitely)
        {
            int xOffset = this._matrixWidth;
            int maxWidth = 0;
            List<Glyph> lines = this.GetLines(lineText, VerticleAlignment.Middle, HorizontalAlignment.Center);

            foreach (Glyph glyph in lines)
            {
                maxWidth = glyph.X + this._font.Width(lineText);
            }

            if (runIndefinitely)
            {
                while (true)
                {
                    this.IncrementMarquee(xOffset, runIndefinitely, maxWidth, lines, animationDelay, true, color);
                    xOffset--;
                }
            }
            else
            {
                while (xOffset > -maxWidth)
                {
                    this.IncrementMarquee(xOffset, runIndefinitely, maxWidth, lines, animationDelay, true, color);
                    xOffset--;
                }
            }
        }

        private void IncrementMarquee(int offset, bool isRunningIndefinitely, int maxNumber, List<Glyph> glyphs,
            int animationDelay, bool isHorizontal, Color color)
        {
            if (isRunningIndefinitely)
            {
                if (offset < -maxNumber)
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

            Sync();
            Thread.Sleep(animationDelay);
        }

        public void SetFont(string fontPath)
        {
            this._font = new RGBLedFont(fontPath);
        }

        public void Clear()
        {
            this._canvas.Clear();
            this._matrix.Clear();
        }

        private void DrawSingleFrame(uint[][] animationFrame, int startX, int startY, bool isTwoBitAnimation, int animationDelay)
        {
            uint maxColorValue = 0xFFFFFF;

            for (int i = 0; i < animationFrame.Length; i++)
            {
                uint[] animationRow = animationFrame[i];

                for (int j = 0; j < animationRow.Length; j++)
                {
                    uint animationPixel = animationRow[j];

                    if (isTwoBitAnimation)
                    {
                        if (animationPixel == 2)
                        {
                            uint randColor = (uint)new Random().Next(0, 0xFFFFFF);
                            Color color = new Color(randColor);

                            SetPixel(startX + j, startY + i, color, false);
                        }
                        else if (animationPixel == 1)
                        {
                            SetPixel(startX + j, startY + i, new Color(maxColorValue), false);
                        }
                        else
                        {
                            SetPixel(startX + j, startY + i, new Color(0), false);
                        }
                    }
                    else
                    {
                        Color color = new Color(animationPixel);
                        SetPixel(startX + j, startY + i, color, false);
                    }
                }
            }

            Thread.Sleep(animationDelay);
        }

        private Tuple<int, int> FindAnimationFrameWidthAndHeight(uint[][][] animation)
        {
            int largestWidth = 0;
            int largestHeight = 0;
                                                                                                                                                                              
            foreach (uint[][] animationFrame in animation)
            {
                if (animationFrame.Length > largestHeight)
                {
                    largestHeight = animationFrame.Length;
                }

                foreach (uint[] animationRow in animationFrame)
                {
                    if (animationRow.Length > largestWidth)
                    {
                        largestWidth = animationRow.Length;
                    }
                }
            }

            return new Tuple<int, int>(largestWidth, largestHeight); 
        }

        public void DoAnimation(uint[][][] animation, int animationDelay, int numberOfTimesToShow, Action action, bool isTwoBitAnimation)
        {
            Tuple<int, int> widthAndHeight = FindAnimationFrameWidthAndHeight(animation);
            int pixelSpriteWidth = widthAndHeight.Item1;
            int pixelSpriteHeight = widthAndHeight.Item2;
            int shown = 0;

            while (shown <= numberOfTimesToShow)
            {
                int randomXStart = new Random().Next(0, this._matrixWidth-pixelSpriteWidth);
                int randomYStart = new Random().Next(0, this._matrixHeight-pixelSpriteHeight);

                foreach (uint[][] animationFrame in animation)
                {
                    DrawSingleFrame(animationFrame, randomXStart, randomYStart, isTwoBitAnimation, animationDelay);
                }

                ClearSprite(pixelSpriteWidth, pixelSpriteHeight, randomXStart, randomYStart, action, animationDelay);
                shown++;
            }
        }

        private void SetPixel(int x, int y, Color color, bool useCanvas)
        {
            if (useCanvas)
            {
                this._canvas.SetPixel(x, y, color);
            }
            else
            {
                this._matrix.SetPixel(x, y, color);
            }
        }

        private void Sync()
        {
            this._matrix.SwapOnVsync(this._canvas);
        }

        private void ClearSprite(int pixelSpriteWidth, int pixelSpriteHeight, int x, int y, Action action, int animationDelay)
        {
            for (int i = 0; i < pixelSpriteHeight; i++)
            {
                for (int j = 0; j < pixelSpriteWidth; j++)
                {
                    SetPixel(x + j, y + i, new Color(0), false);
                }
            }

            action();

            Thread.Sleep(animationDelay);
        }

        private void IncreaseTransition(int w, int xOffset, int row, int animationDelay, Color color)
        {
            int newX = w + xOffset;

            SetPixel(newX, row, color, false);

            Thread.Sleep(animationDelay);
        }

        private void DecreaseTransition(int w, int xOffset, int row, int animationDelay)
        {
            SetPixel(w + xOffset, row, new Color(0, 0, 0), false);

            Thread.Sleep(animationDelay);
        }

        public void RainbowTransition(int animationDelay, int heightOfColorBlock, int widthOfColorBlock, Action action)
        {
            int row = 0;
            uint[] colors = new uint[]
            {
                0xff0000, //red
                0xff7f00, //orange
                0xffff00, //yellow
                0x00ff00, //green
                0x0000ff, //blue
                0x4b0082, //indigo
                0x7F00FF //violet
            };
            int colorIndex = 0;
            int xOffset = 0;

            while (xOffset < this._matrixWidth)
            {
                Console.WriteLine($"{xOffset}");
                while (row < this._matrixHeight)
                {
                    for (int w = 0; w < widthOfColorBlock; w++)
                    {
                        IncreaseTransition(w, xOffset, row, animationDelay, new Color(colors[colorIndex]));
                    }

                    row++;

                    if (row % heightOfColorBlock == 0)
                    {
                        colorIndex++;

                        if (colorIndex >= colors.Length)
                        {
                            colorIndex = 0;
                        }
                    }
                }

                xOffset += widthOfColorBlock;
                row = 0;
            }

            xOffset = this._matrixWidth - widthOfColorBlock;
            row = this._matrixHeight;

            while (xOffset >= 0)
            {
                while (row >= 0)
                {
                    for (int w = widthOfColorBlock-1; w >= 0; w--)
                    {
                        DecreaseTransition(w, xOffset, row, animationDelay);
                    }

                    row--;
                }

                xOffset -= widthOfColorBlock;
                row = this._matrixHeight;
            }

            action();
        }
    }
}
