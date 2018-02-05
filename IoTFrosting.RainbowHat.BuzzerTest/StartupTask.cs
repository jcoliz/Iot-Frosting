using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using System.Diagnostics;
using Windows.Devices.Gpio;
using Microsoft.IoT.Lightning.Providers;
using Windows.Devices.Pwm;
using System.Threading.Tasks;

// https://docs.microsoft.com/en-us/windows/iot-core/develop-your-app/lightningsetup

namespace IoTFrosting.RainbowHat.BuzzerTest
{
    public sealed class StartupTask : IBackgroundTask
    {
        public BackgroundTaskDeferral Deferral { get; private set; }

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // If you start any asynchronous methods here, prevent the task
            // from closing prematurely by using BackgroundTaskDeferral as
            // described in http://aka.ms/backgroundtaskdeferral
            //
            Deferral = taskInstance.GetDeferral();

            try
            {
                if (Microsoft.IoT.Lightning.Providers.LightningProvider.IsLightningEnabled)
                {
                    var pwmControllers = await PwmController.GetControllersAsync(LightningPwmProvider.GetPwmProvider());
                    var pwmController = pwmControllers[1]; // use the on-device controller
                    pwmController.SetDesiredFrequency(50); // try to match 50Hz

                    var pin = pwmController.OpenPin(13);
                    pin.Controller.SetDesiredFrequency(440); // A4, 69
                    pin.SetActiveDutyCyclePercentage(.5);


                    var loop = 10;
                    while(loop-- > 0)
                    {
                        pin.Start();
                        await Task.Delay(TimeSpan.FromSeconds(2));
                        pin.Stop();
                        await Task.Delay(TimeSpan.FromSeconds(2));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
