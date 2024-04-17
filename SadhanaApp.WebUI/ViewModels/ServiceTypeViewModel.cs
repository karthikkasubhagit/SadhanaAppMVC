using SadhanaApp.Domain;

namespace SadhanaApp.WebUI.ViewModels
{
    public class ServiceTypeViewModel
    {
        public List<ServiceType> ActiveServiceTypes { get; set; }
        public List<ServiceType> HiddenServiceTypes { get; set; }
    }
}
