using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Pimoroni.MsIot
{
    public class Input: IDigitalInput
    {
        public Input(int pin, ILight light)
        {
            Light = light;
            Pin = GpioController.GetDefault().OpenPin(pin);
            Pin.SetDriveMode(GpioPinDriveMode.InputPullDown);
        }

        public bool AutoLight { get; set; } = true;

        public ILight Light { get; private set; }

        public bool State => Pin.Read() == GpioPinValue.High;

        public static void DoAutoLight(IDigitalInput v)
        {
            if (v.AutoLight)
                v.Light.State = v.State;
        }

        private GpioPin Pin;
    }
}
