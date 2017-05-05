using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        CAP1XXX Cap1;
        CAP1XXX Cap2;
        Dictionary<KeyNames, IMediaPlaybackSource> NoteFiles = new Dictionary<KeyNames, IMediaPlaybackSource>();
        MediaPlayer Player = new MediaPlayer();

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
                Cap2 = await CAP1XXX.Open(0x2B, 27);

                for (int i = 0; i < 8; i++)
                {
                    Cap1.Inputs[i] = new Input() { Name = (KeyNames)i };
                    Cap2.Inputs[i] = new Input() { Name = (KeyNames)(i+8) };
                }

                Notes.AddRange(Cap1.Inputs);
                Notes.AddRange(Cap2.Inputs.Take(5));

                Player.AutoPlay = false;

                Instrument.Updated += Instrument_Updated;
                OctaveUp.Updated += OctaveUp_Updated;
                OctaveDown.Updated += OctaveDown_Updated;
                Notes.Updated += Notes_Updated;

                for (int i = 0; i < 13; i++)
                {
                    var file = MediaSource.CreateFromUri(new Uri($"ms-appx:///Assets/Piano/{i+25}.wav"));
                    NoteFiles[(KeyNames)i] = file;
                }

            }
            catch (Exception ex)
            {
                var dialog = new Common.MessageDialog(ex.Message, ex.GetType().Name);
                await dialog.ShowAsync();
                Cap1?.Dispose();
                Cap2?.Dispose();
            }
        }

        private void Notes_Updated(Input sender, EventArgs args)
        {
            try
            {
                if (sender.State)
                {
                    // Serious popping noises
                    // Problem documented here: https://social.msdn.microsoft.com/Forums/en-US/7c312972-6a09-4acd-8a3f-c59485a81d74/clicking-sound-during-start-and-stop-of-audio-playback?forum=WindowsIoT
                    // Solution here: http://ian.bebbs.co.uk/posts/CombiningUwpSpeechSynthesizerWithAudioGraph
                    var file = NoteFiles[sender.Name];
                    Player.Source = file;
                    Player.Play();

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

        private void Pad_Updated(IInput sender, EventArgs args)
        {
            AddMessage( $"State:{sender.State} From:{sender}" );
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TB.Text = Cap1.R_MainControl.ToString() + " / " + Cap2.R_MainControl.ToString();

            Cap1.R_MainControl &= 0xfe;
            Cap2.R_MainControl &= 0xfe;
        }

        public enum KeyNames { C = 0, Csharp, D, Dsharp, E, F, Fsharp, G, Gsharp, A, Asharp, B, C2, OctaveUp, OctaveDown, Instrument };

        IInput Instrument => Cap2.Inputs[7];
        IInput OctaveUp => Cap2.Inputs[6];
        IInput OctaveDown => Cap2.Inputs[5];

        MultiplexInput Notes = new MultiplexInput();

        public class Input: CAP1XXX.Input
        {
            public KeyNames Name { get; set; }
        }

        public delegate void InputUpdateEventHandler(Input sender, EventArgs args);

        public class MultiplexInput
        {
            public void AddRange(IEnumerable<IInput> inputs)
            {
                foreach (var i in inputs)
                {
                    i.Updated += (s, a) => Updated?.Invoke(s as Input, a);
                }
            }

            public event InputUpdateEventHandler Updated;
        }
    }
}
