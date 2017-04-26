using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotFrosting.Pimoroni
{
    /// <summary>
    /// A drum pad, driven by a Cap1xxx
    /// </summary>
    /// <remarks>
    /// This is just a straight Input (including autolight), adding a 'Hit' event,
    /// raised only when the input goes high
    /// </remarks>
    public class Pad: IAutoLight
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="number">Which input are we on the Cap1xxx bank?</param>
        /// <param name="light">Automatic light</param>
        public Pad(int number, ILight light)
        {
            Number = number;
            Light = light;
        }

        /// <summary>
        /// Raised when we are first touched
        /// </summary>
        public event EventHandler<EventArgs> Hit;

        /// <summary>
        /// Whether the autolight should in fact be updated with our state
        /// </summary>
        public bool AutoLight { get; set; } = true;

        /// <summary>
        /// The light which shows our state automatically
        /// </summary>
        public ILight Light { get; private set; }

        /// <summary>
        /// Call regularly to update the status of the auto light
        /// </summary>
        /// <remarks>
        /// Recommend calling on your timer tick
        /// </remarks>
        public void Tick()
        {
            if (AutoLight)
                Light.State = true;  // State 
        }

        /// <summary>
        /// Which input ## are we on the Cap1xxx bank?
        /// </summary>
        private int Number;
    }
}
