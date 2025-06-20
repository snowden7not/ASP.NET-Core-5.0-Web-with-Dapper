using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Rest_Api_A3.Models.Database;
using Rest_Api_A3.Repositories.Interfaces;

namespace Rest_Api_A3.Repositories
{
    public class ActorRepository
        : BaseRepository<Actor>, IActorRepository
    {
        public ActorRepository(IOptions<ConnectionString> options)
            : base(options.Value.IMDBDB)
        { }

        public async Task<IEnumerable<Actor>> GetAllAsync()
        {
            const string query = @"
SELECT 
    Id,
    FirstName + ' ' + LastName AS Name,
    Bio,
    DateOfBirth AS DOB,
    Gender
FROM Foundation.Actors WITH (NOLOCK);";

            return await GetAsync(query);
        }

        public async Task<Actor> GetByIdAsync(int id)
        {
            const string query = @"
SELECT 
    Id,
    FirstName + ' ' + LastName AS Name,
    Bio,
    DateOfBirth AS DOB,
    Gender
FROM Foundation.Actors WITH (NOLOCK)
WHERE Id = @Id;";

            return await GetAsync(query, new { Id = id });
        }

        public Task<IEnumerable<Actor>> GetActorsByMovieIdAsync(int movieId)
        {
            const string query = @"
SELECT 
    a.Id,
    (a.FirstName + ' ' + a.LastName) AS Name,
    a.Bio,
    a.DateOfBirth             AS DOB,
    a.Gender
FROM Foundation.Actors a WITH (NOLOCK)
JOIN Foundation.Actors_Movies ma WITH (NOLOCK) ON ma.ActorId = a.Id
WHERE ma.MovieId = @MovieId";
            return QueryAsync<Actor>(query, new { MovieId = movieId });
        }

        public async Task CreateAsync(Actor entity)
        {
            const string query = @"
INSERT INTO Foundation.Actors (FirstName, LastName, Bio, DateOfBirth, Gender)
OUTPUT INSERTED.Id
VALUES (@FirstName, @LastName, @Bio, @DOB, @Gender)";

            var nameParts = (entity.Name ?? "").Split(' ', 2);
            var firstName = nameParts.Length > 0 ? nameParts[0] : "";
            var lastName = nameParts.Length > 1 ? nameParts[1] : "";

            var newId = await CreateAsync(query, new
            {
                FirstName = firstName,
                LastName = lastName,
                entity.Bio,
                DOB = entity.DOB,
                Gender = entity.Gender
            });
            entity.Id = newId;
        }

        public async Task UpdateAsync(Actor entity)
        {
            const string query = @"
UPDATE Foundation.Actors
SET 
    FirstName   = @FirstName,
    LastName    = @LastName,
    Bio         = @Bio,
    DateOfBirth = @DOB,
    Gender      = @Gender
WHERE Id = @Id;";

            var nameParts = (entity.Name ?? "").Split(' ', 2);
            var firstName = nameParts.Length > 0 ? nameParts[0] : "";
            var lastName = nameParts.Length > 1 ? nameParts[1] : "";

            await ExecuteAsync(query, new
            {
                FirstName = firstName,
                LastName = lastName,
                entity.Bio,
                DOB = entity.DOB,
                Gender = entity.Gender,
                entity.Id
            });
        }

        public Task DeleteAsync(int id)
        {
            // Use the stored procedure to delete actor and related join rows
            return ExecuteProcAsync(
                "Foundation.Usp_DeleteActor",
                new { ActorId = id, debug = false }
            );
        }
    }
}


