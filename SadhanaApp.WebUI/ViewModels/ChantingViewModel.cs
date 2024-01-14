using System.ComponentModel.DataAnnotations;

namespace SadhanaApp.WebUI.ViewModels
{
    public class ChantingViewModel
    {
        [Required(ErrorMessage = "Date is a mandatory field")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [CustomDate(ErrorMessage = "Future dates are not allowed.")]
        public DateTime? Date { get; set; }

        // A list to hold the names of selected service types
        public List<string> SelectedServiceTypeNames { get; set; } = new List<string>();


        // Property to get/set the selected service type names as a semicolon-separated string
        public string SelectedServiceTypeNamesAsString
        {
            get => string.Join(";", SelectedServiceTypeNames);
            set => SelectedServiceTypeNames = value?.Split(';').ToList() ?? new List<string>();
        }
        public bool IsOtherServiceTypeSelected { get; set; }

        // Custom service type input for 'other'
        public string? CustomServiceTypeInput { get; set; }

        [Range(0, 200)]
        public int? MorningRounds { get; set; }

        [Range(0, 200)]
        public int? DayRounds { get; set; }

        [Range(0, 200)]
        public int? EveningRounds { get; set; }

        public string? ReadingTitle { get; set; }


        [Range(0, 500)]
        public int? ReadingDurationInMinutes { get; set; }

        public string? HearingTitle { get; set; }

        [Range(0, 500)]
        public int? HearingDurationInMinutes { get; set; }        
        public string? ServiceType { get; set; }

        [Range(0, 500)]
        public int? ServiceDurationInMinutes { get; set; }

        [MaxLength(100)]
        public string? Notes { get; set; }

    }

    public class CustomDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null || !(value is DateTime))
            {
                return false;
            }

            DateTime inputDate = (DateTime)value;
            DateTime currentTimePlus2Hours = DateTime.Now.AddHours(2);

            return inputDate <= currentTimePlus2Hours;
        }
    }
}
