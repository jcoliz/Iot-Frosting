using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IotFrosting.Pimoroni;
using Windows.Devices.I2c;

namespace IotFrosting
{
    /// <summary>
    /// https://github.com/pimoroni/cap1xxx
    /// https://cdn-shop.adafruit.com/datasheets/CAP1188.pdf
    /// </summary>
    public class CAP1XXX: IDisposable
    {
        public static async Task<CAP1XXX> Open(int i2c_address,int alert_pin)
        {
            var i2cSettings = new I2cConnectionSettings(i2c_address);
            var controller = await I2cController.GetDefaultAsync();
            var device = controller.GetDevice(i2cSettings);
            var result = new CAP1XXX(device,alert_pin);

            return result;
        }

        /// <summary>
        /// All the pads we control
        /// </summary>
        public List<Pad> Pads;

        /// <summary>
        /// Direct access to control the lights
        /// </summary>
        public List<Light> Lights;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="alert_pin">Which pin is the interrupt tied to</param>
        protected CAP1XXX(I2cDevice device, int alert_pin)
        {
            Device = device;

            // Device setup
            for(byte r = R_INPUT_1_THRESH; r < R_INPUT_1_THRESH + 8; ++r )
                this[r] = b2 | b1;

            this[R_LED_BEHAVIOUR_1] = 0;
            this[R_LED_BEHAVIOUR_2] =  0;
            this[R_LED_LINKING] = 0xff;
            this[R_SAMPLING_CONFIG] = 0;
            this[R_SENSITIVITY] = b6 | b5;
            this[R_GENERAL_CONFIG] = b5 | b4 | b3;
            this[R_CONFIGURATION2] = b6 | b5;
            this[R_MTOUCH_CONFIG] = 0xff;
            this[R_MAIN_CONTROL] &= 0xfe; // clear interrupt flag

            // Property setup

            Pads = new List<Pad>();
            Lights = new List<Light>();
            for (int i = 0;i<8;i++)
            {
                Lights.Add(new Light(this, i));
                Pads.Add(new Pad(this, i));
            }

            var Alert = new InputPin(alert_pin,pulldown:false);
            Alert.Pin.DebounceTimeout = TimeSpan.Zero;
            Alert.Updated += Alert_Updated;
        }

        /// <summary>
        /// Quick access to single-byte registers
        /// </summary>
        /// <param name="register">Which register</param>
        /// <returns>Current register value</returns>
        private byte this[byte register]
        {
            get
            {
                var result = new byte[1];
                Device.WriteRead(new byte[] { register }, result);
                return result[0];
            }
            set
            {
                Device.Write(new byte[] { register, value });
            }
        }

        /// <summary>
        /// Controls access to any action coming off the alert pin
        /// </summary>
        SemaphoreSlim Alert_Sem = new SemaphoreSlim(1);

        /// <summary>
        /// Catch interrupts on the 'alert' pin
        /// </summary>
        /// <remarks>
        /// This is the primary driver of action on this class. The alert pin
        /// interrupt tells us we have something to do
        /// </remarks>
        /// <param name="sender">Alert input pin</param>
        /// <param name="e">Empty event args</param>
        private async void Alert_Updated(IInput sender, EventArgs e)
        {
            // Alert pin is active low
            if (!sender.State)
            {
                try
                {
                    await Alert_Sem.WaitAsync();
                    Check_Inputs();
                    this[R_MAIN_CONTROL] &= 0xfe; // clear interrupt flag
                    Alert_Sem.Release();
                }
                catch (Exception ex)
                {
                    // TODO: SHould have a way to push exceptions out
                }
            }
        }

