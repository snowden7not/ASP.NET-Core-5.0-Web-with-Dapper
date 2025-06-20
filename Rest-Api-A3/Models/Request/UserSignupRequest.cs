using System.ComponentModel.DataAnnotations;

namespace Rest_Api_A3.Models.Request
{
    public class UserSignupRequest
    {
        [Required, EmailAddress] public string Email { get; set; }
        [Required, MinLength(6)] public string Password { get; set; }
    }
}
