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
    public interface IAnalogInput : IAutoLight
    {
        /// <summary>
        /// Current voltage on the input
        /// </summary>
        double Voltage { get; }

        /// <summary>
        /// Call regularly to update the autolight
        /// </summary>
        void Tick();
    };

    public class AnalogInput : IAnalogInput
    {
        /// <summary>
        /// Current voltage on the input
        /// </summary>
        public double Voltage => Values[Channel] * MaxVoltage;

        /// <summary>
        /// Whether the autolight should in fact be updated with our state
        /// </summary>
        public bool AutoLight { get; set; } = true;

        /// <summary>
        /// The light which shows our state automatically
        /// </summary>
        public ILight Light { get; private set; }

        /// <summary>
        /// Call regularly to update the status of the auto light. The auto light will be
        /// set to PWM brightness corresponding to the analog input level.
        /// </summary>
        /// <remarks>
        /// Recommend calling on your timer tick
        /// </remarks>
        public void Tick()
        {
            if (AutoLight && Light != null)
                Light.Value = Values[Channel];
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="channel">Which ADC channel (0-4) is the input connected to </param>
        /// <param name="maxvoltage">The voltage which would drive a 1.0 reading on the underlying ADC</param>
        /// <param name="light">The light which will show status</param>
        public AnalogInput(int channel, double maxvoltage, ILight light)
        {
            if (channel < 0 || channel > NumberOfAnalogInputs)
                throw new ArgumentException("Invalid channel", nameof(channel));

            Light = light;
            Channel = channel;
            MaxVoltage = maxvoltage;
        }

        /// <summary>
        /// Which ADC channel (0-4) is the input connected to 
        /// </summary>
        private int Channel;

        /// <summary>
        /// The voltage which would drive a 1.0 reading on the underlying ADC
        /// </summary>
        private double MaxVoltage;

        /// <summary>
        /// All of the analog input values 
        /// </summary>
        public static double[] Values = new double[NumberOfAnalogInputs];

        /// <summary>
        /// How many total lights are there in an ADS1015 bank
        /// </summary>
        private const int NumberOfAnalogInputs = 4;
    }
}
