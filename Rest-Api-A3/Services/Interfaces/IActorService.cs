using System.Collections.Generic;
using System.Threading.Tasks;
using Rest_Api_A3.Models.Database;
using Rest_Api_A3.Models.Request;
using Rest_Api_A3.Models.Response;

namespace Rest_Api_A3.Services.Interfaces
{
    public interface IActorService
    {
        Task<IEnumerable<ActorResponse>> GetAllAsync();
        Task<ActorResponse> GetByIdAsync(int id);
        Task<IEnumerable<Actor>> GetActorsByMovieIdAsync(int movieId);
        Task<ActorResponse> CreateAsync(ActorRequest request);
        Task UpdateAsync(int id, ActorRequest request);
        Task DeleteAsync(int id);
    }
}
