using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pimoroni.MsIot
{
    /// <summary>
    /// A light with PWM control, which is part of a bank of 18 lights.
    /// Suitable for use with SN3218.
    /// </summary>
    public class Light: ILight
    {
        /// <summary>
        /// Digital state, true if on, false if off
        /// </summary>
        public bool State
        {
            get
            {
                return !(0.0 == Value);
            }
            set
            {
                if (value)
                    Value = Brightness;
                else
                    Value = 0.0;
            }
        }

        /// <summary>
        /// PWM brightness value (0.0-1.0)
        /// </summary>
        public double Value
        {
            get
            {
                return Values[Pin];
            }
            set
            {
                Values[Pin] = value;
                if (value > 0.0)
                    Brightness = value;

                Updated?.Invoke(this, new EventArgs());
            }
        }

        public Light(int pin)
        {
            Pin = pin;
            State = false;
        }

        public event EventHandler<EventArgs> Updated;

        public void Toggle()
        {
            State = !State;
        }

        public static double[] Values = new double[18];

        private int Pin;
        private double Brightness = 1.0;
    }

    /// <summary>
    /// A directly connected light (like an LED)
    /// </summary>
    /// <remarks>
    /// Wire the LED between the pin and VCC. pin to ground should turn light on.
    /// </remarks>
    public class DirectLight : OutputPin, ILight
    {
        public DirectLight(int pin): base(pin)
        {
        }

        public double Value
        {
            get
            {
                return State ? 1.0 : 0.0;
            }

            set
            {
                if (value == 0.0)
                    State = false;
                else
                    State = true;
            }
        }

        /// <summary>
        /// DirectLight is wired such that a 'true' state (lit) has to be a 'false'
        /// state (to ground) on the pin itself.
        /// </summary>
        public override bool State
        {
            get
            {
                return ! base.State;
            }

            set
            {
                base.State = ! value;
            }
        }
    }
}
