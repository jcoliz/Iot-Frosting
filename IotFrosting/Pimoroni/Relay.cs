namespace IotFrosting.Pimoroni
{
    /// <summary>
    /// Interface for a relay with two autolights
    /// </summary>
    public interface IRelay : IOutputPin
    {
        IAutoLight NO { get; }
        IAutoLight NC { get; }
    }

    /// <summary>
    /// Control a relay with two autolights, one for each state
    /// </summary>
    public class Relay: OutputPin, IRelay
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pin">Pin where the relay is connected</param>
        /// <param name="light_no">Light to show state when normally-open side is connected (that is, relay is closed)</param>
        /// <param name="light_nc">Light to show state when normally-closed side is connected (that is, relay is open)</param>
        public Relay(int pin, ILight light_no, ILight light_nc): base(pin)
        {
            // By default the (annoying) NC light is off. You can turn the autolight on if you like
            _NO = new SingleAutoLight(this, false, light_no) { AutoLight = true };
            _NC = new SingleAutoLight(this, true, light_nc);
        }

        /// <summary>
        /// The Normally open light. The IAutoLight is public in the IRelay interface
        /// </summary>
        public IAutoLight NO => _NO;

        /// <summary>
        /// The Normally closed light. The IAutoLight is public in the IRelay interface
        /// </summary>
        public IAutoLight NC => _NC;

        private SingleAutoLight _NO;
        private SingleAutoLight _NC;
    }

}
