using Akka.Actor;
using System;
using VideoSubscriptionsSaver.Actors;
using VideoSubscriptionsSaver.Messages;

namespace VideoSubscriptionsSaver
{
    class Program
    {
        public static ActorSystem actorSystem;

        static void Main()
        {
            const string downloadFolder = "C:\\YoutubeDownloads";
            actorSystem = ActorSystem.Create("YouTubeDownloaderSystem");

            var httpDownloader = actorSystem.ActorOf(Props.Create(() => new HttpDownloaderActor()), "httpDownloader");
            var xmlParser = actorSystem.ActorOf(Props.Create(() => new XmlParserActor()), "xmlParser");
            var youtubeSubsTracker = actorSystem.ActorOf(Props.Create(() => new YoutubeSubscriptionsTrackerActor()), "youtubeSubsTracker");
            var channelsTracker = actorSystem.ActorOf(Props.Create(() => new ChannelsTrackerActor(xmlParser.Path.ToString(), httpDownloader.Path.ToString(), downloadFolder)), "channelsTracker");

            actorSystem
                .Scheduler
                .ScheduleTellRepeatedly(
                    TimeSpan.FromSeconds(0),
                    TimeSpan.FromHours(6),
                    channelsTracker,
                    new ChannelsTrackerMessages.CheckChannels(),
                    ActorRefs.NoSender
                );

            actorSystem.WhenTerminated.Wait(); // TODO: Gracefully shut down. Not apparent in docs how to handle this.
        }
    }
}
