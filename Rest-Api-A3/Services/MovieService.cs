using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Rest_Api_A3.Models.Database;
using Rest_Api_A3.Models.Request;
using Rest_Api_A3.Models.Response;
using Rest_Api_A3.Repositories.Interfaces;
using Rest_Api_A3.Services.Interfaces;

namespace Rest_Api_A3.Services.Implementations
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository    _movieRepo;

        private readonly IProducerService _producerService;
        private readonly IActorService _actorService;
        private readonly IGenreService _genreService;

        private readonly IMapper             _mapper;
        private readonly IStorageService _storageService;  //addsup

        public MovieService(
            IMovieRepository movieRepo,

            IProducerService producerService,
            IActorService actorService,
            IGenreService genreService,

            IMapper mapper,
            IStorageService storageService)
        {
            _movieRepo    = movieRepo;

            _producerService = producerService;
            _actorService = actorService;
            _genreService = genreService;

            _mapper       = mapper;
            _storageService = storageService;
        }

        public async Task<IEnumerable<MovieResponse>> GetAllAsync(int? year = null)
        {
            var movies = await _movieRepo.GetAllAsync();
            if (year.HasValue)
                movies = movies.Where(m => m.YearOfRelease == year.Value);

            var result = new List<MovieResponse>(movies.Count());
            foreach (var m in movies)
            {
                result.Add(await MapToDtoAsync(m));
            }

            return result;
        }

        public async Task<MovieResponse> GetByIdAsync(int id)
        {
            var movie = await GetAndValidateMovieAsync(id);
            return await MapToDtoAsync(movie);
        }

        public async Task<MovieResponse> CreateAsync(MovieRequest request)
        {
            // Validation
            ValidateRequest(request);

            // Ensure producer exists & get producer response
            var producerResponse = await _producerService.GetByIdAsync(request.ProducerId.Value);

            //Ensure actors exists & get actor responses
            var actorResponses = await Task.WhenAll(
                request.ActorIds
                       .Select(actorId => _actorService.GetByIdAsync(actorId))
            );

            //Ensure genres exists & get genre responses
            var genreResponses = await Task.WhenAll(
                request.GenreIds
                       .Select(genreId => _genreService.GetByIdAsync(genreId))
            );

            // map request into entity 
            var movie = _mapper.Map<Movie>(request);

            // Create via SP, sets movie.Id via OUTPUT 
            await _movieRepo.CreateAsync(movie, request.ActorIds, request.GenreIds);

            // map entity into response
            var dto = _mapper.Map<MovieResponse>(movie);
            dto.Producer = producerResponse;
            dto.Actors = actorResponses.ToList();
            dto.Genres = genreResponses.ToList();

            return dto;
        }

        public async Task UpdateAsync(int id, MovieRequest request)
        {
            // Validation 
            ValidateRequest(request);

            // Fetch existing movie (throws if not found)
            var existing = await GetAndValidateMovieAsync(id);

            // Only validate ProducerId if it has changed
            if (existing.ProducerId != request.ProducerId.Value)
            {
                await _producerService.GetByIdAsync(request.ProducerId.Value);
            }

            // Only validate ActorIds if the list has changed
            var existingActors = (await _actorService.GetActorsByMovieIdAsync(id))
                                 .Select(a => a.Id)
                                 .OrderBy(a => a)
                                 .ToList();

            var updatedActors = request.ActorIds
                                   .OrderBy(a => a)
                                   .ToList();

            if (!existingActors.SequenceEqual(updatedActors))
            {
                await Task.WhenAll(
                    request.ActorIds.Select(actorId => _actorService.GetByIdAsync(actorId))
                );
            }

            // Only validate GenreIds if the list has changed
            var existingGenres = (await _genreService.GetGenresByMovieIdAsync(id))
                                 .Select(g => g.Id)
                                 .OrderBy(g => g)
                                 .ToList();

            var updatedGenres = request.GenreIds
                                   .OrderBy(g => g)
                                   .ToList();

            if (!existingGenres.SequenceEqual(updatedGenres))
            {
                await Task.WhenAll(
                    request.GenreIds.Select(genreId => _genreService.GetByIdAsync(genreId))
                );
            }

            // Map updated fields
            _mapper.Map(request, existing); //corrects existing using request data
            existing.Id = id;

            // 7) Update via stored procedure (passing actor and genre IDs)
            await _movieRepo.UpdateAsync(existing, request.ActorIds, request.GenreIds);
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await GetAndValidateMovieAsync(id);
            await _movieRepo.DeleteAsync(id);
        }

        //addsup
        public async Task<string> UploadPosterAsync(int movieId, IFormFile file)
        {
            // Fetch current movie from DB
            var movie = await GetAndValidateMovieAsync(movieId);

            // If poster exist in bucket, delete it
            if (movie.CoverImageUrl != null)
            {
                await _storageService.DeleteMoviePosterByUrlAsync(movie.CoverImageUrl.ToString());
            }

            // Upload the new poster
            var newUrl = await _storageService.UploadMoviePosterAsync(movieId, file);

            // Update sql movie record with the new URL using Usp_UpdateMoviePoster sp.
            await _movieRepo.UpdatePosterAsync(movieId, newUrl);

            return newUrl;
        }

        private async Task<Movie> GetAndValidateMovieAsync(int id)
        {
            return await _movieRepo.GetByIdAsync(id)
                   ?? throw new KeyNotFoundException($"Movie {id} not found");
        }


        // Helper to map Movie model to MovieResponse Model (DTO) by fetching its Producer, Actors, and Genres in parallel.
        private async Task<MovieResponse> MapToDtoAsync(Movie m)
        {
            // Fetch associated data in parallel
            var prodTask = _producerService.GetByIdAsync(m.ProducerId);
            var actorsTask = _actorService.GetActorsByMovieIdAsync(m.Id);
            var genresTask = _genreService.GetGenresByMovieIdAsync(m.Id);

            await Task.WhenAll(prodTask, actorsTask, genresTask);

            // Map to DTO and assign nested objects
            var dto = _mapper.Map<MovieResponse>(m);
            dto.Producer = prodTask.Result;
            dto.Actors = actorsTask.Result
                                 .Select(a => _mapper.Map<ActorResponse>(a))
                                 .ToList();
            dto.Genres = genresTask.Result
                                 .Select(g => _mapper.Map<GenreResponse>(g))
                                 .ToList();
            return dto;
        }

        //basic validation of request
        private void ValidateRequest(MovieRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Movie name is required", nameof(request.Name));
            if (request.YearOfRelease <= 0)
                throw new ArgumentException("Valid YearOfRelease is required", nameof(request.YearOfRelease));
            if (string.IsNullOrWhiteSpace(request.Plot))
                throw new ArgumentException("Plot is required", nameof(request.Plot));
            if (string.IsNullOrWhiteSpace(request.Language))
                throw new ArgumentException("Language is required", nameof(request.Language));
            if (request.ProducerId <= 0)
                throw new ArgumentException("Valid ProducerId is required", nameof(request.ProducerId));
            if (request.ActorIds == null || !request.ActorIds.Any())
                throw new ArgumentException("At least one ActorId is required", nameof(request.ActorIds));
            if (request.GenreIds == null || !request.GenreIds.Any())
                throw new ArgumentException("At least one GenreId is required", nameof(request.GenreIds));
        }
    }
}



