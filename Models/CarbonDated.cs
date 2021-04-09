using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace BYUFagElGamous1_5.Models
{
    public partial class CarbonDated
    {
        public int CarbonDatedId { get; set; }
        public int? MummyId { get; set; }
        public int? LocationId { get; set; }
        public int? RackNumber { get; set; }
        public int? Size { get; set; }
        public int? Foci { get; set; }
        public int? C14sample { get; set; }
        public string Location { get; set; }
        public string Questions { get; set; }
        public string ConventionalAgeBp { get; set; }
        public int? CalendarDate { get; set; }
        public int? Calibrated95CalendarDateMax { get; set; }
        public int? Calibrated95CalendarDateMin { get; set; }
        public int? Calibrated95CalendarDateSpan { get; set; }
        public int? Calibrated95CalendarDateAvg { get; set; }
        public string Category { get; set; }
    }
}
