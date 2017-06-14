using FeedService.Intrefaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FeedService.Models
{
    public class RssFeedReader : IFeedReader
    {
        public IEnumerable<IFeedItem> ReadFeed(string url)
        {
            var rssFeed = XDocument.Load(url);

            var posts = from item in rssFeed.Descendants("item")
                        select new RssPost(item);
            //{
            //    Title = item.Element("title").Value,
            //    Description = item.Element("description").Value,
            //    PublishedDate = item.Element("pubDate").Value
            //};

            return posts;
        }
    }
}
