using System;
using System.Collections.Generic;
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

        public ChannelsTrackerActor(string xmlParserActorPath, string downloadActorPath, string downloadFolder)
        {
            _xmlParserActorPath = xmlParserActorPath;
            _downloadActorPath = downloadActorPath;
            _downloadFolder = downloadFolder;

            Receive<ChannelsTrackerMessages.CheckChannels>(_ => CheckChannels());
            Receive<ChannelsTrackerMessages.CheckLatestChannelEntries>(CheckForLatestVideos);
        }

        private void CheckChannels()
        {
            // TODO: Rather just look at a "subscription files" folder for subscriptions.
            // They will need to be uniquely named. But purpose of the app is for a single person's channel.
            var files = new List<string>
            {
                "SubscriptionFiles/subscription_manager.xml"
            };
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