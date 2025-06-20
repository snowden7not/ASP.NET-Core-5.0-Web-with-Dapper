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
    public class ActorService : IActorService
    {
        private readonly IActorRepository _actorRepo;
        private readonly IMapper _mapper;

        public ActorService(IActorRepository actorRepo, IMapper mapper)
        {
            _actorRepo = actorRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ActorResponse>> GetAllAsync()
        {
            var entities = await _actorRepo.GetAllAsync();
            return entities.Select(e => _mapper.Map<ActorResponse>(e));
        }

        public async Task<ActorResponse> GetByIdAsync(int id)
        {
            var entity = await GetAndValidateActorAsync(id);
            return _mapper.Map<ActorResponse>(entity);
        }

        // It will be used in MovieService to get actors by movieId
        public async Task<IEnumerable<Actor>> GetActorsByMovieIdAsync(int movieId)
        {
            return await _actorRepo.GetActorsByMovieIdAsync(movieId);
        }


        public async Task<ActorResponse> CreateAsync(ActorRequest request)
        {
            // Validate
            ValidateRequest(request);

            // Map & insert
            var entity = _mapper.Map<Actor>(request);
            await _actorRepo.CreateAsync(entity);

            // Return newly created
            return _mapper.Map<ActorResponse>(entity);
        }

        public async Task UpdateAsync(int id, ActorRequest request)
        {
            // Fetch existing
            var existing = await GetAndValidateActorAsync(id);

            // Validate
            ValidateRequest(request);

            // Map changes & save
            _mapper.Map(request, existing);
            existing.Id = id;
            await _actorRepo.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await GetAndValidateActorAsync(id);
            await _actorRepo.DeleteAsync(id);
        }

        private async Task<Actor> GetAndValidateActorAsync(int id)
        {
            return await _actorRepo.GetByIdAsync(id)
                   ?? throw new KeyNotFoundException($"Actor {id} not found");
        }


        private void ValidateRequest(ActorRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Name is required.", nameof(request.Name));

            if (request.DOB >= DateTime.UtcNow)
                throw new ArgumentException("DOB must be in the past.", nameof(request.DOB));

            if (string.IsNullOrWhiteSpace(request.Gender))
                throw new ArgumentException("Gender is required.", nameof(request.Gender));

            if (string.IsNullOrWhiteSpace(request.Bio))
                throw new ArgumentException("Bio is required.", nameof(request.Bio));
        }
    }
}


