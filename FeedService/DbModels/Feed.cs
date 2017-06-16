using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FeedService.DbModels
{
    public class Feed
    {
        public int Id { get; set; }
        [Required]
        public string Url { get; set; }
        [Required]
        public FeedType Type { get; set; }

        public virtual ICollection<CollectionFeed> FeedCollections { get; set; }

        public Feed()
        {
            FeedCollections = new HashSet<CollectionFeed>();
        }
    }
}
