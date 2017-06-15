using System.Collections.Generic;

namespace FeedService.DbModels
{
    public class Feed
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public FeedType Type { get; set; }
        public virtual ICollection<Collection> Collections { get; set; }
    }
}
