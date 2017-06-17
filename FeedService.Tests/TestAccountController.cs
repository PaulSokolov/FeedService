using FeedService.Controllers;
using FeedService.DbModels;
using FeedService.DbModels.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FeedService.Tests
{
    public class TestAccountController
    {
        [Fact]
        public async Task RegisterExistingUser_BadRequest()
        {
            var userRepository = new Mock<IRepository<User>>();
            userRepository.Setup(ur => ur.AddAsync(It.IsAny<User>()));
            userRepository.Setup(ur => ur.SaveAsync());
            userRepository.Setup(r => r.GetAll()).Returns(GetAllUsers());
            var feedServiceUnit = new Mock<IFeedServiceUoW>();
            feedServiceUnit.SetupGet(fsu => fsu.Users).Returns(userRepository.Object);
            var logger = new Mock<ILogger<AccountController>>();
            
            // Arrange
            AccountController controller = new AccountController(feedServiceUnit.Object, logger.Object);
            User user = new User { Login = "Paul", Password = "password" };
            // Act
            var actionRes = controller.Register(user).GetAwaiter().GetResult();
            var redirectToActionResult = Assert.IsType<BadRequestObjectResult>(actionRes);

            // Assert
            Assert.Equal(StatusCodes.Status400BadRequest, redirectToActionResult.StatusCode);
            Assert.Equal("User with such login already exists", JObject.FromObject(redirectToActionResult.Value)["Error"]);
           // Assert.
        }

        [Fact]
        public async Task RegisterNewUser_Success()
        {
            var userRepository = new Mock<IRepository<User>>();
            var response = new Mock<HttpResponse>();
            userRepository.Setup(r => r.GetAll()).Returns(GetAllUsers());

            var feedServiceUnit = new Mock<IFeedServiceUoW>();
            feedServiceUnit.SetupGet(fsu => fsu.Users).Returns(userRepository.Object);

            var logger = new Mock<ILogger<AccountController>>();
            // Arrange
            AccountController controller = new AccountController(feedServiceUnit.Object, logger.Object);
            User user = new User { Login = "Petya", Password = "password" };
            // Act
            var actionRes = controller.Register(user).GetAwaiter().GetResult();
            var redirectToActionResult = Assert.IsType<OkObjectResult>(actionRes);

            // Assert
            Assert.Equal(StatusCodes.Status200OK, redirectToActionResult.StatusCode);
            Assert.Equal($"User {user.Login} registered successfully", JObject.FromObject(redirectToActionResult.Value)["Success"]);
        }

        [Fact]
        public async Task Login_Success()
        {
            User user = new User { Login = "Paul", Password = "password" };

            var httpContext = new Mock<HttpContext>();
            var formCollection = new Dictionary<string, StringValues>();
            formCollection.Add("username", $"{user.Login}");
            formCollection.Add("password", $"{user.Password}");
            FormCollection col = new FormCollection(formCollection);

            httpContext.SetupGet(hc => hc.Request.Form).Returns(col);

            var userRepository = new Mock<IRepository<User>>();
            userRepository.Setup(r => r.GetAll()).Returns(GetAllUsers());

            var feedServiceUnit = new Mock<IFeedServiceUoW>();
            feedServiceUnit.SetupGet(fsu => fsu.Users).Returns(userRepository.Object);

            var logger = new Mock<ILogger<AccountController>>();
            // Arrange
            AccountController controller = new AccountController(feedServiceUnit.Object, logger.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext.Object
            };
           
            // Act
            var actionRes = controller.Token().GetAwaiter().GetResult();
            var redirectToActionResult = Assert.IsType<OkObjectResult>(actionRes);

            // Assert
            Assert.Equal(StatusCodes.Status200OK, redirectToActionResult.StatusCode);
            Assert.True(JObject.FromObject(redirectToActionResult.Value).TryGetValue("access_token", out JToken i));
        }

        private IQueryable<User> GetAllUsers()
        {
            return new List<User>
            {
                new User { Login = "Paul", Password = "password", Role="user" },
                new User{Login = "Vas", Password="pass", Role="user"},
                new User{Login = "Ser", Password="pass", Role="user"},
                new User{Login = "Evh", Password="pass", Role="user"}
            }.AsQueryable();
        }
    }
}
