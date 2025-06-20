using System.Collections.Generic;
using System.Threading.Tasks;
using Rest_Api_A3.Models.Database;

namespace Rest_Api_A3.Repositories.Interfaces
{
    public interface IActorRepository
    {
        Task<IEnumerable<Actor>> GetAllAsync();
        Task<Actor> GetByIdAsync(int id);
        Task<IEnumerable<Actor>> GetActorsByMovieIdAsync(int movieId);
        Task CreateAsync(Actor entity);
        Task UpdateAsync(Actor entity);
        Task DeleteAsync(int id);

    }
}
