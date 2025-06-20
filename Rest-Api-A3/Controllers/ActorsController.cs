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
    public class ActorsController : ControllerBase
    {
        private readonly IActorService _actorService;

        public ActorsController(IActorService actorService)
        {
            _actorService = actorService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActorResponse>>> GetAll()
        {
            var actors = await _actorService.GetAllAsync();
            return Ok(actors); // 200
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ActorResponse>> GetById(int id)
        {
            try
            {
                var actor = await _actorService.GetByIdAsync(id);
                return Ok(actor); // 200
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
        public async Task<ActionResult<ActorResponse>> Create([FromBody] ActorRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // 400

            try
            {
                var created = await _actorService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created); // 201
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
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ActorRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // 400

            try
            {
                await _actorService.UpdateAsync(id, request);
                return NoContent(); // 204
            }
            catch (KeyNotFoundException)
            {
                return NotFound(); // 404
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _actorService.DeleteAsync(id);
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
    }
}
