using FeedService.DbModels;
using FeedService.Intrefaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;

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
                case FeedType.RSS:
                    return new RssFeedReader();
                case FeedType.RDF:
                    return new RdfFeedReader();
            }

            throw new NotImplementedException("There is no such reader implemented");
        }

       
    }
}
