using System.ComponentModel.DataAnnotations;

namespace Rest_Api_A3.Models.Request
{
    public class GenreRequest
    {
        [Required] public string Name { get; set; }
    }
}
