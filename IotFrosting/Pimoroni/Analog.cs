using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotFrosting.Pimoroni
{
    /// <summary>
    /// Interface for an analog input with an auto light
    /// </summary>
    /// <remarks>
    /// Incomplete. Analog inputs are not yet implemented.
    /// </remarks>
    public interface IAnalogInput : IAutoLight
    {
        double Value { get; }

        /// <summary>
        /// Call regularly to update the autolight
        /// </summary>
        void Tick();
    };

    public class AnalogInput : IAnalogInput
    {
        public double Value => Values[Channel];

        public bool AutoLight { get; set; }

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
                Light.State = (Value > 0);
        }

        public AnalogInput(int channel, ILight light)
        {
            Light = light;
            Channel = channel;
        }

        private int Channel;

        /// <summary>
        /// All of the analog input values 
        /// </summary>
        public static double[] Values = new double[NumberOfAnalogInputs];

        /// <summary>
        /// How many total lights are there in an SN3218 bank
        /// </summary>
        private const int NumberOfAnalogInputs = 4;
    }
}
