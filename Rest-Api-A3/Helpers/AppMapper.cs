using System;
using AutoMapper;
using Rest_Api_A3.Models.Database;
using Rest_Api_A3.Models.Request;
using Rest_Api_A3.Models.Response;

namespace Rest_Api_A3.Helpers
{
    public class AppMapper : Profile
    {
        public AppMapper()
        {
            // Actor <source type, destination type>
            CreateMap<Actor, ActorResponse>();
            CreateMap<ActorRequest, Actor>();

            // Producer
            CreateMap<Producer, ProducerResponse>();
            CreateMap<ProducerRequest, Producer>();

            // Genre
            CreateMap<Genre, GenreResponse>();
            CreateMap<GenreRequest, Genre>();

            // Review
            CreateMap<Review, ReviewResponse>();
            CreateMap<ReviewRequest, Review>();

            // maps MovieModel to MovieResponseModel,
                // but ignores Producer, Actors, and Genres. we'll populate these manually in MovieService.cs
            CreateMap<Movie, MovieResponse>()
                .ForMember(dest => dest.Producer, opt => opt.Ignore())
                .ForMember(dest => dest.Actors, opt => opt.Ignore())
                .ForMember(dest => dest.Genres, opt => opt.Ignore())
                .ForMember(dest => dest.CoverImageUrl,
                           opt => opt.MapFrom(src => src.CoverImageUrl != null ? src.CoverImageUrl.ToString() : null))
                .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Language));
            CreateMap<MovieRequest, Movie>()
                 .ForMember(dest => dest.CoverImageUrl,
                           opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.CoverImageUrl) ? null : new Uri(src.CoverImageUrl)))
                 .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Language));

            // no need to map signup/login to UserModel, Identity handles User
        }
    }
}
