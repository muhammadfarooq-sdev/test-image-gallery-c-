using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGalleryApp.ViewModels
{
    public class FileInfoViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
        public string Type { get; set; }
        public long SizeInBytes { get; set; }
        public IFormFile FormFile { get; set; }
    }
}
