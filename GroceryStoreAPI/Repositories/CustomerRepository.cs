using GroceryStoreAPI.Contracts;
using GroceryStoreAPI.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace GroceryStoreAPI.Repositories
{
    /// <summary>
    /// A trivial in memory <see cref="Customer"/> repository implementation.
    /// Access method support async operation but in this implementation
    /// most run synchronously
    /// </summary>
    public partial class CustomerRepository : ICustomerRepository
    {
        public CustomerRepository() : this(null) { }

        public CustomerRepository(Func<IEnumerable<Customer>> loader)
        {
            if (loader == null)
            {
                loader = Load;
            }
            foreach (var item in loader())
            {
                _customers.TryAdd(item.Id, item);
            }
            _currentId = _customers.Keys.Max();
        }

        public int CurrentId => _currentId;

        public async Task<Customer> Add(CancellationToken token, Customer customer)
        {
            if (customer.Id != 0)
            {
                throw new RepositoryException("Attempt to add a customer with non zero Id.");
            }
            if (token.IsCancellationRequested)
            {
                return null;
            }
            try
            {
                //
                // Synchronize customer ID access
                //
                await _lock.WaitAsync();
                _currentId++;
                customer.Id = _currentId;
            } 
            finally
            {
                _lock.Release();
            }
            _customers.TryAdd(_currentId, customer);
            return customer;
        }

        public Task<Customer> GetById(CancellationToken token, int id)
        {
            if (token.IsCancellationRequested)
            {
                return null;
            }
            if (!_customers.TryGetValue(id, out Customer customer))
            {
                throw new RepositoryException("No matching customer found.");
            }
            return Task.FromResult(customer);
        }

        public Task<IEnumerable<Customer>> GetCollection(CancellationToken token, int fromRow = 0, int count = int.MaxValue, string sortBy = "id")
        {
            if (token.IsCancellationRequested)
            {
                return null;
            }
            var numKeys = _customers.Keys.Count;
            if (count + fromRow > numKeys)
            {
                count = numKeys - fromRow;
            }
            if (fromRow >= numKeys || fromRow > count) 
            {
                throw new RepositoryException($"Invalid range {fromRow} .. {count}.");
            }
            if (!string.IsNullOrEmpty(sortBy))
            {
                if (sortBy.Equals("id", StringComparison.OrdinalIgnoreCase))
                {
                    return Task.FromResult(_customers.Keys.OrderBy(d => d).Skip(fromRow).Take(count).Select(e => _customers[e]));                }
                if (sortBy != null && sortBy.Equals("name", StringComparison.OrdinalIgnoreCase))
                {
                    return Task.FromResult(_customers.Values.OrderBy(d => d.Name).Skip(fromRow).Take(count));
                } else
                {
                    throw new RepositoryException($"Invalid sortBy parameter '{sortBy}");
                }
            }
            return Task.FromResult(_customers.Values.Select(d => d));
        }

        public Task<Customer> Update(CancellationToken token, Customer customer)
        {
            if (token.IsCancellationRequested)
            {
                return null;
            }
            if (!_customers.ContainsKey(customer.Id))
            {
                throw new RepositoryException("No matching customer to update found.");
            }
            _customers[customer.Id] = customer;
            return Task.FromResult(customer);
        }

        private static IEnumerable<Customer> Load()
        {
            var fileName = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("InMemoryRepository")["FileName"];
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), fileName);
            using (FileStream openStream = File.OpenRead(path))
            {
                var list = JsonSerializer.DeserializeAsync<CustomerList>(openStream).Result;
                return list.Customers;
            }
        }

        /// <summary>
        /// A helper class to get us a list of customers from the sample JSON file
        /// </summary>
        private class CustomerList
        {
            [JsonPropertyName("customers")]
            public List<Customer> Customers { get; set; }
        }

        //
        // The current "autoincrement" customer Id
        //
        private int _currentId;
        //
        // In an async method, use SemaphoreSlim for thread synchronization. This allows only one
        // thread at a time, analogous to a plain lock() block
        //
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        //
        // The "in memory database". ConcurrentDictionary is not the optimal solution for large amount of data but
        // should suffice here.
        //
        private readonly ConcurrentDictionary<int, Customer> _customers = new ConcurrentDictionary<int, Customer>();
    }
}
