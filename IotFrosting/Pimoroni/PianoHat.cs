using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotFrosting.Pimoroni
{

    public class PianoHat: IDisposable
    {
        #region Child classes
        /// <summary>
        /// All the valid key names on a PianoHat
        /// </summary>
        public enum KeyName { C = 0, Csharp, D, Dsharp, E, F, Fsharp, G, Gsharp, A, Asharp, B, C2, OctaveUp, OctaveDown, Instrument };

        /// <summary>
        /// One particular piano key
        /// </summary>
        public class Key : CAP1XXX.Pad
        {
            public KeyName Name { get; set; }
        }

        /// <summary>
        /// Handler for Key.Updated events
        /// </summary>
        /// <param name="sender">The key which was updated</param>
        /// <param name="args">Empty args, may be used for expansion</param>
        public delegate void KeyUpdateEventHandler(Key sender, EventArgs args);

        public class KeySet
        {
            public void AddRange(IEnumerable<IInput> keys)
            {
                foreach (var input in keys)
                {
                    var key = input as Key;
                    key.Updated += (s, a) => Updated?.Invoke(s as Key, a);
                    Keys[key.Name] = key;
                }
            }

            public Key this[KeyName name] => Keys[name];

            public event KeyUpdateEventHandler Updated;

            private Dictionary<KeyName, Key> Keys = new Dictionary<PianoHat.KeyName, Key>();
        }
        #endregion

        #region Public properties
        public IInput Instrument => Cap2.Inputs[7];
        public IInput OctaveUp => Cap2.Inputs[6];
        public IInput OctaveDown => Cap2.Inputs[5];
        public KeySet Notes = new KeySet();
        #endregion

        #region Public methods
        public static async Task<PianoHat> Open()
        {
            var result = new PianoHat();
            result.Cap1 = await CAP1XXX.Open(0x28, 4);
            result.Cap2 = await CAP1XXX.Open(0x2B, 27);

            for (int i = 0; i < 8; i++)
            {
                result.Cap1.Inputs[i] = new Key() { Name = (KeyName)i };
                result.Cap2.Inputs[i] = new Key() { Name = (KeyName)(i + 8) };
            }

            result.Notes.AddRange(result.Cap1.Inputs);
            result.Notes.AddRange(result.Cap2.Inputs.Take(5));

            return result;
        }
        #endregion

        #region Internal methods

        protected PianoHat()
        {

        }
        #endregion

        #region Internal properties
        CAP1XXX Cap1;
        CAP1XXX Cap2;
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    Cap1?.Dispose();
                    Cap2?.Dispose();
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
