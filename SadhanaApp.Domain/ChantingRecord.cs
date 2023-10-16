public class ChantingRecord
{
    public int Id { get; set; }
    public int RecordId { get; set; }
    public DateTime Date { get; set; }
    public int MorningRounds { get; set; }
    public int DayRounds { get; set; }
    public int EveningRounds { get; set; }

    // Navigation property
    public int UserId { get; set; }
    public User User { get; set; }
}
