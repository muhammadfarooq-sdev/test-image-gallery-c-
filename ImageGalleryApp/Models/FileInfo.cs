using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGalleryApp.Models
{
    public class FileInfo
    {
        public int Id { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string URL { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public long SizeInBytes { get; set; }
    }
}
