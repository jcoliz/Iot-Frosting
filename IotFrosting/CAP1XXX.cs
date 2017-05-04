using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.I2c;

namespace IotFrosting
{
    /// <summary>
    /// https://github.com/pimoroni/cap1xxx
    /// </summary>
    public class CAP1XXX: IDisposable
    {
        public static async Task<CAP1XXX> Open(int alert_pin)
        {
            var result = new CAP1XXX(alert_pin);
            var i2cSettings = new I2cConnectionSettings(I2C_ADDRESS);
            var controller = await I2cController.GetDefaultAsync();
            result.Device = controller.GetDevice(i2cSettings);

            return result;
        }

        public List<Input> Inputs;

        protected CAP1XXX(int alert_pin)
        {
            Inputs = new List<Input>();
            for(int i = 0;i<18;i++)
            {
                Inputs.Add(new Input());
            }

            var Alert = new InputPin(alert_pin);
            Alert.Updated += Alert_Updated;
        }

        private void Alert_Updated(object sender, EventArgs e)
        {
            throw new NotImplementedException();

            /*
                def _handle_alert(self, pin=-1):
                    inputs = self.get_input_status()
                    for x in range(self.number_of_inputs):
                        self._trigger_handler(x, inputs[x])
                    self.clear_interrupt()
            */
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
            Device.Write(new byte[] { R_INPUT_STATUS });
            byte[] touched = new byte[1];
            Device.Read(touched);

            Device.Write(new byte[] { R_INPUT_1_THRESH });
            byte[] threshold = new byte[8];
            Device.Read(threshold);

            Device.Write(new byte[] { R_INPUT_1_DELTA });
            byte[] delta = new byte[8];
            Device.Read(delta);

            for (int i=0;i<8;i++)
            {
                if (((touched[0] >> i) & 1) == 1)
                {
                    Inputs[i].Check_Input(threshold[i], delta[i]);
                }
            }

        }
        private const byte I2C_ADDRESS = 0x2C;
        private const byte R_INPUT_STATUS = 0x03;
        private const byte R_INPUT_1_DELTA = 0x10;
        private const byte R_INPUT_1_THRESH = 0x30;

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

        public class Input
        {
            public bool State { get; private set; } = false;

            public void Check_Input(byte delta_2c, byte threshold)
            {
                var oldstate = State;

                int delta = ~delta_2c;
                if (delta > threshold)
                    State = true;
                else
                    State = false;

                if (State != oldstate)
                    Updated?.Invoke(this, new EventArgs());

                // Set state
                // Trigger updated if needed

                /*
                _delta = self._get_twos_comp(delta[x]) 
                #threshold = self._read_byte(R_INPUT_1_THRESH + x)
                # We only ever want to detect PRESS events
                # If repeat is disabled, and release detect is enabled
                if _delta >= threshold[x]: # self._delta:
                    self.input_delta[x] = _delta
                    #  Touch down event
                    if self.input_status[x] in ['press','held']:
                        if self.repeat_enabled & (1 << x):
                            status = 'held'
                    if self.input_status[x] in ['none','release']:
                        if self.input_pressed[x]:
                            status = 'none'
                        else:
                            status = 'press'
                else:
                    # Touch release event
                    if self.release_enabled & (1 << x) and not self.input_status[x] == 'release':
                        status = 'release'
                    else:
                        status = 'none'

                self.input_status[x] = status
                self.input_pressed[x] = status in ['press','held','none']
            else:
                self.input_status[x] = 'none'
                self.input_pressed[x] = False
                 */
            }
            public event EventHandler<EventArgs> Updated;
        }
    }
}
