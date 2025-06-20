using System.Threading.Tasks;
using Rest_Api_A3.Models.Request;
using Rest_Api_A3.Models.Response;

namespace Rest_Api_A3.Services.Interfaces
{
    public interface IAuthService
    {
        // Registers a new user and returns a JWT on success
        Task<AuthResponse> SignupAsync(UserSignupRequest request);

        // Validates credentials and returns a JWT on success.
        Task<AuthResponse> LoginAsync(UserLoginRequest request);
    }
}
