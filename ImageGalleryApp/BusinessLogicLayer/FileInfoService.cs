using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using ImageGalleryApp.Data;
using ImageGalleryApp.Extensions;
using ImageGalleryApp.Models;
using ImageGalleryApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ImageGalleryApp.BusinessLogicLayer
{
    public class FileInfoService : IFileInfoService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IAmazonS3 _amazonS3;

        public FileInfoService(ApplicationDbContext applicationDbContext, IAmazonS3 amazonS3)
        {
            this._applicationDbContext = applicationDbContext;
            this._amazonS3 = amazonS3;
        }

        public Task<FileInfoViewModel> GetFile(int Id)
        {
            return this._applicationDbContext.FileInfo
                .Where(fileInfo => fileInfo.Id == Id)
                .ToFileInfoViewModel().FirstOrDefaultAsync();
        }

        public async Task<FileInfoViewModel> SaveFile(FileInfoViewModel fileInfoViewModel)
        {
            var signedURL = generatePreSignedURL(fileInfoViewModel.FormFile.FileName);
            using (var httpClient = new HttpClient())
            using (var fileStream = fileInfoViewModel.FormFile.OpenReadStream())
            {
                var putResponseMessage = await httpClient.PutAsync(signedURL, new StreamContent(fileStream));
                putResponseMessage.EnsureSuccessStatusCode();
            }
            FileInfo fileInfo;
            try
            {
                fileInfoViewModel.URL = new Uri(signedURL).AbsolutePath;
                fileInfo = fileInfoViewModel.ToFileInfo();
                this._applicationDbContext.Entry(fileInfo).State = EntityState.Added;
                await this._applicationDbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                using (var httpClient = new HttpClient())
                {
                    await httpClient.DeleteAsync(signedURL);
                }
                throw;
            }
            return fileInfo.ToFileInfoViewModel();
        }

        private string generatePreSignedURL(string fileName)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = "test-image-gallery",
                Key = $"uploaded-images/{fileName}",
                Verb = HttpVerb.PUT,
                Expires = DateTime.Now.AddMinutes(5)
            };

            string url = this._amazonS3.GetPreSignedURL(request);
            return url;
        }
    }
}
