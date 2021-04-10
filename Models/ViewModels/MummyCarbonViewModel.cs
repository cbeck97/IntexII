using System;
using System.Collections.Generic;

namespace BYUFagElGamous1_5.Models.ViewModels
{
    public class MummyCarbonViewModel
    {
        public IEnumerable<CarbonDated> Carbons { get; set; }
        public int MummyId { get; set; }
        public CarbonDated NewCarbon { get; set; }
    }
}
