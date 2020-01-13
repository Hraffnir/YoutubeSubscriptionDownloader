using System.Collections.Generic;

namespace VideoSubscriptionsSaver.Messages
{
    public class XmlParserMessages
    {
        public class GetChannels
        {
            public GetChannels(List<string> files)
            {
                Files = files;
            }

            public List<string> Files { get; set; }
        }
    }
}
