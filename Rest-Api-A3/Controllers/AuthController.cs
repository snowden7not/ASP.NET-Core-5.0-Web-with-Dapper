using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rest_Api_A3.Models.Request;
using Rest_Api_A3.Services.Interfaces;

namespace Rest_Api_A3.Controllers
{
    [Route("api/auth")]
    [ApiController]
    
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] UserSignupRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);                // 400

            try
            {
                var response = await _authService.SignupAsync(request);
                return Ok(response);                          // 200 
            }
            catch (ArgumentException ex)
            {
                // e.g. password too weak, missing fields
                return BadRequest(new { error = ex.Message }); // 400
            }
            catch (InvalidOperationException ex)
            {
                // e.g. email already in use
                return Conflict(new { error = ex.Message });   // 409
            }
            catch (Exception ex)
            {
                return StatusCode(500,                         // 500 Internal Server Error
                    new { error = $"An unexpected error occurred: {ex.Message}" });
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);                // 400

            try
            {
                var response = await _authService.LoginAsync(request);
                return Ok(response);                          // 200
            }
            catch (ArgumentException ex)
            {
                // e.g. malformed request payload
                return BadRequest(new { error = ex.Message }); // 400
            }
            catch (UnauthorizedAccessException)
            {
                // e.g. wrong email/password
                return Unauthorized();                        // 401
            }
            catch (Exception ex)
            {
                return StatusCode(500,                         // 500 Internal Server Error
                    new { error = $"An unexpected error occurred: {ex.Message}" });
            }
        }
    }
}
