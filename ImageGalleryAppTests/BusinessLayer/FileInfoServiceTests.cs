using Amazon.S3;
using Amazon.S3.Model;
using AutoFixture;
using ImageGalleryApp.BusinessLogicLayer;
using ImageGalleryApp.Data;
using ImageGalleryApp.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using Moq.Protected;
using Shouldly;
using System;
using System.Collections.Generic;
using SystemIO = System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using ImageGalleryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageGalleryAppTests.BusinessLayer
{
    public class FileInfoServiceTests
    {
        public FileInfoServiceTests()
        {

        }
        [Fact]
        public async Task WhenNoErrorThenSaveFileTestReturnsFileInfoViewModel()
        {
            // Arrange
            var formFileMock = new Mock<IFormFile>();

            const string testQuery = "?testquery";
            const string urlWithoutQuery = "http://signed_Url/test.png";
            string s3SignedURL = $"{urlWithoutQuery}{testQuery}";
            var fileInfoViewModel = getFileInfoViewModel(formFileMock);

            var fileInfoService = getFileInfoService(s3SignedURL, formFileMock, getApplicationDbContext());

            // Act
            FileInfoViewModel fileInfoViewModelResult = await fileInfoService.SaveFile(fileInfoViewModel);

            // Assert
            fileInfoViewModelResult.ShouldSatisfyAllConditions(
                () => fileInfoViewModelResult.Id.ShouldBeGreaterThan(0),
                () => fileInfoViewModelResult.URL.ShouldBe(urlWithoutQuery),
                () => fileInfoViewModelResult.Type.ShouldBe(fileInfoViewModel.Type),
                () => fileInfoViewModelResult.SizeInBytes.ShouldBe(fileInfoViewModel.SizeInBytes),
                () => fileInfoViewModelResult.Description.ShouldBe(fileInfoViewModel.Description));
        }
        [Fact]
        public async Task WhenErrorAfterUploadToS3ThenSaveFileDeletedFileAndThrowsException()
        {
            // Arrange
            var formFileMock = new Mock<IFormFile>();

            const string testQuery = "?testquery";
            const string urlWithoutQuery = "http://signed_Url/test.png";
            string s3SignedURL = $"{urlWithoutQuery}{testQuery}";

            var fileInfoViewModel = getFileInfoViewModel(formFileMock);
            var fileInfoService = getFileInfoService(s3SignedURL, formFileMock, null);

            // Act, Assert
            await fileInfoService.SaveFile(fileInfoViewModel).ShouldThrowAsync<Exception>();
        }
        [Fact]
        public async Task WhenErrorInUploadToS3ThenSaveFileThrowsException()
        {
            // Arrange
            var formFileMock = new Mock<IFormFile>();

            const string testQuery = "?testquery";
            const string urlWithoutQuery = "http://signed_Url/test.png";
            string s3SignedURL = $"{urlWithoutQuery}{testQuery}";

            var fileInfoViewModel = new FileInfoViewModel { };
            var fileInfoService = getFileInfoService(s3SignedURL, formFileMock, null);

            // Act, Assert
            await fileInfoService.SaveFile(fileInfoViewModel).ShouldThrowAsync<Exception>();
        }
        private FileInfoViewModel getFileInfoViewModel(Mock<IFormFile> formFileMock)
        {
            return new FileInfoViewModel
            {
                Description = "test desc",
                SizeInBytes = 400,
                Type = "image/png",
                FormFile = formFileMock.Object
            };
        }
        private ApplicationDbContext getApplicationDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("ApplicationDb")
            .Options;

            return new ApplicationDbContext(options);
        }
        private IFileInfoService getFileInfoService(string s3SignedURL, Mock<IFormFile> formFileMock,
            ApplicationDbContext applicationDbContext)
        {
            var amazonS3Mock = new Mock<IAmazonS3>();

            const string fileName = "test.png";

            var streamMock = new Mock<SystemIO.Stream>();
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            formFileMock.SetupGet<String>(formFile => formFile.FileName).Returns(fileName);
            formFileMock.Setup(formFile => formFile.OpenReadStream()).Returns(streamMock.Object);
            var fixture = new Fixture();

            amazonS3Mock.Setup(amazonS3 =>
                amazonS3.GetPreSignedURL(It.IsAny<GetPreSignedUrlRequest>())).Returns(s3SignedURL);

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(fixture.Create<String>()),
                });
            
            httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>()))
                .Returns(() => new HttpClient(mockHttpMessageHandler.Object)
                {
                    BaseAddress = fixture.Create<Uri>()
                });

            FileInfoService fileInfoService = new FileInfoService(
                applicationDbContext,
                amazonS3Mock.Object,
                httpClientFactoryMock.Object);
            return fileInfoService;
        }
    }
}
