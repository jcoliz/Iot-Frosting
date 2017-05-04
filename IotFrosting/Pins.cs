using System;
using Windows.Devices.Gpio;

namespace IotFrosting
{
    /// <summary>
    /// The most basic kind of input, which can be used for more complex types
    /// </summary>
    public interface IInput
    {
        /// <summary>
        /// Whether the line is currently HIGH
        /// </summary>
        bool State { get; }

        /// <summary>
        /// Raised when State changes
        /// </summary>
        event InputUpdateEventHandler Updated;
    }

    /// <summary>
    /// Delegate for use in IInput.Updated event
    /// </summary>
    /// <param name="sender">The input being update</param>
    /// <param name="args">Empty ags for now, may be extended in the future.</param>
    public delegate void InputUpdateEventHandler(IInput sender, EventArgs args);

    /// <summary>
    /// Minimum generic interface for all pins, input or output
    /// </summary>
    /// <remarks>
    /// Useful for dependency injection
    /// </remarks>
    public interface IPin: IInput, IDisposable
    {
    }

    /// <summary>
    /// Generic interface for output pins
    /// </summary>
    /// <remarks>
    /// Useful for dependency injection
    /// </remarks>
    public interface IOutputPin: IPin
    {
        /// <summary>
        /// Whether the line is currently HIGH
        /// </summary>
        new bool State { get; set; }

        /// <summary>
        /// Toggle the state
        /// </summary>
        void Toggle();
    }

    /// <summary>
    /// Wraps the platform GpioPin with extra functionality
    /// </summary>
    public class OutputPin : IOutputPin
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pin">Which pin to control</param>
        public OutputPin(int pin)
        {
            Pin = GpioController.GetDefault().OpenPin(pin);
            Pin.SetDriveMode(GpioPinDriveMode.Output);
            Pin.Write(_Value);
        }

        /// <summary>
        /// Raised when the pin changes state
        /// </summary>
        public event InputUpdateEventHandler Updated;

        /// <summary>
        /// Current state of the pin, true is High
        /// </summary>
        public virtual bool State
        {
            get { return GpioPinValue.High == _Value; }
            set
            {
                _Value = value ? GpioPinValue.High : GpioPinValue.Low;
                Pin.Write(_Value);
                Updated?.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// Toggle the state
        /// </summary>
        public void Toggle()
        {
            State = !State;
        }

        /// <summary>
        /// Return the pin back to the system, we're done!
        /// </summary>
        public void Dispose()
        {
            Pin.Dispose();
        }

        /// <summary>
        /// The underlying pin we are controlling
        /// </summary>
        public GpioPin Pin;

        private GpioPinValue _Value = GpioPinValue.Low;
    }

    /// <summary>
    /// Wraps the platform GpioPin with extra functionality
    /// </summary>
    public class InputPin : IPin
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pin">Which pin to control</param>
        /// <param name="pulldown">Whether we pull down to ground (true) or up to 3v3 (false)</param>
        public InputPin(int pin, bool pulldown=true)
        {
            Pin = GpioController.GetDefault().OpenPin(pin);
            if (pulldown)
                Pin.SetDriveMode(GpioPinDriveMode.InputPullDown);
            else
                Pin.SetDriveMode(GpioPinDriveMode.InputPullUp);

            Pin.DebounceTimeout = TimeSpan.FromMilliseconds(20);
            Pin.ValueChanged += (s, e) =>
            {
                Updated?.Invoke(this, new EventArgs());
            };
        }

        /// <summary>
        /// Raised when the value is updated
        /// </summary>
        public event InputUpdateEventHandler Updated;

        /// <summary>
        /// Current state of the pin, true is high
        /// </summary>
        public bool State => Pin.Read() == GpioPinValue.High;

        /// <summary>
        /// The underlying pin we are controlling
        /// </summary>
        public GpioPin Pin;

        /// <summary>
        /// Return the pin to the system
        /// </summary>
        public void Dispose()
        {
            Pin.Dispose();
        }
    }
}
