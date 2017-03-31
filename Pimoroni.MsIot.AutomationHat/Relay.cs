using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pimoroni.MsIot
{
    public class Relay: IDigitalOutput
    {
        public Relay(int pin, Light light_no, Light light_nc)
        {
            Light = light_no;
            LightNO = light_no;
            LightNC = light_nc;
        }

        public bool AutoLight { get; set; }

        public IRawDigitalOutput Light { get; private set; }
        public IRawDigitalOutput LightNO { get; private set; }
        public IRawDigitalOutput LightNC { get; private set; }

        public bool State { get; set; }

        public void Toggle()
        {
            State = !State;
        }

        private int pin;
    }
}
