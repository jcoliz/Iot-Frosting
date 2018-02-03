using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using IotFrosting.Pimoroni;
using System.Threading.Tasks;
using Windows.UI;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace IotFrosting.RainbowHat.Sample
{
    public sealed class StartupTask : IBackgroundTask
    {
        public BackgroundTaskDeferral Deferral { get; private set; }

        private Pimoroni.RainbowHat Hat;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {

            // If you start any asynchronous methods here, prevent the task
            // from closing prematurely by using BackgroundTaskDeferral as
            // described in http://aka.ms/backgroundtaskdeferral
            //
            Deferral = taskInstance.GetDeferral();

            try
            {
                using (Hat = await Pimoroni.RainbowHat.Open())
                {
                    Hat.RainbowLed.Pixels[0] = Colors.Red;
                    Hat.RainbowLed.Pixels[1] = Colors.Red;
                    Hat.RainbowLed.Pixels[2] = Colors.Red;
                    Hat.RainbowLed.Pixels[3] = Colors.Red;
                    Hat.RainbowLed.Pixels[4] = Colors.Red;
                    Hat.RainbowLed.Pixels[5] = Colors.Red;
                    Hat.RainbowLed.Pixels[6] = Colors.Red;
                    Hat.RainbowLed.Show();
                }

            }
            catch (Exception ex)
            {

            }
        }
    }
}
