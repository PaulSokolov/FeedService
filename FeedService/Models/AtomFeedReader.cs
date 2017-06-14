﻿using FeedService.Intrefaces;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FeedService.Models
{
    public class AtomFeedReader : IFeedReader, IFeed
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string PublishedDate { get; set; }
        public IEnumerable<IFeedItem> Items { get; set; }

        public IEnumerable<IFeedItem> ReadFeed(string url)
        {
            var rssFeed = XDocument.Load(url);

            var posts = from item in rssFeed.Descendants("item")
                        select new AtomPost(item);
            //{
            //    Title = item.Element("title").Value,
            //    Description = item.Element("description").Value,
            //    PublishedDate = item.Element("pubDate").Value
            //};

            return posts;
        }
    }
}
