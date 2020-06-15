using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageGalleryApp.Extensions;
using ImageGalleryApp.ViewModels;

namespace ImageGalleryApp.BusinessLogicLayer
{
    public class FileInfoService : IFileInfoService
    {
        public FileInfoViewModel SaveFile(FileInfoViewModel fileInfoViewModel)
        {
            var fileInfo = fileInfoViewModel.ToFileInfo();
            return fileInfo.ToFileInfoViewModel();
        }
    }
}
