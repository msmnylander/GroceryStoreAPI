using GroceryStoreAPI.Contracts;
using GroceryStoreAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GroceryStoreAPI.Controllers
{
    /// <summary>
    /// The Customer API endpoint.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        public CustomerController(ICustomerService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieve a single customer by <see cref="Customer.Id"/>.
        /// </summary>
        /// <param name="token">A <see cref="CancellationToken"/> for terminating the query.</param>
        /// <param name="id">The <see cref="Customer.Id"/>.</param>
        /// <returns>HTTP status 200 if successful, HTTP status 404 if the <see cref="Customer"/> does not exist, HTTP status 400 if failed. The result is in the response body.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Customer))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOne(CancellationToken token, int id)
        {
            var result = await _service?.GetOne(token, id);
            if (result?.Data == null)
            {
                return result.Errors == null 
                    ? Problem(statusCode: StatusCodes.Status404NotFound, detail: "No matching customer found.")
                    : Problem(statusCode: StatusCodes.Status400BadRequest, detail: string.Join(Environment.NewLine, result.Errors));
            }
            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieve a list of <see cref="Customer"/>s. If no query parameters given, will retrieve all customers ordered by <see cref="Customer.Id"/>.
        /// </summary>
        /// <param name="token">A <see cref="CancellationToken"/> for terminating the query</param>
        /// <param name="fromRow">Optional, the first row in sorted order to retrieve.</param>
        /// <param name="pageSize">Optional, the number of rows to retrieve.</param>
        /// <param name="sortBy">Optional, the field by which to sort.</param>
        /// <returns>HTTP status 200 if successful, HTTP status 204 nothing found, HTTP status 400 if failed. The result is in the response body.</returns>
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Customer>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Get(CancellationToken token, [FromQuery] int fromRow = 0, [FromQuery] int pageSize = int.MaxValue, [FromQuery] string sortBy = "id")
        {
            var result = await _service?.Get(token, fromRow, pageSize, sortBy);
            if (result?.Data == null)
            {
                return Problem(statusCode: StatusCodes.Status400BadRequest, detail: string.Join(Environment.NewLine, result.Errors));
            }
            if (!result.Data.Any())
            {
                return Problem(statusCode: StatusCodes.Status204NoContent, detail: "The query returned no data.");
            }
            return Ok(result.Data);
        }

        /// <summary>
        /// Update an existing customer. 
        /// <see cref="Customer.Id"/> must contain the Id of an existing customer.
        /// <see cref="Customer.Name"/> is a mandatory field and cannot be empty.
        /// </summary>
        /// <param name="token">A <see cref="CancellationToken"/> for terminating the query.</param>
        /// <param name="customer">The <see cref="Customer"/> instance to update.</param>
        /// <returns>HTTP status 200 if successful, HTTP status 400 if failed. The result is in the response body.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Customer>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(CancellationToken token, [FromBody] Customer customer)
        {
            var result = await _service?.Update(token, customer);
            if (result?.Data == null)
            {
                return Problem(statusCode: StatusCodes.Status400BadRequest, detail: string.Join(Environment.NewLine, result.Errors));
            }
            return Ok(result.Data);
        }
        /// <summary>
        /// Add a new customer. 
        /// A new customer <see cref="Customer.Id"/> must set to zero.
        /// <see cref="Customer.Name"/> is a mandatory field and cannot be empty.
        /// </summary>
        /// <param name="token">A <see cref="CancellationToken"/> for terminating the query.</param>
        /// <param name="customer">The <see cref="Customer"/> instance to add.</param>
        /// <returns>HTTP status 201 if successful, HTTP status 400 if failed. The result is in the response body.</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(IEnumerable<Customer>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add(CancellationToken token, [FromBody] Customer customer)
        {
            var result = await _service?.Add(token, customer);
            if (result?.Data == null)
            {
                return Problem(statusCode: StatusCodes.Status400BadRequest, detail: string.Join(Environment.NewLine, result.Errors));
            }
            return CreatedAtAction("add", result.Data);
        }

        private readonly ICustomerService _service;
    }
}
