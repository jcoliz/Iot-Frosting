using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pimoroni.MsIot
{
    /// <summary>
    /// Interface for something with an automatic light
    /// </summary>
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
        IRawDigitalOutput Light { get; }
    }
    /// <summary>
    /// Interface for a digital input line
    /// </summary>
    public interface IDigitalInput: IAutoLight
    {
        /// <summary>
        /// Whether the line is currently HIGH
        /// </summary>
        bool State { get; }
    }
    public interface IRawDigitalOutput 
    {
        bool State { get; set; }
        void Toggle();
    }
    public interface IDigitalOutput: IAutoLight, IRawDigitalOutput
    {
    }
    public interface IAnalogInput: IAutoLight
    {
        int Value { get; }
    }
    public interface IAnalogOutput : IAutoLight
    {
        int Value { get; set; }
    }
    public class Lights
    {
        IRawDigitalOutput Power = new Light(17);
        IRawDigitalOutput Comms = new Light(16);
        IRawDigitalOutput Warn = new Light(15);
    }
    public static class AutomationHat
    {
        public static List<IAnalogInput> Analog;
        public static List<IDigitalInput> Input;
        public static List<IDigitalOutput> Output;
        public static IList<IDigitalOutput> Relay = new List<IDigitalOutput>()
        {
            new Relay(13, new Light(6), new Light(7) ),
            new Relay(19, new Light(8), new Light(9) ),
            new Relay(16, new Light(10), new Light(11) )
        };
        public static Lights Light;
    };
}
