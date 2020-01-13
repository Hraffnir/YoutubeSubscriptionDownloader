using Akka.Actor;
using System;
using System.IO;
using System.Linq;
using VideoLibrary;
using VideoSubscriptionsSaver.Messages;

namespace VideoSubscriptionsSaver.Actors
{
    public class HttpDownloaderActor : ReceiveActor
    {
        public HttpDownloaderActor()
        {
            Receive<HttpDownloaderMessages.DownloadVideo>(Message => DownloadVideo(Message.Url, Message.Channel, Message.Title, Message.DownloadsDirectory));
        }

        private void DownloadVideo(string url, string channel, string title, string downloadsDirectory)
        {
            Console.WriteLine($"Downloading {channel}: {title}");

            using (var service = Client.For(YouTube.Default))
            {
                var videos = service.GetAllVideos(url);

                // TODO: Better handling for video quality and falling back.
                var video = videos.FirstOrDefault(v => v.Resolution > 720);

                var videoBytes = video?.GetBytes();

                if (videoBytes != null)
                {
                    File.WriteAllBytes(@$"{downloadsDirectory}\{title}.mp4", videoBytes);
                }
            }
        }
    }
}