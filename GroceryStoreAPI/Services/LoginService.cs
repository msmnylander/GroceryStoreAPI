using GroceryStoreAPI.Contracts;
using GroceryStoreAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryStoreAPI.Services
{
    public class LoginService : ILoginService
    {
        /// <summary>
        /// Performs user authentication.
        /// </summary>
        /// <param name="username">The given username</param>
        /// <param name="password">The given password</param>
        /// <returns>An authenticated <see cref="User"/> or null if the user was not authenticated.</returns>
        public async Task<User> Authenticate(string username, string password)
        {
            // Spinning off a task is really unnecessary because this will run faster as a synchronized call but done for demo purposes.
            return await Task.Run(() =>
                _logins
                    .Where(d => d.Username != null && d.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && d.Password != null && d.Password == password)
                    .Select(u =>
                    { // Do not pass back the password
                        u.Password = null;
                        return u;
                    }).FirstOrDefault());
        }

        private static readonly List<User> _logins = new List<User> { new User { Id = 1, Username = "user", Password = "a733tc0d3r" } };
    }
}
