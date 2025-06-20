using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rest_Api_A3.Models.Request;
using Rest_Api_A3.Models.Response;
using Rest_Api_A3.Services.Interfaces;

namespace Rest_Api_A3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieResponse>>> GetAll([FromQuery] int? year)
        {
            try
            {
                var movies = await _movieService.GetAllAsync(year);
                return Ok(movies); // 200
            }
            catch (KeyNotFoundException ex)
            {
                // e.g. related entity not found
                return NotFound(ex.Message); // 404
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // 400
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An internal server error occurred"); // 500 Internal Server Error
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<MovieResponse>> GetById(int id)
        {
            try
            {
                var movie = await _movieService.GetByIdAsync(id);
                return Ok(movie); // 200
            }
            catch (KeyNotFoundException)
            {
                return NotFound(); // 404
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An internal server error occurred"); // 500 Internal Server Error
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<MovieResponse>> Create([FromBody] MovieRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // 400

            try
            {
                var created = await _movieService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created); // 201
            }
            catch (KeyNotFoundException ex)
            {
                // e.g. invalid referenced entity (producerId, genreIds, actorIds)
                return NotFound(new { error = ex.Message }); // 404
            }
            catch (ArgumentException ex)
            {
                // e.g. business‐rule validation
                return BadRequest(new { error = ex.Message }); // 400
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An internal server error occurred"); // 500 Internal Server Error
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MovieRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // 400

            try
            {
                await _movieService.UpdateAsync(id, request);
                return NoContent(); // 204
            }
            catch (KeyNotFoundException)
            {
                return NotFound(); // 404
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message }); // 400
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An internal server error occurred"); // 500 Internal Server Error
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _movieService.DeleteAsync(id);
                return NoContent(); // 204
            }
            catch (KeyNotFoundException)
            {
                return NotFound(); // 404
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An internal server error occurred"); // 500 Internal Server Error
            }
        }

        // addsup
        // Upload or replace the poster image for a movie.
        [Authorize]
        [HttpPost("{id}/poster")]
        [RequestSizeLimit(5_000_000)] // 5 MB
        public async Task<IActionResult> UploadPoster(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file provided.");

            if (file.Length > 5_000_000)
                return StatusCode(StatusCodes.Status413PayloadTooLarge, "File too large (max 5 MB).");

            var allowed = new[] { "image/jpeg", "image/png" };
            if (!allowed.Contains(file.ContentType))
                return StatusCode(StatusCodes.Status415UnsupportedMediaType,
                                  "Only JPEG or PNG images are allowed.");

            try
            {
                var posterUrl = await _movieService.UploadPosterAsync(id, file);
                return Ok(new { PosterUrl = posterUrl }); // 200
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);                  // 404
            }
            catch (ArgumentException ex)
            {
                // e.g. service-level validation failure
                return BadRequest(ex.Message);                 // 400
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An internal server error occurred: {ex.Message}"); // 500 Internal server error.
            }
        }
    }
}
