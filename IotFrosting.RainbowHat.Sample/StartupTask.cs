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
                Hat = await Pimoroni.RainbowHat.Open();
                Hat.RainbowLed.Clear();
                Hat.RainbowLed.Show();
                Hat.Pads[0].Updated += StartupTask_Updated_0;
                Hat.Pads[1].Updated += StartupTask_Updated_2;
                Hat.Pads[2].Updated += StartupTask_Updated_4;

                /*
                var color = Colors.Red;
                color.A = 36;
                Hat.RainbowLed.Pixels[0] = color;
                color.A += 36;
                Hat.RainbowLed.Pixels[1] = color;
                color.A += 36;
                Hat.RainbowLed.Pixels[2] = color;
                color.A += 36;
                Hat.RainbowLed.Pixels[3] = color;
                color.A += 36;
                Hat.RainbowLed.Pixels[4] = color;
                color.A += 36;
                Hat.RainbowLed.Pixels[5] = color;
                color.A += 36;
                Hat.RainbowLed.Pixels[6] = color;
                Hat.RainbowLed.Show();
                */
            }
            catch (Exception ex)
            {

            }
        }

        private void StartupTask_Updated_0(IInput sender, EventArgs args)
        {
            Hat.RainbowLed.Pixels[0] = sender.State ? Colors.Black : Colors.Green;
            Hat.RainbowLed.Show();
        }
        private void StartupTask_Updated_2(IInput sender, EventArgs args)
        {
            Hat.RainbowLed.Pixels[2] = sender.State ? Colors.Black : Colors.Green;
            Hat.RainbowLed.Show();
        }
        private void StartupTask_Updated_4(IInput sender, EventArgs args)
        {
            Hat.RainbowLed.Pixels[4] = sender.State ? Colors.Black : Colors.Green;
            Hat.RainbowLed.Show();
        }
    }
}
