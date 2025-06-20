using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Rest_Api_A3.Models.Database;
using Rest_Api_A3.Models.Request;
using Rest_Api_A3.Models.Response;
using Rest_Api_A3.Repositories.Interfaces;
using Rest_Api_A3.Services.Interfaces;

namespace Rest_Api_A3.Services.Implementations
{
    public class ProducerService : IProducerService
    {
        private readonly IProducerRepository _repo;
        private readonly IMapper _mapper;

        public ProducerService(
            IProducerRepository repo,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProducerResponse>> GetAllAsync()
        {
            var entities = await _repo.GetAllAsync();
            return entities.Select(e => _mapper.Map<ProducerResponse>(e));
        }

        public async Task<ProducerResponse> GetByIdAsync(int id)
        {
            var entity = await GetAndValidateProducerAsync(id);
            return _mapper.Map<ProducerResponse>(entity);
        }

        public async Task<ProducerResponse> CreateAsync(ProducerRequest request)
        {
            // Validate
            ValidateRequest(request);

            // Map and insert
            var entity = _mapper.Map<Producer>(request);
            await _repo.CreateAsync(entity);

            // Return newly created
            return _mapper.Map<ProducerResponse>(entity);
        }

        public async Task UpdateAsync(int id, ProducerRequest request)
        {
            // Fetch existing
            var existing = await GetAndValidateProducerAsync(id);

            // Validate
            ValidateRequest(request);

            // Map and save
            _mapper.Map(request, existing);
            existing.Id = id;
            await _repo.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await GetAndValidateProducerAsync(id);
            await _repo.DeleteAsync(id);
        }

        private async Task<Producer> GetAndValidateProducerAsync(int id)
        {
            return await _repo.GetByIdAsync(id)
                   ?? throw new KeyNotFoundException($"Producer {id} not found");
        }


        private void ValidateRequest(ProducerRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Name is required", nameof(request.Name));
            if (request.DOB >= DateTime.UtcNow)
                throw new ArgumentException("DOB must be in the past", nameof(request.DOB));
            if (string.IsNullOrWhiteSpace(request.Gender))
                throw new ArgumentException("Gender is required", nameof(request.Gender));
            if (string.IsNullOrWhiteSpace(request.Bio))
                throw new ArgumentException("Bio is required", nameof(request.Bio));
        }
    }
}


