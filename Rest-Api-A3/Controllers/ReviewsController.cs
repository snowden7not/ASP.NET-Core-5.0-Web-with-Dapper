using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rest_Api_A3.Models.Request;
using Rest_Api_A3.Models.Response;
using Rest_Api_A3.Services.Interfaces;

namespace Rest_Api_A3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewResponse>>> GetAll()
        {
            try
            {
                var reviews = await _reviewService.GetAllAsync();
                return Ok(reviews); // 200
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An internal server error occurred"); // 500 Internal Server Error
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewResponse>> GetById(int id)
        {
            try
            {
                var review = await _reviewService.GetByIdAsync(id);
                return Ok(review); // 200
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
        public async Task<ActionResult<ReviewResponse>> Create([FromBody] ReviewRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // 400

            try
            {
                var created = await _reviewService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created); // 201
            }
            catch (KeyNotFoundException ex)
            {
                // e.g. invalid userId or movieId reference
                return NotFound(new { error = ex.Message }); // 404
            }
            catch (ArgumentException ex)
            {
                // e.g. business rule violation
                return BadRequest(new { error = ex.Message }); // 400
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An internal server error occurred"); // 500 Internal Server Error
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ReviewRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // 400

            try
            {
                await _reviewService.UpdateAsync(id, request);
                return NoContent(); // 204
            }
            catch (KeyNotFoundException ex)
            {
                // e.g. missing review or related entity
                return NotFound(new { error = ex.Message }); // 404
            }
            catch (ArgumentException ex)
            {
                // e.g. invalid rating value
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
                await _reviewService.DeleteAsync(id);
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

        // get all reviews using movieId
        [Authorize]
        [HttpGet("~/api/movies/{movieId}/reviews")]
        public async Task<ActionResult<IEnumerable<ReviewResponse>>> GetByMovieId(int movieId)
        {
            try
            {
                var reviews = await _reviewService.GetReviewsByMovieIdAsync(movieId);
                return Ok(reviews); // 200
            }
            catch (KeyNotFoundException ex)
            {
                // movieId not found
                return NotFound(new { error = ex.Message }); // 404
            }
            catch (Exception)
            {
                return StatusCode(500, "An internal server error occurred"); // 500
            }
        }

    }
}
