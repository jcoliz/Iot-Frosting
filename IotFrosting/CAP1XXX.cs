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

        public List<Pad> Inputs;

        public byte R_MainControl
        {
            get
            {
                return this[R_MAIN_CONTROL];
            }
            set
            {
                this[R_MAIN_CONTROL] = value;
            }
        }

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
            this[R_LED_BEHAVIOUR_2] =  0 ;
            this[R_LED_LINKING] = 0xff;
            this[R_SAMPLING_CONFIG] = 0;
            this[R_SENSITIVITY] = b6 | b5;
            this[R_GENERAL_CONFIG] = b5 | b4 | b3;
            this[R_CONFIGURATION2] = b6 | b5;
            this[R_MTOUCH_CONFIG] = 0xff;

            Clear_Interrupt();

            // Property setup

            Inputs = new List<Pad>();
            for(int i = 0;i<8;i++)
            {
                Inputs.Add(new Pad());
            }

            var Alert = new InputPin(alert_pin,pulldown:false);
            Alert.Pin.DebounceTimeout = TimeSpan.Zero;
            Alert.Updated += Alert_Updated;
        }

        SemaphoreSlim Alert_Sem = new SemaphoreSlim(1);

        private async void Alert_Updated(IInput sender, EventArgs e)
        {
            /*
                def _handle_alert(self, pin=-1):
                    inputs = self.get_input_status()
                    for x in range(self.number_of_inputs):
                        self._trigger_handler(x, inputs[x])
                    self.clear_interrupt()
            */
            // Alert pin is active low
            if (!sender.State)
                try
                {
                    await Alert_Sem.WaitAsync();
                    Check_Inputs();
                    Clear_Interrupt();
                    Alert_Sem.Release();
                }
                catch (Exception ex)
                {

                }
        }

        private void Clear_Interrupt()
        {
            R_MainControl &= 0xfe;
        }
        private void Check_Inputs()
        {
            /*
                    touched = self._read_byte(R_INPUT_STATUS)
                    threshold = self._read_block(R_INPUT_1_THRESH, self.number_of_inputs)
                    delta = self._read_block(R_INPUT_1_DELTA, self.number_of_inputs)
                    #status = ['none'] * 8
                    for x in range(self.number_of_inputs):
                        if (1 << x) & touched:
                            status = 'none'
                            _delta = self._get_twos_comp(delta[x]) 
                            #threshold = self._read_byte(R_INPUT_1_THRESH + x)
                            # We only ever want to detect PRESS events
                            # If repeat is disabled, and release detect is enabled
                            if _delta >= threshold[x]: # self._delta:
             */
            byte[] touched = new byte[1];
            Device.WriteRead(new byte[] { R_INPUT_STATUS },touched);

            byte[] threshold = new byte[8];
            Device.WriteRead(new byte[] { R_INPUT_1_THRESH }, threshold);

            byte[] delta = new byte[8];
            Device.WriteRead(new byte[] { R_INPUT_1_DELTA }, delta);

            for (int i=0;i<8;i++)
            {
                if (((touched[0] >> i) & 1) == 1)
                {
                    Inputs[i].Check_Input(delta[i],threshold[i]);
                }
            }

            // Trigger events if needed
            Task.Run(() => 
            {
                Inputs.ForEach(x => x.DoUpdated());
            });
        }
        const byte R_INPUT_STATUS = 0x03;
        const byte R_INPUT_1_DELTA = 0x10;
        const byte R_INPUT_1_THRESH = 0x30;
        const byte R_LED_BEHAVIOUR_1 = 0x81; // # For LEDs 1-4
        const byte R_LED_BEHAVIOUR_2 = 0x82; // # For LEDs 5-8
        const byte R_LED_LINKING     = 0x72;
        const byte R_SAMPLING_CONFIG = 0x24; // # Default 0x00111001
        const byte R_MAIN_CONTROL = 0x00;
        const byte R_INTERRUPT_EN = 0x27;
        const byte R_INPUT_ENABLE = 0x21;
        const byte R_SENSITIVITY = 0x1F;
        const byte R_MTOUCH_CONFIG = 0x2A;

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

        private I2cDevice Device;

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

        public class Pad: IInput
        {
            public bool State { get; private set; } = false;

            private bool OldState { get; set; } = false;

            public void Check_Input(byte delta_2c, byte threshold)
            {
                int delta = delta_2c;
                if ((byte)(delta_2c & 0x80) == 0x80)
                    delta = - (~delta_2c + 1);

                if (delta > threshold)
                    State = true;
                else
                    State = false;

            }
            public void DoUpdated()
            {
                if (State != OldState)
                {
                    OldState = State;
                    Updated?.Invoke(this, new EventArgs());
                }
            }

            public event InputUpdateEventHandler Updated;
        }

    }
}
