using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GroceryStoreAPI.Models
{
    /// <summary>
    /// A User model for authentication.
    /// </summary>
    public class User
    {
        [JsonPropertyName("id")]
        public virtual int Id { get; set; }
        [JsonPropertyName("username")]
        public virtual string Username { get; set; }
        [JsonPropertyName("password")]
        public virtual string Password { get; set; }
    }
}
