using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Rest_Api_A3.Models.Request;
using Rest_Api_A3.Models.Response;

namespace Rest_Api_A3.Services.Interfaces
{
    public interface IMovieService
    {
        //params year is optional filter
        Task<IEnumerable<MovieResponse>> GetAllAsync(int? year = null);
        Task<MovieResponse> GetByIdAsync(int id);
        Task<MovieResponse> CreateAsync(MovieRequest request);
        Task UpdateAsync(int id, MovieRequest request);
        Task DeleteAsync(int id);

        //addsup
        Task<string> UploadPosterAsync(int movieId, IFormFile file);

    }
}
