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
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _repo;
        private readonly IMapper _mapper;

        public GenreService(IGenreRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GenreResponse>> GetAllAsync()
        {
            var entities = await _repo.GetAllAsync();
            return entities.Select(e => _mapper.Map<GenreResponse>(e));
        }

        public async Task<GenreResponse> GetByIdAsync(int id)
        {
            var entity = await GetAndValidateGenreAsync(id);
            return _mapper.Map<GenreResponse>(entity);
        }

        //It will be used in MovieService to get genres by movieId
        public Task<IEnumerable<Genre>> GetGenresByMovieIdAsync(int movieId)
        {
            return _repo.GetGenresByMovieIdAsync(movieId);
        }


        public async Task<GenreResponse> CreateAsync(GenreRequest request)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Genre name is required", nameof(request.Name));

            // Map & insert
            var entity = _mapper.Map<Genre>(request);
            await _repo.CreateAsync(entity);

            // Return created DTO
            return _mapper.Map<GenreResponse>(entity);
        }

        public async Task UpdateAsync(int id, GenreRequest request)
        {
            // Fetch existing
            var existing = await GetAndValidateGenreAsync(id);

            // Validate
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Genre name is required", nameof(request.Name));

            // Map changes & save
            _mapper.Map(request, existing);
            existing.Id = id;
            await _repo.UpdateAsync(existing);
        }

        private async Task<Genre> GetAndValidateGenreAsync(int id)
        {
            return await _repo.GetByIdAsync(id)
                   ?? throw new KeyNotFoundException($"Genre {id} not found");
        }


        public async Task DeleteAsync(int id)
        {
            var existing = await _repo.GetByIdAsync(id)
                           ?? throw new KeyNotFoundException($"Genre {id} not found");
            await _repo.DeleteAsync(id);
        }
    }
}


