using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Windows.Storage;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace IotFrosting.PianoOrDrumHat.Background.Sample
{
    public sealed class StartupTask : IBackgroundTask
    {
        BackgroundTaskDeferral Deferral;
        Pimoroni.DrumHat Drum;
        Pimoroni.PianoHat Piano;
        Common.Player Player;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // If you start any asynchronous methods here, prevent the task
            // from closing prematurely by using BackgroundTaskDeferral as
            // described in http://aka.ms/backgroundtaskdeferral
            //
            Deferral = taskInstance.GetDeferral();

            // 1. Load up all media assets
            Player = await Common.Player.Open();
            foreach (var file in new string[] { "000_base", "001_cowbell", "002_clash", "003_whistle", "004_rim", "005_hat", "006_snare", "007_clap" })
            {
                var sf = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/Drums2/{file}.wav"));
                await Player.AddToCache(sf);
            }
            for (int i = 0; i < 13; i++)
            {
                var sf = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/Piano/{i + 25}.wav"));
                await Player.AddToCache(sf);
            }

            // 2. Try to start the drum hat. OK to fail if not connected
            try
            {
                Drum = await Pimoroni.DrumHat.Open();
                Drum.Pads.Updated += (s, _) => { if (s.State) Player.Play(s.Id); };
            }
            catch (Exception)
            {
                Drum?.Dispose();
            }

            // 3. Try to start the piano hat. OK to fail if not connected
            try
            {
                Piano = await Pimoroni.PianoHat.Open();
                Piano.Notes.Updated += (s, _) => { if (s.State) Player.Play(8 + (int)s.Name); };
            }
            catch (Exception)
            {
                Piano?.Dispose();
            }
        }
    }
}
