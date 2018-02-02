using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public class AlphaDisplayDigit
        {
            public IOutputPin[] Segment;

            public void Clear()
            {

            }

            public char Character { get; set; }
        }

        public class AlphaDisplayController
        {
            public AlphaDisplayDigit this[int index]
            {
                get
                {
                    return Digits[index];
                }
            }

            public string Message { get; set; }

            public void Clear() => Digits.ForEach(ClearDigit);

            private List<AlphaDisplayDigit> Digits;

            private void ClearDigit(AlphaDisplayDigit d) => d.Clear();
        }

        #region Public methods
        /// <summary>
        /// Open a connection to the piano hat
        /// </summary>
        /// <returns>Piano Hat controller</returns>
        public static async Task<RainbowHat> Open()
        {
            var result = new RainbowHat();

            return result;
        }
        #endregion

        #region Public Properties

        ColorLed[] RainbowLed;

        AlphaDisplayController AlphaDisplay;

        float Temperature { get; }

        float Pressure { get; }

        List<IDigitalInput> Pads;

        float Piezo { get; set; }

        #endregion

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
                    // TODO: dispose managed state (managed objects).
                    
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
