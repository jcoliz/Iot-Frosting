using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Audio;
using Windows.Storage;

namespace IotFrosting.PianoOrDrumHat.Background.Sample.Common
{
    /// <summary>
    /// Built-in media player has popping issues on RPi, so we use AudioGraph
    /// </summary>
    /// <remarks>
    /// Problem: https://social.msdn.microsoft.com/Forums/en-US/7c312972-6a09-4acd-8a3f-c59485a81d74/clicking-sound-during-start-and-stop-of-audio-playback?forum=WindowsIoT
    /// Solution: https://mtaulty.com/2017/01/15/windows-10-uwp-iot-core-speechsynthesizer-raspberry-pi-and-audio-popping/
    /// </remarks>
    public class Player
    {
        // https://marcominerva.wordpress.com/2013/03/21/how-to-expose-async-methods-in-a-windows-runtime-component/
        public static IAsyncOperation<Player> Open()
        {
            return Open_().AsAsyncOperation<Player>();
        }

        private static async Task<Player> Open_()
        {
            var result = new Player();
            var graphresult = await AudioGraph.CreateAsync(new AudioGraphSettings(Windows.Media.Render.AudioRenderCategory.Media));
            result.SoundGraph = graphresult.Graph;
            var outputresult = await result.SoundGraph.CreateDeviceOutputNodeAsync();
            result.SoundGraph.Start();
            result.SoundOutputNode = outputresult.DeviceOutputNode;

            return result;
        }

        // https://marcominerva.wordpress.com/2013/03/21/how-to-expose-async-methods-in-a-windows-runtime-component/
        public IAsyncAction AddToCache(StorageFile file)
        {
            return AddToCache_(file).AsAsyncAction();
        }

        private async Task AddToCache_(StorageFile file)
        {
            var inputresult = await SoundGraph.CreateFileInputNodeAsync(file);
            var node = inputresult.FileInputNode;
            FileCache.Add(node);
        }

        public void Play(int i)
        {
            var node = FileCache[i];
            if (node.OutgoingConnections.FirstOrDefault() == null)
                node.AddOutgoingConnection(SoundOutputNode);
            node.Seek(TimeSpan.Zero);
        }

        private Player()
        {
        }

        private AudioGraph SoundGraph;
        private AudioDeviceOutputNode SoundOutputNode;
        private List<AudioFileInputNode> FileCache = new List<AudioFileInputNode>();
    }
}
