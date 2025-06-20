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
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _repo;
        private readonly IMovieService _movieService;
        private readonly IMapper _mapper;

        public ReviewService(
            IReviewRepository repo,
            IMovieService movieService,
            IMapper mapper)
        {
            _repo = repo;
            _movieService = movieService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReviewResponse>> GetAllAsync()
        {
            var entities = await _repo.GetAllAsync();
            return entities.Select(e => _mapper.Map<ReviewResponse>(e));
        }

        public async Task<ReviewResponse> GetByIdAsync(int id)
        {
            var entity = await GetAndValidateReviewAsync(id);
            return _mapper.Map<ReviewResponse>(entity);
        }

        public async Task<ReviewResponse> CreateAsync(ReviewRequest request)
        {
            // Validate request fields
            validateRequest(request);

            // Ensure the movie exists
            await _movieService.GetByIdAsync(request.MovieId);

            // Map into entity and insert
            var entity = _mapper.Map<Review>(request);
            await _repo.CreateAsync(entity);

            // Return DTO
            return _mapper.Map<ReviewResponse>(entity);
        }

        public async Task UpdateAsync(int id, ReviewRequest request)
        {
            // Fetch existing
            var existing = await GetAndValidateReviewAsync(id);

            // Validate request fields
            validateRequest(request);

            // Ensure the movie exists
            await _movieService.GetByIdAsync(request.MovieId);

            // Map updated fields and save
            _mapper.Map(request, existing);
            existing.Id = id;
            await _repo.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await GetAndValidateReviewAsync(id);
            await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<ReviewResponse>> GetReviewsByMovieIdAsync(int movieId)
        {
            // Validate that the movie exists (throws if not)
            await _movieService.GetByIdAsync(movieId);

            // Fetch reviews filtered by MovieId
            var entities = await _repo.GetReviewsByMovieIdAsync(movieId);

            // Map to response DTOs
            return entities.Select(e => _mapper.Map<ReviewResponse>(e));
        }


        private async Task<Review> GetAndValidateReviewAsync(int id)
        {
            return await _repo.GetByIdAsync(id)
                   ?? throw new KeyNotFoundException($"Review {id} not found");
        }


        private void validateRequest(ReviewRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
                throw new ArgumentException("Review message is required", nameof(request.Message));
            if (request.MovieId <= 0)
                throw new ArgumentException("Valid MovieId is required", nameof(request.MovieId));
        }
    }
}



