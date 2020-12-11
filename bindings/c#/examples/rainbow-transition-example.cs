using rpi_rgb_led_matrix_sharp.Helpers;
using rpi_rgb_led_matrix_sharp;
using rpi_rgb_led_matrix_sharp.Models;
using System.Threading;
using System.Collections.Generic;
using System;

namespace rainbow_transition_example
{
    class Program
    {
        static int Main(string[] args)
        {
            RGBLedMatrix matrix = new RGBLedMatrix(new RGBLedMatrixOptions
            {
                Rows = 32,
                Cols = 64,
                HardwareMapping = "adafruit-hat"
            });
            CanvasHelper screen = new CanvasHelper(matrix, 32, 64, "../../../fonts/4x6.bdf");
            List<RGBLedCanvas> fireworkFrames = screen.RainbowTransition(4, 4);

            while (true)
            {
                Console.WriteLine("Showing rainbow");
                foreach (RGBLedCanvas canvas in fireworkFrames)
                {
                    matrix.SwapOnVsync(canvas);

                    Thread.Sleep(10);
                }
            }

            return 0;
        }
    }
}
