using System.Collections.Generic;
using System.Threading;

namespace rpi_rgb_led_matrix_sharp.Models
{
    public class Scene
    {
        public List<RGBLedCanvas> Frames { get; private set; }
        public int FrameDelay { get; private set; }
        public RGBLedMatrix Matrix { get; private set; }

        public Scene(RGBLedMatrix Matrix, int FrameDelay, List<RGBLedCanvas> Frames)
        {
            this.Frames = Frames;
            this.FrameDelay = FrameDelay;
            this.Matrix = Matrix;
        }

        public void Render()
        {
            foreach (RGBLedCanvas canvas in Frames)
            {
                this.Matrix.SwapOnVsync(canvas);
                Thread.Sleep(FrameDelay);
            }
        }
    }
    
}
