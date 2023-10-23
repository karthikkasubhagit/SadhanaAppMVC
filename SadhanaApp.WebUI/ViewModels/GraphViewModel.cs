using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SadhanaApp.WebUI.ViewModels
{
    public class GraphViewModel
    {
        public List<string> Dates { get; set; }  // For daily progress
        public List<int> TotalScoresPerDay { get; set; }

        public List<string> Months { get; set; }  // For monthly progress
        public List<int> TotalScoresPerMonth { get; set; }

        public List<string> Years { get; set; }  // For yearly progress
        public List<int> TotalScoresPerYear { get; set; }
    }


}