using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Rest_Api_A3.Models.Database;
using Rest_Api_A3.Repositories.Interfaces;

namespace Rest_Api_A3.Repositories
{
    public class GenreRepository
        : BaseRepository<Genre>, IGenreRepository
    {
        public GenreRepository(IOptions<ConnectionString> opts)
            : base(opts.Value.IMDBDB)
        { }

        public async Task<IEnumerable<Genre>> GetAllAsync()
        {
            const string sql = @"
SELECT Id, Name
FROM Foundation.Genres WITH (NOLOCK);";

            return await GetAsync(sql);
        }

        public async Task<Genre> GetByIdAsync(int id)
        {
            const string sql = @"
SELECT Id, Name
FROM Foundation.Genres WITH (NOLOCK)
WHERE Id = @Id;";

            return await GetAsync(sql, new { Id = id });
        }

        public Task<IEnumerable<Genre>> GetGenresByMovieIdAsync(int movieId)
        {
            const string query = @"
SELECT 
    g.Id,
    g.Name
FROM Foundation.Genres g WITH (NOLOCK)
JOIN Foundation.Genres_Movies mg WITH (NOLOCK) ON mg.GenreId = g.Id
WHERE mg.MovieId = @MovieId";
            return QueryAsync<Genre>(query, new { MovieId = movieId });
        }

        public async Task CreateAsync(Genre entity)
        {
            const string sql = @"
INSERT INTO Foundation.Genres (Name)
OUTPUT INSERTED.Id
VALUES (@Name);";

            var newId = await CreateAsync(sql, new { entity.Name });
            entity.Id = newId;
        }

        public Task UpdateAsync(Genre entity)
        {
            const string sql = @"
UPDATE Foundation.Genres
SET Name = @Name
WHERE Id = @Id;";

            return ExecuteAsync(sql, new
            {
                entity.Name,
                entity.Id
            });
        }

        public Task DeleteAsync(int id)
        {
            // Call the stored procedure to delete genre and related join rows
            return ExecuteProcAsync(
                "Foundation.Usp_DeleteGenre",
                new { GenreId = id, debug = false }
            );
        }
    }
}


