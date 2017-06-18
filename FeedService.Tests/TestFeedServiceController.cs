using FeedService.Controllers;
using FeedService.DbModels;
using FeedService.DbModels.Interfaces;
using FeedService.Tests.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FeedService.Tests
{
    public class TestFeedServiceController
    {
        [Fact]
        public void GetNewsFromCollection()
        {
            var collections = GetAllCollections().ToAsyncDbSetMock();
            var collectionFeeds = GetAllCollectionFeeds().ToAsyncDbSetMock();

            var collectionRepository = new Mock<IRepository<Collection>>();
            collectionRepository.Setup(r => r.GetAll()).Returns(collections.Object);
            var feedRepository = new Mock<IRepository<Feed>>();
            feedRepository.Setup(r => r.GetAll()).Returns(new List<Feed> { new Feed { Type = FeedType.RSS, Url = "" } }.AsQueryable());
            var collectionFeedsRepository = new Mock<IRepository<CollectionFeed>>();
            collectionFeedsRepository.Setup(r => r.GetAll()).Returns(collectionFeeds.Object);

            var feedServiceUnit = new Mock<IFeedServiceUoW>();
            feedServiceUnit.SetupGet(fsu => fsu.Feeds).Returns(feedRepository.Object);
            feedServiceUnit.SetupGet(fsu => fsu.Collections).Returns(collectionRepository.Object);
            feedServiceUnit.SetupGet(fsu => fsu.CollectionsFeeds).Returns(collectionFeedsRepository.Object);

            var cache = new Mock<IMemoryCache>();
            var cacheEntry = new Mock<ICacheEntry>();
            cache.Setup(c => c.CreateEntry(It.IsAny<object>())).Returns(cacheEntry.Object);

            var logger = new Mock<ILogger<FeedServiceController>>();

            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(h => h.User.Identity.Name).Returns("Paul");

            FeedServiceController controller = new FeedServiceController(feedServiceUnit.Object, cache.Object, logger.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext.Object
            };

            var actionRes = controller.Get(1).GetAwaiter().GetResult();
            dynamic redirectToActionResult = Assert.IsType<OkObjectResult>(actionRes);
            Assert.Equal(StatusCodes.Status200OK, redirectToActionResult.StatusCode);
            Assert.NotNull(redirectToActionResult.Value.Result);
        }

        private IQueryable<Collection> GetAllCollections()
        {
            return new List<Collection>
            {
                new Collection
                {
                    Id = 1,
                    Name = "col",
                    User = new User{ Id = 1, Login = "Paul", Password = "pass", Role = "user"},
                    CollectionFeeds = new List<CollectionFeed>{ new CollectionFeed{ Feed = new Feed { Id = 1, Type = FeedType.RSS, Url = "https://www.cnet.com/rss/news/" } } }
                }
            }.AsQueryable();
        }

        private IQueryable<CollectionFeed> GetAllCollectionFeeds()
        {
            return new List<CollectionFeed>
            {
                     new CollectionFeed
                     {
                         Feed = new Feed { Id = 1, Type = FeedType.RSS, Url = "https://www.cnet.com/rss/news/" },
                         FeedId =1,
                         Collection = new Collection{Id = 1,Name = "col" }
                     ,
                         CollectionId = 1
                     }

            }.AsQueryable();
        }
    }
}
