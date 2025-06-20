using System.Collections.Generic;
using System.Threading.Tasks;
using Rest_Api_A3.Models.Database;
using Rest_Api_A3.Models.Request;
using Rest_Api_A3.Models.Response;

namespace Rest_Api_A3.Services.Interfaces
{
    public interface IGenreService
    {
        Task<IEnumerable<GenreResponse>> GetAllAsync();
        Task<GenreResponse> GetByIdAsync(int id);
        Task<IEnumerable<Genre>> GetGenresByMovieIdAsync(int movieId);
        Task<GenreResponse> CreateAsync(GenreRequest request);
        Task UpdateAsync(int id, GenreRequest request);
        Task DeleteAsync(int id);
    }
}
