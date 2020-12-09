using System;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using rpi_rgb_led_matrix_sharp.Enums;
using rpi_rgb_led_matrix_sharp.Models;

namespace rpi_rgb_led_matrix_sharp.Utils
{
    public static class LayoutUtils
    {
        private static bool debug = true;

        private static IEnumerable<T> Slice<T>(List<T> source, int from, int to = -1)
        {
            if (to == -1)
            {
                return source.GetRange(from, source.Count - from);
            }
            return source.GetRange(from, to - from);
        }


        private static bool IsSeperator(char c)
        {
            return c == ' ';
        }

        private static int GetIndexOfWhiteSpace(List<Glyph> glyphs)
        {
            IEnumerable<char> glyphIenum = glyphs.Where((glyph, i) =>
            {
                if (debug) Console.WriteLine($"Found glyph: {glyph.Character} at index: {i}");

                return !(i == 0 && IsSeperator(glyph.Character));
            })
            .Select(glyph =>
            {
                if (debug) Console.WriteLine($"Selecting Glyph {glyph.Character}");

                return glyph.Character;
            });

            char[] charArray = glyphIenum.ToArray();

            if (debug) Console.WriteLine($"Found this Ienum for glyphs {glyphIenum}");
            if (debug) Console.WriteLine($"This is it as an array {charArray}");
            if (debug) Console.WriteLine($"This is it as a string {new string(charArray)}");

            return new string(charArray).IndexOf(' ');
        }

        private static List<List<Glyph>> GlphysToWords(List<Glyph> glyphs)
        {
            int index = GetIndexOfWhiteSpace(glyphs);
            List<Glyph> leftOverGlyphs = glyphs;
            List<List<Glyph>> ret = new List<List<Glyph>>();

            if (index > 0)
            {
                if (debug) Console.WriteLine($"Found index past zero");
                ret.Add(Slice<Glyph>(leftOverGlyphs, 0, index).ToList());

                while (index > 0)
                {
                    List<Glyph> glyphs1 = Slice<Glyph>(leftOverGlyphs, 0, index).ToList();

                    if (debug) Console.WriteLine($"Found word with {glyphs1.Count} letters");
                    leftOverGlyphs = Slice<Glyph>(leftOverGlyphs, index).ToList();
                    index = GetIndexOfWhiteSpace(leftOverGlyphs);
                    ret.Add(Slice<Glyph>(leftOverGlyphs, 0, index).ToList());
                }
            }
            else
            {
                if (debug) Console.WriteLine($"Didn't Find index past zero");
                ret.Add(Slice<Glyph>(leftOverGlyphs, 0, index).ToList());
            }

            return ret;
        }

        private static int CalcWordWidth(List<Glyph> glyphList)
        {
            int ret = 0;

            foreach (Glyph glyph in glyphList)
            {
                ret += glyph.Width;
            }

            return ret;
        }

        private static List<List<List<Glyph>>> WordToLines(int maxWidth, List<List<Glyph>> words)
        {
            List<List<List<Glyph>>> lines = new List<List<List<Glyph>>>();
            List<List<Glyph>> tmpLine = new List<List<Glyph>>();
            int tmpLineWidth = 0;
            IEnumerable<List<Glyph>> glyphLists = words.Where(glyphList => glyphList.Count > 0)
                .Select(glyphList => glyphList);

            foreach (List<Glyph> glyphList in glyphLists)
            {
                int wordWidth = CalcWordWidth(glyphList);

                if (tmpLineWidth + wordWidth > maxWidth)
                {
                    if (debug) Console.WriteLine($"new line: tmpLineWidth: {tmpLineWidth} wordWidth: {wordWidth} maxWidth {maxWidth}");
                    lines.Add(tmpLine);
                    tmpLine = new List<List<Glyph>>();
                    tmpLine.Add(glyphList);
                    tmpLineWidth = CalcWordWidth(glyphList);
                }
                else
                {
                    if (debug) Console.WriteLine($"same line: tmpLineWidth: {tmpLineWidth} wordWidth: {wordWidth} maxWidth {maxWidth}");
                    tmpLine.Add(glyphList);
                    tmpLineWidth += wordWidth;
                }
            }

            if (tmpLine.Count > 0) lines.Add(tmpLine);

            return lines;
        }

        public static List<List<List<Glyph>>> TextToLines(RGBLedFont font, int maxW, string text)
        {
            int fontHeight = font.Height();
            List<Glyph> glyphs = text.Select(c => c).Select(c => new Glyph(c, font)).ToList<Glyph>();

            return WordToLines(maxW, GlphysToWords(glyphs));
        }

        public static List<Glyph> LinesToMappedGlyphs(List<List<List<Glyph>>> lines, int lineH, int containerW,
            int containerH, HorizontalAlignment alignH = HorizontalAlignment.Center,
            VerticleAlignment alignV = VerticleAlignment.Middle)
        {
            int blockH = lineH * lines.Count;
            int offsetY = 0;
            int i = 0;
            List<Glyph> ret = new List<Glyph>();

            switch (alignV)
            {
                case VerticleAlignment.Middle:
                    offsetY = Convert.ToInt32(Math.Floor((double)((containerH - blockH) / 2)));
                    break;
                case VerticleAlignment.Bottom:
                    offsetY = containerH - blockH;
                    break;
            }

            foreach (List<List<Glyph>> line in lines)
            {
                foreach (List<Glyph> word in line)
                {
                    int lineW = CalcWordWidth(word);
                    int offsetX = 0;

                    switch (alignH)
                    {
                        case HorizontalAlignment.Center:
                            offsetX = Convert.ToInt32(Math.Floor((double)((containerW - lineW) / 2)));
                            break;

                        case HorizontalAlignment.Right:
                            offsetX = containerW - lineW;
                            break;
                    }

                    foreach (Glyph glyph in word)
                    {
                        glyph.X = offsetX;
                        if (debug) Console.WriteLine($"offsetY: {offsetY} i: {i}, lineH: {lineH}");
                        glyph.Y = offsetY + i * lineH;

                        offsetX += glyph.Width;
                        ret.Add(glyph);
                    }

                    i++;
                }
            }

            return ret;
        }

        private static int FindHorizontalAlignment(HorizontalAlignment align, RGBLedFont font, string message)
        {
            return Convert.ToInt32(Math.Floor((double)((64 - font.Width(message)) / 2)));
        }
    }
}