        /// <summary>
        /// Update the Inputs in software to what's there on the hardware
        /// </summary>
        /// <remarks>
        /// Not really sure why I have this as separate from Alert_Updated...
        /// </remarks>
        private void Check_Inputs()
        {
            byte touched = this[R_INPUT_STATUS];

            byte[] threshold = new byte[8];
            Device.WriteRead(new byte[] { R_INPUT_1_THRESH }, threshold);

            byte[] delta = new byte[8];
            Device.WriteRead(new byte[] { R_INPUT_1_DELTA }, delta);

            for (int i=0;i<8;i++)
            {
                if (((touched >> i) & b0) == b0)
                {
                    Pads[i].Check_Input(delta[i],threshold[i]);
                }
            }

            // Trigger events if needed
            Task.Run(() => Pads.ForEach(x => x.DoUpdated()));
        }

        #region Registers
        const byte R_INPUT_STATUS = 0x03;
        const byte R_INPUT_1_DELTA = 0x10;
        const byte R_INPUT_1_THRESH = 0x30;
        const byte R_LED_BEHAVIOUR_1 = 0x81; // # For LEDs 1-4
        const byte R_LED_BEHAVIOUR_2 = 0x82; // # For LEDs 5-8
        const byte R_LED_LINKING     = 0x72;
        const byte R_LED_OUTPUT_CON = 0x74;
        const byte R_SAMPLING_CONFIG = 0x24; // # Default 0x00111001
        const byte R_MAIN_CONTROL = 0x00;
        const byte R_INTERRUPT_EN = 0x27;
        const byte R_INPUT_ENABLE = 0x21;
        const byte R_MTOUCH_CONFIG = 0x2A;

        const byte R_SENSITIVITY = 0x1F;
        /*  # B7     = N/A
            #   B6..B4 = Sensitivity
            # B3..B0 = Base Shift
            SENSITIVITY = {128: 0b000, 64:0b001, 32:0b010, 16:0b011, 8:0b100, 4:0b100, 2:0b110, 1:0b111    }
            */

        const byte R_GENERAL_CONFIG = 0x20;
        /*
        # B7 = Timeout
        # B6 = Wake Config ( 1 = Wake pin asserted )
        # B5 = Disable Digital Noise ( 1 = Noise threshold disabled )
        # B4 = Disable Analog Noise ( 1 = Low frequency analog noise blocking disabled )
        # B3 = Max Duration Recalibration ( 1 =  Enable recalibration if touch is held longer than max duration )
        # B2..B0 = N/A
        */

        const byte R_CONFIGURATION2 = 0x44;
        /*
        # B7 = Linked LED Transition Controls ( 1 = LED trigger is !touch )
        # B6 = Alert Polarity ( 1 = Active Low Open Drain, 0 = Active High Push Pull )
        # B5 = Reduce Power ( 1 = Do not power down between poll )
        # B4 = Link Polarity/Mirror bits ( 0 = Linked, 1 = Unlinked )
        # B3 = Show RF Noise ( 1 = Noise status registers only show RF, 0 = Both RF and EMI shown )
        # B2 = Disable RF Noise ( 1 = Disable RF noise filter )
        # B1..B0 = N/A
        */

        const byte b0 = 1;
        const byte b1 = 1 << 1;
        const byte b2 = 1 << 2;
        const byte b3 = 1 << 3;
        const byte b4 = 1 << 4;
        const byte b5 = 1 << 5;
        const byte b6 = 1 << 6;
        const byte b7 = 1 << 7;
        #endregion

        #region Internal properties
        /// <summary>
        /// I2C Device we're attached to
        /// </summary>
        private I2cDevice Device;
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
                    Device.Dispose();
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

        #region Member Classes
        /// <summary>
        /// A single capacitive touch pad
        /// </summary>
        public class Pad: IInput, Pimoroni.IAutoLight
        {
            /// <summary>
            /// External constructor
            /// </summary>
            /// <param name="copy">The existing pad we're overriding</param>
            public Pad(Pad copy)
            {
                Parent = copy.Parent;
                Id = copy.Id;
            }

            /// <summary>
            /// Internal Constructor
            /// </summary>
            /// <param name="parent">Capacitive controller who controls us</param>
            /// <param name="id">Which light are we, starting at 0</param>
            public Pad(CAP1XXX parent, int id)
            {
                Parent = parent;
                Id = id;
            }

