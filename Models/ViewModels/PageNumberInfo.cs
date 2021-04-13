using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BYUFagElGamous1_5.Models.ViewModels
{
    public class PageNumberInfo
    {
        public int NumItemsPerPage { get; set; } = 5;
        public int CurrentPage { get; set; } = 1;
        public int TotalNumItems { get; set; }
        public int numItems { get; set; }
        //Calculate Number of Pages
        //When given a decimal, returns next int up
        public int NumPages => NumItemsPerPage != TotalNumItems ? (int)Math.Ceiling((decimal)TotalNumItems / NumItemsPerPage) : 1;
    }
}
