using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pimoroni.MsIot
{
    public class Light: IRawDigitalOutput
    {
        public bool State { get; set; }

        public bool AutoLight { get; set; }

        public Light(int pin)
        {
            Pin = pin;
        }
        public void Toggle()
        {
            State = !State;
        }

        private int Pin;
    }
}
