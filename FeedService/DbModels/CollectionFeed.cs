using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FeedService.DbModels
{
    public class CollectionFeed
    {
        [Key]
        public int FeedId { get; set; }
        public virtual Feed Feed { get; set; }
        [Key]
        public int CollectionId { get; set; }
        public virtual Collection Collection { get; set; }
    }
}
