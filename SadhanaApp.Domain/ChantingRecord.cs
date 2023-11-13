using System.ComponentModel.DataAnnotations;

public class ChantingRecord
{
    public int Id { get; set; }
    public int RecordId { get; set; }

    [Required]
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

    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }

    // Navigation property
    public int UserId { get; set; }
    public User User { get; set; }

    public int? TotalScore
    {
        get
        {
            double? scoreFromRounds = (MorningRounds.GetValueOrDefault() * 4.5) + (DayRounds.GetValueOrDefault() * 3) + (EveningRounds.GetValueOrDefault() * 1.5);
            scoreFromRounds = Math.Min(scoreFromRounds.GetValueOrDefault(), 72);  // Cap chanting to max 72 points

            // Combine Reading and Hearing Duration
            int? combinedDuration = ReadingDurationInMinutes + HearingDurationInMinutes;

            double? scoreFromReadingHearing;

            if (!ReadingDurationInMinutes.HasValue && !HearingDurationInMinutes.HasValue)
            {
                scoreFromReadingHearing = 0;
            }
            else if (combinedDuration < 15)
            {
                scoreFromReadingHearing = -20;
            }
            else if (combinedDuration <= 60)
            {
                scoreFromReadingHearing = (18.0 / 45.0) * (combinedDuration - 15);
            }
            else
            {
                scoreFromReadingHearing = 18;
            }
            scoreFromReadingHearing = Math.Min(scoreFromReadingHearing.GetValueOrDefault(), 18);  // Cap reading & hearing to max 18 points

            // Assuming linear points for service duration as per the description for other sections
            double? scoreFromService;
            if (ServiceDurationInMinutes < 15)
            {
                scoreFromService = 0;
            }
            else if (ServiceDurationInMinutes <= 30)
            {
                scoreFromService = 10;
            }
            else
            {
                scoreFromService = 10 + (ServiceDurationInMinutes - 30) * (10.0 / 30.0);  // This could be adjusted based on exact criteria for service time
            }
            scoreFromService = Math.Min(scoreFromService.GetValueOrDefault(), 10);  // Cap service to max 10 points

            double? result = scoreFromRounds + scoreFromReadingHearing + scoreFromService;
            int? roundedTotal = result.HasValue ? (int?)Math.Round(result.Value) : null;
            return roundedTotal;
        }
    }
}
