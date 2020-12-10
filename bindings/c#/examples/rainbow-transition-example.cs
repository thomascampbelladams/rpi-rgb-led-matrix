using rpi_rgb_led_matrix_sharp.Helpers;

namespace rainbow_transition_example
{
    class Program
    {
        static int Main(string[] args)
        {
            ScreenHelper screen = new ScreenHelper();

            screen.RainbowTransition(1, 4, 4, () => { });

            return 0;
        }
    }
}