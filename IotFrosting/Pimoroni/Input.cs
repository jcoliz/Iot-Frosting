using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace IotFrosting.Pimoroni
{
    public class Input: InputPin, IDigitalInput
    {
        bool pressedhigh;

        public Input(int pin, ILight light, bool pulldown=true): base(pin,pulldown)
        {
            Light = light;
            pressedhigh = pulldown;
        }

        public bool AutoLight { get; set; } = true;

        public ILight Light { get; private set; }

        public void Tick()
        {
            if (AutoLight)
                Light.State = (State == pressedhigh);
        }
    }
}
