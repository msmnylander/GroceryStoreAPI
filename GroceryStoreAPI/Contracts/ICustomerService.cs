using GroceryStoreAPI.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GroceryStoreAPI.Contracts
{
    /// <summary>
    /// The definition of the <see cref="Customer"/> service.
    /// It is assumed that this service will be configured as a singleton.
    /// </summary>
    public interface ICustomerService
    {
        /// <summary>
        /// Contains the process(es) for adding a new customer.
        /// </summary>
        /// <param name="token">A <see cref="CancellationToken"/> for terminating the query</param>
        /// <param name="customer">The <see cref="Customer"/> instance to add</param>
        /// <returns>A <see cref="Result{T}"/> instance where <see cref="Result{T}.Data"/> is null if the query failed and <see cref="Result{T}.Errors"/> describe the errors that have occurred.</returns>
        Task<Result<Customer>> Add(CancellationToken token, Customer customer);
        /// <summary>
        /// Contains the process(es) fdr updating an existing customer.
        /// </summary>
        /// <param name="token">A <see cref="CancellationToken"/> for terminating the query</param>
        /// <param name="customer">The <see cref="Customer"/> instance to update</param>
        /// <returns>A <see cref="Result{T}"/> instance where <see cref="Result{T}.Data"/> is null if the query failed and <see cref="Result{T}.Errors"/> describe the errors that have occurred.</returns>
        Task<Result<Customer>> Update(CancellationToken token, Customer customer);
        /// <summary>
        /// Contains the logic to retrieve a single customer.
        /// </summary>
        /// <param name="token">A <see cref="CancellationToken"/> for terminating the query</param>
        /// <param name="id">The <see cref="Customer.Id"/></param>
        /// <returns>A <see cref="Result{T}"/> instance where <see cref="Result{T}.Data"/> is null if the query failed and <see cref="Result{T}.Errors"/> describe the errors that have occurred.</returns>
        Task<Result<Customer>> GetOne(CancellationToken token, int id);
        /// <summary>
        /// Contains the logic for retrieving a list of customers.
        /// </summary>
        /// <param name="token">A <see cref="CancellationToken"/> for terminating the query</param>
        /// <param name="fromRow">The first row in sorted order to retrieve.</param>
        /// <param name="pageSize">The number of rows to retrieve.</param>
        /// <param name="sortBy">The field by which to sort.</param>
        /// <returns>A <see cref="Result{T}"/> instance where <see cref="Result{T}.Data"/> is null if the query failed and <see cref="Result{T}.Errors"/> describe the errors that have occurred.</returns>
        Task<Result<IEnumerable<Customer>>> Get(CancellationToken token, int fromRow = 0, int pageSize = int.MaxValue, string sortBy = "Id");

    }
}
