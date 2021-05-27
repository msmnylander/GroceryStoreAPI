using GroceryStoreAPI.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GroceryStoreAPI.Contracts
{
    /// <summary>
    /// The definition of a repository for <see cref="Customer"/> data.
    /// It is assumed that the repository is fully re-entrant and will handle multiple concurrent requests.
    /// </summary>
    public interface ICustomerRepository
    {
        /// <summary>
        /// Retrieve a single customer by <see cref="Customer.Id"/>.
        /// The repository should throw a descriptive exception in case of an error.
        /// </summary>
        /// <param name="token">A <see cref="CancellationToken"/> for terminating the query</param>
        /// <param name="id">The desired <see cref="Customer.Id"/></param>
        /// <returns>A <see cref="Customer"/> instance or null if not found.</returns>
        Task<Customer> GetById(CancellationToken token, int id);
        /// <summary>
        /// Retrieve a collection of customers.
        /// </summary>
        /// <param name="token">A <see cref="CancellationToken"/> for terminating the query</param>
        /// <param name="fromRow">The first row in sorted order to retrieve.</param>
        /// <param name="count">The number of rows to retrieve.</param>
        /// <param name="sortBy">The field by which to sort.</param>
        /// <returns>A collection of <see cref="Customer"/> instances</returns>
        Task<IEnumerable<Customer>> GetCollection(CancellationToken token, int fromRow = 0, int count = int.MaxValue, string sortBy = "id");
        /// <summary>
        /// Add a new <see cref="Customer"/>.
        /// </summary>
        /// <param name="token">A <see cref="CancellationToken"/> for terminating the query.</param>
        /// <param name="customer">The <see cref="Customer"/> instance to add</param>
        /// <returns>The newly added <see cref="Customer"/></returns>
        Task<Customer> Add(CancellationToken token, Customer customer);
        /// <summary>
        /// Update an existing <see cref="Customer"/>
        /// </summary>
        /// <param name="token">A <see cref="CancellationToken"/> for terminating the query.</param>
        /// <param name="customer">The <see cref="Customer"/> instance to update</param>
        /// <returns></returns>
        Task<Customer> Update(CancellationToken token, Customer customer);
    }
}
