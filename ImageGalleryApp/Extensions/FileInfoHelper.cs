using ImageGalleryApp.Models;
using ImageGalleryApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ImageGalleryApp.Extensions
{
    public static class FileInfoHelper
    {
        public static Expression<Func<FileInfo, FileInfoViewModel>> expFileInfoToFileInfoViewModel = (fileInfo) => new FileInfoViewModel
        {
            Id = fileInfo.Id,
            Description = fileInfo.Description,
            SizeInBytes = fileInfo.SizeInBytes,
            Type = fileInfo.Type,
            URL = fileInfo.URL
        };
        public static Expression<Func<FileInfoViewModel, FileInfo>> expFileInfoViewModelToFileInfo = (fileInfoViewModel) => new FileInfo
        {
            Id = fileInfoViewModel.Id,
            Description = fileInfoViewModel.Description,
            SizeInBytes = fileInfoViewModel.SizeInBytes,
            Type = fileInfoViewModel.Type,
            URL = fileInfoViewModel.URL
        };
        public static IQueryable<FileInfoViewModel> ToFileInfoViewModel(this IQueryable<FileInfo> fileInfo)
        {
            return fileInfo.Select(expFileInfoToFileInfoViewModel);
        }
        public static FileInfo ToFileInfo(this FileInfoViewModel fileInfoViewModel)
        {
            return expFileInfoViewModelToFileInfo.Compile()(fileInfoViewModel);
        }
        public static FileInfoViewModel ToFileInfoViewModel(this FileInfo fileInfo)
        {
            return expFileInfoToFileInfoViewModel.Compile()(fileInfo);
        }
    }
}
