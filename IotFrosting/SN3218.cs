using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.I2c;

namespace IotFrosting
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

    public class SN3218: IDisposable, ITick
    {
        public static async Task<SN3218> Open()
        {
            var result = new SN3218();
            var i2cSettings = new I2cConnectionSettings(I2C_ADDRESS);
            var controller = await I2cController.GetDefaultAsync();
            result.Device = controller.GetDevice(i2cSettings);

            return result;
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

        /// <summary>
        /// Call regularly to update the hardware from the software light values
        /// </summary>
        public void Tick()
        {
            Output(Light.Values);
        }

        private void Output(IEnumerable<double> values)
        {
            var output_buffer = new byte[19];
            output_buffer[0] = CMD_SET_PWM_VALUES;
            int i = 1;
            // Write gamma-corrected values
            foreach (var value in values)
                output_buffer[i++] = (byte)(Math.Round(Math.Pow(256,value))-1);
            Device?.Write( output_buffer );
            Device?.Write(new byte[] { CMD_UPDATE, 0xff });
        }

        public async Task Test()
        {
            Enable();
            EnableLeds();

            var values = new double[NumberOfLights];

            // All on
            int i = NumberOfLights;
            while(i-->0)
                values[i] = 1.0;
            Output(values);
            await Task.Delay(500);

            // All off
            Array.Clear(values,0, NumberOfLights);

            // Cycle through
            i = NumberOfLights;
            while (i-- > 0)
            {
                values[i] = 1.0;
                Output(values);
                await Task.Delay(500);
                values[i] = 0.25;
                Output(values);
            }

            // All off
            Array.Clear(values, 0, NumberOfLights);
            Output(values);

        }

        /// <summary>
        /// Do not construct directly. Use Open()
        /// </summary>
        protected SN3218()
        {
        }

        private const byte I2C_ADDRESS = 0x54;
        private const byte CMD_ENABLE_OUTPUT = 0x00;
        private const byte CMD_SET_PWM_VALUES = 0x01;
        private const byte CMD_ENABLE_LEDS = 0x13;
        private const byte CMD_UPDATE = 0x16;
        private const byte CMD_RESET = 0x17;

        /// <summary>
        /// How many total lights are there in an SN3218 bank
        /// </summary>
        private const int NumberOfLights = 18;

        private I2cDevice Device;
        
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    EnableLeds(0);
                    Disable();
                    Device.Dispose();
                    Device = null;
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

        /// <summary>
        /// A light with PWM control, which is part of a bank of 18 lights.
        /// </summary>
        public class Light : Pimoroni.ILight
        {
            /// <summary>
            /// Digital state, true if on, false if off
            /// </summary>
            public bool State
            {
                get
                {
                    return !(0.0 == Value);
                }
                set
                {
                    if (value)
                        Value = Brightness;
                    else
                        Value = 0.0;
                }
            }

            /// <summary>
            /// PWM brightness value (0.0-1.0)
            /// </summary>
            public double Value
            {
                get
                {
                    return Values[Number];
                }
                set
                {
                    Values[Number] = value;
                    if (value > 0.0)
                        Brightness = value;

                    Updated?.Invoke(this, new EventArgs());
                }
            }

            /// <summary>
            /// How bright the light is when it's on
            /// </summary>
            public double Brightness { get; set; } = 1.0;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="number">Which light number on the SN3218 bank are we</param>
            public Light(int number)
            {
                Number = number;
                State = false;
            }

            /// <summary>
            /// Raised when the value of the light changes
            /// </summary>
            public event EventHandler<EventArgs> Updated;

            public void Toggle()
            {
                State = !State;
            }

            /// <summary>
            /// Which light ## are we on the SM3218 bank?
            /// </summary>
            private int Number;

            /// <summary>
            /// All of the light values 
            /// </summary>
            public static double[] Values = new double[NumberOfLights];
        }
    }
}
