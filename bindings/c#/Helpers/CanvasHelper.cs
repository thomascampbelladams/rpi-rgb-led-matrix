using rpi_rgb_led_matrix_sharp;
using rpi_rgb_led_matrix_sharp.Utils;
using rpi_rgb_led_matrix_sharp.Models;
using rpi_rgb_led_matrix_sharp.Enums;
using System.Collections.Generic;
using System;
using System.Threading;

namespace rpi_rgb_led_matrix_sharp.Helpers
{
    public class CanvasHelper
    {
        private RGBLedMatrix _matrix;
        private RGBLedFont _font;
        private int _matrixWidth = 64;
        private int _matrixHeight = 32;
        private Dictionary<string, List<Glyph>> cachesMappedGlyphs = new Dictionary<string, List<Glyph>>();
        private Dictionary<string, RGBLedCanvas> cachedCanvases = new Dictionary<string, RGBLedCanvas>();

        public CanvasHelper(RGBLedMatrix matrix, int matrixHeight, int matrixWidth, string font)
        {
            this._matrix = matrix;
            this._matrixHeight = matrixHeight;
            this._matrixWidth = matrixWidth;
            SetFont(font);
        }

        public RGBLedCanvas CreateNewCanvas()
        {
            return this._matrix.CreateOffscreenCanvas();
        }

        public RGBLedCanvas DrawVerticallyCenteredText(string lineText, Color color)
        {
            string cachedCanvasKey = $"{lineText}--DrawVerticallyCenteredText--{color.R},{color.B},{color.G}";
            RGBLedCanvas ret = this._matrix.CreateOffscreenCanvas();
            List<Glyph> glyphs;

            if (cachesMappedGlyphs.ContainsKey(lineText))
            {
                glyphs = cachesMappedGlyphs[lineText];
            }
            else
            {
                glyphs = GetLines(lineText, VerticleAlignment.Middle, HorizontalAlignment.Left);
                cachesMappedGlyphs.Add(lineText, glyphs);
            }


            foreach (Glyph glyph in glyphs)
            {
                ret.DrawText(this._font, glyph.X, glyph.Y, color, $"{glyph.Character}");
            }

            cachedCanvases.Add(cachedCanvasKey, ret);

            return ret;
        }

        public RGBLedCanvas DrawHorizontallyCenteredText(string lineText, Color color)
        {
            RGBLedCanvas ret = this._matrix.CreateOffscreenCanvas();
            List<Glyph> glyphs;
            
            if (cachesMappedGlyphs.ContainsKey(lineText))
            {
                glyphs = cachesMappedGlyphs[lineText];
            }
            else
            {
                glyphs = GetLines(lineText, VerticleAlignment.Top, HorizontalAlignment.Center);
                cachesMappedGlyphs.Add(lineText, glyphs);
            }

            foreach (Glyph glyph in glyphs)
            {
                ret.DrawText(this._font, glyph.X, glyph.Y, color, $"{glyph.Character}");
            }

            return ret;
        }

        public RGBLedCanvas DrawCenteredText(string lineText, Color color)
        {
            RGBLedCanvas ret = this._matrix.CreateOffscreenCanvas();
            List<Glyph> glyphs;
           
            if (cachesMappedGlyphs.ContainsKey(lineText))
            {
                glyphs = cachesMappedGlyphs[lineText];
            }
            else
            {
                glyphs = GetLines(lineText, VerticleAlignment.Middle, HorizontalAlignment.Center);
                cachesMappedGlyphs.Add(lineText, glyphs);
            }

            foreach (Glyph glyph in glyphs)
            {
                ret.DrawText(this._font, glyph.X, glyph.Y, color, $"{glyph.Character}");
            }

            return ret;
        }

        private List<Glyph> GetLines(string lineText, VerticleAlignment alignV, HorizontalAlignment alignh)
        {
            return LayoutUtils.LinesToMappedGlyphs(
                LayoutUtils.TextToLines(this._font, this._matrixWidth, lineText),
                this._font.Height(), this._matrixWidth, this._matrixHeight, alignh, alignV);
        }

        public List<RGBLedCanvas> VerticalMarqueeText(string lineText, Color color)
        {
            int yOffset = this._matrixHeight;
            int maxHeight = 0;
            List<Glyph> lines = this.GetLines(lineText, VerticleAlignment.Middle, HorizontalAlignment.Center);
            List<RGBLedCanvas> canvasFrames = new List<RGBLedCanvas>();

            foreach (Glyph glyph in lines)
            {
                maxHeight = glyph.Y + this._font.Height();
            }
            
            while (yOffset > -maxHeight)
            {
                canvasFrames.Add(IncrementMarquee(yOffset, maxHeight, lines, false, color));
                yOffset--;
            }

            return canvasFrames;
        }

        public List<RGBLedCanvas> HorizontalMarqueeText(string lineText, Color color)
        {
            int xOffset = this._matrixWidth;
            int maxWidth = 0;
            List<Glyph> lines = this.GetLines(lineText, VerticleAlignment.Middle, HorizontalAlignment.Center);
            List<RGBLedCanvas> canvasFrames = new List<RGBLedCanvas>();

            foreach (Glyph glyph in lines)
            {
                maxWidth = this._font.Width(lineText);
            }
            
            while (xOffset > -maxWidth)
            {
                canvasFrames.Add(IncrementMarquee(xOffset, maxWidth, lines, true, color));
                xOffset--;
            }

            return canvasFrames;
        }

