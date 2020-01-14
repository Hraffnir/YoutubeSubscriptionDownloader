using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Xml;
using System.Xml.Linq;
using VideoSubscriptionsSaver.Extensions;
using VideoSubscriptionsSaver.Messages;
using VideoSubscriptionsSaver.Models;

namespace VideoSubscriptionsSaver.Actors
{
    public class XmlParserActor : ReceiveActor
    {
        private List<string> videoXmlSources = new List<string>();

        public XmlParserActor()
        {
            Receive<XmlParserMessages.GetChannels>(message => GetChannels(message.Files));
        }

        private void GetChannels(List<string> files)
        {
            foreach (var file in files)
            {
                var doc = new XmlDocument();
                doc.Load(file);

                // TODO: Better way of traversing the XML tree
                RecursiveWalk(doc.FirstChild, "xmlUrl");

                if (videoXmlSources.Any())
                {
                    foreach (var url in videoXmlSources)
                    {
                        XNamespace ns = "http://www.w3.org/2005/Atom";
                        var httpClient = new HttpClient();
                        var result = httpClient.GetAsync(url).Result;

                        var stream = result.Content.ReadAsStreamAsync().Result;

                        var itemXml = XElement.Load(stream);
                        var authorName = itemXml.Element(ns + "author").Element(ns + "name").Value;
                        var videoEntries = itemXml.Elements(ns + "entry");

                        if (videoEntries.Any())
                        {
                            var results = (from entry in videoEntries
                                        select new YoutubeFeedEntry
                                        {
                                            Title = IllegalCharacterReplacer.Replace(entry.Element(ns + "title").Value),
                                            Published = DateTime.Parse(entry.Element(ns + "published").Value),
                                            VideoLink = entry.Element(ns + "link").Attribute("href").Value
                                        }).ToList();

                            var youtubeFeed = new YoutubeFeed();
                            youtubeFeed.Name = IllegalCharacterReplacer.Replace(authorName);
                            youtubeFeed.Entries.AddRange(results);

                            var message = new ChannelsTrackerMessages.CheckLatestChannelEntries(youtubeFeed);
                            Context.ActorSelection(Sender.Path).Tell(message);
                        }
                    }

                }
            }
        }

        private bool GetLatestVideoFromFeed(XmlNode node, string attribute)
        {
            if (node.HasChildNodes)
            {
                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    GetLatestVideoFromFeed(node.ChildNodes[i], attribute);
                }
            }

            return false;
        }

        private bool RecursiveWalk(XmlNode node, string attribute)
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
    }
}
