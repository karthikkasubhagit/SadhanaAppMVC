namespace SadhanaApp.WebUI.ViewModels
{
    public class SadhanaHistoryViewModel
    {
        public List<ChantingRecord> Records { get; set; }
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int DaysFilter { get; set; }
        public string HeadingText { get; set; }

    }
}
