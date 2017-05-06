using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Audio;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
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
        Pimoroni.PianoHat Hat;
        Dictionary<Pimoroni.PianoHat.KeyName, AudioFileInputNode> NoteFiles = new Dictionary<Pimoroni.PianoHat.KeyName, AudioFileInputNode>();
        IAudioNode SoundOutputNode;
        AudioGraph SoundGraph;

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
                Hat = await Pimoroni.PianoHat.Open();

                Hat.Instrument.Updated += Instrument_Updated;
                Hat.OctaveUp.Updated += OctaveUp_Updated;
                Hat.OctaveDown.Updated += OctaveDown_Updated;
                Hat.Notes.Updated += Notes_Updated;

                // Set up the player
                var graphresult = await AudioGraph.CreateAsync(new AudioGraphSettings(Windows.Media.Render.AudioRenderCategory.Media));
                SoundGraph = graphresult.Graph;
                var outputresult = await SoundGraph.CreateDeviceOutputNodeAsync();
                SoundGraph.Start();
                SoundOutputNode = outputresult.DeviceOutputNode;

                // Pre-load all the note sounds 
                for (int i = 0; i < 13; i++)
                {
                    var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/Piano/{i+25}.wav"));
                    var inputresult = await SoundGraph.CreateFileInputNodeAsync(file);
                    var node = inputresult.FileInputNode;
                    NoteFiles[(Pimoroni.PianoHat.KeyName)i] = node;
                }

            }
            catch (Exception ex)
            {
                var dialog = new Common.MessageDialog(ex.Message, ex.GetType().Name);
                await dialog.ShowAsync();
                Hat?.Dispose();
            }
        }

        private async void Notes_Updated(Pimoroni.PianoHat.Key sender, EventArgs args)
        {
            try
            {
                if (sender.State)
                {
                    // Serious popping noises
                    // Problem documented here: https://social.msdn.microsoft.com/Forums/en-US/7c312972-6a09-4acd-8a3f-c59485a81d74/clicking-sound-during-start-and-stop-of-audio-playback?forum=WindowsIoT
                    // Solution here: http://ian.bebbs.co.uk/posts/CombiningUwpSpeechSynthesizerWithAudioGraph
                    //Player.Source = file;
                    //Player.Play();
                    var node = NoteFiles[sender.Name];
                    if (node.OutgoingConnections.FirstOrDefault() == null)
                        node.AddOutgoingConnection(SoundOutputNode);
                    node.Seek(TimeSpan.Zero);
                    AddMessage($"NOTE {sender.Name}");
                }
            }
            catch (Exception ex)
            {
                AddMessage($"{ex.GetType().Name}: {ex.Message}");
            }
        }

        private void OctaveDown_Updated(IInput sender, EventArgs args)
        {
            if (sender.State)
            {
                AddMessage($"DOWN");
            }
        }

        private void OctaveUp_Updated(IInput sender, EventArgs args)
        {
            if (sender.State)
            {
                AddMessage($"UP");
            }
        }

        private void AddMessage(string text)
        {
            var background = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Messages.Insert(0, text);
            });
        }

        private void Instrument_Updated(IInput sender, EventArgs args)
        {
            if (sender.State)
            {
                AddMessage( $"INSTRUMENT" );
            }
        }
    }
}
