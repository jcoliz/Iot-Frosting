using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.I2c;

namespace IotFrosting
{
    /// <summary>
    /// https://cdn-shop.adafruit.com/datasheets/ads1015.pdf
    /// </summary>
    class ADS1015 : IDisposable
    {
        /// <summary>
        /// Open a connection to the device
        /// </summary>
        /// <remarks>
        /// Use this instead of a constructor
        /// </remarks>
        /// <returns>Newly opened ADS1015</returns>
        public static async Task<ADS1015> Open()
        {
            var i2cSettings = new I2cConnectionSettings(I2C_ADDRESS);
            var controller = await I2cController.GetDefaultAsync();
            var device = controller.GetDevice(i2cSettings);
            var result = new ADS1015(device);

            return result;
        }

        /// <summary>
        /// Read the analog value on a given channel
        /// </summary>
        /// <param name="channel">Which channel to read</param>
        /// <returns>Value of input from 0.0 (0V) to 1.0 (Max V)</returns>
        public async Task<double> Read(int channel = 0)
        {
            double result = 0.0;

            UInt16 config = ConfigValues[channel];

            byte[] writeBuf = { REG_CFG, (byte)(config >> 8), (byte)(config & 0xff) };
            Device.Write(writeBuf);

            byte[] config_read = new byte[2];
            int timeout = 10;
            do
            {
                // Make sure we don't get stuck here foreever
                if (--timeout <= 0)
                    return 0.0;

                // Wait for the sample to become available
                await Task.Delay(TimeSpan.FromSeconds(1.0 / (UInt16)SamplesPerSecond));

                // Check the status of the device to see if the sample is ready
                Device.Read(config_read);
            }
            // Continue this until the sampe is ready
            while ((config_read[1] & 0x80) != 0x80);

            Device.Write(new byte[] { REG_CONV });

            byte[] data = new byte[2];
            Device.Read(data);

            UInt16 value = (UInt16)((UInt16)((byte)(data[0]) << 4) | (UInt16)((byte)(data[1]) >> 4));
            if ((value & 0x800) != 0)
                value -= 1 << 12;

            result = value / 2047.0; //  Divide down to percentage of FS
            result *= (UInt16)PGA;

            result /= 3300.0; // Divide by VCC

            if (result > 1.0)
                result = 0.0;

            return result;
        }

        /// <summary>
        /// Semaphore to protect against multiple concurrent all-channel reads
        /// </summary>
        SemaphoreSlim sem = new SemaphoreSlim(1);

        /// <summary>
        /// Read all channels into a single buffer
        /// </summary>
        /// <param name="result">Buffer of values</param>
        /// <returns>Awaitable task</returns>
        public async Task ReadInto(double[] result)
        {
            if (sem.CurrentCount >= 0)
            {
                await sem.WaitAsync();
                for (int i = 0; i < NumberOfChannels; i++)
                    result[i] = await Read(i);
                sem.Release();
            }
        }

        /// <summary>
        /// Current voltage range value for the PGA
        /// </summary>
        public PGAValues PGA
        {
            get
            {
                return _PGA;
            }
            set
            {
                PGA = value;
                CalculateConfigValues();
            }
        }
        private PGAValues _PGA = PGAValues.PGA_4_096V;

        /// <summary>
        /// Current value for how many samples per secound do we want
        /// </summary>
        public SPSValues SamplesPerSecond
        {
            get
            {
                return _SamplesPerSecond;
            }
            set
            {
                _SamplesPerSecond = value;
                CalculateConfigValues();
            }
        }
        private SPSValues _SamplesPerSecond = SPSValues.SPS_1600;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="device">I2C device we attach to</param>
        protected ADS1015(I2cDevice device)
        {
            Device = device;
            CalculateConfigValues();
        }

        /// <summary>
        /// Pre-calculate the correct CFG register values for each channel
        /// </summary>
        private void CalculateConfigValues()
        {
            for (int channel = 0; channel < NumberOfChannels; channel++)
            {
                // sane defaults
                UInt16 config = 0x0003 | 0x0100;
                config |= SAMPLES_PER_SECOND_MAP[SamplesPerSecond];
                config |= CHANNEL_MAP[channel];
                config |= PROGRAMMABLE_GAIN_MAP[PGA];

                // set "single shot" mode
                config |= 0x8000;

                ConfigValues[channel] = config;
            }
        }

