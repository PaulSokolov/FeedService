using FeedService.Intrefaces;
using System;

namespace FeedService.Models
{
    public class RdfPost : IFeedItem
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Link { get; set; }
        public DateTime PublishedDate { get; set; }
    }
}
