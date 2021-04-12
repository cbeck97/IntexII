using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace BYUFagElGamous1_5.Models
{
    public partial class MummyImage
    {
        public int MummyImageId { get; set; }
        public int? MummyId { get; set; }
        public int? ImageId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
    }
}
