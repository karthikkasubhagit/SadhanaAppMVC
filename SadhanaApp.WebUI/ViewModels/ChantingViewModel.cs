using System.ComponentModel.DataAnnotations;

namespace SadhanaApp.WebUI.ViewModels
{
    public class ChantingViewModel
    {
        [Required(ErrorMessage = "Date is a mandatory field")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [CustomDate(ErrorMessage = "Date cannot be in the future")]
        public DateTime? Date { get; set; }

        // Existing service type (e.g., ID of the selected service type)
        public string? SelectedServiceTypeId { get; set; }

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
            DateTime d = Convert.ToDateTime(value);
            return d <= DateTime.Now;
        }
    }
}
