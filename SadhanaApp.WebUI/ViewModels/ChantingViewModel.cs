using System.ComponentModel.DataAnnotations;

namespace SadhanaApp.WebUI.ViewModels
{
    public class ChantingViewModel
    {
        [Required, DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

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
}
