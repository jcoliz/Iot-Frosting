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
        async Task Scenario1()
        {
            var Controller = GpioController.GetDefault();
            using (var LED1 = Controller.OpenPin(17))
            using (var Switch1 = Controller.OpenPin(27))
            {
                LED1.SetDriveMode(GpioPinDriveMode.Output);
                Switch1.SetDriveMode(GpioPinDriveMode.InputPullUp);
                Switch1.DebounceTimeout = TimeSpan.FromMilliseconds(20);

                Switch1.ValueChanged += (s, a) =>
                {
                    if (a.Edge == GpioPinEdge.FallingEdge)
                        LED1.Write((LED1.Read() == GpioPinValue.High) ? GpioPinValue.Low : GpioPinValue.High);
                };
                await Task.Delay(TimeSpan.FromSeconds(5));
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
            using(var LED2 = new OutputPin(17))
            using (var Switch2 = new InputPin(27))
            {
                LED2.State = false;
                Switch2.Updated += (s, a) =>
                {
                    if (!Switch2.State)
                        LED2.Toggle();
                };
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }

        /// <summary>
        /// LED acts as an autolight for the switch
        /// </summary>
        async Task Scenario3()
        {
            var Switch3 = new Input(27, new DirectLight(17));
            DispatcherTimer Timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(20) };
            Timer.Start();
            Timer.Tick += (s, e) =>
            {
                Switch3.Tick();
            };
            await Task.Delay(TimeSpan.FromSeconds(5));
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
                Hat.Light.Power.Value = 1.0;
                await Task.Delay(500);
                Hat.Light.Comms.Value = 1.0;
                await Task.Delay(500);
                Hat.Light.Warn.Value = 1.0;
                await Task.Delay(500);
                Hat.Light.Power.Value = 0.0;
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
                Hat.Light.Power.Value = 1.0;
                Hat.Relay.ForEach(x => { x.State = false; x.NC.AutoLight = false; x.NC.Light.State = false; } );
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
                Hat.Light.Power.Value = 1.0;
                Hat.Relay.ForEach(x => { x.State = false; x.NC.AutoLight = false; x.NC.Light.State = false; });

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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var Button = sender as Button;
            Button.IsEnabled = false;

            try
            {
                if (R1.IsChecked == true)
                    await Scenario1();
                if (R2.IsChecked == true)
                    await Scenario2();
                if (R3.IsChecked == true)
                    await Scenario3();
                if (R4.IsChecked == true)
                    await Scenario4();
                if (R5.IsChecked == true)
                    await Scenario5();
                if (R6.IsChecked == true)
                    await Scenario6();
                if (R7.IsChecked == true)
                    await Scenario7();
            }
            catch (Exception ex)
            {
                TB.Text = ex.Message;
            }

            Button.IsEnabled = true;
        }
    }
}
