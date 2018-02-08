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

            double[] notes = { 440.0, 493.883, 523.251, 587.330, 659.255, 698.456, 783.991, 880 };

            try
            {
                if (Microsoft.IoT.Lightning.Providers.LightningProvider.IsLightningEnabled)
                {
                    var pwmControllers = await PwmController.GetControllersAsync(LightningPwmProvider.GetPwmProvider());
                    var pwmController = pwmControllers[1]; // use the on-device controller

                    var pin = pwmController.OpenPin(13);
                    pin.SetActiveDutyCyclePercentage(0.5);

                    foreach(var freq in notes)
                    {
                        pin.Controller.SetDesiredFrequency(freq);
                        pin.Start();
                        await Task.Delay(TimeSpan.FromSeconds(1));
                        pin.Stop();
                        await Task.Delay(TimeSpan.FromSeconds(1));
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
