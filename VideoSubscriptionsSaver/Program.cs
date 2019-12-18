using System;
using System.IO;
using VideoLibrary;
using System.Linq;
using System.Xml;
using System.Collections.Generic;
using System.Net.Http;
using System.Xml.Linq;

namespace VideoSubscriptionsSaver
{
    class Program
    {
        public static List<string> videoXmlSources = new List<string>();
        public static List<string> videoEntries = new List<string>();
        public static List<string> videoDownloadUrls = new List<string>();

        static void Main(string[] args)
        {

            var doc = new XmlDocument();
            doc.Load("SubscriptonFiles/subscription_manager.xml");
            RecursiveWalk(doc.FirstChild, "xmlUrl");

            if (videoXmlSources.Any())
            {
                foreach (var url in videoXmlSources)
                {
                    var httpClient = new HttpClient();
                    var result = httpClient.GetAsync(url).Result;

                    var stream = result.Content.ReadAsStreamAsync().Result;

                    var itemXml = XElement.Load(stream);
                    

                    var allElements = itemXml.Elements();
                    if(allElements.Any())
                    {
                        XNamespace ns = "http://www.w3.org/2005/Atom";
                        var lastEntry = allElements.Last();
                        var link = lastEntry.Element(ns + "link");

                        if (link != null && link.HasAttributes)
                        {
                            videoDownloadUrls.Add(link.Attributes("href").FirstOrDefault().Value);
                        }
                    }
                }

                GetVideos();
            }

            Console.ReadLine();
        }

        static bool RecursiveWalk(XmlNode node, string attribute)
        {
            if (node.HasChildNodes)
            {
                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    RecursiveWalk(node.ChildNodes[i], attribute);
                }
            }
            else
            {
                videoXmlSources.Add(node.Attributes[attribute].Value);
            }

            return false;
        }
        static bool GetLatestVideoFromFeed(XmlNode node, string attribute)
        {
            if (node.HasChildNodes)
            {
                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    GetLatestVideoFromFeed(node.ChildNodes[i], attribute);
                }
            }
            else
            {
            }

            return false;
        }

        static void GetVideos()
        {
            var youtubeDownloader = new YouTube();

            foreach(var url in videoDownloadUrls)
            {
                var videos = youtubeDownloader.GetAllVideos(url);
                var video = videos.Where(c => c.Resolution == 240);

                if (video.Any())
                {
                    var first = video.First();
                    var videoBytes = first.GetBytes();
                    var videoName = first.FullName;

                    File.WriteAllBytes(@"C:\YoutubeDownloads\" + videoName, videoBytes);
                }
            }
        }
    }
}