            private CAP1XXX Parent;
            private int Id;

            /// <summary>
            /// Whether we are currently being pressed
            /// </summary>
            public bool State { get; private set; } = false;

            /// <summary>
            /// Whether the cap1xxx hardware automatically controls our light
            /// </summary>
            public bool AutoLight
            {
                get
                {
                    return _Light.AutoLight;
                }
                set
                {
                    _Light.AutoLight = value;
                }
            }

            /// <summary>
            /// Direct manual access to the underlying light
            /// </summary>
            public ILight Light => _Light;

            /// <summary>
            /// Update the state based on the known hardware state
            /// </summary>
            /// <param name="delta_2c">Current hardware delta value(2's complement)</param>
            /// <param name="threshold">Current hardware threshold value</param>
            /// <remarks>
            /// Why not just move this out into the cap1xxx class??
            /// </remarks>
            public void Check_Input(byte delta_2c, byte threshold)
            {
                int delta = delta_2c;
                if ((byte)(delta_2c & b7) == b7)
                    delta = - (~delta_2c + 1);

                if (delta > threshold)
                    State = true;
                else
                    State = false;
            }

            /// <summary>
            /// Raise the Updated event if we have indeed been updated
            /// </summary>
            public void DoUpdated()
            {
                if (State != OldState)
                {
                    OldState = State;
                    Updated?.Invoke(this, new EventArgs());
                }
            }

            /// <summary>
            /// Event raised when our state is updated
            /// </summary>
            public event InputUpdateEventHandler Updated;

            /// <summary>
            /// Direct manual access to the underlying light
            /// </summary>
            private Light _Light => Parent.Lights[Id];

            /// <summary>
            /// State last time we raised the updated event
            /// </summary>
            private bool OldState { get; set; } = false;
        }

        /// <summary>
        /// This is the cap1xxx-controlled auto light
        /// </summary>
        /// <remarks>
        /// This is only here in case you want to manually control one of the autolights.
        /// Otherwise, the chip takes care of the auto light.
        /// </remarks>
        public class Light : ILight
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="parent">Capacitive controller who controls us</param>
            /// <param name="id">Which light are we, starting at 0</param>
            public Light(CAP1XXX parent, int id)
            {
                Parent = parent;
                Bit = 1 << id;
            }

            private CAP1XXX Parent;
            private int Bit;

            /// <summary>
            /// Whether we are automatically tied our corresponding pad
            /// </summary>
            public bool AutoLight
            {
                get
                {
                    return (Parent[CAP1XXX.R_LED_LINKING] & Bit) == Bit;
                }
                set
                {
                    var r = Parent[CAP1XXX.R_LED_LINKING] & ~Bit;
                    if (value)
                        r |= Bit;
                    Parent[CAP1XXX.R_LED_LINKING] = (byte)r;
                }
            }

            /// <summary>
            /// Current Analog light state
            /// </summary>
            public double Value
            {
                get
                {
                    return State ? 1.0 : 0.0;
                }

                set
                {
                    State = value != 0.0;
                }
            }

            /// <summary>
            /// Current binary light state
            /// </summary>
            public bool State
            {
                get
                {
                    return (Parent[CAP1XXX.R_LED_OUTPUT_CON] & Bit) == Bit;
                }

                set
                {
                    var r = Parent[CAP1XXX.R_LED_OUTPUT_CON] & ~Bit;
                    if (value)
                        r |= Bit;
                    Parent[CAP1XXX.R_LED_OUTPUT_CON] = (byte)r;
                }
            }

            /// <summary>
            /// Ignored. Supplied for interface compliance
            /// </summary>
            public double Brightness { get; set; }

            /// <summary>
            /// Toggle the state
            /// </summary>
            public void Toggle() => State = !State;
        }
        #endregion
    }
}
