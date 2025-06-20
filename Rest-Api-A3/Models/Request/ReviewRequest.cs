using System.ComponentModel.DataAnnotations;

namespace Rest_Api_A3.Models.Request
{
    public class ReviewRequest
    {
        [Required] public int MovieId { get; set; }
        [Required] public string Message { get; set; }
    }
}
