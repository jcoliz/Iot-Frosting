using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Pimoroni.MsIot
{
    public class Output: OutputPin, IDigitalOutput
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
