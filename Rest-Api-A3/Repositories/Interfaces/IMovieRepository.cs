using System.Collections.Generic;
using System.Threading.Tasks;
using Rest_Api_A3.Models.Database;

namespace Rest_Api_A3.Repositories.Interfaces
{
    public interface IMovieRepository
    {
        Task CreateAsync(Movie movie, IEnumerable<int> actorIds, IEnumerable<int> genreIds);
        Task DeleteAsync(int id);
        
        Task<IEnumerable<Movie>> GetAllAsync();
        Task<Movie> GetByIdAsync(int id);
        
        Task UpdateAsync(Movie movie, IEnumerable<int> actorIds, IEnumerable<int> genreIds);

        //addsup
        // Updates only the PosterLink of a movie via Usp_UpdateMoviePoster.
        Task UpdatePosterAsync(int movieId, string posterUrl);
    }
}