using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Pimoroni.MsIot.Sample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            try
            {
                this.InitializeComponent();
            }
            catch (Exception ex)
            {
                TB.Text = ex.Message;
            }
        }

        /// <summary>
        /// Each time you close this switch, it toggles the light
        /// </summary>
        /// <remarks>
        /// Same as scenario 1, but uses toolkit classes
        /// </remarks>
        async Task Scenario2()
        {
            using (var Hat = await AutomationHat.Open())
            {
                Hat.Input[0].Updated += (s, a) =>
                {
                    if (Hat.Input[0].State)
                        Hat.Output[0].Toggle();
                };
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }

        /// <summary>
        /// Cycle through automation hat LED's
        /// </summary>
        async Task Scenario4()
        {
            using (var device = new SN3218())
                await device.Test();
        }

        async Task Scenario5()
        {
            using (var Hat = await AutomationHat.Open())
            {
                await Task.Delay(500);
                Hat.Light.Comms.Value = 1.0;
                await Task.Delay(500);
                Hat.Light.Warn.Value = 1.0;
                await Task.Delay(500);
                Hat.Light.Comms.Value = 0.0;
                await Task.Delay(500);
                Hat.Light.Warn.Value = 0.0;
                await Task.Delay(500);
            }
        }

        async Task Scenario6()
        {
            using (var Hat = await AutomationHat.Open())
            {
                await Task.Delay(500);

                for (int i = 0;i< 3;i++)
                {
                    Hat.Relay[i].State = true;
                    await Task.Delay(500);
                }
                for (int i = 0; i < 3; i++)
                {
                    Hat.Relay[i].State = false;
                    await Task.Delay(500);
                }
            }
        }

        async Task Scenario7()
        {
            using (var Hat = await AutomationHat.Open())
            {
                // Here we just sit here until input 0 has been high for 3 seconds, then released
                var semaphore = new SemaphoreSlim(1);
                await semaphore.WaitAsync();
                DateTime? pressed_at = null;
                Hat.Input[0].Updated += (s, e) =>
                {
                    if (pressed_at.HasValue)
                    {
                        if (DateTime.Now - pressed_at.Value > TimeSpan.FromSeconds(3))
                            semaphore.Release();
                        else
                            pressed_at = null;
                    }
                    else
                    {
                        if (Hat.Input[0].State == true)
                            pressed_at = DateTime.Now;
                    }
                };

                await semaphore.WaitAsync();
            }
        }

        async Task Scenario8()
        {
            using (var Hat = await AutomationHat.Open())
            {
                await Task.Delay(500);

                for (int i = 0; i < 3; i++)
                {
                    Hat.Output[i].State = true;
                    await Task.Delay(500);
                }
                for (int i = 0; i < 3; i++)
                {
                    Hat.Output[i].State = false;
                    await Task.Delay(500);
                }
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var Button = sender as Button;
            Button.IsEnabled = false;

            try
            {
                if (R2.IsChecked == true)
                    await Scenario2();
                if (R4.IsChecked == true)
                    await Scenario4();
                if (R5.IsChecked == true)
                    await Scenario5();
                if (R6.IsChecked == true)
                    await Scenario6();
                if (R7.IsChecked == true)
                    await Scenario7();
                if (R8.IsChecked == true)
                    await Scenario8();
            }
            catch (Exception ex)
            {
                TB.Text = ex.Message;
            }

            Button.IsEnabled = true;
        }
    }
}
