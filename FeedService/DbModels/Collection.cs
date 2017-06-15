using System.Collections.Generic;

namespace FeedService.DbModels
{
    public class Collection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Feed> Feeds { get; set; }

        public Collection()
        {
            Feeds = new HashSet<Feed>();
        }
    }
}
