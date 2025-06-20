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
    
    public class ProducersController : ControllerBase
    {
        private readonly IProducerService _producerService;

        public ProducersController(IProducerService producerService)
        {
            _producerService = producerService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProducerResponse>>> GetAll()
        {
            try
            {
                var producers = await _producerService.GetAllAsync();
                return Ok(producers); // 200
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An internal server error occurred"); // 500 Internal Server Error
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProducerResponse>> GetById(int id)
        {
            try
            {
                var producer = await _producerService.GetByIdAsync(id);
                return Ok(producer); // 200
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
        public async Task<ActionResult<ProducerResponse>> Create([FromBody] ProducerRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // 400

            try
            {
                var created = await _producerService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created); // 201
            }
            catch (ArgumentException ex)
            {
                // e.g. invalid payload
                return BadRequest(new { error = ex.Message }); // 400
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An internal server error occurred"); // 500 Internal Server Error
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProducerRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // 400

            try
            {
                await _producerService.UpdateAsync(id, request);
                return NoContent(); // 204
            }
            catch (KeyNotFoundException)
            {
                return NotFound(); // 404
            }
            catch (ArgumentException ex)
            {
                // e.g. invalid payload values
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
                await _producerService.DeleteAsync(id);
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
