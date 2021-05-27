using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryStoreAPI.Models
{
    /// <summary>
    /// A helper object returning a result and possible messages to the caller
    /// </summary>
    /// <typeparam name="T">The type of data returned</typeparam>
    public class Result<T> where T : class
    {
        /// <summary>
        /// The instance of data
        /// </summary>
        public T Data { get; set; }
        /// <summary>
        /// A collection of diagnostics messages in case of a failure
        /// </summary>
        public IEnumerable<string> Errors { get; set; }
    }
}
