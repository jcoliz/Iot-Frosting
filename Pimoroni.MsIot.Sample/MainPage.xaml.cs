using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class MainPage : Page, IDisposable
    {
        GpioPin LED1;
        GpioPin Switch1;
        OutputPin LED2;
        InputPin Switch2;
        Input Switch3;
        GpioController Controller;
        DispatcherTimer Timer;

        public MainPage()
        {
            try
            {
                this.InitializeComponent();

                Timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(20) };
                Timer.Start();

                Controller = GpioController.GetDefault();
                Scenario4_Setup();
            }
            catch (Exception ex)
            {
                TB.Text = ex.Message;
            }
        }

        /// <summary>
        /// Each time you close this switch, it toggles the light
        /// </summary>
        void Scenario1_Setup()
        {
            LED1 = Controller.OpenPin(17);
            LED1.SetDriveMode(GpioPinDriveMode.Output);
            Switch1 = Controller.OpenPin(27);
            Switch1.SetDriveMode(GpioPinDriveMode.InputPullUp);
            Switch1.DebounceTimeout = TimeSpan.FromMilliseconds(20);

            Switch1.ValueChanged += (s, a) =>
            {
                if (a.Edge == GpioPinEdge.FallingEdge)
                    LED1.Write((LED1.Read() == GpioPinValue.High) ? GpioPinValue.Low : GpioPinValue.High);
            };
        }

        /// <summary>
        /// Each time you close this switch, it toggles the light
        /// </summary>
        /// <remarks>
        /// Same as scenario 1, but uses toolkit classes
        /// </remarks>
        void Scenario2_Setup()
        {
            LED2 = new OutputPin(17);
            Switch2 = new InputPin(27);
            Switch2.Updated += (s, a) =>
            {
                if (!Switch2.State)
                    LED2.Toggle();
            };
        }

        /// <summary>
        /// LED acts as an autolight for the switch
        /// </summary>
        void Scenario3_Setup()
        {
            Switch3 = new Input(27, new DirectLight(17));
            Timer.Tick += (s, e) =>
            {
                Switch3.Tick();
            };
        }

        /// <summary>
        /// Cycle through automation hat LED's
        /// </summary>
        async void Scenario4_Setup()
        {
            var device = new SN3218();
            await device.Test();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    LED1?.Dispose();
                    Switch1?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~MainPage() {
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
