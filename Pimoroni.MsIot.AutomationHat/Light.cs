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
                Brightness = value;
            }
        }

        public Light(int pin)
        {
            Pin = pin;
            State = false;
        }

        public void Toggle()
        {
            State = !State;
        }

        public static double[] Values = new double[18];

        private int Pin;
        private double Brightness = 1.0;
    }
}
