namespace IotFrosting.Pimoroni
{
    /// <summary>
    /// Interface for a digital output line with an autolight
    /// </summary>
    public interface IDigitalOutput : IOutputPin, IAutoLight
    {
    }

    /// <summary>
    /// An output pin with a light showing its current state
    /// </summary>
    public class Output: OutputPin, IDigitalOutput
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pin">Unerlying GPIO pin to output values</param>
        /// <param name="autolight">Light which should show our current state</param>
        public Output(int pin, ILight autolight): base(pin)
        {
            Light = autolight;
            base.Updated += (s, e) =>
            {
                if (AutoLight)
                    Light.State = base.State;
            };
        }

        /// <summary>
        /// Whether the autolight should in fact be updated with our state
        /// </summary>
        public bool AutoLight { get; set; } = true;

        /// <summary>
        /// The light which shows our state automatically
        /// </summary>
        public ILight Light { get; private set; }
    }
}
