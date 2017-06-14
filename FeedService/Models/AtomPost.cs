using FeedService.Intrefaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FeedService.Models
{
    public class AtomPost : IFeedItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string PublishedDate { get; set; }

        public AtomPost(XElement post)
        {
            Title = post.Element("title").Value;
            Description = post.Element("description").Value;
            PublishedDate = post.Element("pubDate").Value;
        }
    }
}
