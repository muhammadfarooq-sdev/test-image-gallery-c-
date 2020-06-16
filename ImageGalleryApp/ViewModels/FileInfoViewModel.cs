using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGalleryApp.ViewModels
{
    public class FileInfoViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
        //[Required(ErrorMessage = "URL is required")]
        public string URL { get; set; }
        [Required(ErrorMessage = "Type is required")]
        public string Type { get; set; }
        [Required(ErrorMessage = "SizeInBytes is required")]
        public long SizeInBytes { get; set; }
        public IFormFile FormFile { get; set; }
    }
}
