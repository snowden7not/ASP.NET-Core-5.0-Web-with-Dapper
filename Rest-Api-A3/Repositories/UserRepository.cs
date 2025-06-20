using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Rest_Api_A3.Models.Database;
using Rest_Api_A3.Repositories.Interfaces;

namespace Rest_Api_A3.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;

        public UserRepository(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> CreateUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
    }
}
