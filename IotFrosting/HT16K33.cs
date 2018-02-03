using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.I2c;

namespace IotFrosting
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Ported from https://github.com/pimoroni/rainbow-hat/blob/master/library/rainbowhat/HT16K33.py
    /// Datasheet https://cdn-shop.adafruit.com/datasheets/ht16K33v110.pdf
    /// </remarks>
    public class HT16K33 : IDisposable
    {
        public static async Task<HT16K33> Open(int i2c_address = DEFAULT_ADDRESS)
        {
            var i2cSettings = new I2cConnectionSettings(i2c_address);
            var controller = await I2cController.GetDefaultAsync();
            var device = controller.GetDevice(i2cSettings);
            var result = new HT16K33(device);

            return result;
        }

        public int BlinkSpeed
        {
            get
            {
                return _BlinkSpeed;
            }
            set
            {
                byte blinkcommand = 0;
                switch (value)
                {
                    case 0:
                        blinkcommand = HT16K33_BLINK_OFF;
                        break;
                    case 1:
                        blinkcommand = HT16K33_BLINK_HALFHZ;
                        break;
                    case 2:
                        blinkcommand = HT16K33_BLINK_1HZ;
                        break;
                    case 3:
                        blinkcommand = HT16K33_BLINK_2HZ;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(BlinkSpeed), "BlinkSpeed must be 0-3");
                }
                _BlinkSpeed = value;

                Device.Write(new[] { (byte)(HT16K33_BLINK_CMD | HT16K33_BLINK_DISPLAYON | blinkcommand) });
            }
        }
        private int _BlinkSpeed;

        public float Brightness
        {
            get
            {
                return _Brightness;
            }
            set
            {
                if (Brightness < 0 || Brightness > 1.0f)
                    throw new ArgumentOutOfRangeException(nameof(Brightness), "Brightness must be 0.0-1.0");

                _Brightness = value;

                Device.Write(new[] { (byte)(HT16K33_CMD_BRIGHTNESS | (byte)(value*15)) });
            }
        }
        private float _Brightness;

        /// <summary>
        /// Whether this particular LED is lit
        /// </summary>
        /// <param name="index">which led</param>
        /// <returns>true if it's lit</returns>
        public bool this[int index]
        {
            get
            {
                if (Brightness < 0 || Brightness > 1.0f)
                    throw new ArgumentOutOfRangeException(nameof(index), "Index must be 0-127");

                var whichbit = index & 0xf;
                var whichbyte = index >> 8;

                return (Buffer[1 + whichbyte] & (byte)(1 << whichbit)) != 0;
            }
            set
            {
                if (Brightness < 0 || Brightness > 1.0f)
                    throw new ArgumentOutOfRangeException(nameof(index), "Index must be 0-127");

                var whichbit = index & 0xf;
                var whichbyte = index >> 8;

                if (value)
                    Buffer[1 + whichbyte] |= (byte)(1 << whichbit);
                else
                    Buffer[1 + whichbyte] &= (byte)~(1 << whichbit);

            }
        }

        public void SetWordAt(int index, UInt16 word)
        {
            Buffer[1 + index] = (byte)(word & 0xff);
            Buffer[2 + index] = (byte)((word >> 8) & 0xff);
        }

        /// <summary>
        /// Write the accumulated bit values to the chip
        /// </summary>
        public void Write()
        {
            Device.Write(Buffer);
        }

        protected HT16K33(I2cDevice device)
        {
            Device = device;
            Device.Write(new[] { (byte)(HT16K33_SYSTEM_SETUP | HT16K33_OSCILLATOR) });

            BlinkSpeed = 0;
            Brightness = 1.0f;
        }

        #region Device Constants
        private const byte DEFAULT_ADDRESS = 0x70;
        private const byte HT16K33_BLINK_CMD = 0x80;
        private const byte HT16K33_BLINK_DISPLAYON = 0x01;
        private const byte HT16K33_BLINK_OFF = 0x00;
        private const byte HT16K33_BLINK_2HZ = 0x02;
        private const byte HT16K33_BLINK_1HZ = 0x04;
        private const byte HT16K33_BLINK_HALFHZ = 0x06;
        private const byte HT16K33_SYSTEM_SETUP = 0x20;
        private const byte HT16K33_OSCILLATOR = 0x01;
        private const byte HT16K33_CMD_BRIGHTNESS = 0xE0;
        #endregion

        #region Internal properties
        /// <summary>
        /// I2C Device we're attached to
        /// </summary>
        private I2cDevice Device;

        /// <summary>
        /// Buffer of x16's LEDs, plus a '0x00' command byte in front
        /// </summary>
        byte[] Buffer = new byte[17];

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
                    Device?.Dispose();
                    Device = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~CAP1XXX() {
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
