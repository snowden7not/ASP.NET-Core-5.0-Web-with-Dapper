using System.Collections.Generic;
using System.Threading.Tasks;
using Rest_Api_A3.Models.Database;

namespace Rest_Api_A3.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task CreateAsync(Review entity);
        Task DeleteAsync(int id);
        Task<IEnumerable<Review>> GetAllAsync();
        Task<Review> GetByIdAsync(int id);
        Task UpdateAsync(Review entity);
        Task<IEnumerable<Review>> GetReviewsByMovieIdAsync(int movieId);
    }
}