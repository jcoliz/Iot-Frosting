using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace IotFrosting.Pimoroni
{
    public class Output: OutputPin, IDigitalOutput
    {
        public Output(int pin, ILight autolight): base(pin)
        {
            Light = autolight;
            base.Updated += (s, e) =>
            {
                if (AutoLight)
                    Light.State = base.State;
            };
        }

        public bool AutoLight { get; set; } = true;

        public ILight Light { get; private set; }
    }
}
