using System;

namespace Rest_Api_A3.Models.Database
{
    public class Movie
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int YearOfRelease { get; set; }
        public string Plot { get; set; }
        public int ProducerId { get; set; }
        public Uri CoverImageUrl { get; set; }

        public string Language { get; set; }
        public int Profit { get; set; }

    }
}
