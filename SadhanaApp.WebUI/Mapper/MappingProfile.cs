using AutoMapper;
using SadhanaApp.Domain;
using SadhanaApp.WebUI.ViewModels;

namespace SadhanaApp.WebUI.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ChantingViewModel, ChantingRecord>()
            .ForMember(dest => dest.ServiceTypeNames, opt => opt.MapFrom(src => src.SelectedServiceTypeNamesAsString))
            // Map CustomServiceTypeInput and IsOtherServiceTypeSelected
            .ForMember(dest => dest.CustomServiceTypeInput, opt => opt.MapFrom(src => src.CustomServiceTypeInput))
            .ForMember(dest => dest.IsOtherServiceTypeSelected, opt => opt.MapFrom(src => src.SelectedServiceTypeNames.Contains("Others")));

            CreateMap<ChantingRecord, ChantingViewModel>()
                .ForMember(dest => dest.SelectedServiceTypeNamesAsString, opt => opt.MapFrom(src => src.ServiceTypeNames))
                // Map back CustomServiceTypeInput and calculate if 'Others' is selected
                .ForMember(dest => dest.CustomServiceTypeInput, opt => opt.MapFrom(src => src.CustomServiceTypeInput))
                .AfterMap((src, dest) => dest.SelectedServiceTypeNames.Add("Others"));
        }
    }
}
