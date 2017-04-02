using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.I2c;

namespace Pimoroni.MsIot
{
    public class DS3231: IDisposable
    {
        private const byte I2C_ADDRESS = 0x68;

        private I2cDevice Device;

        public async Task Initialize()
        {

            var i2cSettings = new I2cConnectionSettings(I2C_ADDRESS);
            var controller = await I2cController.GetDefaultAsync();
            Device = controller.GetDevice(i2cSettings);
        }

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

        public void Update()
        {
            if (_UserSetTime.HasValue)
                SetTime(_UserSetTime.Value);

            _InternalTime = ReadTime();
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
            byte write_year = decToBcd(value.Year);

            byte[] write_time = { 0x00, write_seconds, write_minutes, write_hours, write_dayofweek, write_day, write_month, write_year };

            Device.Write(write_time);
        }

        // Convert normal decimal numbers to binary coded decimal
        private static byte decToBcd(int val)
        {
            return (byte)((val / 10 * 16) + (val % 10));
        }

        private static int bcdToDec(int val)
        {
            return ((val / 16 * 10) + (val % 16));
        }

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
