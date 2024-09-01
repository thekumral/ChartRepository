using AutoMapper;
using ChartProject.Api.Models;
using ChartProject.Api.Models.Dtos;

namespace ChartProject.Api.Configurations
{
    public class ChartMappingProfile : Profile
    {
        public ChartMappingProfile()
        {
            CreateMap<ChartData, ChartDataDTO>()
                .ForMember(dest => dest.Label, opt => opt.MapFrom(src => src.Label))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                .ReverseMap();
        }
    }
}
