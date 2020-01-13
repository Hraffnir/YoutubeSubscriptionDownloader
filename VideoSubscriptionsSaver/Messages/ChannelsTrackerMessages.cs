using VideoSubscriptionsSaver.Models;

namespace VideoSubscriptionsSaver.Messages
{
    public class ChannelsTrackerMessages
    {
        public class CheckChannels { }

        public class CheckLatestChannelEntries
        {
            public CheckLatestChannelEntries(YoutubeFeed feed)
            {
                Feed = feed;
            }

            public YoutubeFeed Feed { get; set; }
        }
    }
}
