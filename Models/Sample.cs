using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace BYUFagElGamous1_5.Models
{
    public partial class Sample
    {
        public int SampleId { get; set; }
        public int? SampleNumber { get; set; }
        public int? MummyId { get; set; }
        public int? RackNumber { get; set; }
        public int? Bag { get; set; }
        public int? Date { get; set; }
        public string PreviouslySampled { get; set; }
        public string Initials { get; set; }
        public int? ClusterNumber { get; set; }
        public string Area { get; set; }
    }
}
