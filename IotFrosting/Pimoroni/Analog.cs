using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotFrosting.Pimoroni
{
    /// <summary>
    /// Interface for an analog input with an auto light
    /// </summary>
    /// <remarks>
    /// Incomplete. Analog inputs are not yet implemented.
    /// </remarks>
    public interface IAnalogInput : IAutoLight
    {
        int Value { get; }
    }
}
