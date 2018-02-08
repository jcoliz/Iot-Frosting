using Microsoft.IoT.Lightning.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Pwm;

namespace IotFrosting.Pimoroni
{
    public class Buzzer: IDisposable
    {
        PwmPin Pin = null;

        SemaphoreSlim semaphore = new SemaphoreSlim(1);

        public static async Task<Buzzer> Open(int number)
        {
            if (LightningProvider.IsLightningEnabled)
            {
                var pwmControllers = await PwmController.GetControllersAsync(LightningPwmProvider.GetPwmProvider());
                var pwmController = pwmControllers[1]; // use the on-device controller

                var pin = pwmController.OpenPin(number);
                pin.SetActiveDutyCyclePercentage(0.5);

                var result = new Buzzer(pin);

                return result;
            }
            else
                throw new PlatformNotSupportedException("Please enable the Direct Memory Mapped Driver on your device");
        }

        public void Start(double frequency)
        {
            if (semaphore.CurrentCount > 0)
            {
                semaphore.Wait();
                Pin.Controller.SetDesiredFrequency(frequency);
                Pin.Start();
            }
        }

        public void Stop()
        {
            if (semaphore.CurrentCount == 0)
                semaphore.Release();

            Pin.Stop();
        }

        protected Buzzer(PwmPin pin)
        {
            Pin = pin;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    Pin?.Dispose();
                    Pin = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Buzzer() {
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
