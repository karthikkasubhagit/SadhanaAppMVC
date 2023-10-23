public class ChantingRecord
{
    public int Id { get; set; }
    public int RecordId { get; set; }
    public DateTime Date { get; set; }
    public int? MorningRounds { get; set; }
    public int? DayRounds { get; set; }
    public int? EveningRounds { get; set; }

    public string? ReadingTitle { get; set; }
    public int? ReadingDurationInMinutes { get; set; }

    public string? HearingTitle { get; set; }
    public int? HearingDurationInMinutes { get; set; }

    public string? ServiceType { get; set; }
    public int? ServiceDurationInMinutes { get; set; }

    public string? Notes { get; set; }

    // Navigation property
    public int UserId { get; set; }
    public User User { get; set; }

    public int? TotalScore
    {
        get
        {
            double? scoreFromRounds = (MorningRounds * 4.5) + (DayRounds * 3) + (EveningRounds * 1.5);
            scoreFromRounds = Math.Min(scoreFromRounds.GetValueOrDefault(), 72);  // Cap chanting to max 72 points

            // Combine Reading and Hearing Duration
            int? combinedDuration = ReadingDurationInMinutes + HearingDurationInMinutes;

            double? scoreFromReadingHearing;
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
