using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Options;
using Rest_Api_A3.Models.Database;
using Rest_Api_A3.Repositories.Interfaces;

namespace Rest_Api_A3.Repositories
{
    public class ReviewRepository
        : BaseRepository<Review>, IReviewRepository
    {
        public ReviewRepository(IOptions<ConnectionString> opts)
            : base(opts.Value.IMDBDB)
        { }

        public async Task<IEnumerable<Review>> GetAllAsync()
        {
            const string sql = @"
SELECT Id, Message, MovieId
FROM Foundation.Reviews WITH (NOLOCK);";

            return await GetAsync(sql);
        }

        public async Task<Review> GetByIdAsync(int id)
        {
            const string sql = @"
SELECT Id, Message, MovieId
FROM Foundation.Reviews WITH (NOLOCK)
WHERE Id = @Id;";

            return await GetAsync(sql, new { Id = id });
        }

        public async Task CreateAsync(Review entity)
        {
            const string sql = @"
INSERT INTO Foundation.Reviews (Message, MovieId)
OUTPUT INSERTED.Id
VALUES (@Message, @MovieId);";

            var newId = await CreateAsync(sql, new
            {
                entity.Message,
                entity.MovieId
            });
            entity.Id = newId;
        }

        public Task UpdateAsync(Review entity)
        {
            const string sql = @"
UPDATE Foundation.Reviews
SET Message = @Message,
    MovieId = @MovieId
WHERE Id = @Id;";

            return ExecuteAsync(sql, new
            {
                entity.Message,
                entity.MovieId,
                entity.Id
            });
        }

        public Task DeleteAsync(int id)
        {
            const string sql = @"
DELETE FROM Foundation.Reviews
WHERE Id = @Id;";

            return ExecuteAsync(sql, new { Id = id });
        }

        public async Task<IEnumerable<Review>> GetReviewsByMovieIdAsync(int movieId)
        {
            const string sql = @"
SELECT Id, Message, MovieId
FROM Foundation.Reviews WITH (NOLOCK)
WHERE MovieId = @MovieId;";

            return await QueryAsync<Review>(sql, new { MovieId = movieId });
        }
    }
}


