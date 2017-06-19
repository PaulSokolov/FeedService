using FeedService.Controllers;
using FeedService.DbModels;
using FeedService.DbModels.Interfaces;
using FeedService.Infrastructure.Response;
using FeedService.Tests.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace FeedService.Tests
{
    public class TestCollectionsController
    {
        [Fact]
        public void GetCollections_returns_3_collections_in_result()
        {
            #region Arrange

            var userRepository = new Mock<IRepository<User>>();
            userRepository.Setup(r => r.GetAll()).Returns(GetAllUsers());
            var collectionRepository = new Mock<IRepository<Collection>>();
            collectionRepository.Setup(r => r.GetAll()).Returns(GetAllCollections());

            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(h => h.User.Identity.Name).Returns("Paul");
            var feedServiceUnit = new Mock<IFeedServiceUoW>();
            feedServiceUnit.SetupGet(fsu => fsu.Users).Returns(userRepository.Object);
            feedServiceUnit.SetupGet(fsu => fsu.Collections).Returns(collectionRepository.Object);

            var logger = new Mock<ILogger<CollectionsController>>();

            CollectionsController controller = new CollectionsController(feedServiceUnit.Object, logger.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext.Object
            };
            var expected = 2;

            #endregion
            #region Act

            var actionRes = controller.GetCollections();
            var redirectToActionResult = Assert.IsType<OkObjectResult>(actionRes);

            #endregion
            #region Assert

            var successAnswer = Assert.IsType<SuccessObject>(redirectToActionResult.Value);
            JArray ob = JArray.FromObject(successAnswer.Result);
            Assert.Equal(StatusCodes.Status200OK, redirectToActionResult.StatusCode);
            Assert.Equal(expected, ob.Count); 

            #endregion

        }

        [Fact]
        public void GetCollection_another_user_collectionId_returnsNotFound()
        {

            #region Arrange

            var collections = GetAllCollections().ToAsyncDbSetMock();
            var collectionRepository = new Mock<IRepository<Collection>>();
            collectionRepository.Setup(r => r.GetAll()).Returns(collections.Object);

            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(h => h.User.Identity.Name).Returns("Paul");
            var feedServiceUnit = new Mock<IFeedServiceUoW>();
            feedServiceUnit.SetupGet(fsu => fsu.Collections).Returns(collectionRepository.Object);

            var logger = new Mock<ILogger<CollectionsController>>();

            CollectionsController controller = new CollectionsController(feedServiceUnit.Object, logger.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext.Object
            };

            #endregion
            #region Act

            var actionRes = controller.GetCollection(3).GetAwaiter().GetResult();

            #endregion
            #region Assert

            var redirectToActionResult = Assert.IsType<NotFoundObjectResult>(actionRes);
            Assert.Equal(StatusCodes.Status404NotFound, redirectToActionResult.StatusCode); 

            #endregion

        }

        [Fact]
        public void AddFeedToCollection_AddingExistingFeedToCollection_BadRequestExopected()
        {
            #region Arrange

            var collections = GetAllCollections().ToAsyncDbSetMock();
            var feeds = GetAllFeeds().ToAsyncDbSetMock();
            var collectionFeeds = GetAllCollectionFeeds().ToAsyncDbSetMock();

            var collectionRepository = new Mock<IRepository<Collection>>();
            collectionRepository.Setup(r => r.GetAll()).Returns(collections.Object);

            var collectionFeedsRepository = new Mock<IRepository<CollectionFeed>>();
            collectionFeedsRepository.Setup(r => r.GetAll()).Returns(collectionFeeds.Object);

            var feedRepository = new Mock<IRepository<Feed>>();
            feedRepository.Setup(r => r.GetAll()).Returns(feeds.Object);

            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(h => h.User.Identity.Name).Returns("Paul");

            var feedServiceUnit = new Mock<IFeedServiceUoW>();
            feedServiceUnit.SetupGet(fsu => fsu.Collections).Returns(collectionRepository.Object);
            feedServiceUnit.SetupGet(fsu => fsu.Feeds).Returns(feedRepository.Object);
            feedServiceUnit.SetupGet(fsu => fsu.CollectionsFeeds).Returns(collectionFeedsRepository.Object);

            var logger = new Mock<ILogger<CollectionsController>>();

            CollectionsController controller = new CollectionsController(feedServiceUnit.Object, logger.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext.Object
            };

            #endregion
            #region Act

            var actionRes = controller.AddFeedToCollection(1, GetAllFeeds().ToList()[0]).GetAwaiter().GetResult();

            #endregion
            #region Assert

            var redirectToActionResult = Assert.IsType<BadRequestObjectResult>(actionRes);
            Assert.Equal(StatusCodes.Status400BadRequest, redirectToActionResult.StatusCode);

            #endregion
        }

        [Fact]
        public void AddFeedToCollection_AddingNewFeedToCollection_OkResult()
        {

            #region Arrange

            var collections = GetAllCollections().ToAsyncDbSetMock();
            var feeds = GetAllFeeds().ToAsyncDbSetMock();
            var collectionFeeds = GetAllCollectionFeeds().ToAsyncDbSetMock();

            var collectionRepository = new Mock<IRepository<Collection>>();
            collectionRepository.Setup(r => r.GetAll()).Returns(collections.Object);

            var collectionFeedsRepository = new Mock<IRepository<CollectionFeed>>();
            collectionFeedsRepository.Setup(r => r.GetAll()).Returns(collectionFeeds.Object);

            var feedRepository = new Mock<IRepository<Feed>>();
            feedRepository.Setup(r => r.GetAll()).Returns(feeds.Object);

            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(h => h.User.Identity.Name).Returns("Paul");

            var feedServiceUnit = new Mock<IFeedServiceUoW>();
            feedServiceUnit.SetupGet(fsu => fsu.Collections).Returns(collectionRepository.Object);
            feedServiceUnit.SetupGet(fsu => fsu.Feeds).Returns(feedRepository.Object);
            feedServiceUnit.SetupGet(fsu => fsu.CollectionsFeeds).Returns(collectionFeedsRepository.Object);

            var logger = new Mock<ILogger<CollectionsController>>();

            CollectionsController controller = new CollectionsController(feedServiceUnit.Object, logger.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext.Object
            };

            var feed = new Feed { Id = 2, Type = FeedType.Atom, Url = "newFeed" };

            #endregion
            #region Act

            var actionRes = controller.AddFeedToCollection(1, feed).GetAwaiter().GetResult();

            #endregion
            #region Assert

            var redirectToActionResult = Assert.IsType<OkObjectResult>(actionRes);
            Assert.Equal(StatusCodes.Status200OK, redirectToActionResult.StatusCode); 

            #endregion

        }

        private IQueryable<User> GetAllUsers()
        {
            return new List<User>
            {
                new User { Id=1, Login = "Paul", Password = "password", Role="user" },
                new User{Id=2,Login = "Vas", Password="pass", Role="user"},
                new User{Id=3,Login = "Ser", Password="pass", Role="user"},
                new User{Id=4,Login = "Evh", Password="pass", Role="user"}
            }.AsQueryable();
        }

        private IQueryable<Collection> GetAllCollections()
        {
            var users = GetAllUsers().ToList();
            return new List<Collection>
            {
                new Collection { Id=1,Name = "Feed", User=users[0]},
                 new Collection { Id=2,Name = "Col",User=users[0]},
                  new Collection { Id=3,Name = "Pot", User= users[1] }
            }.AsQueryable();
        }

        private IQueryable<Feed> GetAllFeeds()
        {
            return new List<Feed>
            {
                new Feed{ Id = 1, Type = FeedType.RSS, Url="exists"}
            }.AsQueryable();
        }

        private IQueryable<CollectionFeed> GetAllCollectionFeeds()
        {
            var collections = GetAllCollections().ToList();
            var feeds = GetAllFeeds().ToList();

            return new List<CollectionFeed>
            {
                new CollectionFeed{ CollectionId = collections[0].Id, Collection = collections[0], FeedId = feeds[0].Id, Feed = feeds[0]}
            }.AsQueryable();
        }
    }
}
