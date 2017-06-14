using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FeedService.DbModels
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public virtual ICollection<Collection> Collections{ get; set; }

        public User()
        {
            Collections = new HashSet<Collection>();
        }
    }
    public class Collection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Feed> Feeds { get; set; }

        public Collection()
        {
            Feeds = new HashSet<Feed>();
        }
    }
    public class Feed
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public FeedType Type { get; set; }
    }
    public enum FeedType
    {
        Atom,
        Rss
    }
}
