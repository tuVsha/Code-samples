using AutoMapper;
using Saritasa.OMDbScrubber.Domain;
using Saritasa.OMDbScrubber.Infrastructure.Abstractions.OMDb_Entities;

namespace Saritasa.OMDbScrubber.Infrastructure.OMDbAutoMapper
{
    /// <summary>
    /// Profile provides mapping <see cref="OmdbMovie"/> to <see cref="Domain.Movie"/>.
    /// </summary>
    public class OmdbMovieMappingProfile : Profile
    {
        /// <summary>
        /// OMDb tag means information not available.
        /// </summary>
        private const string NotAvailable = "N/A";

        /// <summary>
        /// Length of the end of runtime string from OMDb response.
        /// </summary>
        private const int LengthOfMin = 4;

        /// <summary>
        /// Initialize of all mapping rules.
        /// </summary>
        public OmdbMovieMappingProfile()
        {
            CreateMap<OmdbMovie, Movie>()
                .ForMember(dest => dest.Title,
                           opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Genre,
                           opt => opt.MapFrom(src => src.Genre))
                .ForMember(dest => dest.ImdbId,
                           opt => opt.MapFrom(src => src.ImdbId))
                .ForMember(dest => dest.Rating,
                           opt => opt.MapFrom(src => src.ImdbRating))
                .ForMember(dest => dest.Actors,
                           opt => opt.MapFrom(src => src.Actors))
                .ForMember(dest => dest.ReleaseDate,
                           opt => opt.MapFrom(src => src.Released))
                .ForMember(dest => dest.RunTimeMins,
                           opt => opt.MapFrom(src => src.Runtime));

            CreateMap<string, ICollection<Actor>>().ConvertUsing(new ActorsConverter());
            CreateMap<string, decimal?>().ConvertUsing(new RatingConverter(NotAvailable));
            CreateMap<string, DateTime?>().ConvertUsing(new ReleaseDateConverter(NotAvailable));
            CreateMap<string, int?>().ConvertUsing(new RuntimeConverter(NotAvailable));
        }
    }
}
