namespace VideoSubscriptionsSaver.Messages
{
    public class HttpDownloaderMessages
    {
        public class DownloadVideo 
        {
            public DownloadVideo(string url, string channel, string title, string downloadsDirectory)
            {
                Url = url;
                Channel = channel;
                Title = title;
                DownloadsDirectory = downloadsDirectory;
            }

            public string Url { get; set; }

            public string Channel { get; set; }

            public string Title { get; set; }

            public string DownloadsDirectory { get; set; }
        }
    }
}
