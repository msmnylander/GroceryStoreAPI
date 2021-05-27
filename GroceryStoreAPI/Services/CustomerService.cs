using GroceryStoreAPI.Contracts;
using GroceryStoreAPI.Extensions;
using GroceryStoreAPI.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GroceryStoreAPI.Services
{
    /// <summary>
    /// An <see cref="ICustomerService"/> interface implementation.
    /// This where the <see cref="Customer"/> data is sent to and from the REST protocol handler.
    /// In this trivial example, we just query the repository but in a real app, this could be used for
    /// caching, compositing and passing the data to other backends.
    /// </summary>
    public class CustomerService : ICustomerService
    {
        public CustomerService(ICustomerRepository repository, ILogger<CustomerService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// Add a new customer
        /// </summary>
        /// <param name="token">A <see cref="CancellationToken"/> for terminating the query.</param>
        /// <param name="customer">The <see cref="Customer"/>.</param>
        /// <returns>A <see cref="Result"/> where <see cref="Data"/> contains the <see cref="Customer"/> or null if the operation failed, and <see cref="Result{T}.Errors"/> contain any errors.</returns>
        public async Task<Result<Customer>> Add(CancellationToken token, Customer customer)
        {
            try
            {
                //
                // Validation rules should really go to a business rules class but here for brevity
                //
                if (!customer.Validate(out var whyNot))
                {
                    return new Result<Customer> { Errors = whyNot };
                }
                return new Result<Customer> { Data = await _repository.Add(token, customer) };
            } catch (Exception ex)
            {
                _logger.LogError($"Exception: {ex}");
                return new Result<Customer> { Errors = new [] { ex.ToClientMessage() } };
            }
        }

        /// <summary>
        /// Retrieve a list of <see cref="Customer"/>s. If no query parameters given, will retrieve all customers ordered by <see cref="Customer.Id"/>.
        /// </summary>
        /// <param name="token">A <see cref="CancellationToken"/> for terminating the query</param>
        /// <param name="fromRow">Optional, the first row in sorted order to retrieve.</param>
        /// <param name="count">Optional, the number of rows to retrieve.</param>
        /// <param name="sortBy">Optional, the field by which to sort.</param>
        /// <returns>A collection of matching <see cref="Customer"/>s or null if the operation failed, and <see cref="Result{T}.Errors"/> contain any errors.</returns>
        public async Task<Result<IEnumerable<Customer>>> Get(CancellationToken token, int fromRow = 0, int count = int.MaxValue, string sortBy = "id")
        {
            try
            {
                return new Result<IEnumerable<Customer>> { Data = await _repository.GetCollection(token, fromRow, count, sortBy) };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception: {ex}");
                return new Result<IEnumerable<Customer>> { Errors = new[] { ex.ToClientMessage() } };
            }
        }

        /// <summary>
        /// Retrieve a single customer by <see cref="Customer.Id"/>.
        /// </summary>
        /// <param name="token">A <see cref="CancellationToken"/> for terminating the query.</param>
        /// <param name="id">The <see cref="Customer.Id"/>.</param>
        /// <returns>A matching <see cref="Customer"/> or null if the operation failed, and <see cref="Result{T}.Errors"/> contain any errors.</returns>
        public async Task<Result<Customer>> GetOne(CancellationToken token, int id)
        {
            try
            {
                return new Result<Customer> { Data = await _repository.GetById(token, id) };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception: {ex}");
                return new Result<Customer> { Errors = new[] { ex.ToClientMessage() } };
            }
        }

        /// <summary>
        /// Update an existing customer. 
        /// <see cref="Customer.Id"/> must contain the Id of an existing customer.
        /// <see cref="Customer.Name"/> is a mandatory field and cannot be empty.
        /// </summary>
        /// <param name="customer">The <see cref="Customer"/> instance to update.</param>
        /// <param name="token">A <see cref="CancellationToken"/> for terminating the query.</param>
        /// <returns>The updated <see cref="Customer"/> or null if the operation failed, and <see cref="Result{T}.Errors"/> contain any errors.</returns>
        public async Task<Result<Customer>> Update(CancellationToken token, Customer customer)
        {
            try
            {
                //
                // Validation rules should really go to a business rules class but here for brevity
                //
                if (!customer.Validate(out var whyNot))
                {
                    return new Result<Customer> { Errors = whyNot };
                }
                return new Result<Customer> { Data = await _repository.Update(token, customer) };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception: {ex}");
                return new Result<Customer> { Errors = new[] { ex.ToClientMessage() } };
            }
        }

        private readonly ICustomerRepository _repository;
        private readonly ILogger<CustomerService> _logger;
    }
}
