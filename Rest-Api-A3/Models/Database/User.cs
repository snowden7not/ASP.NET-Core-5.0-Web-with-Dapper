using Microsoft.AspNetCore.Identity;

namespace Rest_Api_A3.Models.Database
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
