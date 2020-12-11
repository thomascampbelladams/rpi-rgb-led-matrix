using rpi_rgb_led_matrix_sharp;
using rpi_rgb_led_matrix_sharp.Helpers;
using rpi_rgb_led_matrix_sharp.Models;
using System;

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
            CanvasHelper screen = new CanvasHelper(matrix, 32, 64, args[0]);
            Scene testScene1 = new Scene(matrix, 60, screen.HorizontalMarqueeText(text, new Color(12237498)));
            Scene testScene2 = new Scene(matrix, 60, screen.VerticalMarqueeText(text, new Color(10000536)));

            while (true)
            {
                testScene1.Render();
                testScene2.Render();

                if (Console.KeyAvailable)
                {
                    break;
                }
            }

            return 0;
        }
    }
}
