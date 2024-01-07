using SadhanaApp.Domain;
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
    //public string? ServiceType { get; set; }

    [Range(0, 500)]
    public int? ServiceDurationInMinutes { get; set; }

    [MaxLength(400)]
    public string? Notes { get; set; }

    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }

    // Navigation property
    public int UserId { get; set; }
    public User User { get; set; }
    public string? ServiceTypeNames { get; set; }
    public bool IsOtherServiceTypeSelected { get; set; }
    public string? CustomServiceTypeInput { get; set; }
    public int? TotalScore
    {
        get
        {
            // Chanting score calculation remains the same
            double scoreFromRounds = (MorningRounds.GetValueOrDefault() * 4.5) +
                                     (DayRounds.GetValueOrDefault() * 3) +
                                     (EveningRounds.GetValueOrDefault() * 1.5);
            scoreFromRounds = Math.Min(scoreFromRounds, 72); // Cap chanting to max 72 points

            // Reading & Hearing score calculation
            double scoreFromReadingHearing = 0;
            int combinedDuration = ReadingDurationInMinutes.GetValueOrDefault() +
                                   HearingDurationInMinutes.GetValueOrDefault();

            if (combinedDuration < 15)
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
            scoreFromReadingHearing = Math.Min(scoreFromReadingHearing, 18); // Cap reading & hearing to max 18 points

            // Service score calculation
            double scoreFromService = 0;
            if (ServiceDurationInMinutes.GetValueOrDefault() >= 15)
            {
                scoreFromService = (ServiceDurationInMinutes.GetValueOrDefault() / 30.0) * 10;
            }
            scoreFromService = Math.Min(scoreFromService, 10); // Cap service to max 10 points

            // Sum of all scores
            double result = scoreFromRounds + scoreFromReadingHearing + scoreFromService;
            return (int)Math.Round(result); // Returning the rounded total score
        }   

}
}
