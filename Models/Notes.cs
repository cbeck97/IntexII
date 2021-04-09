using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace BYUFagElGamous1_5.Models
{
    public partial class Notes
    {
        public int NotesId { get; set; }
        public int? MeasurmentsId { get; set; }
        public int? SampleId { get; set; }
        public int? LocationId { get; set; }
        public string Category { get; set; }
        public string Text { get; set; }
    }
}