        const int I2C_ADDRESS = 0x48;
        const byte REG_CONV = 0x00;
        const byte REG_CFG = 0x01;
        const int NumberOfChannels = 4;

        UInt16[] ConfigValues = new UInt16[NumberOfChannels];

        /// <summary>
        /// Acceptable voltage range values for the PGA
        /// </summary>
        /// <remarks>
        /// In units of 1/1000V. E.g. "4096" is 4.096V
        /// </remarks>
        public enum PGAValues { PGA_0_256V = 256, PGA_0_512V = 512, PGA_1_024V = 1024, PGA_2_048V = 2048, PGA_4_096V = 4096, PGA_6_144V = 6144 };

        /// <summary>
        /// Acceptable values for samples per second
        /// </summary>
        public enum SPSValues { SPS_128 = 128, SPS_250 = 250, SPS_490 = 490, SPS_920 = 920, SPS_1600 = 1600, SPS_2400 = 2400, SPS_3300 = 3300 };

        static readonly Dictionary<SPSValues, UInt16> SAMPLES_PER_SECOND_MAP = new Dictionary<SPSValues, ushort>()
            { { SPSValues.SPS_128, 0x0000 }, { SPSValues.SPS_250, 0x0020 }, { SPSValues.SPS_490, 0x0040 }, { SPSValues.SPS_920, 0x0060 }, { SPSValues.SPS_1600, 0x0080 }, { SPSValues.SPS_2400, 0x00A0 }, { SPSValues.SPS_3300, 0x00C0} };

        static readonly Dictionary<int, UInt16> CHANNEL_MAP = new Dictionary<int, ushort>()
            { { 0, 0x4000 }, { 1, 0x5000 }, { 2, 0x6000 }, { 3, 0x7000 } };

        static readonly Dictionary<PGAValues, UInt16> PROGRAMMABLE_GAIN_MAP = new Dictionary<PGAValues, ushort>()
            { { PGAValues.PGA_6_144V, 0x0000 }, { PGAValues.PGA_4_096V, 0x0200 }, { PGAValues.PGA_2_048V, 0x0400 }, { PGAValues.PGA_1_024V, 0x0600 }, { PGAValues.PGA_0_512V, 0x0800 }, { PGAValues.PGA_0_256V, 0x0A00} };

        I2cDevice Device;

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
        // ~ADS1015() {
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

        public class Input : Pimoroni.IAnalogInput
        {
            /// <summary>
            /// Current voltage on the input
            /// </summary>
            public double Voltage => Values[Channel] * MaxVoltage;

            /// <summary>
            /// Whether the autolight should in fact be updated with our state
            /// </summary>
            public bool AutoLight { get; set; } = true;

            /// <summary>
            /// The light which shows our state automatically
            /// </summary>
            public Pimoroni.ILight Light { get; private set; }

            /// <summary>
            /// Call regularly to update the status of the auto light. The auto light will be
            /// set to PWM brightness corresponding to the analog input level.
            /// </summary>
            /// <remarks>
            /// Recommend calling on your timer tick
            /// </remarks>
            public void Tick()
            {
                if (AutoLight && Light != null)
                    Light.Value = Values[Channel];
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="channel">Which ADC channel (0-4) is the input connected to </param>
            /// <param name="maxvoltage">The voltage which would drive a 1.0 reading on the underlying ADC</param>
            /// <param name="light">The light which will show status</param>
            public Input(int channel, double maxvoltage, Pimoroni.ILight light)
            {
                if (channel < 0 || channel > NumberOfAnalogInputs)
                    throw new ArgumentException("Invalid channel", nameof(channel));

                Light = light;
                Channel = channel;
                MaxVoltage = maxvoltage;
            }

            /// <summary>
            /// Which ADC channel (0-4) is the input connected to 
            /// </summary>
            private int Channel;

            /// <summary>
            /// The voltage which would drive a 1.0 reading on the underlying ADC
            /// </summary>
            private double MaxVoltage;

            /// <summary>
            /// All of the analog input values 
            /// </summary>
            public static double[] Values = new double[NumberOfAnalogInputs];

            /// <summary>
            /// How many total lights are there in an ADS1015 bank
            /// </summary>
            private const int NumberOfAnalogInputs = 4;
        }

    }
}
