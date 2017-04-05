using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Threading;

namespace Pimoroni.MsIot
{
    public interface IInputPin
    {
        /// <summary>
        /// Whether the line is currently HIGH
        /// </summary>
        bool State { get; }
    }
    /// <summary>
    /// Interface for a single digital pin
    /// </summary>
    public interface IOutputPin
    {
        bool State { get; set; }
        void Toggle();

        event EventHandler<EventArgs> Updated;
    }
    /// <summary>
    /// Interface for a light with PWM control
    /// </summary>
    public interface ILight : IOutputPin
    {
        double Value { get; set; }
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
    /// Interface for a digital input line with an autolight
    /// </summary>
    public interface IDigitalInput: IAutoLight, IInputPin
    {
        void Tick();
    }
    /// <summary>
    /// Interface for a digital output line with an autolight
    /// </summary>
    public interface IDigitalOutput: IAutoLight, IOutputPin
    {
    }
    /// <summary>
    /// Interface for a relay with two autolights
    /// </summary>
    public interface IRelay: IOutputPin, IDisposable
    {
        IAutoLight NO { get; }
        IAutoLight NC { get; }
    }
    public interface IAnalogInput: IAutoLight
    {
        int Value { get; }
    }
    public class Lights
    {
        public ILight Power = new Light(17);
        public ILight Comms = new Light(16);
        public ILight Warn = new Light(15);
    }
    public class AutomationHat: IDisposable
    {
        private SN3218 LedController = new SN3218();
        private ThreadPoolTimer Timer;

        /// <summary>
        /// Constructor. Do not call directly. Use AutomationHat.Open()
        /// </summary>
        protected AutomationHat()
        {
        }

        public static async Task<AutomationHat> Open()
        {
            var result = new AutomationHat();
            await result.LedController.Initialize();
            result.LedController.Enable();
            result.LedController.EnableLeds();

            result.Timer = ThreadPoolTimer.CreatePeriodicTimer(x => result.Tick(), TimeSpan.FromMilliseconds(20));
            return result;
        }

        /*
        public List<IAnalogInput> Analog
        {
            get { throw new NotImplementedException(); }
        }
        public List<IDigitalInput> Input = new List<IDigitalInput>()
        {
            new Input(26, new Light(14)),
            new Input(20, new Light(13)),
            new Input(21, new Light(12)),
        };
        public List<IDigitalOutput> Output = new List<IDigitalOutput>()
        {
            new Output(5, new Light(3)),
            new Output(12, new Light(4)),
            new Output(6, new Light(5))
        };
        };*/
        public List<IRelay> Relay = new List<IRelay>()
        {
            new Relay(13, new Light(6), new Light(7) ),
            new Relay(19, new Light(8), new Light(9) ),
            new Relay(16, new Light(10), new Light(11) )
        };
        public Lights Light = new Lights();

        /// <summary>
        /// Call this regularly from a timer thread to update the state of
        /// everything
        /// </summary>
        public void Tick()
        {
            if (!disposing)
            {
                //Input.ForEach(x=>x.Tick());
                LedController.Output(MsIot.Light.Values);
            }
        }

        private bool disposing = false;
        public void Dispose()
        {
            disposing = true;
            Timer.Cancel();
            Relay.ForEach(x => x.Dispose());
            ((IDisposable)LedController).Dispose();
        }
    };
}
