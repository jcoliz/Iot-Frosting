namespace IotFrosting.Pimoroni
{
    /// <summary>
    /// Interface for a digital input line with an autolight
    /// </summary>
    public interface IDigitalInput : IPin, IAutoLight
    {
        /// <summary>
        /// Call regularly to update the autolight
        /// </summary>
        void Tick();
    }

    /// <summary>
    /// A digital input with an automatic light showing its state
    /// </summary>
    public class Input: InputPin, IDigitalInput
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pin">The underlying pin we are watching</param>
        /// <param name="light">The automatic light which shows the state</param>
        /// <param name="pulldown">Whether the switch is wired to VCC (true) or GND (false)</param>
        public Input(int pin, ILight light, bool pulldown=true): base(pin,pulldown)
        {
            Light = light;
            pressedhigh = pulldown;
        }

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
                Light.State = (State == pressedhigh);
        }

        /// <summary>
        /// Whether the switch is wired to VCC (true) or GND (false)
        /// </summary>
        private bool pressedhigh;
    }
}
