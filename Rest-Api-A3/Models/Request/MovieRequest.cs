using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Rest_Api_A3.Models.Request
{
    public class MovieRequest
    {
        [Required] public string Name { get; set; }
        [Required] public int? YearOfRelease { get; set; }
        [Required] public string Plot { get; set; }
        [Required] public int? ProducerId { get; set; }
        [Required] public string CoverImageUrl { get; set; }

        [Required] public string Language { get; set; }
        [Required] public int? Profit { get; set; }


        //tells which related entities to link
        public List<int> ActorIds { get; set; } = new List<int>();
        public List<int> GenreIds { get; set; } = new List<int>();
    }
}
