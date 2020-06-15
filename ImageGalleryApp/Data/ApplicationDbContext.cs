using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageGalleryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageGalleryApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<FileInfo> FileInfo { get; set; }
    }
}
