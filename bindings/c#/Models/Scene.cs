using System.Collections.Generic;
using System.Threading;

namespace rpi_rgb_led_matrix_sharp.Models
{
    /// <summary>
    /// Represents a complete animation block
    /// </summary>
    public class Scene
    {
        /// <summary>
        /// The frames of the animation
        /// </summary>
        public List<RGBLedCanvas> Frames { get; private set; }
        /// <summary>
        /// How long to delay between frames
        /// </summary>
        public int FrameDelay { get; private set; }
        /// <summary>
        /// Matrix to display the animations on
        /// </summary>
        public RGBLedMatrix Matrix { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Matrix">Matrix to display the animations on</param>
        /// <param name="FrameDelay">How long to delay between frames</param>
        /// <param name="Frames">The frames of the animation</param>
        public Scene(RGBLedMatrix Matrix, int FrameDelay, List<RGBLedCanvas> Frames)
        {
            this.Frames = Frames;
            this.FrameDelay = FrameDelay;
            this.Matrix = Matrix;
        }

        /// <summary>
        /// Renders the scene.
        /// </summary>
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
