using AutoMapper;
using System.Collections.Generic;
using TvMazeScraper.Domain.Model;

namespace TvMazeScraper.API.AutoMapper
{
    public class DomainProfile : Profile
    {
        public DomainProfile()
        {
            CreateMap<Show, ShowDto>()
                .ForMember(src => src.Id, opt => opt.MapFrom(dest => dest.ApiId))
                .ForMember(src => src.CastsDto, opt => opt.MapFrom(dest => dest.Casts));
            CreateMap<ShowDto, Show>()
                .ForMember(src => src.Id, opt => opt.Ignore())
                .ForMember(src => src.ApiId, opt => opt.MapFrom(dest => dest.Id))
                .ForMember(src => src.Casts, opt => opt.MapFrom(dest => dest.CastsDto));

            CreateMap<List<Show>, List<ShowDto>>().ReverseMap();

            CreateMap<Cast, CastDto>()
                 .ForMember(src => src.Id, opt => opt.MapFrom(dest => dest.ApiId));
            CreateMap<CastDto, Cast>()                 
                 .ForMember(src => src.Id, opt => opt.Ignore())
                 .ForMember(src => src.ApiId, opt => opt.MapFrom(dest => dest.Id));

            CreateMap<List<Cast>, List<CastDto>>().ReverseMap();
        }
    }
}
