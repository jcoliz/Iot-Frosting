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
        IDigitalOutput Light { get; }
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
    public interface IDigitalOutput: IAutoLight
    {
        bool State { get; set; }
        void Toggle();
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
        IDigitalOutput Power;
        IDigitalOutput Comms;
        IDigitalOutput Warn;
    }
    public static class AutomationHat
    {
        public static List<IAnalogInput> Analog;

        public static List<IDigitalInput> Input;

        public static List<IDigitalOutput> Output;

        public static List<IDigitalOutput> Relay;

        public static Lights Light;
    };
}
