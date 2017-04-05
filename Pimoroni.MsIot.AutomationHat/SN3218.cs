using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.I2c;

namespace Pimoroni.MsIot
{
    /// <summary>
    /// Control an SN3218 18-port LED driver
    /// </summary>
    /// <remarks>
    /// Influenced by:
    /// https://github.com/pimoroni/sn3218/blob/master/library/sn3218.py
    /// and
    /// https://github.com/ms-iot/samples/blob/develop/I2cPortExpander/CS/MainPage.xaml.cs
    /// </remarks>

    public class SN3218: IDisposable
    {
        private const byte I2C_ADDRESS = 0x54;
        private const byte CMD_ENABLE_OUTPUT = 0x00;
        private const byte CMD_SET_PWM_VALUES = 0x01;
        private const byte CMD_ENABLE_LEDS = 0x13;
        private const byte CMD_UPDATE = 0x16;
        private const byte CMD_RESET = 0x17;

        private I2cDevice Device;

        public async Task Initialize()
        {

            var i2cSettings = new I2cConnectionSettings(I2C_ADDRESS);
            var controller = await I2cController.GetDefaultAsync();
            Device = controller.GetDevice(i2cSettings);
        }

        public void Enable()
        {
            Device.Write(new byte[] { CMD_ENABLE_OUTPUT, 0x1 });
        }

        public void Disable()
        {
            Device.Write(new byte[] { CMD_ENABLE_OUTPUT, 0x0 });
        }
        public void Reset()
        {
            Device.Write(new byte[] { CMD_RESET, 0xff });
        }

        public void EnableLeds(UInt32 enable_mask = 0x3ffff)
        {
            Device.Write(new byte[] { CMD_ENABLE_LEDS, (byte)(enable_mask & 0x3F), (byte)((enable_mask >> 6) & 0x3F), (byte)((enable_mask >> 12) & 0X3F) });
            Device.Write(new byte[] { CMD_UPDATE, 0xff });
        }

        public void Output(IEnumerable<double> values)
        {
            var output_buffer = new byte[19];
            output_buffer[0] = CMD_SET_PWM_VALUES;
            int i = 1;
            foreach (var value in values)
                output_buffer[i++] = (byte)(value * 255.0);
            Device.Write( output_buffer );
            Device.Write(new byte[] { CMD_UPDATE, 0xff });
        }

        public async Task Test()
        {
            await Initialize();
            Enable();
            EnableLeds();

            const int num_leds = 18;
            var values = new double[num_leds];

            // All on
            int i = num_leds;
            while(i-->0)
                values[i] = 1.0;
            Output(values);
            await Task.Delay(500);

            // All off
            Array.Clear(values,0,num_leds);

            // Cycle through
            i = num_leds;
            while (i-- > 0)
            {
                values[i] = 1.0;
                Output(values);
                await Task.Delay(500);
                values[i] = 0.25;
                Output(values);
            }

            // All off
            Array.Clear(values, 0, num_leds);
            Output(values);

        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Device.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SN3218() {
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
