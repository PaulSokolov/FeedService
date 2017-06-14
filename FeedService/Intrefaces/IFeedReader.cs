using System.Collections.Generic;

namespace FeedService.Intrefaces
{
    public interface IFeedReader
    {
        IEnumerable<IFeedItem> ReadFeed(string source);
    }
}
