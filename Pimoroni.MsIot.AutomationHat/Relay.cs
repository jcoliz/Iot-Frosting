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
            _NO = new SingleAutoLight(this,false,light_no);
            _NC = new SingleAutoLight(this,true,light_nc);
        }

        public IAutoLight NO => _NO;
        public IAutoLight NC => _NC;

        private SingleAutoLight _NO;
        private SingleAutoLight _NC;

        private class SingleAutoLight : IAutoLight
        {
            public SingleAutoLight(IOutputPin source, bool inverted, ILight light)
            {
                Light = light;
                Light.State = source.State != inverted;

                source.Updated += (s, e) =>
                {
                    if (AutoLight)
                        Light.State = source.State != inverted;
                };
            }

            public ILight Light { get; private set; }

            public bool AutoLight { get; set; } = true;
        }
    }

}
