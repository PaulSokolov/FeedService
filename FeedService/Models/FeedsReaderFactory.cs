using FeedService.DbModels;
using FeedService.Intrefaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeedService.Models
{
    public class FeedsReaderFactory
    {
        private static IDictionary<string, IFeed> Feeds;
        private static IMemoryCache _cache;

        static FeedsReaderFactory()
        {
            Feeds = new Dictionary<string,IFeed>();
        }

        public FeedsReaderFactory(IMemoryCache cache)
        {
            _cache = cache;
        }

        public static IFeedReader CreateReader(FeedType type)
        {
            switch (type)
            {
                case FeedType.Atom:
                    return new AtomFeedReader();
                case FeedType.Rss:
                    return new RssFeedReader();
            }

            throw new NotImplementedException("There is no such reader implemented");
        }

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
                        Feeds.Add(feed.Url, (IFeed)reader);
                        break;
                    case FeedType.Rss:
                        reader = new RssFeedReader();
                        news.AddRange(reader.ReadFeed(feed.Url));
                        Feeds.Add(feed.Url, (IFeed)reader);
                        break;
                }
            }

            return news;
        }

        public static  IFeed GetFeed(string url)
        {
            if (Feeds.ContainsKey(url))
                return Feeds[url];

            return null;
        }

        public static void CacheNews(IMemoryCache cache)
        {
            _cache = cache;

            foreach(var feed in Feeds)
            {
                IFeed _feed;
                if(!_cache.TryGetValue(feed.Key, out _feed))
                {
                    _cache.Set(feed.Key, feed.Value,new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                }
            }
        }
    }
}
