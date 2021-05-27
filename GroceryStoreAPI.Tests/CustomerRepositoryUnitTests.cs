using GroceryStoreAPI.Models;
using GroceryStoreAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace GroceryServiceAPI.Tests
{
    public class CustomerRepositoryUnitTests
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
        public void WillLoadExistingCustomers()
        {
            var repo = new CustomerRepository(() => customers);
            Assert.Equal(6, repo.CurrentId);
        }

        [Fact]
        public async Task WillFindCustomer()
        {
            var repo = new CustomerRepository(() => customers);
            var token = new CancellationTokenSource().Token;
            var customer = await repo.GetById(token, 1);
            Assert.NotNull(customer);
            Assert.Equal(1, customer.Id);
        }

        [Fact]
        public async Task WillNotFindUnknownCustomer()
        {
            var repo = new CustomerRepository(() => customers);
            var token = new CancellationTokenSource().Token;
            Func<Task> act = () => repo.GetById(token, 9);
            var exception = await Assert.ThrowsAsync<RepositoryException>(act);
            Assert.NotNull(exception);
        }

        [Fact]
        public async Task WillAddValidCustomer()
        {
            var repo = new CustomerRepository(() => customers);
            var token = new CancellationTokenSource().Token;
            var customer = await repo.Add(token, new Customer { Name = "Leo" });
            Assert.NotNull(customer);
            Assert.NotEqual(0, customer.Id);
            customer = await repo.GetById(token, customer.Id);
            Assert.NotNull(customer);
        }

        [Fact]
        public async Task WillNotAddInvalidCustomer()
        {
            var repo = new CustomerRepository(() => customers);
            var token = new CancellationTokenSource().Token;
            Func<Task> act = () => repo.Add(token, new Customer { Id = 3, Name = "Leo" });
            var exception = await Assert.ThrowsAsync<RepositoryException>(act);
            Assert.NotNull(exception);
        }

        [Fact]
        public async Task WillUpdateValidCustomer()
        {
            var repo = new CustomerRepository(() => customers);
            var token = new CancellationTokenSource().Token;
            var customer = new Customer { Id = 2, Name = "Barbara" };
            customer = await repo.Update(token, customer);
            Assert.NotNull(customer);
            Assert.Equal(2, customer.Id);
            Assert.Equal("Barbara", customer.Name);
        }

        [Fact]
        public async Task WillNotUpdateUnknownCustomer()
        {
            var repo = new CustomerRepository(() => customers);
            var token = new CancellationTokenSource().Token;
            Func<Task> act = () => repo.Update(token, new Customer { Id = 99, Name = "Leo" });
            var exception = await Assert.ThrowsAsync<RepositoryException>(act);
            Assert.NotNull(exception);
        }
        
        [Fact]
        public async Task WillFindValidRange()
        {
            var repo = new CustomerRepository(() => customers);
            var token = new CancellationTokenSource().Token;
            var results = (await repo.GetCollection(token, 0, 3))?.ToList();
            Assert.NotNull(results);
            Assert.Equal(3, results.Count);
        }

        [Fact]
        public async Task WillAdjustEndRange()
        {
            var repo = new CustomerRepository(() => customers);
            var token = new CancellationTokenSource().Token;
            var results = (await repo.GetCollection(token, 3, 6))?.ToList();
            Assert.NotNull(results);
            Assert.Equal(3, results.Count);
        }

        [Fact]
        public async Task WillSortByName()
        {
            var repo = new CustomerRepository(() => customers);
            var token = new CancellationTokenSource().Token;
            var results = (await repo.GetCollection(token, 3, 3, "name"))?.ToList();
            Assert.NotNull(results);
            Assert.Equal("George", results[0].Name);
            Assert.Equal(3, results.Count);
        }

        [Fact]
        public async Task WillNotAllowInvalidSortBy()
        {
            var repo = new CustomerRepository(() => customers);
            var token = new CancellationTokenSource().Token;
            Func<Task> act = () => repo.GetCollection(token, 0, 3, "noSuchField");
            var exception = await Assert.ThrowsAsync<RepositoryException>(act);
            Assert.NotNull(exception);
        }

        [Fact]
        public async Task WillGetAll()
        {
            var repo = new CustomerRepository(() => customers);
            var token = new CancellationTokenSource().Token;
            var results = (await repo.GetCollection(token))?.ToList();
            Assert.NotNull(results);
            Assert.Equal(6, results.Count);
        }

    }
}
