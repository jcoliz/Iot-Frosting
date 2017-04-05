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
            // By default the (annoying) NC light is off. You can turn the autolight on if you like
            _NO = new SingleAutoLight(this, false, light_no) { AutoLight = true };
            _NC = new SingleAutoLight(this, true, light_nc);
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
                Inverted = inverted;
                Source = source;
                Light.State = false;

                source.Updated += (s, e) =>
                {
                    if (AutoLight)
                        Light.State = source.State != Inverted;
                };
            }

            public ILight Light { get; private set; }

            public bool AutoLight
            {
                get { return _AutoLight; }
                set
                {
                    _AutoLight = value;
                    if (_AutoLight)
                        Light.State = Source.State != Inverted;
                    else
                        Light.State = false;
                }
            } 
            private bool _AutoLight;

            private bool Inverted;

            private IOutputPin Source;
        }
    }

}
