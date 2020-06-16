using ImageGalleryApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGalleryApp.BusinessLogicLayer
{
    public interface IFileInfoService
    {
        Task<FileInfoViewModel> SaveFile(FileInfoViewModel fileInfoViewModel);
        Task<FileInfoViewModel> GetFile(int Id);
    }
}
