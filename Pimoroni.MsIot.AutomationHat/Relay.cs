using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pimoroni.MsIot
{
    public class Relay: RawOutput, IRelay
    {
        public Relay(int pin, ILight light_no, ILight light_nc): base(pin)
        {
            LightNO = light_no;
            LightNC = light_nc;
        }

        public bool AutoLight { get; set; } = true;
        public ILight Light => LightNO;
        public ILight LightNO { get; private set; }
        public ILight LightNC { get; private set; }

        protected override void Update(bool v)
        {
            if (AutoLight)
            {
                LightNO.State = v;
                LightNC.State = !v;
            }
        }
    }
}
