using System.Collections.Generic;

namespace VideoSubscriptionsSaver.Models
{
    public class YoutubeFeed
    {
        public YoutubeFeed()
        {
            Entries = new List<YoutubeFeedEntry>();
        }

        public string Name { get; set; }

        public List<YoutubeFeedEntry> Entries { get; set; }
    }
}
