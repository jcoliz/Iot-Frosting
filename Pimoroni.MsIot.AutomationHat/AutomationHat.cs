using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public interface IRelay: IOutputPin
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
        ILight Power = new Light(17);
        ILight Comms = new Light(16);
        ILight Warn = new Light(15);
    }
    public static class AutomationHat
    {
        public static List<IAnalogInput> Analog
        {
            get { throw new NotImplementedException(); }
        }
        public static List<IDigitalInput> Input = new List<IDigitalInput>()
        {
            new Input(26, new Light(14)),
            new Input(20, new Light(13)),
            new Input(21, new Light(12)),
        };
        public static List<IDigitalOutput> Output = new List<IDigitalOutput>()
        {
            new Output(5, new Light(3)),
            new Output(12, new Light(4)),
            new Output(6, new Light(5))
        };
        public static IList<IRelay> Relay = new List<IRelay>()
        {
            new Relay(13, new Light(6), new Light(7) ),
            new Relay(19, new Light(8), new Light(9) ),
            new Relay(16, new Light(10), new Light(11) )
        };
        public static Lights Light = new Lights();

        /// <summary>
        /// Call this regularly from a timer thread to update the state of
        /// everything
        /// </summary>
        public static void Tick()
        {
            Input.ForEach(x=>x.Tick());
        }
    };
}
