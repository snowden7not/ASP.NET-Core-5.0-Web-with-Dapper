using System;
using Microsoft.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Rest_Api_A3.Models.Database;

namespace Rest_Api_A3.Repositories
{
    public class DapperUserStore :
        IUserStore<User>,
        IUserPasswordStore<User>,
        IUserEmailStore<User>
    {
        private readonly string _conn;
        public DapperUserStore(IOptions<ConnectionString> options)
            => _conn = options.Value.IMDBDB;

        // === IUserStore ===

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken ct)
        {
            const string sql = @"
INSERT INTO Foundation.AspNetUsers
    (Id, UserName, NormalizedUserName, Email, NormalizedEmail,
     EmailConfirmed, PasswordHash, SecurityStamp)
VALUES
    (@Id, @UserName, @NormalizedUserName, @Email, @NormalizedEmail,
     @EmailConfirmed, @PasswordHash, @SecurityStamp)";
            user.Id = Guid.NewGuid().ToString();
            using var db = new SqlConnection(_conn);
            var affected = await db.ExecuteAsync(sql, new
            {
                user.Id,
                user.UserName,
                user.NormalizedUserName,
                user.Email,
                user.NormalizedEmail,
                user.EmailConfirmed,
                user.PasswordHash,
                user.SecurityStamp
            });
            return affected == 1
                ? IdentityResult.Success
                : IdentityResult.Failed(new IdentityError { Description = "Could not insert user" });
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken ct)
        {
            using var db = new SqlConnection(_conn);
            var affected = await db.ExecuteAsync(
                "DELETE FROM Foundation.AspNetUsers WHERE Id = @Id",
                new { user.Id });
            return affected == 1
                ? IdentityResult.Success
                : IdentityResult.Failed(new IdentityError { Description = "Could not delete user" });
        }

        public void Dispose() { /* no-op */ }

        public async Task<User> FindByIdAsync(string userId, CancellationToken ct)
        {
            using var db = new SqlConnection(_conn);
            return await db.QuerySingleOrDefaultAsync<User>(
                "SELECT * FROM Foundation.AspNetUsers WHERE Id = @Id",
                new { Id = userId });
        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken ct)
        {
            using var db = new SqlConnection(_conn);
            return await db.QuerySingleOrDefaultAsync<User>(
                "SELECT * FROM Foundation.AspNetUsers WHERE NormalizedUserName = @n",
                new { n = normalizedUserName });
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken ct)
            => Task.FromResult(user.NormalizedUserName);

        public Task<string> GetUserIdAsync(User user, CancellationToken ct)
            => Task.FromResult(user.Id);

        public Task<string> GetUserNameAsync(User user, CancellationToken ct)
            => Task.FromResult(user.UserName);

        public Task SetNormalizedUserNameAsync(User user, string name, CancellationToken ct)
        {
            user.NormalizedUserName = name;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(User user, string name, CancellationToken ct)
        {
            user.UserName = name;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken ct)
        {
            const string sql = @"
UPDATE Foundation.AspNetUsers
SET UserName           = @UserName,
    NormalizedUserName = @NormalizedUserName,
    Email              = @Email,
    NormalizedEmail    = @NormalizedEmail,
    EmailConfirmed     = @EmailConfirmed,
    PasswordHash       = @PasswordHash,
    SecurityStamp      = @SecurityStamp
WHERE Id = @Id";
            using var db = new SqlConnection(_conn);
            var affected = await db.ExecuteAsync(sql, new
            {
                user.UserName,
                user.NormalizedUserName,
                user.Email,
                user.NormalizedEmail,
                user.EmailConfirmed,
                user.PasswordHash,
                user.SecurityStamp,
                user.Id
            });
            return affected == 1
                ? IdentityResult.Success
                : IdentityResult.Failed(new IdentityError { Description = "Could not update user" });
        }

        // === IUserPasswordStore ===

        public Task SetPasswordHashAsync(User user, string hash, CancellationToken ct)
        {
            user.PasswordHash = hash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken ct)
            => Task.FromResult(user.PasswordHash);

        public Task<bool> HasPasswordAsync(User user, CancellationToken ct)
            => Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));

        // === IUserEmailStore ===

        public Task SetEmailAsync(User user, string email, CancellationToken ct)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(User user, CancellationToken ct)
            => Task.FromResult(user.Email);

        public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken ct)
            => Task.FromResult(user.EmailConfirmed);

        public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken ct)
        {
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedEmailAsync(User user, CancellationToken ct)
            => Task.FromResult(user.NormalizedEmail);

        public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken ct)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }

        public async Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken ct)
        {
            using var db = new SqlConnection(_conn);
            return await db.QuerySingleOrDefaultAsync<User>(
                "SELECT * FROM Foundation.AspNetUsers WHERE NormalizedEmail = @e",
                new { e = normalizedEmail });
        }
    }
}


