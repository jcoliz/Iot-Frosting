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
        }

        public event EventHandler<EventArgs> Updated;

        public virtual bool State
        {
            get { return GpioPinValue.High == _Value; }
            set
            {
                _Value = value ? GpioPinValue.High : GpioPinValue.Low;
                Pin.Write(_Value);
                Updated?.Invoke(this, new EventArgs());
            }
        }

        public void Toggle()
        {
            State = !State;
        }

        public GpioPin Pin;

        private GpioPinValue _Value = GpioPinValue.High;
    }

    public class InputPin : IInputPin
    {
        public InputPin(int pin)
        {
            Pin = GpioController.GetDefault().OpenPin(pin);
            Pin.SetDriveMode(GpioPinDriveMode.InputPullDown);
            Pin.DebounceTimeout = TimeSpan.FromMilliseconds(20);
            Pin.ValueChanged += (s, e) =>
            {
                Updated?.Invoke(this, new EventArgs());
            };
        }

        public event EventHandler<EventArgs> Updated;

        public bool State => Pin.Read() == GpioPinValue.High;

        public GpioPin Pin;
    }
}
