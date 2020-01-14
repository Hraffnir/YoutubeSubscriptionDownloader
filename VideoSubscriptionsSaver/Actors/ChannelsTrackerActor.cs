using System.IO;
using System.Linq;
using Akka.Actor;
using VideoSubscriptionsSaver.Messages;

namespace VideoSubscriptionsSaver.Actors
{
    public class ChannelsTrackerActor : ReceiveActor
    {
        private readonly string _xmlParserActorPath;
        private readonly string _downloadActorPath;
        private readonly string _downloadFolder;
        private readonly string _subscriptionsFolder;

        public ChannelsTrackerActor(string xmlParserActorPath, string downloadActorPath, string downloadFolder, string subscriptionsFolder)
        {
            _xmlParserActorPath = xmlParserActorPath;
            _downloadActorPath = downloadActorPath;
            _downloadFolder = downloadFolder;
            _subscriptionsFolder = subscriptionsFolder;

            Receive<ChannelsTrackerMessages.CheckChannels>(_ => CheckChannels());
            Receive<ChannelsTrackerMessages.CheckLatestChannelEntries>(CheckForLatestVideos);
        }

        private void CheckChannels()
        {
            var subscriptionsDirectory = new DirectoryInfo(_subscriptionsFolder); // TODO: Config value for this..
            var files = subscriptionsDirectory.GetFiles().Select(f => f.FullName).ToList();
            var message = new XmlParserMessages.GetChannels(files);
            Context.ActorSelection(_xmlParserActorPath).Tell(message);
        }

        private void CheckForLatestVideos(ChannelsTrackerMessages.CheckLatestChannelEntries videoInfo)
        {
            var downloadDir = new DirectoryInfo(_downloadFolder);
            var channelDir = downloadDir.CreateSubdirectory(videoInfo.Feed.Name);
            
            var videos = channelDir.GetFiles().Select(f => f.Name);
            var missingVideos = videoInfo.Feed.Entries.Where(v => !videos.Contains(v.Title));

            foreach (var item in missingVideos)
            {
                var message = new HttpDownloaderMessages.DownloadVideo(item.VideoLink, videoInfo.Feed.Name, item.Title, channelDir.FullName);
                Context.ActorSelection(_downloadActorPath).Tell(message);
            }
        }
    }
}