using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.I2c;

namespace IotFrosting
{
    /// <summary>
    /// Control for the DS3231 real-time clock
    /// </summary>
    public class DS3231: IDisposable
    {
        #region Public interface
        /// <summary>
        /// Open a connection to the DS3231 chip
        /// </summary>
        /// <returns></returns>
        public static async Task<DS3231> Open()
        {
            var result = new DS3231();
            var i2cSettings = new I2cConnectionSettings(I2C_ADDRESS);
            var controller = await I2cController.GetDefaultAsync();
            result.Device = controller.GetDevice(i2cSettings);

            return result;
        }

        /// <summary>
        /// The current time
        /// </summary>
        /// <remarks>
        /// Be sure to call Tick() first to ensure the time is set
        /// </remarks>
        public DateTime Now
        {
            get
            {
                return _InternalTime;
            }
            set
            {
                _UserSetTime = value;
                _InternalTime = value;
            }
        }

        /// <summary>
        /// Call regularly to update the time
        /// </summary>
        public void Tick()
        {
            if (_UserSetTime.HasValue)
            {
                SetTime(_UserSetTime.Value);
                _UserSetTime = null;
            }

            _InternalTime = ReadTime();
        }
        #endregion

        #region Internals
        /// <summary>
        /// Do not call direcrtly, use Open()
        /// </summary>
        protected DS3231()
        {
        }

        private DateTime _InternalTime;
        private DateTime? _UserSetTime;

        private DateTime ReadTime()
        {
            byte[] writeBuf = { 0x00 };
            Device.Write(writeBuf);

            byte[] readBuf = new byte[7];
            Device.Read(readBuf);
            int second = bcdToDec((byte)(readBuf[0] & 0x7f));
            int minute = bcdToDec(readBuf[1]);
            int hour = bcdToDec((byte)(readBuf[2] & 0x3f));
            int dayOfWeek = bcdToDec(readBuf[3]);
            int dayOfMonth = bcdToDec(readBuf[4]);
            int month = bcdToDec(readBuf[5]);
            int year = bcdToDec(readBuf[6]);

            year += 2000;
            return new DateTime(year, month, dayOfMonth, hour, minute, second);
        }

        private void SetTime(DateTime value)
        {
            byte write_seconds = decToBcd(value.Second);
            byte write_minutes = decToBcd(value.Minute);
            byte write_hours = decToBcd(value.Hour);
            byte write_dayofweek = decToBcd((int)value.DayOfWeek);
            byte write_day = decToBcd(value.Day);
            byte write_month = decToBcd(value.Month);
            byte write_year = decToBcd(value.Year % 100);

            byte[] write_time = { 0x00, write_seconds, write_minutes, write_hours, write_dayofweek, write_day, write_month, write_year };

            Device.Write(write_time);
        }

        /// <summary>
        /// Convert normal decimal numbers to binary coded decimal
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>

        private static byte decToBcd(int val)
        {
            return (byte)((val / 10 * 16) + (val % 10));
        }

        private static int bcdToDec(int val)
        {
            return ((val / 16 * 10) + (val % 16));
        }

        private const byte I2C_ADDRESS = 0x68;
        private I2cDevice Device;
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Device.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DS3231() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
