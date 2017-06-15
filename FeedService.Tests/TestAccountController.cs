using FeedService.Controllers;
using FeedService.DbModels;
using FeedService.DbModels.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using Newtonsoft.Json;
using System;
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
            var mock = new Mock<IRepository<User>>();
            mock.Setup(ur => ur.AddAsync(It.IsAny<User>()));
            mock.Setup(ur => ur.SaveAsync());
            var response = new Mock<HttpResponse>();
            
            
            mock.Setup(r => r.GetAll()).Returns(GetAllUsers());
            // Arrange
            AccountController controller = new AccountController(mock.Object);
            User user = new User { Login = "Paul", Password = "password" };
            // Act
            var actionRes = controller.Register(user).GetAwaiter().GetResult();
            var redirectToActionResult = Assert.IsType<BadRequestObjectResult>(actionRes);

            // Assert
            Assert.Equal(StatusCodes.Status400BadRequest, redirectToActionResult.StatusCode);
            Assert.Equal("User with such login already exists", redirectToActionResult.Value.ToString());
           // Assert.
        }

        [Fact]
        public async Task RegisterNewUser_Success()
        {
            var userRepository = new Mock<IRepository<User>>();
            var response = new Mock<HttpResponse>();
            userRepository.Setup(r => r.GetAll()).Returns(GetAllUsers());
            // Arrange
            AccountController controller = new AccountController(userRepository.Object);
            User user = new User { Login = "Petya", Password = "password" };
            // Act
            var actionRes = controller.Register(user).GetAwaiter().GetResult();
            var redirectToActionResult = Assert.IsType<OkObjectResult>(actionRes);

            // Assert
            Assert.Equal(StatusCodes.Status200OK, redirectToActionResult.StatusCode);
            Assert.Equal($"User {user.Login} registered successfully", redirectToActionResult.Value.ToString());
        }

        [Fact]
        public async Task Login_Success()
        {
            User user = new User { Login = "Paul", Password = "password" };

            var httpContext = new Mock<HttpContext>();
            //var formCollectionMock = new Mock<IFormCollection>();
            var formCollection = new Dictionary<string, StringValues>();

            formCollection.Add("username", $"{user.Login}");
            formCollection.Add("password", $"{user.Password}");
            FormCollection col = new FormCollection(formCollection);
            httpContext.SetupGet(hc => hc.Request.Form).Returns(col);

            var userRepository = new Mock<IRepository<User>>();
            var response = new Mock<HttpResponse>();
            userRepository.Setup(r => r.GetAll()).Returns(GetAllUsers());
            // Arrange
            AccountController controller = new AccountController(userRepository.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext.Object
            };
           
            // Act
            var actionRes = controller.Token().GetAwaiter().GetResult();
            var redirectToActionResult = Assert.IsType<OkObjectResult>(actionRes);

            // Assert
            Assert.Equal(StatusCodes.Status200OK, redirectToActionResult.StatusCode);
            Assert.Equal($"User {user.Login} registered successfully", redirectToActionResult.Value.ToString());
        }

        private IQueryable<User> GetAllUsers()
        {
            return new List<User>
            {
                new User { Login = "Paul", Password = "password" },
                new User{Login = "Vas", Password="pass"},
                new User{Login = "Ser", Password="pass"},
                new User{Login = "Evh", Password="pass"}
            }.AsQueryable();
        }
    }
}