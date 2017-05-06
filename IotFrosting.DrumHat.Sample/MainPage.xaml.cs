using IotFrosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Audio;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace IotFrosting.DrumHat.Sample
{
    public sealed partial class MainPage : Page
    {
        Pimoroni.DrumHat Hat;
        Common.Player Player;

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                Hat = await Pimoroni.DrumHat.Open();
                Hat.Pads.Updated += (s, _) => { if (s.State) Player.Play(s.Id); };

                Player = await Common.Player.Open();
                foreach (var file in new string[] { "000_base", "001_cowbell", "002_clash", "003_whistle", "004_rim", "005_hat", "006_snare", "007_clap" } )
                {
                    var sf = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/Drums2/{file}.wav"));
                    Player.AddToCache(sf);
                }

            }
            catch (Exception ex)
            {
                var dialog = new Common.MessageDialog(ex.Message, ex.GetType().Name);
                await dialog.ShowAsync();
                Hat?.Dispose();
            }
        }
    }
}
