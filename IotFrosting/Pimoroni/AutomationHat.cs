using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.System.Threading;

namespace IotFrosting.Pimoroni
{
    /// <summary>
    /// The master control class for a single Automation Hat
    /// </summary>
    public class AutomationHat: IDisposable
    {
        /// <summary>
        /// Open a connection to the hat
        /// </summary>
        /// <returns>An AutomstionHat object you can use to control the hat</returns>
        public static async Task<AutomationHat> Open()
        {
            var result = new AutomationHat();
            result.LedController = await SN3218.Open();
            result.LedController.Enable();
            result.LedController.EnableLeds();
            result.Light.Power.State = true;

            result.AnalogController = await ADS1015.Open();

            // Debugging, so no timer rightnow.
            result.FastTimer = ThreadPoolTimer.CreatePeriodicTimer(x => result.FastTick(), TimeSpan.FromMilliseconds(20));
            result.SlowTimer = ThreadPoolTimer.CreatePeriodicTimer(x => result.SlowTick(), TimeSpan.FromMilliseconds(100));
            return result;
        }

        /// <summary>
        /// The analog inputs
        /// </summary>
        public List<IAnalogInput> Analog = new List<IAnalogInput>()
        {
            new ADS1015.Input(0, 25.85, new SN3218.Light(0)),
            new ADS1015.Input(1, 25.85, new SN3218.Light(1)),
            new ADS1015.Input(2, 25.85, new SN3218.Light(2)),
            new ADS1015.Input(3, 3.3, null)
        };

        /// <summary>
        /// The digital inputs
        /// </summary>
        public List<IDigitalInput> Input = new List<IDigitalInput>()
        {
            new Input(26, new SN3218.Light(14)),
            new Input(20, new SN3218.Light(13)),
            new Input(21, new SN3218.Light(12)),
        };

        /// <summary>
        /// The outputs
        /// </summary>
        public List<IDigitalOutput> Output = new List<IDigitalOutput>()
        {
            new Output(5, new SN3218.Light(3)),
            new Output(12, new SN3218.Light(4)),
            new Output(6, new SN3218.Light(5))
        };

        /// <summary>
        /// The relays
        /// </summary>
        public List<IRelay> Relay = new List<IRelay>()
        {
            new Relay(13, new SN3218.Light(6), new SN3218.Light(7) ),
            new Relay(19, new SN3218.Light(8), new SN3218.Light(9) ),
            new Relay(16, new SN3218.Light(10), new SN3218.Light(11) )
        };

        /// <summary>
        /// The user-controllable lights
        /// </summary>
        public Lights Light = new Lights();

        /// <summary>
        /// Called regularly from our own internal timer thread to update the state of
        /// everything
        /// </summary>
        private void FastTick()
        {
            if (!disposing)
            {                
                Analog.ForEach(x => x.Tick());
                Input.ForEach(x => x.Tick());
                LedController.Output(SN3218.Light.Values);
            }
        }

        /// <summary>
        /// Called regularly from our own internal timer, less frequently
        /// </summary>
        /// <remarks>
        /// This is used for the ADC which takes some time, so we don't want the next
        /// interval coming along while we're still workign on the current one
        /// </remarks>
        private async void SlowTick()
        {
            if (!disposing)
            {
                await AnalogController.ReadInto(ADS1015.Input.Values);
            }
        }

        private SN3218 LedController;
        private ADS1015 AnalogController;
        private ThreadPoolTimer FastTimer;
        private ThreadPoolTimer SlowTimer;

        /// <summary>
        /// Constructor. Do not call directly. Use AutomationHat.Open()
        /// </summary>
        protected AutomationHat()
        {
        }

        private bool disposing = false;
        public void Dispose()
        {
            disposing = true;
            FastTimer.Cancel();
            Relay.ForEach(x => x.Dispose());
            Input.ForEach(x => x.Dispose());
            Output.ForEach(x => x.Dispose());
            ((IDisposable)LedController).Dispose();
            ((IDisposable)AnalogController).Dispose();
        }

        /// <summary>
        /// Class containing the user-controlled lights
        /// </summary>
        /// <remarks>
        /// We use this instead of a list so that each light can have easy names
        /// </remarks>
        public class Lights
        {
            public ILight Power = new SN3218.Light(17);
            public ILight Comms = new SN3218.Light(16);
            public ILight Warn = new SN3218.Light(15);
        }
    };
}
