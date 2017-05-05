using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace IotFrosting.PianoHat.Sample
{
    public sealed partial class MainPage : Page
    {
        CAP1XXX Cap1;
        CAP1XXX Cap2;

        public ObservableCollection<string> Messages { get; } = new ObservableCollection<string>();

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                Cap1 = await CAP1XXX.Open(0x28, 4);
                Cap1.Inputs.ForEach(x => x.Updated += Pad_Updated);
                Cap2 = await CAP1XXX.Open(0x2B, 27);
                Cap2.Inputs.ForEach(x => x.Updated += Pad_Updated);
            }
            catch (Exception ex)
            {
                var dialog = new Common.MessageDialog(ex.Message, ex.GetType().Name);
                await dialog.ShowAsync();
                Cap1?.Dispose();
                Cap2?.Dispose();
            }
        }

        private void Pad_Updated(IInput sender, EventArgs args)
        {
            var background = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, ()=> 
            {
                Messages.Insert(0, $"State:{sender.State} From:{sender}");
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TB.Text = Cap1.R_MainControl.ToString() + " / " + Cap2.R_MainControl.ToString();

            Cap1.R_MainControl &= 0xfe;
            Cap2.R_MainControl &= 0xfe;
        }
    }
}
