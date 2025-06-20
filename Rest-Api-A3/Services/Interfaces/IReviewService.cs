using System.Collections.Generic;
using System.Threading.Tasks;
using Rest_Api_A3.Models.Request;
using Rest_Api_A3.Models.Response;

namespace Rest_Api_A3.Services.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewResponse>> GetAllAsync();
        Task<ReviewResponse> GetByIdAsync(int id);
        Task<ReviewResponse> CreateAsync(ReviewRequest request);
        Task UpdateAsync(int id, ReviewRequest request);
        Task DeleteAsync(int id);
        Task<IEnumerable<ReviewResponse>> GetReviewsByMovieIdAsync(int movieId);
    }
}
