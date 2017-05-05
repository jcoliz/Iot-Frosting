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
    public interface IAnalogInput : IAutoLight
    {
        /// <summary>
        /// Current voltage on the input
        /// </summary>
        double Voltage { get; }

        /// <summary>
        /// Call regularly to update the autolight
        /// </summary>
        void Tick();
    };
}
