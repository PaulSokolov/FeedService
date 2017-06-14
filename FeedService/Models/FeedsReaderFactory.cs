using FeedService.DbModels;
using FeedService.Intrefaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeedService.Models
{
    public class FeedsReaderFactory
    {
        public static IEnumerable<IFeedItem> GetNews(Collection feedCollection)
        {
            List<IFeedItem> news = new List<IFeedItem>();

            foreach (var feed in feedCollection.Feeds)
            {
                IFeedReader reader;
                switch (feed.Type)
                {
                    case FeedType.Atom:
                        reader = new AtomFeedReader();
                        news.AddRange(reader.ReadFeed(feed.Url));
                        break;
                    case FeedType.Rss:
                        reader = new RssFeedReader();
                        news.AddRange(reader.ReadFeed(feed.Url));
                        break;
                }
            }

            return news;
        }
    }
}
