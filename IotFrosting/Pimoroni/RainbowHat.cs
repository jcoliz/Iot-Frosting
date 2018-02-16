using Microsoft.IoT.Lightning.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices;
using Windows.System.Threading;
using Windows.UI;

namespace IotFrosting.Pimoroni
{
    /// <summary>
    /// Pimoroni Rainbow Hat
    /// </summary>
    /// <remarks>
    /// https://github.com/androidthings/contrib-drivers/tree/master/rainbowhat
    /// https://github.com/pimoroni/rainbow-hat
    /// </remarks>
    public class RainbowHat: IDisposable
    {
        public class ColorLed
        {
            /// <summary>
            /// Current color of this light
            /// </summary>
            public Color Color { get; set; }
        }


        #region Public methods
        /// <summary>
        /// Open a connection to the piano hat
        /// </summary>
        /// <returns>Piano Hat controller</returns>
        public static async Task<RainbowHat> Open()
        {

            var result = new RainbowHat();
            result.RainbowLed = await APA102.Open(0, 7);
            result.Pads = new List<IDigitalInput>() { new Input(21, new DirectLight(6)), new Input(20, new DirectLight(19)), new Input(16, new DirectLight(26)) };

            result.Display = await AlphaDisplay.Open();

            result.SlowTicks.Add(result.Display);
            result.SlowTimer = ThreadPoolTimer.CreatePeriodicTimer(result.SlowTick, TimeSpan.FromMilliseconds(100));

            result.Buzzer = await Buzzer.Open(13);

            return result;
        }

        private void SlowTick(ThreadPoolTimer timer)
        {
            //TODO: promote disposing status out
            //if (!disposing)
            {
                SlowTicks?.ForEach(x => x.Tick());
            }
        }
        #endregion

        #region Public Properties

        public APA102 RainbowLed;

        public AlphaDisplay Display;

        public Buzzer Buzzer;

        float Temperature { get; }

        float Pressure { get; }

        public List<IDigitalInput> Pads;

        float Piezo { get; set; }

        #endregion

        private ThreadPoolTimer SlowTimer;
        private List<ITick> SlowTicks = new List<ITick>();

        #region Internal methods

        /// <summary>
        /// Don't call consturctor directly, use PianoHat.Open()
        /// </summary>
        protected RainbowHat()
        {
        }
        #endregion


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing">Really dispose??</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    RainbowLed?.Dispose();
                    RainbowLed = null;
                    Buzzer?.Dispose();
                    Buzzer = null;
                    Display?.Dispose();
                    Display = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~PianoHat() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
