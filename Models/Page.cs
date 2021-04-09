using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace BYUFagElGamous1_5.Models
{
    public partial class Page
    {
        public int MeasurementId { get; set; }
        public int BookId { get; set; }
        public int? PageNum { get; set; }
        public string DataEntryExpertInitials { get; set; }
        public string DataEntryCheckerInitials { get; set; }
    }
}
