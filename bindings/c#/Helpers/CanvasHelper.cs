using rpi_rgb_led_matrix_sharp;
using rpi_rgb_led_matrix_sharp.Utils;
using rpi_rgb_led_matrix_sharp.Models;
using rpi_rgb_led_matrix_sharp.Enums;
using System.Collections.Generic;
using System;
using System.Threading;

namespace rpi_rgb_led_matrix_sharp.Helpers
{
    /// <summary>
    /// Used to help construct certain animations, like marquees, a rainbow transition, or custom animations defined in frames of pixel color values.
    /// </summary>
    public class CanvasHelper
    {
        private RGBLedMatrix _matrix;
        private RGBLedFont _font;
        private int _matrixWidth = 64;
        private int _matrixHeight = 32;
        private Dictionary<string, List<Glyph>> cachesMappedGlyphs = new Dictionary<string, List<Glyph>>();
        private Dictionary<string, RGBLedCanvas> cachedCanvases = new Dictionary<string, RGBLedCanvas>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="matrix">Matrix to create <see cref="RGBLedCanvas"/> from</param>
        /// <param name="matrixHeight">Height of the matrix screen</param>
        /// <param name="matrixWidth">Width of the matrix screen</param>
        /// <param name="font">Initial font to use</param>
        public CanvasHelper(RGBLedMatrix matrix, int matrixHeight, int matrixWidth, string font)
        {
            this._matrix = matrix;
            this._matrixHeight = matrixHeight;
            this._matrixWidth = matrixWidth;
            SetFont(font);
        }

        /// <summary>
        /// Creates a new <see cref="RGBLedCanvas"/> from the matrix screen
        /// </summary>
        /// <returns>A new canvas</returns>
        public RGBLedCanvas CreateNewCanvas()
        {
            return this._matrix.CreateOffscreenCanvas();
        }

        /// <summary>
        /// Returns a set of canvases with vertically centered text drawn on it
        /// </summary>
        /// <param name="lineText">Text to render</param>
        /// <param name="color">Color for the text</param>
        /// <returns>Canvas with the text on it</returns>
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

        /// <summary>
        /// Returns a <see cref="RGBLedCanvas"/> with horizontally centered text rendered on it.
        /// </summary>
        /// <param name="lineText">Text to render</param>
        /// <param name="color">Color to render the text in</param>
        /// <returns>Canvas with horizontally centered text drawn on it.</returns>
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

        /// <summary>
        /// Returns a <see cref="RGBLedCanvas"/> with centered text rendered on it.
        /// </summary>
        /// <param name="lineText">Text to render</param>
        /// <param name="color">Color to render the text in</param>
        /// <returns>Canvas with horizontally centered text drawn on it.</returns>
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

        /// <summary>
        /// Takes text and returns a list of Glyphs mapped to coordinates with line wrapping taken into account
        /// </summary>
        /// <param name="lineText">Text to render</param>
        /// <param name="alignV">Vertical alignment for the text</param>
        /// <param name="alignh">Horizontal alignment for the text</param>
        /// <returns><see cref="List{Glyph}"/> that are mapped to base coordinates.</returns>
        private List<Glyph> GetLines(string lineText, VerticleAlignment alignV, HorizontalAlignment alignh)
        {
            return LayoutUtils.LinesToMappedGlyphs(
                LayoutUtils.TextToLines(this._font, this._matrixWidth, lineText),
                this._font.Height(), this._matrixWidth, this._matrixHeight, alignh, alignV);
        }

        /// <summary>
        /// Returns a <see cref="List{RGBLedCanvas}"/> that represent the frames for a vertical marquee animation.
        /// </summary>
        /// <param name="lineText">Text to render</param>
        /// <param name="color">Color for the text</param>
        /// <returns>A series of canvases that should show a marquee if rendered in order.</returns>
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

        /// <summary>
        ///  Returns a <see cref="List{RGBLedCanvas}"/> that represent the frames for a horizontal marquee animation.
        /// </summary>
        /// <param name="lineText">Text to render</param>
        /// <param name="color">Color for the text</param>
        /// <returns>A series of canvases that should show a marquee if rendered in order.</returns>
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

