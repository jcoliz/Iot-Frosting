using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Devices.Spi;
using Windows.UI;

namespace IotFrosting
{
    /// <summary>
    /// Library to control APA102 LED lights
    /// </summary>
    /// <remarks>
    /// https://cdn-shop.adafruit.com/datasheets/APA102.pdf
    /// </remarks>
    public class APA102: IDisposable
    {
        public static async Task<APA102> Open(int chipSelectLine, int numPixels)
        {
            var spi0Aqs = SpiDevice.GetDeviceSelector("SPI0");
            var devicesInfo = await DeviceInformation.FindAllAsync(spi0Aqs);
            var settings = new SpiConnectionSettings(chipSelectLine);
            var spiDev = await SpiDevice.FromIdAsync(devicesInfo[0].Id, settings);
            var result = new APA102(spiDev,numPixels);

            return result;
        }

        protected APA102(SpiDevice device, int numPixels)
        {
            Device = device;

            int loopindex = numPixels;
            while (loopindex-- > 0)
                Pixels.Add(Colors.Black);

            Buffer = new byte[4 * numPixels + 8];
        }

        public List<Color> Pixels { get; private set; } = new List<Color>();

        public void Clear() => Pixels.ForEach(x=>x = Colors.Black);

        public void Show()
        {
            int index = 0;

            // Start frame
            int loop = 4;
            while(loop-->0)
            {
                Buffer[index++] = 0;
            }

            // Pixels

            foreach(var pixel in Pixels)
            {
                Buffer[index++] = (byte)((byte)(pixel.A >> 3) | 0b1110000);
                Buffer[index++] = pixel.B;
                Buffer[index++] = pixel.G;
                Buffer[index++] = pixel.R;
            }

            // End frame
            loop = 4;
            while (loop-- > 0)
            {
                Buffer[index++] = 0xff;
            }

            // Go!

            Device.Write(Buffer);
        }

        private SpiDevice Device;
        private GpioPin ChipSelectPin;
        private byte[] Buffer;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (Device != null)
                    {
                        Clear();
                        Show();
                    }
                    // TODO: dispose managed state (managed objects).
                    Device?.Dispose();
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
