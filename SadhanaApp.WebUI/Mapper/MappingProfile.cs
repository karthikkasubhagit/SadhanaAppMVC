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
                // Assuming 'ServiceTypeNames' is the property in ChantingRecord to store service type names
                .ForMember(dest => dest.ServiceType, opt => opt.Ignore()); // Ignore ServiceType in mapping

            CreateMap<ChantingRecord, ChantingViewModel>()
                .ForMember(dest => dest.SelectedServiceTypeNamesAsString, opt => opt.MapFrom(src => src.ServiceTypeNames))
                // Assuming 'ServiceTypeNames' is the property in ChantingRecord for service type names
                .ForMember(dest => dest.CustomServiceTypeInput, opt => opt.MapFrom(src => src.ServiceType != null
                                                                                        ? src.ServiceType.ServiceName
                                                                                        : null));
        }
    }
}
