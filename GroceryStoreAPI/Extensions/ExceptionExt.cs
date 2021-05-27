using GroceryStoreAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryStoreAPI.Extensions
{
    public static class ExceptionExt
    {
        /// <summary>
        /// Get the innermost exception in nested exceptions
        /// </summary>
        /// <param name="exception">The exception</param>
        /// <returns></returns>
        public static Exception GetRootException(this Exception exception)
        {
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
            }
            return exception;
        }

        /// <summary>
        /// Translate exceptions to hopefully something meaningful to the client if possible,
        /// possibly even taking into account i18n translations.
        /// </summary>
        /// <param name="exception">The exception</param>
        /// <returns></returns>
        public static string ToClientMessage(this Exception exception)
        {
            var ex = exception.GetRootException();
            if (ex is OperationCanceledException)
            {
                return "The query was interrupted due to query timeout or server error.";
            }
            if (ex is RepositoryException)
            {
                return ex.GetRootException().Message;
            }
            return "Internal Server Error";
        }
    }
}
