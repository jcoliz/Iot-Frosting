using System;

namespace IotFrosting.Pimoroni
{
    /// <summary>
    /// Interface for a light with PWM control
    /// </summary>
    /// <remarks>
    /// Useful for dependency injection
    /// </remarks>
    public interface ILight
    {
        /// <summary>
        /// Whether the light is currently lit
        /// </summary>
        bool State { get; set; }

        /// <summary>
        /// Toggle the state
        /// </summary>
        void Toggle();

        /// <summary>
        /// Value from 0.0-1.0 for an analog light
        /// </summary>
        double Value { get; set; }

        /// <summary>
        /// Default brightness value 0.0-1.0 when the light is lit
        /// </summary>
        double Brightness { get; set; }
    }

    /// <summary>
    /// Interface for something with an automatic light.
    /// </summary>
    /// <remarks>
    /// If you implement this interface, YOU are responsible for setting the light state
    /// </remarks>
    public interface IAutoLight
    {
        /// <summary>
        /// True if the light should be automatically set to match the state of the
        /// undelying component
        /// </summary>
        bool AutoLight { get; set; }

        /// <summary>
        /// Access to the light related this component, so you can set it on or off yourself
        /// </summary>
        ILight Light { get; }
    }

    /// <summary>
    /// A light with PWM control, which is part of a bank of 18 lights.
    /// Suitable for use with SN3218.
    /// </summary>
    public class Light: ILight
    {
        /// <summary>
        /// Digital state, true if on, false if off
        /// </summary>
        public bool State
        {
            get
            {
                return !(0.0 == Value);
            }
            set
            {
                if (value)
                    Value = Brightness;
                else
                    Value = 0.0;
            }
        }

        /// <summary>
        /// PWM brightness value (0.0-1.0)
        /// </summary>
        public double Value
        {
            get
            {
                return Values[Number];
            }
            set
            {
                Values[Number] = value;
                if (value > 0.0)
                    Brightness = value;

                Updated?.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// How bright the light is when it's on
        /// </summary>
        public double Brightness { get; set; } = 1.0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="number">Which light number on the SN3218 bank are we</param>
        public Light(int number)
        {
            Number = number;
            State = false;
        }

        /// <summary>
        /// Raised when the value of the light changes
        /// </summary>
        public event EventHandler<EventArgs> Updated;

        public void Toggle()
        {
            State = !State;
        }

        /// <summary>
        /// Which light ## are we on the SM3218 bank?
        /// </summary>
        private int Number;

        /// <summary>
        /// All of the light values 
        /// </summary>
        public static double[] Values = new double[NumberOfLights];

        /// <summary>
        /// How many total lights are there in an SN3218 bank
        /// </summary>
        private static int NumberOfLights = 18;
    }

    /// <summary>
    /// A directly connected light (like an LED)
    /// </summary>
    /// <remarks>
    /// Wire the LED between the pin and VCC. pin to ground should turn light on.
    /// </remarks>
    public class DirectLight : OutputPin, ILight
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pin">Which GPIO pin we are wired to</param>
        /// <param name="activelow">Whether the light is wired between pin and VCC (true) or pin and GND (false)</param>
        public DirectLight(int pin,bool activelow = true): base(pin)
        {
            ActiveLow = activelow;
        }

        /// <summary>
        /// Current value of the light (0.0-1.0)
        /// </summary>
        /// <remarks>
        /// Currently only 0.0 and 1.0 are supported, as this is a digital light.
        /// For the sake of the ILight interface, we take a double value
        /// </remarks>
        public double Value
        {
            get
            {
                return State ? 1.0 : 0.0;
            }

            set
            {
                if (value == 0.0)
                    State = false;
                else
                    State = true;
            }
        }

        /// <summary>
        /// Whether the light is currently lit
        /// </summary>
        public override bool State
        {
            get
            {
                return base.State != ActiveLow;
            }

            set
            {
                base.State = value != ActiveLow;
            }
        }

        /// <summary>
        /// Implemented for the interface. Ignored for a digital light.
        /// </summary>
        public double Brightness { get; set; }

        /// <summary>
        /// Whether the light is wired between pin and VCC (true) or pin and GND (false)
        /// </summary>
        private bool ActiveLow;
    }

    /// <summary>
    /// General-purpose automatic light which follows any pin
    /// </summary>
    public class SingleAutoLight : IAutoLight
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">Pin to show status for</param>
        /// <param name="inverted">Whether we are inverted from the usual logic. True if we show false when source is true.</param>
        /// <param name="light">The particular light to control</param>
        public SingleAutoLight(IPin source, bool inverted, ILight light)
        {
            Light = light;
            Inverted = inverted;
            Source = source;
            Light.State = false;

            source.Updated += (s, e) =>
            {
                if (AutoLight)
                    Light.State = source.State != Inverted;
            };
        }

        /// <summary>
        /// The particular light we are controlling
        /// </summary>
        public ILight Light { get; private set; }

        /// <summary>
        /// Whether we are connected to our controlled source
        /// </summary>
        public bool AutoLight
        {
            get { return _AutoLight; }
            set
            {
                _AutoLight = value;
                if (_AutoLight)
                    Light.State = Source.State != Inverted;
                else
                    Light.State = false;
            }
        }
        private bool _AutoLight = false;

        /// <summary>
        /// Whether we are inverted from the usual logic. True if we show false when source is true
        /// </summary>
        private bool Inverted;

        /// <summary>
        /// Pin to show status for
        /// </summary>
        private IPin Source;
    }

}
