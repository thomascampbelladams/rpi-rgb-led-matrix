using rpi_rgb_led_matrix_sharp;
using rpi_rgb_led_matrix_sharp.Utils;
using rpi_rgb_led_matrix_sharp.Models;
using System.Collections.Generic;

namespace InternetScreenProject
{
    public class ScreenHelper
    {
        private RGBLedMatrix _matrix;
        private RGBLedCanvas _canvas;
        private RGBLedFont _font;
        private int _backgroundColor;
        private int _matrixWidth = 64;
        private int _matrixHeight = 32;
        private string _hardwareMapping = "adafruit-hat"
        private string _currentFontName;

        public ScreenHelper()
        {
            this._matrix = new RGBLedMatrix(new RGBLedMatrixOptions
            {
                Rows = this._matrixHeight,
                Cols = this._matrixWidth,
                HardwareMapping = this._hardwareMapping
            });

            this._canvas = this._matrix.CreateOffscreenCanvas();
        }

        public DrawVerticallyCenteredTest(string lineText, int color, bool doASync)
        {
            
        }
    }
}
