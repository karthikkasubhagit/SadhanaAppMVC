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
                .ForMember(dest => dest.ServiceTypeId, opt => opt.MapFrom(src => src.SelectedServiceTypeId != "other"
                                                                                ? int.Parse(src.SelectedServiceTypeId)
                                                                                : (int?)null))
                .ForMember(dest => dest.ServiceType, opt => opt.Ignore()); // Ignore ServiceType in mapping

            CreateMap<ChantingRecord, ChantingViewModel>()
                .ForMember(dest => dest.SelectedServiceTypeId, opt => opt.MapFrom(src => src.ServiceTypeId.HasValue
                                                                                        ? src.ServiceTypeId.ToString()
                                                                                        : "other"))
                .ForMember(dest => dest.CustomServiceTypeInput, opt => opt.MapFrom(src => src.ServiceType != null
                                                                                        ? src.ServiceType.ServiceName
                                                                                        : null));
        }
    }
}
