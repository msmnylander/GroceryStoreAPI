using GroceryStoreAPI.Contracts;
using GroceryStoreAPI.Controllers;
using GroceryStoreAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace GroceryStoreAPI.Tests
{
    public class CustomerControllerUnitTests
    {
        private static IEnumerable<Customer> customers = new List<Customer>
        {
            new Customer { Id = 1, Name = "Bob" },
            new Customer { Id = 2, Name = "Alice" },
            new Customer { Id = 3, Name = "Mary" },
            new Customer { Id = 4, Name = "George" },
            new Customer { Id = 5, Name = "Matthew" },
            new Customer { Id = 6, Name = "Emma" },
        };

        [Fact]
        public async Task WillGetAll()
        {
            var service = new Mock<ICustomerService>();
            var token = new CancellationTokenSource().Token;
            service.Setup(p => p.Get(token, 0, int.MaxValue, string.Empty)).Returns(Task.FromResult(new Result<IEnumerable<Customer>> { Data = customers }));
            var controller = new CustomerController(service.Object);
            var result = await controller.Get(token, 0, int.MaxValue, string.Empty) as ObjectResult;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async Task WillHandleGetAllFailure()
        {
            var service = new Mock<ICustomerService>();
            var token = new CancellationTokenSource().Token;
            service.Setup(p => p.Get(token, 0, int.MaxValue, string.Empty)).Returns(Task.FromResult(new Result<IEnumerable<Customer>> { Errors = new[] { "Error" } }));
            var controller = new CustomerController(service.Object);
            var result = await controller.Get(token, 0, int.MaxValue, string.Empty) as ObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task WillHandleGetAllEmptyResult()
        {
            var service = new Mock<ICustomerService>();
            var token = new CancellationTokenSource().Token;
            service.Setup(p => p.Get(token, 0, int.MaxValue, string.Empty)).Returns(Task.FromResult(new Result<IEnumerable<Customer>> { Data = Enumerable.Empty<Customer>() }));
            var controller = new CustomerController(service.Object);
            var result = await controller.Get(token, 0, int.MaxValue, string.Empty) as ObjectResult;
            Assert.Equal(StatusCodes.Status204NoContent, result.StatusCode);
        }

        [Fact]
        public async Task WillGetOne()
        {
            var service = new Mock<ICustomerService>();
            var token = new CancellationTokenSource().Token;
            service.Setup(p => p.GetOne(token, 1)).Returns(Task.FromResult(new Result<Customer> { Data = customers.First() }));
            var controller = new CustomerController(service.Object);
            var result = await controller.GetOne(token, 1) as ObjectResult;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async Task WillHandleGetOneFailure()
        {
            var service = new Mock<ICustomerService>();
            var token = new CancellationTokenSource().Token;
            service.Setup(p => p.GetOne(token, 1)).Returns(Task.FromResult(new Result<Customer> { }));
            var controller = new CustomerController(service.Object);
            var result = await controller.GetOne(token, 1) as ObjectResult;
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }

        [Fact]
        public async Task WillAdd()
        {
            var service = new Mock<ICustomerService>();
            var token = new CancellationTokenSource().Token;
            var customer = new Customer();
            service.Setup(p => p.Add(token, customer)).Returns(Task.FromResult(new Result<Customer> { Data = customer }));
            var controller = new CustomerController(service.Object);
            var result = await controller.Add(token, customer) as ObjectResult;
            Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        }

        [Fact]
        public async Task WillHandleAddFailure()
        {
            var service = new Mock<ICustomerService>();
            var token = new CancellationTokenSource().Token;
            var customer = new Customer();
            service.Setup(p => p.Add(token, customer)).Returns(Task.FromResult(new Result<Customer> { Errors = new[] { "Error" } }));
            var controller = new CustomerController(service.Object);
            var result = await controller.Add(token, customer) as ObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task WillUpdate()
        {
            var service = new Mock<ICustomerService>();
            var token = new CancellationTokenSource().Token;
            var customer = new Customer();
            service.Setup(p => p.Update(token, customer)).Returns(Task.FromResult(new Result<Customer> { Data = customer }));
            var controller = new CustomerController(service.Object);
            var result = await controller.Update(token, customer) as ObjectResult;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async Task WillHandleUpdateFailure()
        {
            var service = new Mock<ICustomerService>();
            var token = new CancellationTokenSource().Token;
            var customer = new Customer();
            service.Setup(p => p.Update(token, customer)).Returns(Task.FromResult(new Result<Customer> { Errors = new[] { "Error" } }));
            var controller = new CustomerController(service.Object);
            var result = await controller.Update(token, customer) as ObjectResult;
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }



    }
}
