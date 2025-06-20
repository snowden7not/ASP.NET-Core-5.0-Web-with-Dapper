using System.Collections.Generic;

namespace Rest_Api_A3.Models.Response
{
    public class MovieResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int YearOfRelease { get; set; }
        public string Plot { get; set; }
        public ProducerResponse Producer { get; set; }
        public string CoverImageUrl { get; set; }

        public string Language { get; set; }
        public int Profit { get; set; }


        // nested collections
        public List<ActorResponse> Actors { get; set; } = new();
        public List<GenreResponse> Genres { get; set; } = new();
    }
}
