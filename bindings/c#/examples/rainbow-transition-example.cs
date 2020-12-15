using rpi_rgb_led_matrix_sharp.Helpers;
using rpi_rgb_led_matrix_sharp;
using rpi_rgb_led_matrix_sharp.Models;
using System;

namespace rainbow_transition_example
{
    class Program
    {
        static string signWithAccountKey(string stringToSign, string accountKey)
        {
            var hmacsha = new System.Security.Cryptography.HMACSHA256();
            hmacsha.Key = Convert.FromBase64String(accountKey);
            var signature = hmacsha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(stringToSign));
            return Convert.ToBase64String(signature);
        }

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
            Console.WriteLine($"{System.DateTime.UtcNow.ToString("ddd, dd MMM yyyy HH:mm:ss")} GMT");
            string stringToSign = $"GET\n\ntext/plain; charset=utf-8\n\nx-ms-date:{System.DateTime.UtcNow.ToString("ddd, dd MMM yyyy HH:mm:ss")} GMT\nx-ms-version:2020-04-08\n/solonggaylogstorage/rgbscreenqueue";
            Console.WriteLine(stringToSign);
            string signature = $"bNOF75Ln3i9P8tw7JaX6GsjNT1vWNQsT1MJo7gf2KTKjumfWXlYlIVq/4FVFQxlVghPM34sXWLxjDbjl3n3/ig==";

            Console.WriteLine(signWithAccountKey(stringToSign, signature));

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
