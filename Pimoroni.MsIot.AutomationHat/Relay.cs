using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pimoroni.MsIot
{
    public class Relay: OutputPin, IRelay
    {
        public Relay(int pin, ILight light_no, ILight light_nc): base(pin)
        {
            _NO = new SingleAutoLight(light_no);
            _NC = new SingleAutoLight(light_nc);
        }

        public IAutoLight NO => _NO;
        public IAutoLight NC => _NC;

        private SingleAutoLight _NO;
        private SingleAutoLight _NC;

        protected override void Update(bool v)
        {
            _NO.Update(v);
            _NC.Update(!v);
        }
        private class SingleAutoLight : IAutoLight
        {
            public SingleAutoLight(ILight light)
            {
                Light = light;
            }

            public ILight Light { get; private set; }

            public bool AutoLight { get; set; }

            public void Update(bool v)
            {
                if (AutoLight)
                    Light.State = v;
            }
        }
    }

}
