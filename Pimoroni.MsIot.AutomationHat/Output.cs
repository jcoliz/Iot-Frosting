using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Pimoroni.MsIot
{
    public class RawOutput: IPin
    {
        public RawOutput(int pin)
        {
            Pin = GpioController.GetDefault().OpenPin(pin);
            Pin.SetDriveMode(GpioPinDriveMode.OutputOpenSourcePullDown);
            State = false;
        }

        public virtual bool State
        {
            get { return GpioPinValue.High == _Value; }
            set
            {
                _Value = value ? GpioPinValue.High : GpioPinValue.Low;
                Pin.Write(_Value);
            }
        }

        public void Toggle()
        {
            State = !State;
        }

        private GpioPin Pin;
        private GpioPinValue _Value = GpioPinValue.High;

        protected virtual void Update(bool v)
        {
        }
    }
    public class Output: RawOutput, IDigitalOutput
    {
        public Output(int pin, ILight autolight): base(pin)
        {
            Light = autolight;
        }

        public bool AutoLight { get; set; } = true;

        public ILight Light { get; private set; }

        protected override void Update(bool v)
        {
            if (AutoLight)
                Light.State = v;
        }
    }
}
