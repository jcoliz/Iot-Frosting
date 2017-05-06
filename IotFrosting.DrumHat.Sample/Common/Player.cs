using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.Storage;

namespace Common
{
    public class Player
    {
        private AudioGraph SoundGraph;
        private AudioDeviceOutputNode SoundOutputNode;
        private List<AudioFileInputNode> FileCache = new List<AudioFileInputNode>();

        public static async Task<Player> Open()
        {
            var result = new Player();
            var graphresult = await AudioGraph.CreateAsync(new AudioGraphSettings(Windows.Media.Render.AudioRenderCategory.Media));
            result.SoundGraph = graphresult.Graph;
            var outputresult = await result.SoundGraph.CreateDeviceOutputNodeAsync();
            result.SoundGraph.Start();
            result.SoundOutputNode = outputresult.DeviceOutputNode;

            return result;
        }

        public async void AddToCache(StorageFile file)
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
    }
}
