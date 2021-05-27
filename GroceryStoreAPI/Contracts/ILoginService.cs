using GroceryStoreAPI.Models;
using System.Threading.Tasks;

namespace GroceryStoreAPI.Contracts
{
    public interface ILoginService
    {
        Task<User> Authenticate(string username, string password);
    }
}
