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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CollectionFeed>()
            .HasKey(t => new { t.CollectionId, t.FeedId});

            modelBuilder.Entity<CollectionFeed>()
                .HasOne(pt => pt.Collection)
                .WithMany(p => p.CollectionFeeds)
                .HasForeignKey(pt => pt.CollectionId);

            modelBuilder.Entity<CollectionFeed>()
                .HasOne(pt => pt.Feed)
                .WithMany(t => t.FeedCollections)
                .HasForeignKey(pt => pt.FeedId);
            base.OnModelCreating(modelBuilder);
        }
    }
}
