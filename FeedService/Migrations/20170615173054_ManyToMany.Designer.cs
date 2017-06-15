using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using FeedService.DbModels;

namespace FeedService.Migrations
{
    [DbContext(typeof(FeedServiceContext))]
    [Migration("20170615173054_ManyToMany")]
    partial class ManyToMany
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("FeedService.DbModels.Collection", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Collections");
                });

            modelBuilder.Entity("FeedService.DbModels.CollectionFeed", b =>
                {
                    b.Property<int>("CollectionId");

                    b.Property<int>("FeedId");

                    b.HasKey("CollectionId", "FeedId");

                    b.HasIndex("FeedId");

                    b.ToTable("CollectionFeed");
                });

            modelBuilder.Entity("FeedService.DbModels.Feed", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Type");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.ToTable("Feeds");
                });

            modelBuilder.Entity("FeedService.DbModels.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Login");

                    b.Property<string>("Password");

                    b.Property<string>("Role");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("FeedService.DbModels.Collection", b =>
                {
                    b.HasOne("FeedService.DbModels.User", "User")
                        .WithMany("Collections")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("FeedService.DbModels.CollectionFeed", b =>
                {
                    b.HasOne("FeedService.DbModels.Collection", "Collection")
                        .WithMany("CollectionFeeds")
                        .HasForeignKey("CollectionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FeedService.DbModels.Feed", "Feed")
                        .WithMany("FeedCollections")
                        .HasForeignKey("FeedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
