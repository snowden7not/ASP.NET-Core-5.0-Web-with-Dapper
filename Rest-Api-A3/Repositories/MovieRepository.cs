using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Options;
using Rest_Api_A3.Models.Database;
using Rest_Api_A3.Repositories.Interfaces;

namespace Rest_Api_A3.Repositories
{
    public class MovieRepository
        : BaseRepository<Movie>, IMovieRepository
    {
        public MovieRepository(IOptions<ConnectionString> opts)
            : base(opts.Value.IMDBDB)
        { }

        public async Task<IEnumerable<Movie>> GetAllAsync()
        {
            const string query = @"
SELECT 
     Id,
     Title        AS Name,
     YearOfRelease,
     Plot,
     ProducerId,
     PosterLink   AS CoverImageUrl,
     MovieLanguage AS Language,
     Profit
FROM Foundation.Movies WITH (NOLOCK)";
            return await GetAsync(query);
        }

        public async Task<Movie> GetByIdAsync(int id)
        {
            const string query = @"
SELECT 
     Id,
     Title        AS Name,
     YearOfRelease,
     Plot,
     ProducerId,
     PosterLink   AS CoverImageUrl,
     MovieLanguage AS Language,
     Profit
FROM Foundation.Movies WITH (NOLOCK)
WHERE Id = @Id";
            return await GetAsync(query, new { Id = id });
        }

        public async Task CreateAsync(Movie movie, IEnumerable<int> actorIds, IEnumerable<int> genreIds)
        {
            var actorCsv = string.Join(",", actorIds);
            var genreCsv = string.Join(",", genreIds);

            var dp = new DynamicParameters();
            dp.Add("Title", movie.Name);
            dp.Add("YearOfRelease", movie.YearOfRelease);
            dp.Add("Plot", movie.Plot);
            dp.Add("ProducerId", movie.ProducerId);
            dp.Add("PosterLink", movie.CoverImageUrl?.ToString()); // convert Uri to string
            dp.Add("ActorIds", actorCsv);
            dp.Add("GenreIds", genreCsv);

            dp.Add("MovieLanguage", movie.Language);
            dp.Add("Profit", movie.Profit);

            dp.Add("NewMovieId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var newId = await ExecuteProcWithOutputAsync("Foundation.Usp_InsertMovie", dp, "NewMovieId");
            movie.Id = newId;
        }

        public Task UpdateAsync(Movie movie, IEnumerable<int> actorIds, IEnumerable<int> genreIds)
        {
            var actorCsv = string.Join(",", actorIds);
            var genreCsv = string.Join(",", genreIds);

            return ExecuteProcAsync("Foundation.Usp_UpdateMovie", new
            {
                MovieId = movie.Id,
                Title = movie.Name,
                YearOfRelease = movie.YearOfRelease,
                Plot = movie.Plot,
                PosterLink = movie.CoverImageUrl?.ToString(), // convert Uri to string
                ProducerId = movie.ProducerId,
                MovieLanguage = movie.Language,
                Profit = movie.Profit,
                ActorIds = actorCsv,
                GenreIds = genreCsv,
                debug = false
            });
        }

        public Task DeleteAsync(int id)
        {
            return ExecuteProcAsync("Foundation.Usp_DeleteMovie", new { MovieId = id });
        }

        //addsup
        // Updates only the PosterLink of a movie via Usp_UpdateMoviePoster
        public Task UpdatePosterAsync(int movieId, string posterUrl)
        {
            return ExecuteProcAsync(
                "Foundation.Usp_UpdateMoviePoster",
                new
                {
                    MovieId = movieId,
                    PosterLink = posterUrl,
                    debug = false
                }
            );
        }

    }
}

