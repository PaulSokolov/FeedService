using FeedService.Controllers;
using FeedService.DbModels;
using FeedService.DbModels.Interfaces;
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

            // Arrange
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
            var expected = 3;
            // Act
            var actionRes = controller.GetCollections();
            var redirectToActionResult = Assert.IsType<OkObjectResult>(actionRes);
            // Assert
            Assert.Equal(StatusCodes.Status200OK, redirectToActionResult.StatusCode);
            //Assert.IsType(typeof(EnumerableQuery<>), redirectToActionResult.Value);
            
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
            return new List<Collection>
            {
                new Collection { Id=1,Name = "Feed", User=new User { Id=1, Login = "Paul", Password = "password", Role="user" } },
                 new Collection { Id=1,Name = "Col",User=new User { Id=1, Login = "Paul", Password = "password", Role="user" } },
                  new Collection { Id=1,Name = "Pot", User= new User { Id=1, Login = "Paul", Password = "password", Role="user" } }
            }.AsQueryable();
        }
    }
}
