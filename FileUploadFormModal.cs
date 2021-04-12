using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

public class FileUploadFormModal
{
    [Required]
    [Display(Name = "File")]
    public IFormFile FormFile { get; set; }
}
