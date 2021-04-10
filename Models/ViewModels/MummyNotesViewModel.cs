using System;
using System.Collections.Generic;

namespace BYUFagElGamous1_5.Models.ViewModels
{
    public class MummyNotesViewModel
    {
        public IEnumerable<Notes> Notes { get; set; }
        public int MummyId { get; set; }
        public Notes NewNote { get; set; }
    }
}
