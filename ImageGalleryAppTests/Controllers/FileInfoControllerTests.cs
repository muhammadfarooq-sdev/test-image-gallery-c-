using AutoFixture;
using ImageGalleryApp.BusinessLogicLayer;
using ImageGalleryApp.Controllers;
using ImageGalleryApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ImageGalleryAppTests.Controllers
{
    public class FileInfoControllerTests
    {

        [Fact]
        public async Task WhenExceptionThenUploadFileReturnsInternalServerError()
        {
            // Arrange
            var fileInfoViewModel = new FileInfoViewModel { };
            var fileInfoServiceMock = new Mock<IFileInfoService>();
            var fileInfoController = new FileInfoController(fileInfoServiceMock.Object);

            // Act
            var contentResult = (ContentResult) await fileInfoController.UploadFile(fileInfoViewModel);

            // Assert
            contentResult.ShouldNotBeNull();
            contentResult.StatusCode.ShouldBe((int)HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task WhenNoExceptionThenUploadFileReturnsCreated()
        {
            // Arrange
            var fileInfoViewModel = new FileInfoViewModel { };
            var fileInfoViewModelFromSave = new FileInfoViewModel { };
            var fileInfoServiceMock = new Mock<IFileInfoService>();
            var fixture = new Fixture();
            fileInfoServiceMock.Setup(fileInfoService => fileInfoService.SaveFile(fileInfoViewModel))
                .ReturnsAsync(new FileInfoViewModel
                {
                    Id = fixture.Create<int>()
                });
            var fileInfoController = new FileInfoController(fileInfoServiceMock.Object);

            // Act
            var createdAtActionResult = (CreatedAtActionResult)await fileInfoController.UploadFile(fileInfoViewModel);

            // Assert
            createdAtActionResult.ShouldNotBeNull();
            createdAtActionResult.StatusCode.ShouldBe((int)HttpStatusCode.Created);
        }
    }
}
