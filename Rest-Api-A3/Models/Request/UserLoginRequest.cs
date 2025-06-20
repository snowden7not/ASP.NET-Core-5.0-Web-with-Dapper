using System.ComponentModel.DataAnnotations;

namespace Rest_Api_A3.Models.Request
{
    public class UserLoginRequest
    {
        [Required, EmailAddress] public string Email { get; set; }
        [Required] public string Password { get; set; }
    }
}
