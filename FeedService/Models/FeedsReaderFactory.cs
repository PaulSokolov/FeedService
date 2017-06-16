using FeedService.DbModels;
using FeedService.Intrefaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;

namespace FeedService.Models
{
    public class FeedsReaderFactory
    {

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

        public static IFeedReader CreateReader(FeedType type, Feed feed)
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
