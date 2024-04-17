namespace SadhanaApp.WebUI.ViewModels
{
    public class SadhanaHistoryViewModel
    {
        public List<ChantingRecord> Records { get; set; }
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int EntriesToShow { get; set; }
        public string HeadingText { get; set; }

        // Additions for week-based navigation
        public int WeekOffset { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Days { get; set; } // Number of days for the history view
    }

}
