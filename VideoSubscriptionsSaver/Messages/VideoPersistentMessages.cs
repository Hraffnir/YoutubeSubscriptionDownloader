namespace VideoSubscriptionsSaver.Messages
{
    public class VideoPersistentMessages
    {
        public class Add
        {
            public Add(string title, string url, string file)
            {
                Title = title;
                Url = url;
                File = file;
            }

            public string Title { get; set; }

            public string Url { get; set; }

            public string File { get; set; }
        }

        public class Delete
        {
            public Delete(string title)
            {
                Title = title;
            }

            public string Title { get; set; }
        }

        public class Get
        {
            public Get(string title)
            {
                Title = title;
            }

            public string Title { get; set; }
        }
    }
}
