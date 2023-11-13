using AutoMapper;
using SadhanaApp.WebUI.ViewModels;

namespace SadhanaApp.WebUI.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Define mappings
            CreateMap<ChantingViewModel, ChantingRecord>();
            // Can also add reverse mapping if needed
            // CreateMap<ChantingRecord, ChantingViewModel>();
        }
    }
}
