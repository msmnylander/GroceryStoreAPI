using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;

namespace GroceryStoreAPI.Models
{
    /// <summary>
    /// The Customer data transfer object. 
    /// In a real app, there would probably be a separate internal (data storage) and external (API result) presentation of the object
    /// </summary>
    public class Customer
    {
        [JsonPropertyName("id")]
        public virtual int Id { get; set; }
        [JsonPropertyName("name")]
        [Required]
        public virtual string Name { get; set; }
    }

    /// <summary>
    /// Validation. In a real app would probably use FluentValidation or somesuch
    /// </summary>
    public static class CustomerExt
    {
        public static bool Validate(this Customer customer, out IEnumerable<string> whyNot)
        {
            whyNot = Enumerable.Empty<string>();
            if (string.IsNullOrWhiteSpace(customer.Name))
            {
                whyNot = new[] { "Name is required" };
                return false;
            }
            // An arbitrary length just for demo purposes
            if (customer.Name.Length > 100)
            {
                whyNot = new[] { "Name too long" };
                return false;
            }
            return true;
        }
    }
}
