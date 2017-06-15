using FeedService.Controllers;
using FeedService.DbModels;
using FeedService.DbModels.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FeedService.Tests
{
    public class TestFeedServiceController
    {
        [Fact]
        public async Task GetNewsFromCollection()
        {
            var collectionRepository = new Mock<IRepository<Collection>>();
            collectionRepository.Setup(r => r.GetAll()).Returns(GetAllCollections());
            var cache = new Mock<IMemoryCache>();
            FeedServiceController controller = new FeedServiceController(collectionRepository.Object, cache.Object);
            var res = controller.Get(1);
        }

        private IQueryable<Collection> GetAllCollections()
        {
            return new List<Collection>
            {
                new Collection
                {
                    Id = 1,
                    Name = "col",
                    Feeds = new List<Feed>{ new Feed { Id = 1, Type = FeedType.Rss, Url= "https://www.cnet.com/rss/news/" } }
                }
            }.AsQueryable();
        }
    }
}
