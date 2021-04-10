using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace BYUFagElGamous1_5.Models
{
    public partial class Mummy
    {
        public int MummyId { get; set; }
        public int MeasurementId { get; set; }
        public int? DayFound { get; set; }
        public int? MonthFound { get; set; }
        public int? YearFound { get; set; }
        public string HairColor { get; set; }
        public string Gender { get; set; }
        public string BurialWrapping { get; set; }
        public string AdultChild { get; set; }
        public string AgeRange { get; set; }
        public int? SampleId { get; set; }
        public string HeadDirection { get; set; }
        public int? LocationId { get; set; }
        public int? Images { get; set; }
        public string Artifacts { get; set; }
        public string BurialPreservation { get; set; }
        public string BurialGenderMethod { get; set; }
        public string BurialAgeAtDeath { get; set; }
        public string BurialAgeMethod { get; set; }
        public bool? BurialSampleTaken { get; set; }
        public string FaceBundle { get; set; }
        public int? Gamous { get; set; }
        public int? ClusterId { get; set; }
        public double? LengthOfRemains { get; set; }
        public bool? PhotoTaken { get; set; }
        public virtual Location Location { get; set; }
        public virtual Notes Notes { get; set; }
    }
}
