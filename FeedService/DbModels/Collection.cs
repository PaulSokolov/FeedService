using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FeedService.DbModels
{
    public class Collection
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        //public virtual ICollection<Feed> Feeds { get; set; }
        public virtual ICollection<CollectionFeed> CollectionFeeds { get; set; }

        public Collection()
        {
            CollectionFeeds = new HashSet<CollectionFeed>();
        }
    }
}
