using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Rest_Api_A3.Models.Database;
using Rest_Api_A3.Repositories.Interfaces;

namespace Rest_Api_A3.Repositories
{
    public class ProducerRepository
        : BaseRepository<Producer>, IProducerRepository
    {
        public ProducerRepository(IOptions<ConnectionString> opts)
            : base(opts.Value.IMDBDB)
        { }

        public async Task<IEnumerable<Producer>> GetAllAsync()
        {
            const string sql = @"
SELECT 
    Id,
    FirstName + ' ' + LastName AS Name,
    Bio,
    DateOfBirth AS DOB,
    Gender
FROM Foundation.Producers WITH (NOLOCK)";
            return await GetAsync(sql);
        }

        public async Task<Producer> GetByIdAsync(int id)
        {
            const string sql = @"
SELECT 
    Id,
    FirstName + ' ' + LastName AS Name,
    Bio,
    DateOfBirth AS DOB,
    Gender
FROM Foundation.Producers WITH (NOLOCK)
WHERE Id = @Id";
            return await GetAsync(sql, new { Id = id });
        }

        public async Task CreateAsync(Producer entity)
        {
            const string sql = @"
INSERT INTO Foundation.Producers (FirstName, LastName, Bio, DateOfBirth, Gender)
OUTPUT INSERTED.Id
VALUES (@FirstName, @LastName, @Bio, @DOB, @Gender)";
            var nameParts = (entity.Name ?? "").Split(' ', 2);
            var firstName = nameParts.Length > 0 ? nameParts[0] : "";
            var lastName = nameParts.Length > 1 ? nameParts[1] : "";

            var newId = await CreateAsync(sql, new
            {
                FirstName = firstName,
                LastName = lastName,
                entity.Bio,
                DOB = entity.DOB,
                Gender = entity.Gender
            });
            entity.Id = newId;
        }

        public Task UpdateAsync(Producer entity)
        {
            const string sql = @"
UPDATE Foundation.Producers
SET FirstName   = @FirstName,
    LastName    = @LastName,
    Bio         = @Bio,
    DateOfBirth = @DOB,
    Gender      = @Gender
WHERE Id = @Id";

            var nameParts = (entity.Name ?? "").Split(' ', 2);
            var firstName = nameParts.Length > 0 ? nameParts[0] : "";
            var lastName = nameParts.Length > 1 ? nameParts[1] : "";

            return ExecuteAsync(sql, new
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
            // Call the stored procedure to cascade-delete movies, actor/genre links & the producer
            return ExecuteProcAsync(
                "Foundation.Usp_DeleteProducer",
                new { ProducerId = id, debug = false }
            );
        }
    }
}


