namespace SadhanaApp.WebUI.Utilities
{
    public class ReadingHearingScoreCalculator
    {
        public double CalculateScore(TimeSpan duration)
        {
            if (duration.TotalMinutes < 15) return -20;
            if (duration.TotalMinutes <= 60) return (18.0 / 45.0) * duration.TotalMinutes - 6;
            return 18;  // any duration longer than 1 hour still gets 18 points
        }
    }
}
