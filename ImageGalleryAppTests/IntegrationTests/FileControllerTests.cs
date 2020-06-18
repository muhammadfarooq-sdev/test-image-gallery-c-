using AutoFixture;
using ImageGalleryApp;
using ImageGalleryApp.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Moq;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ImageGalleryAppTests.IntegrationTests
{
    public class FileControllerTests
    {
        public FileControllerTests()
        {
        }

        [Fact]
        public async Task WhenNoErrorThenUploadFileReturnsCreated()
        {
            // Arrange
            const string uploadUrl = "api/fileInfo/UploadFile";
            var fixture = new Fixture();
            const string formFileKey = "formFile";
            const string descriptionKey = "description";
            const string typeKey = "type";
            const string sizeInBytesKey = "SizeInBytes";
            const string fileName = "test.png";
            var mockStream = new Mock<Stream>();
            using (var multipartFormDataContent = new MultipartFormDataContent("Upload----" +
                DateTime.Now.ToString(CultureInfo.InvariantCulture)))
            {
                multipartFormDataContent.Add(new StreamContent(mockStream.Object), formFileKey, fileName);
                multipartFormDataContent.Add(fixture.Create<StringContent>(), descriptionKey);
                multipartFormDataContent.Add(fixture.Create<StringContent>(), typeKey);
                multipartFormDataContent.Add(new StringContent(fixture.Create<int>().ToString()),
                    sizeInBytesKey);

                HttpResponseMessage httpResponseMessage;
                using (var httpClient = getHttpClient(getConfiguration()))
                {
                    // Act
                    httpResponseMessage = await httpClient.PostAsync(uploadUrl, multipartFormDataContent);
                }

                // Assert
                httpResponseMessage.StatusCode.ShouldBe(HttpStatusCode.Created);
            }
        }

        [Fact]
        public async Task WhenARequiredFieldIsMissingThenUploadFileReturnsBadRequest()
        {
            // Arrange
            const string uploadUrl = "api/fileInfo/UploadFile";
            var fixture = new Fixture();
            const string formFileKey = "formFile";
            const string typeKey = "type";
            const string sizeInBytesKey = "SizeInBytes";
            const string fileName = "test.png";
            var mockStream = new Mock<Stream>();
            using (var multipartFormDataContent = new MultipartFormDataContent("Upload----" +
                DateTime.Now.ToString(CultureInfo.InvariantCulture)))
            {
                multipartFormDataContent.Add(new StreamContent(mockStream.Object), formFileKey, fileName);
                multipartFormDataContent.Add(fixture.Create<StringContent>(), typeKey);
                multipartFormDataContent.Add(new StringContent(fixture.Create<int>().ToString()),
                    sizeInBytesKey);

                HttpResponseMessage httpResponseMessage;
                using (var httpClient = getHttpClient(getConfiguration()))
                {
                    // Act
                    httpResponseMessage = await httpClient.PostAsync(uploadUrl, multipartFormDataContent);
                }

                // Assert
                httpResponseMessage.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            }
        }

        [Fact]
        public async Task WhenInternalErrorThenUploadFileReturnsInternalServerError()
        {
            // Arrange
            const string uploadUrl = "api/fileInfo/UploadFile";
            var fixture = new Fixture();
            const string formFileKey = "formFile";
            const string descriptionKey = "description";
            const string typeKey = "type";
            const string sizeInBytesKey = "SizeInBytes";
            const string fileName = "test.png";
            var mockStream = new Mock<Stream>();
            using (var multipartFormDataContent = new MultipartFormDataContent("Upload----" +
                DateTime.Now.ToString(CultureInfo.InvariantCulture)))
            {
                multipartFormDataContent.Add(new StreamContent(mockStream.Object), formFileKey, fileName);
                multipartFormDataContent.Add(fixture.Create<StringContent>(), descriptionKey);
                multipartFormDataContent.Add(fixture.Create<StringContent>(), typeKey);
                multipartFormDataContent.Add(new StringContent(fixture.Create<int>().ToString()),
                    sizeInBytesKey);

                var configurationBuilder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.incorrent.aws.configuration.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables();
                var configurationRoot = configurationBuilder.Build();

                HttpResponseMessage httpResponseMessage;
                using (var httpClient = getHttpClient(configurationRoot))
                {
                    // Act
                    httpResponseMessage = await httpClient.PostAsync(uploadUrl, multipartFormDataContent);
                }

                // Assert
                httpResponseMessage.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
            }
        }

        private IConfiguration getConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddEnvironmentVariables();
            var configurationRoot = configurationBuilder.Build();
            return configurationRoot;
        }

        private HttpClient getHttpClient(IConfiguration configuration)
        {
            var webHostBuilder = new WebHostBuilder()
                .UseConfiguration(configuration)
                .UseStartup<Startup>();
            //webHostBuilder.Start();
            var server = new TestServer(webHostBuilder);            
            return server.CreateClient();
        }

    }
}
