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

            RGBLedMatrix matrix = new RGBLedMatrix(new RGBLedMatrixOptions
            {
                Rows = 32,
                Cols = 64,
                HardwareMapping = "adafruit-hat"
            });
            CanvasHelper screen = new CanvasHelper(matrix, 32, 64, "../../../fonts/8x13B.bdf");
            List<RGBLedCanvas> testFrames1;
            List<RGBLedCanvas> testFrames2;

            testFrames1 = screen.HorizontalMarqueeText("HELLO WORLD", new Color(12237498));
            testFrames2 = screen.VerticalMarqueeText("HELLO WORLD", new Color(10000536));

            while (true)
            {
                foreach (RGBLedCanvas canvas in testFrames1)
                {
                    matrix.SwapOnVsync(canvas);
                    Thread.Sleep(60);
                }

                foreach (RGBLedCanvas canvas in testFrames2)
                {
                    matrix.SwapOnVsync(canvas);
                    Thread.Sleep(60);
                }
            }

            while (!Console.KeyAvailable)
            {
                Thread.Sleep(250);
            }

            return 0;
        }
    }
}
