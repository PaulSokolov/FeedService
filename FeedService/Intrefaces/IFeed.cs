using System.Collections.Generic;

namespace FeedService.Intrefaces
{
    public interface IFeed
    {
        string Url { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        string PublishedDate { get; set; }
        IEnumerable<IFeedItem> Items { get; set; }
    }
}
