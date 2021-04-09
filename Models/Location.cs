using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace BYUFagElGamous1_5.Models
{
    public partial class Location
    {
        public int LocationId { get; set; }
        public string BurialLocationNs { get; set; }
        public string BurialLocationEw { get; set; }
        public int? LowPairNs { get; set; }
        public int? HighPairNs { get; set; }
        public int? LowPairEw { get; set; }
        public int? HighPairEw { get; set; }
        public string Subplot { get; set; }
        public int? BurialNumber { get; set; }
        public double? BurialDepth { get; set; }
        public double? SouthToHead { get; set; }
        public double? SouthToFeet { get; set; }
        public double? EastToHead { get; set; }
        public double? EastToFeet { get; set; }
        public double? WestToFeet { get; set; }
        public double? WestToHead { get; set; }
        public string BurialSituation { get; set; }
        public int? Tomb { get; set; }
        public string Area { get; set; }
    }
}
