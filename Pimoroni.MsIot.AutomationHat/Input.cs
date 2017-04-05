using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Pimoroni.MsIot
{
    public class Input: InputPin, IDigitalInput
    {
        public Input(int pin, ILight light): base(pin)
        {
            Light = light;
        }

        public bool AutoLight { get; set; } = true;

        public ILight Light { get; private set; }

        public void Tick()
        {
            if (AutoLight)
                Light.State = State;
        }
    }
}
