using FeedService.Controllers;
using FeedService.DbModels;
using FeedService.DbModels.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FeedService.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task IndexViewDataMessage()
        {
            var mock = new Mock<IRepository<User>>();
            var response = new Mock<HttpResponse>();
            string respMes = null;
            //response.Setup(r => r.WriteAsync(It.IsAny<string>(),default(CancellationToken))).Callback((string s) => respMes = s);
            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(r => r.Response).Returns(response.Object);
            mock.Setup(r => r.GetAll()).Returns(GetAllUsers());
            // Arrange
            AccountController controller = new AccountController(mock.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext.Object
            };
            //controller.Request = new HttpRequestMessage();
            //controller.Request.SetConfiguration(new HttpConfiguration());
            User user = new User { Login = "Paul", Password = "password" };
            // Act
            await controller.Register(user);

            // Assert
            Assert.Equal("", controller.Response.ToString());
           // Assert.
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
