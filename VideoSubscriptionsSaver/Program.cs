using Akka.Actor;
using System;
using System.IO;
using Akka.Routing;
using Microsoft.Extensions.Configuration;
using VideoSubscriptionsSaver.Actors;
using VideoSubscriptionsSaver.Messages;

namespace VideoSubscriptionsSaver
{
    class Program
    {
        private static ActorSystem _actorSystem;

        static void Main()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false);
            var config = builder.Build();

            _actorSystem = ActorSystem.Create("YouTubeDownloaderSystem", @"
                akka {  
                    stdout-loglevel = INFO
                    loglevel = INFO
                    log-config-on-start = on        
                    actor {                
                        debug {  
                        receive = on 
                        autoreceive = on
                        lifecycle = on
                        event-stream = on
                        unhandled = on
                    }
                }");

            var numberOfDownloaders = short.Parse(config["akka:concurrentVideoDownloaders"]);
            var downloaderProps = Props.Create<HttpDownloaderActor>().WithRouter(new RoundRobinPool(numberOfDownloaders));

            var httpDownloader = _actorSystem.ActorOf(downloaderProps, "httpDownloaders");

            var xmlParser = _actorSystem.ActorOf(Props.Create(() => new XmlParserActor()), "xmlParser");

            var channelsTracker = _actorSystem.ActorOf(Props.Create(() => 
                new ChannelsTrackerActor(xmlParser.Path.ToString(), httpDownloader.Path.ToString(), config["downloadDir"], config["subscriptionsDir"])), 
                "channelsTracker");

            _actorSystem
                .Scheduler
                .ScheduleTellRepeatedly(
                    TimeSpan.FromSeconds(0),
                    TimeSpan.FromHours(6),
                    channelsTracker,
                    new ChannelsTrackerMessages.CheckChannels(),
                    ActorRefs.NoSender
                );

            _actorSystem.WhenTerminated.Wait();
        }
    }
}
