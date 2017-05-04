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

namespace IotFrosting.Sample
{
    public sealed partial class MainPage : Page
    {
        DS3231 Clock;
        DispatcherTimer Timer;
        Pimoroni.AutomationHat Hat;

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

        protected override void OnNavigatedTo(NavigationEventArgs ea)
        {
            base.OnNavigatedTo(ea);

            // Don't do any of this automation hat stuff right now.
            // Testing piano hat!!
            return;

            try
            {
                Timer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
                Timer.Tick += (s, e) =>
                {
                    if (Clock != null)
                    {
                        Clock.Tick();
                        var x = Clock.Now.ToString();
                        TheTime.Text = x;
                    }
                    if (Hat != null)
                    {
                        A0.Text = Hat.Analog[0].Voltage.ToString("0.0") + "V";
                        A1.Text = Hat.Analog[1].Voltage.ToString("0.0") + "V";
                        A2.Text = Hat.Analog[2].Voltage.ToString("0.0") + "V";
                        A3.Text = Hat.Analog[3].Voltage.ToString("0.0") + "V";
                    }
                };
                Task.Run(async () =>
                {
                    try
                    {
                        Hat = await Pimoroni.AutomationHat.Open();
                        Hat.Light.Power.Value = 0.2; // That light is way too bright!!
                        Clock = await DS3231.Open();
                        await Dispatcher.RunAsync( Windows.UI.Core.CoreDispatcherPriority.Normal,() =>
                        {
                            Clock.Tick();
                            var x = Clock.Now.ToString();
                            TheTime.Text = x;
                            Timer.Start();
                        });
                    }
                    catch (Exception)
                    {
                    }
                });
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
                Hat.Input[0].Updated += (s, a) =>
                {
                    if (Hat.Input[0].State)
                        Hat.Output[0].Toggle();
                };
                await Task.Delay(TimeSpan.FromSeconds(10));
        }

        /// <summary>
        /// Cycle through automation hat LED's
        /// </summary>
        async Task Scenario4()
        {
            using (var device = await SN3218.Open())
                await device.Test();
        }

        async Task Scenario5()
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

        async Task Scenario6()
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

        async Task Scenario7()
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

        async Task Scenario8()
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

        async Task Scenario9()
        {
            try
            {
                var semaphore = new SemaphoreSlim(1);
                await semaphore.WaitAsync();
                using (var Cap1 = await CAP1XXX.Open(0x28, 4))
                using (var Cap2 = await CAP1XXX.Open(0x2B, 27))
                {
                    // Right now, it waits forever. Just starting our testing!!
                    await semaphore.WaitAsync();
                }
            }
            catch (Exception ex)
            {
                TB.Text = ex.Message;
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
                if (R9.IsChecked == true)
                    await Scenario9();
            }
            catch (Exception ex)
            {
                TB.Text = ex.Message;
            }

            Button.IsEnabled = true;
        }

        private void Button_Set_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var newtime = DateTime.Parse(NewTime.Text);
                Clock.Now = newtime;
            }
            catch (Exception ex)
            {
                TB.Text = ex.Message;
            }

        }
    }
}
