﻿using FeedService.Intrefaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace FeedService.Models
{
    public class RssFeedReader : IFeedReader, IFeed
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PublishedDate { get; set; }
        public IEnumerable<IFeedItem> Items { get; set; }

        public RssFeedReader()
        {
            Url = String.Empty;
            Title = String.Empty;
            Description = String.Empty;
            Items = new List<IFeedItem>();
        }

        public IEnumerable<IFeedItem> ReadFeed(string url)
        {
            try
            {
                var client = new HttpClient();

                var stream = client.GetStreamAsync(url).Result;

                XDocument doc = XDocument.Load(stream);
                // RSS/Channel/item
                var entries = from item in doc.Root.Descendants().First(i => i.Name.LocalName == "channel").Elements().Where(i => i.Name.LocalName == "item")
                              select new RssPost
                              {
                                  Content = item.Elements().First(i => i.Name.LocalName == "description").Value,
                                  Link = item.Elements().First(i => i.Name.LocalName == "link").Value,
                                  PublishedDate = ParseDate(item.Elements().First(i => i.Name.LocalName == "pubDate").Value),
                                  Title = item.Elements().First(i => i.Name.LocalName == "title").Value
                              };
                Items = entries;
                Url = url;
                return entries.ToList();
            }
            catch
            {
                return new List<IFeedItem>();
            }
        }

        private DateTime ParseDate(string date)
        {
            DateTime result;
            if (DateTime.TryParse(date, out result))
                return result;
            else
                return DateTime.MinValue;
        }
    }
}