        /// <summary>
        /// Returns the next frame for a marquee
        /// </summary>
        /// <param name="offset">coordinate offset to begin with</param>
        /// <param name="stringMeasure">width or height of the string</param>
        /// <param name="glyphs">The glyphs in the string</param>
        /// <param name="isHorizontal">Set to true if this is for a horizontal marquee, false for vertical.</param>
        /// <param name="color">Color of the text to use.</param>
        /// <returns>The next frame in the marquee</returns>
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

        /// <summary>
        /// Sets the font for the next scene to use
        /// </summary>
        /// <param name="fontPath">File path to the BDF font to use</param>
        public void SetFont(string fontPath)
        {
            this._font = new RGBLedFont(fontPath);
        }

        /// <summary>
        /// Clears a canvas and then the matrix
        /// </summary>
        /// <param name="canvas">Canvas to clear</param>
        public void Clear(RGBLedCanvas canvas = null)
        {
            canvas.Clear();
            
            this._matrix.Clear();
        }

        /// <summary>
        /// Draws a single frame of a custom animation
        /// </summary>
        /// <param name="animationFrame"><see cref="uint[][]"/> that represents each pixel in the animation frame. Each <see cref="uint[]"/> represents a horizontal row of pixel color values in the animation. The complete set represents a 2d image.</param>
        /// <param name="startX">X coordinate to begin the left of the animation</param>
        /// <param name="startY">Y coordinate to begin the top of the animation</param>
        /// <param name="isTwoBitAnimation">If true, will instead expect to see a 2d array of 0, 1, and 2. If 0, the pixel color value is black for that pixel. If 1, the pixel color value is white. If 2, the pixel color is random.</param>
        /// <param name="canvas">Canvas to draw animation frame to.</param>
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

        /// <summary>
        /// Finds the width/height of a custom animation.
        /// </summary>
        /// <param name="animation"><see cref="uint[][][]"/> that represents a custom animation.</param>
        /// <returns><see cref="Tuple{int, int}"/> where the first value is width and the second is height.</returns>
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

        /// <summary>
        /// Returns a <see cref="List{RGBLedCanvas}"/> the represents a custom animation when displayed in order
        /// The animation is represented by <see cref="uint[][][]"/>. The <see cref="uint[][]"/> represents a single frame in the animation.
        /// The <see cref="uint[]"/> represents a row of pixels in the animation, where each value is hex color value for the pixel.
        /// The animation will play at a random spot on the screen.
        /// </summary>
        /// <param name="animation">Custom animation as described</param>
        /// <param name="numberOfTimesToShow">Number of times to show the animation</param>
        /// <param name="isTwoBitAnimation">If true, instead of color values, the animation can use 0, 1, or 2 instead. 0 representing black, 1 representing white, and 2 representing a random color.</param>
        /// <param name="action">Function to execute to get the background canvas.</param>
        /// <returns>A series of canvases that when displayed in order will show the animation.</returns>
        public List<RGBLedCanvas> DoAnimation(uint[][][] animation, int numberOfTimesToShow, bool isTwoBitAnimation, Func<RGBLedMatrix, RGBLedCanvas> action)
        {
            Tuple<int, int> widthAndHeight = FindAnimationFrameWidthAndHeight(animation);
            List<RGBLedCanvas> ret = new List<RGBLedCanvas>();
            RGBLedCanvas canvas = action(this._matrix);
            int pixelSpriteWidth = widthAndHeight.Item1;
            int pixelSpriteHeight = widthAndHeight.Item2;
            int shown = 0;

            while (shown <= numberOfTimesToShow)
            {
                int randomXStart = new Random().Next(0, this._matrixWidth-pixelSpriteWidth);
                int randomYStart = new Random().Next(0, this._matrixHeight-pixelSpriteHeight);

                foreach (uint[][] animationFrame in animation)
                {
                    RGBLedCanvas canvas1 = this._matrix.CopyCanvas(canvas);
                    DrawSingleFrame(animationFrame, randomXStart, randomYStart, isTwoBitAnimation, canvas1);
                    ret.Add(canvas1);
                }
                
                shown++;
            }

            return ret;
        }

        /// <summary>
        /// Returns a <see cref="List{RGBLedCanvas}"/> that represents frames for a rainbow animation.
        /// </summary>
        /// <param name="heightOfColorBlock">Maximum height to increment the animation</param>
        /// <param name="widthOfColorBlock">Maximum width to increment the animation</param>
        /// <returns></returns>
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
