using System.Collections.Generic;
using System.Threading.Tasks;
using Rest_Api_A3.Models.Request;
using Rest_Api_A3.Models.Response;

namespace Rest_Api_A3.Services.Interfaces
{
    public interface IProducerService
    {
        Task<IEnumerable<ProducerResponse>> GetAllAsync();
        Task<ProducerResponse> GetByIdAsync(int id);
        Task<ProducerResponse> CreateAsync(ProducerRequest request);
        Task UpdateAsync(int id, ProducerRequest request);
        Task DeleteAsync(int id);
    }
}
