using System;
using System.Collections.Generic;
using SystemIO = System.IO;
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
        private readonly IHttpClientFactory _httpClientFactory;

        public FileInfoService(ApplicationDbContext applicationDbContext, 
            IAmazonS3 amazonS3, 
            IHttpClientFactory httpClientFactory)
        {
            this._applicationDbContext = applicationDbContext;
            this._amazonS3 = amazonS3;
            this._httpClientFactory = httpClientFactory;
        }

        public async Task<FileInfoViewModel> SaveFile(FileInfoViewModel fileInfoViewModel)
        {
            var signedURL = generatePreSignedURL(fileInfoViewModel.FormFile.FileName);
            using (var httpClient = _httpClientFactory.CreateClient())
            using (var fileStream = fileInfoViewModel.FormFile.OpenReadStream())
            {
                var putResponseMessage = await httpClient.PutAsync(signedURL, new StreamContent(fileStream));
                putResponseMessage.EnsureSuccessStatusCode();
            }

            FileInfo fileInfo;
            try
            {
                var signedUri = new Uri(signedURL);
                var fileS3URL = signedURL.Substring(0, signedURL.IndexOf(signedUri.Query));
                fileInfoViewModel.URL = fileS3URL;
                fileInfo = fileInfoViewModel.ToFileInfo();
                this._applicationDbContext.Entry(fileInfo).State = EntityState.Added;
                await this._applicationDbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    await httpClient.DeleteAsync(signedURL);
                }
                throw;
            }
            return fileInfo.ToFileInfoViewModel();
        }

        private string generatePreSignedURL(string fileName)
        {
            var fileNameWithoutExtension = SystemIO.Path.GetFileNameWithoutExtension(fileName);
            var fileExtension = SystemIO.Path.GetExtension(fileName);
            var fileNameUnique = $"{fileNameWithoutExtension}_{DateTime.Now.Ticks}{fileExtension}";
            var request = new GetPreSignedUrlRequest
            {
                BucketName = "test-image-gallery",
                Key = $"uploaded-images/{fileNameUnique}",
                Verb = HttpVerb.PUT,
                Expires = DateTime.Now.AddMinutes(5)
            };

            string url = this._amazonS3.GetPreSignedURL(request);
            return url;
        }
    }
}
