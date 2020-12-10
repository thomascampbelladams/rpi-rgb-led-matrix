using rpi_rgb_led_matrix_sharp;
using System;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using rpi_rgb_led_matrix_sharp.Helpers;

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

            ScreenHelper screen = new ScreenHelper();
            screen.SetFont(args[0]);
            //screen.DrawVerticallyCenteredText(text, new Color(0, 255, 0), true);
            //Thread.Sleep(5000);
            //screen.Clear();
            //screen.DrawHorizontallyCenteredText(text, new Color(0, 255, 0), true);
            //Thread.Sleep(5000);
            //screen.Clear();
            //screen.DrawCenteredText(text, new Color(0, 255, 0), true);

            screen.VerticalMarqueeText(text, new Color(0, 255, 0), 25, false);
            screen.HorizontalMarqueeText(text, new Color(0, 255, 0), 25, false);

            while (!Console.KeyAvailable)
            {
                Thread.Sleep(250);
            }

            return 0;
        }
    }
}
