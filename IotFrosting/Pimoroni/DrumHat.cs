using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotFrosting.Pimoroni
{
    /// <summary>
    /// Controls a single Drum Hat
    /// </summary>
    /// <remarks>
    /// https://shop.pimoroni.com/products/drum-hat
    /// https://github.com/pimoroni/drum-hat
    /// https://www.adafruit.com/product/3180
    /// </remarks>
    public class DrumHat: IDisposable
    {
        /// <summary>
        /// One particular drum pad
        /// </summary>
        /// <remarks>
        /// Note that drum hat pads have to manage their own lights, because the
        /// lights on drum hat are mis-wired, so can't use the hardware auto
        /// lights :(
        /// </remarks>
        public class Pad : CAP1XXX.Pad, IAutoLight
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="original">The pad we're overriding</param>
            /// <param name="light">The light showing our state</param>
            public Pad(CAP1XXX.Pad original, ILight light): base(original)
            {
                // Drum hat is wired poorly. The LED's don't match up with the pads, so we
                // can't let the cap controller manage the lights. WE have to do it!! :(
                Light = light;
                base.AutoLight = false;
                base.Updated += (s, _) => { if (AutoLight) Light.State = s.State; };
            }

            /// <summary>
            /// Whether the light is managed automatically
            /// </summary>
            public new bool AutoLight { get; set; } = true;

            /// <summary>
            /// The light showing our state
            /// </summary>
            public new ILight Light { get; private set; }
        }

        /// <summary>
        /// Handler for Pad.Updated events
        /// </summary>
        /// <param name="sender">The pad which was updated</param>
        /// <param name="args">Empty args, may be used for expansion</param>
        public delegate void PadUpdateEventHandler(Pad sender, EventArgs args);

        /// <summary>
        /// A combined set of pads, which shared a combined Updated event
        /// </summary>
        public class PadSet
        {
            /// <summary>
            /// Add keys into the set
            /// </summary>
            /// <param name="keys">Keys to add</param>
            public void AddRange(IEnumerable<IInput> keys)
            {
                foreach (var input in keys)
                {
                    var key = input as Pad;
                    key.Updated += (s, a) => Updated?.Invoke(s as Pad, a);
                    Pads[key.Id] = key;
                }
            }

            /// <summary>
            /// Extract a key by name
            /// </summary>
            /// <param name="id">Identifier for this pad</param>
            /// <returns>Pad with that id</returns>
            public Pad this[int id] => Pads[id];

            /// <summary>
            /// Raised every time any one of the keys are updated
            /// </summary>
            public event PadUpdateEventHandler Updated;

            /// <summary>
            /// Internal dictinoary of keys for fast lookup
            /// </summary>
            private Dictionary<int, Pad> Pads = new Dictionary<int, Pad>();
        }

        #region Public methods
        /// <summary>
        /// Open a connection to the piano hat
        /// </summary>
        /// <returns>Piano Hat controller</returns>
        public static async Task<DrumHat> Open()
        {
            var result = new DrumHat();
            result.Cap = await CAP1XXX.Open(0x2C, 25);

            // Replace the existing pad controllers with our own
            var lightmap = new int[] { 5,4,3,2,1,0,6,7 };
            for (int i = 0; i < 8; i++)
            {
                result.Cap.Pads[i] = new Pad(result.Cap.Pads[i],result.Cap.Lights[lightmap[i]]);
            }

            result.Pads.AddRange(result.Cap.Pads);

            return result;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// All the pads available on the drum hat
        /// </summary>
        public PadSet Pads = new PadSet();
        #endregion

        #region Internal methods

        /// <summary>
        /// Don't call consturctor directly, use PianoHat.Open()
        /// </summary>
        protected DrumHat()
        {
        }
        #endregion

        #region Internal properties
        /// <summary>
        /// Capacitive input controller
        /// </summary>
        CAP1XXX Cap;

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
                    Cap?.Dispose();
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
