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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IotFrosting.DrumHat.Sample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Pimoroni.DrumHat Hat;
        private AudioGraph SoundGraph;
        private AudioDeviceOutputNode SoundOutputNode;
        private List<AudioFileInputNode> NoteFiles = new List<AudioFileInputNode>(); 

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                var Hat = await Pimoroni.DrumHat.Open();
                Hat.Pads.Updated += (s, _) => { if (s.State) Play(s.Id); };

                // Set up the player
                var graphresult = await AudioGraph.CreateAsync(new AudioGraphSettings(Windows.Media.Render.AudioRenderCategory.Media));
                SoundGraph = graphresult.Graph;
                var outputresult = await SoundGraph.CreateDeviceOutputNodeAsync();
                SoundGraph.Start();
                SoundOutputNode = outputresult.DeviceOutputNode;

                // Pre-load all the note sounds 
                var files = new string[] { "000_base","001_cowbell","002_clash","003_whistle","004_rim","005_hat","006_snare","007_clap" };
                for (int i = 0; i < 8; i++)
                {
                    var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/Drums2/{files[i]}.wav"));
                    var inputresult = await SoundGraph.CreateFileInputNodeAsync(file);
                    var node = inputresult.FileInputNode;
                    NoteFiles.Add(node);
                }

            }
            catch (Exception ex)
            {
                var dialog = new Common.MessageDialog(ex.Message, ex.GetType().Name);
                await dialog.ShowAsync();
                Hat?.Dispose();
            }
        }

        private void Play(int i)
        {
            var node = NoteFiles[i];
            if (node.OutgoingConnections.FirstOrDefault() == null)
                node.AddOutgoingConnection(SoundOutputNode);
            node.Seek(TimeSpan.Zero);
        }

    }
}
