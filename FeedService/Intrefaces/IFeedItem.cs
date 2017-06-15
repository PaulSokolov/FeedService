using System;

namespace FeedService.Intrefaces
{
    public interface IFeedItem
    {
        string Title { get; set; }
        string Content { get; set; }
        string Link { get; set; }
        DateTime PublishedDate { get; set; }
    }
}
