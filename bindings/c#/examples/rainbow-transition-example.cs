using rpi_rgb_led_matrix_sharp.Helpers;
using rpi_rgb_led_matrix_sharp;
using rpi_rgb_led_matrix_sharp.Models;
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
            Scene rainbowScene = new Scene(matrix, 10, screen.RainbowTransition(4, 4));

            while (true)
            {
                rainbowScene.Render();

                if (Console.KeyAvailable)
                {
                    break;
                }
            }

            return 0;
        }
    }
}
