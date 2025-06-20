using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Rest_Api_A3.Models.Database;

namespace Rest_Api_A3.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<IdentityResult> CreateUserAsync(User user, string password);
        Task<User> GetByEmailAsync(string email);
    }
}
