using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Rest_Api_A3.Models.Database;
using Rest_Api_A3.Models.Request;
using Rest_Api_A3.Models.Response;
using Rest_Api_A3.Repositories.Interfaces;
using Rest_Api_A3.Services.Interfaces;

namespace Rest_Api_A3.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;

        public AuthService(
            IUserRepository userRepo,
            UserManager<User> userManager,
            IConfiguration config)
        {
            _userRepo = userRepo;
            _userManager = userManager;
            _config = config;
        }

        public async Task<AuthResponse> SignupAsync(UserSignupRequest request)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(request.Email) ||
                !request.Email.Contains("@"))
                throw new ArgumentException("A valid Email is required", nameof(request.Email));
            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
                throw new ArgumentException("Password must be at least 6 characters", nameof(request.Password));

            // Normalize & check existing
            var normEmail = _userManager.NormalizeEmail(request.Email);
            var exists = await _userManager.FindByEmailAsync(normEmail);
            if (exists != null)
                throw new InvalidOperationException("Email already registered");

            // Create user
            var user = new User
            {
                UserName = request.Email,
                Email = request.Email,
                NormalizedEmail = normEmail,
                NormalizedUserName = _userManager.NormalizeName(request.Email)
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join("; ", result.Errors));

            // Return token
            return new AuthResponse
            {
                Token = GenerateJwtToken(user)
            };
        }

        public async Task<AuthResponse> LoginAsync(UserLoginRequest request)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Email and Password are required");

            // Lookup and check password
            var normEmail = _userManager.NormalizeEmail(request.Email);
            var user = await _userManager.FindByEmailAsync(normEmail)
                            ?? throw new UnauthorizedAccessException("Invalid credentials");

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
                throw new UnauthorizedAccessException("Invalid credentials");

            // Return token
            return new AuthResponse
            {
                Token = GenerateJwtToken(user)
            };
        }

        private string GenerateJwtToken(User user)
        {
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var creds = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256
            );

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var jwt = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}


