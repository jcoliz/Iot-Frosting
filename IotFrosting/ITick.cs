using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotFrosting
{
    /// <summary>
    /// An object which needs to be updated regularly
    /// </summary>
    public interface ITick
    {
        /// <summary>
        /// Call regularly to update the status of this thing
        /// </summary>
        void Tick();
    }
}
