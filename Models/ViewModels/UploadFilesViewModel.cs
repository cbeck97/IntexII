using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BYUFagElGamous1_5.Models.ViewModels
{
    public class UploadFilesViewModel
    {
        public string ImageCaption { get; set; }
        public string ImageDescription { get; set; }
        public List<IFormFile> files { get; set; }
    }
}
