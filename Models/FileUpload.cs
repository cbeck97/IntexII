using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace BYUFagElGamous1_5.Models
{
    public class FileUpload
    {
        [Required]
        [Display(Name = "Select Image or File")]
        public IFormFile FormFile { get; set; }
    }
}
