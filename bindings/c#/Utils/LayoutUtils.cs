using System;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using rpi_rgb_led_matrix_sharp.Enums;
using rpi_rgb_led_matrix_sharp.Models;

namespace rpi_rgb_led_matrix_sharp.Utils
{
    /// <summary>
    /// Utilities for laying out text for a <see cref="RGBLedCanvas"/>
    /// </summary>
    public static class LayoutUtils
    {
        /// <summary>
        /// Returns a subsection of the passed in list
        /// </summary>
        /// <typeparam name="T">Type used by the list</typeparam>
        /// <param name="source">List to slice</param>
        /// <param name="from">Starting index</param>
        /// <param name="to">Ending index</param>
        /// <returns>Sliced list</returns>
        private static IEnumerable<T> Slice<T>(List<T> source, int from, int to = -1)
        {
            if (to == -1)
            {
                return source.GetRange(from, source.Count - from);
            }
            return source.GetRange(from, to - from);
        }

        /// <summary>
        /// Determines if character passed in is a whitespace character
        /// </summary>
        /// <param name="c">Character to test</param>
        /// <returns>True is whitespace character, false otherwise</returns>
        private static bool IsSeperator(char c)
        {
            return c == ' ';
        }

        /// <summary>
        /// Gets the index of the next whitespace character
        /// </summary>
        /// <param name="glyphs">Glyphs to scan</param>
        /// <returns>index of the white space character</returns>
        private static int GetIndexOfWhiteSpace(List<Glyph> glyphs)
        {
            IEnumerable<char> glyphIenum = glyphs.Where((glyph, i) =>
            {
                return !(i == 0 && IsSeperator(glyph.Character));
            })
            .Select(glyph =>
            {
                return glyph.Character;
            });

            return new string(glyphIenum.ToArray()).IndexOf(' ');
        }

        /// <summary>
        /// Takes a 2D list of glyphs and translates it into a single list with the appropriate x/y coordinates.
        /// </summary>
        /// <param name="glyphs"></param>
        /// <returns></returns>
        private static List<List<Glyph>> GlphysToWords(List<Glyph> glyphs)
        {
            int index = GetIndexOfWhiteSpace(glyphs);
            List<Glyph> leftOverGlyphs = glyphs;
            List<List<Glyph>> ret = new List<List<Glyph>>();

            if (index > 0)
            {
                ret.Add(Slice<Glyph>(leftOverGlyphs, 0, index).ToList());

                while (index > 0)
                {
                    leftOverGlyphs = Slice<Glyph>(leftOverGlyphs, index).ToList();
                    index = GetIndexOfWhiteSpace(leftOverGlyphs);
                    ret.Add(Slice<Glyph>(leftOverGlyphs, 0, index).ToList());
                }
            }
            else
            {
                ret.Add(Slice<Glyph>(leftOverGlyphs, 0, index).ToList());
            }

            return ret;
        }

        /// <summary>
        /// Returns pixel width for word.
        /// </summary>
        /// <param name="glyphList">List of glyphs representing the word</param>
        /// <returns>Pixel width for word</returns>
        private static int CalcWordWidth(List<Glyph> glyphList)
        {
            int ret = 0;

            foreach (Glyph glyph in glyphList)
            {
                ret += glyph.Width;
            }

            return ret;
        }

        /// <summary>
        /// Converts a list of words to a list of lines with words
        /// </summary>
        /// <param name="maxWidth">Maxmimum width for the text. Used to determine text wrapping</param>
        /// <param name="words">Words to put into lines</param>
        /// <returns>Lines of words</returns>
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
                    lines.Add(tmpLine);
                    tmpLine = new List<List<Glyph>>();
                    tmpLine.Add(glyphList);
                    tmpLineWidth = CalcWordWidth(glyphList);
                }
                else
                {
                    tmpLine.Add(glyphList);
                    tmpLineWidth += wordWidth;
                }
            }

            if (tmpLine.Count > 0) lines.Add(tmpLine);

            return lines;
        }

        /// <summary>
        /// Takes in text and turns it into lines of words
        /// </summary>
        /// <param name="font">Font to use to render the text</param>
        /// <param name="maxW">Max width to use for text, used for text wrapping</param>
        /// <param name="text">Text to use</param>
        /// <returns>Lines of words</returns>
        public static List<List<List<Glyph>>> TextToLines(RGBLedFont font, int maxW, string text)
        {
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
                    offsetY = Convert.ToInt32(Math.Floor((decimal)((containerH - blockH) / 2)));
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
                            offsetX = Convert.ToInt32(Math.Floor((decimal)((containerW - lineW) / 2)));
                            break;

                        case HorizontalAlignment.Right:
                            offsetX = containerW - lineW;
                            break;
                    }

                    foreach (Glyph glyph in word)
                    {
                        glyph.X = offsetX;
                        glyph.Y = offsetY + i * lineH;

                        offsetX += glyph.Width;
                        ret.Add(glyph);
                    }

                    i++;
                }
            }

            return ret;
        }
    }
}
