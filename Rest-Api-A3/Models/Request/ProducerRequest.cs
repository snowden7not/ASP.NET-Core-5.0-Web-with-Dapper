using System.ComponentModel.DataAnnotations;
using System;

namespace Rest_Api_A3.Models.Request
{
    public class ProducerRequest
    {
        [Required] public string Name { get; set; }
        [Required] public string Bio { get; set; }
        [Required] public DateTime? DOB { get; set; }
        [Required] public string Gender { get; set; }
    }
}
