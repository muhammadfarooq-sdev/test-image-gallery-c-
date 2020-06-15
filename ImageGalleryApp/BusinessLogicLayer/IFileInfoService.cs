using ImageGalleryApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGalleryApp.BusinessLogicLayer
{
    public interface IFileInfoService
    {
        FileInfoViewModel SaveFile(FileInfoViewModel fileInfoViewModel);
    }
}
