using System.Collections.Generic;
using System.Threading.Tasks;
using Rest_Api_A3.Models.Database;

namespace Rest_Api_A3.Repositories.Interfaces
{
    public interface IProducerRepository
    {
        Task<IEnumerable<Producer>> GetAllAsync();
        Task<Producer> GetByIdAsync(int id);
        Task CreateAsync(Producer entity);
        Task UpdateAsync(Producer entity);
        Task DeleteAsync(int id);
    }
}