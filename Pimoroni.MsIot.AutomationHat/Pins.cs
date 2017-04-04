using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Pimoroni.MsIot
{
    public class OutputPin : IOutputPin
    {
        public OutputPin(int pin)
        {
            Pin = GpioController.GetDefault().OpenPin(pin);
            Pin.SetDriveMode(GpioPinDriveMode.Output);
            State = false;
        }

        public virtual bool State
        {
            get { return GpioPinValue.High == _Value; }
            set
            {
                _Value = value ? GpioPinValue.High : GpioPinValue.Low;
                Pin.Write(_Value);
                Update(value);
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

    public class InputPin : IInputPin
    {
        public InputPin(int pin)
        {
            Pin = GpioController.GetDefault().OpenPin(pin);
            Pin.SetDriveMode(GpioPinDriveMode.InputPullDown);
        }

        public bool State => Pin.Read() == GpioPinValue.High;

        private GpioPin Pin;
    }
}