        private RGBLedCanvas IncrementMarquee(int offset, int stringMeasure, List<Glyph> glyphs,
            bool isHorizontal, Color color)
        {
            RGBLedCanvas ret = this._matrix.CreateOffscreenCanvas();

            foreach (Glyph glyph in glyphs)
            {
                if (isHorizontal)
                {
                    ret.DrawText(this._font, glyph.X + offset, glyph.Y, color, $"{glyph.Character}");
                }
                else
                {
                    ret.DrawText(this._font, glyph.X, glyph.Y + offset, color, $"{glyph.Character}");
                }
            }

            return ret;
        }

        public void SetFont(string fontPath)
        {
            this._font = new RGBLedFont(fontPath);
        }

        public void Clear(RGBLedCanvas canvas = null)
        {
            canvas.Clear();
            
            this._matrix.Clear();
        }

        private void DrawSingleFrame(uint[][] animationFrame, int startX, int startY, bool isTwoBitAnimation, RGBLedCanvas canvas)
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

                            canvas.SetPixel(startX + j, startY + i, color);
                        }
                        else if (animationPixel == 1)
                        {
                            canvas.SetPixel(startX + j, startY + i, new Color(maxColorValue));
                        }
                        else
                        {
                            canvas.SetPixel(startX + j, startY + i, new Color(0));
                        }
                    }
                    else
                    {
                        Color color = new Color(animationPixel);
                        canvas.SetPixel(startX + j, startY + i, color);
                    }
                }
            }
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

        public List<RGBLedCanvas> DoAnimation(uint[][][] animation, int numberOfTimesToShow, bool isTwoBitAnimation, Func<RGBLedMatrix, RGBLedCanvas> action)
        {
            Tuple<int, int> widthAndHeight = FindAnimationFrameWidthAndHeight(animation);
            List<RGBLedCanvas> ret = new List<RGBLedCanvas>();
            
            int pixelSpriteWidth = widthAndHeight.Item1;
            int pixelSpriteHeight = widthAndHeight.Item2;
            int shown = 0;

            while (shown <= numberOfTimesToShow)
            {
                int randomXStart = new Random().Next(0, this._matrixWidth-pixelSpriteWidth);
                int randomYStart = new Random().Next(0, this._matrixHeight-pixelSpriteHeight);

                foreach (uint[][] animationFrame in animation)
                {
                    RGBLedCanvas canvas = action(this._matrix);
                    DrawSingleFrame(animationFrame, randomXStart, randomYStart, isTwoBitAnimation, canvas);
                    ret.Add(canvas);
                }
                
                shown++;
            }

            return ret;
        }

        public List<RGBLedCanvas> RainbowTransition(int heightOfColorBlock, int widthOfColorBlock)
        {
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
            int y = 0;
            int xBoundForFrame = 0;
            int currentColorIndex = 0;
            int yBoundForFrame = heightOfColorBlock;
            List<RGBLedCanvas> ret = new List<RGBLedCanvas>();

            while (xBoundForFrame < this._matrixWidth)
            {
                while(yBoundForFrame < this._matrixHeight)
                {
                    RGBLedCanvas canvas = this._matrix.CreateOffscreenCanvas();

                    for (int y1 = 0; y1 < yBoundForFrame+heightOfColorBlock; y1++)
                    {
                        for (int x1 = 0; x1 < xBoundForFrame + widthOfColorBlock; x1++)
                        {
                            canvas.SetPixel(x1, y1, new Color(colors[currentColorIndex]));
                        }

                        if (yBoundForFrame + y1 % heightOfColorBlock == 0)
                        {
                            currentColorIndex++;

                            if (currentColorIndex == colors.Length - 1)
                            {
                                currentColorIndex = 0;
                            }
                        }
                    }

                    ret.Add(canvas);
                    yBoundForFrame++;

                    if (yBoundForFrame > this._matrixHeight)
                    {
                        y = 0;
                    }
                }

                yBoundForFrame = 0;
                xBoundForFrame+=widthOfColorBlock;
            }

            while (xBoundForFrame > -1)
            {
                while (yBoundForFrame > -1)
                {
                    RGBLedCanvas canvas = this._matrix.CreateOffscreenCanvas();

                    for (int y1 = 0; y1 < yBoundForFrame+heightOfColorBlock; y1++)
                    {
                        for (int x1 = 0; x1 < xBoundForFrame + widthOfColorBlock; x1++)
                        {
                            canvas.SetPixel(x1, y1, new Color(colors[currentColorIndex]));
                        }

                        if (yBoundForFrame + y1 % heightOfColorBlock == 0)
                        {
                            currentColorIndex--;

                            if (currentColorIndex < 0)
                            {
                                currentColorIndex = colors.Length - 1;
                            }
                        }
                    }

                    ret.Add(canvas);
                    yBoundForFrame--;

                    if (y < 0)
                    {
                        y = this._matrixHeight;
                    }
                }

                yBoundForFrame = this._matrixHeight;
                xBoundForFrame -= widthOfColorBlock;
            }

            return ret;
        }
    }
}
