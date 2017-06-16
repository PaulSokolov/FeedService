using FeedService.Controllers;
using FeedService.DbModels;
using FeedService.DbModels.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            var feedRepository = new Mock<IRepository<Feed>>();
            feedRepository.Setup(r => r.GetAll()).Returns(new List<Feed> { new Feed { Type = FeedType.RSS, Url = ""  } }.AsQueryable());

            var feedServiceUnit = new Mock<IFeedServiceUoW>();
            feedServiceUnit.SetupGet(fsu => fsu.Feeds).Returns(feedRepository.Object);
            feedServiceUnit.SetupGet(fsu => fsu.Collections).Returns(collectionRepository.Object);

            var cache = new Mock<IMemoryCache>();
            var cacheEntry = new Mock<ICacheEntry>();
            cache.Setup(c => c.CreateEntry(It.IsAny<object>())).Returns(cacheEntry.Object);

            FeedServiceController controller = new FeedServiceController(feedServiceUnit.Object, cache.Object);
            var actionRes = controller.Get(1).GetAwaiter().GetResult();
            var redirectToActionResult = Assert.IsType<OkObjectResult>(actionRes);
            Assert.Equal(StatusCodes.Status200OK, redirectToActionResult.StatusCode);
            Assert.NotNull(redirectToActionResult.Value);
        }

        private IQueryable<Collection> GetAllCollections()
        {
            return new List<Collection>
            {
                new Collection
                {
                    Id = 1,
                    Name = "col",
                    CollectionFeeds = new List<CollectionFeed>{ new CollectionFeed{ Feed = new Feed { Id = 1, Type = FeedType.RSS, Url = "https://www.cnet.com/rss/news/" } } }
                }
            }.AsQueryable();
        }
    }
}
