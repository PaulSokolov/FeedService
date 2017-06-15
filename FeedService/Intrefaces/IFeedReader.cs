using System.Collections.Generic;

namespace FeedService.Intrefaces
{
    public interface IFeedReader : IFeed
    {
        IEnumerable<IFeedItem> ReadFeed(string source);
    }
}
