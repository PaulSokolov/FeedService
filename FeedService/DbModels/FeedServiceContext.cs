using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeedService.DbModels
{
    public class FeedServiceContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Feed> Feeds { get; set; }

        public FeedServiceContext(DbContextOptions<FeedServiceContext> options)
            :base(options)
        {

        }
    }
}
