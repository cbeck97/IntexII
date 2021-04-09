using System;
using System.Collections.Generic;

namespace BYUFagElGamous1_5.Models.ViewModels
{
    public class ViewMummyViewModel
    {
        public List<Mummy> Mummies { get; set; }
        //public List<Location> Locations { get; set; }

        public Dictionary<Mummy, Location> mumLocs { get; set; }
        public PageNumberInfo PageNumberInfo { get; set; }
    }
}
