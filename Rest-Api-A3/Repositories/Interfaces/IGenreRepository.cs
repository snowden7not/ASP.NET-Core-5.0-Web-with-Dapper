using System.Collections.Generic;
using System.Threading.Tasks;
using Rest_Api_A3.Models.Database;

namespace Rest_Api_A3.Repositories.Interfaces
{
    public interface IGenreRepository
    {
        Task CreateAsync(Genre entity);
        Task DeleteAsync(int id);
        Task<IEnumerable<Genre>> GetAllAsync();
        Task<Genre> GetByIdAsync(int id);
        Task<IEnumerable<Genre>> GetGenresByMovieIdAsync(int movieId);
        Task UpdateAsync(Genre entity);
    }
}