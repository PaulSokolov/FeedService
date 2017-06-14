using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FeedService.Intrefaces
{
    public interface IFeed
    {
        string Title { get; set; }
        string Description { get; set; }
        string PublishedDate { get; set; }
        IEnumerable<IFeedItem> Items { get; set; }
    }

    public interface IFeedItem
    {
    }

    public interface IFeedSource<T>
    {
        T GetSource();
    }

    public interface IFeedReader<FeedSource>
    {
        IEnumerable<IFeed> ReadFeed(FeedSource source);
    }
    

    public class RssPost:IFeed
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string PublishedDate { get; set; }
        public IEnumerable<IFeedItem> Items { get; set; }

        public RssPost(XElement post)
        {
            Title = post.Element("title").Value;
            Description = post.Element("description").Value;
            PublishedDate = post.Element("pubDate").Value;
        }
    }

    public class AtomPost : IFeed
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string PublishedDate { get; set; }
        public IEnumerable<IFeedItem> Items { get; set; }

        public AtomPost(XElement post)
        {
            Title = post.Element("title").Value;
            Description = post.Element("description").Value;
            PublishedDate = post.Element("pubDate").Value;
        }
    }

    public class RssFeedReader:IFeedReader<string>
    {
        public IEnumerable<IFeed> ReadFeed(string source)
        {
            var rssFeed = XDocument.Load(source);

            var posts = from item in rssFeed.Descendants("item")
                        select new RssPost(item);
                        //{
                        //    Title = item.Element("title").Value,
                        //    Description = item.Element("description").Value,
                        //    PublishedDate = item.Element("pubDate").Value
                        //};

            return posts;
        }
    }

    public class AtomFeedReader : IFeedReader<string>
    {
        public IEnumerable<IFeed> ReadFeed(string source)
        {
            var rssFeed = XDocument.Load(source);

            var posts = from item in rssFeed.Descendants("item")
                        select new AtomPost(item);
            //{
            //    Title = item.Element("title").Value,
            //    Description = item.Element("description").Value,
            //    PublishedDate = item.Element("pubDate").Value
            //};

            return posts;
        }
    }
}
