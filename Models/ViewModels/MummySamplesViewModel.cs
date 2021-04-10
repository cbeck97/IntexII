using System;
using System.Collections.Generic;

namespace BYUFagElGamous1_5.Models.ViewModels
{
    public class MummySamplesViewModel
    {
        public IEnumerable<Sample> Samples { get; set; }
        public int MummyId { get; set; }
        public Sample NewSample { get; set; }
    }
}
