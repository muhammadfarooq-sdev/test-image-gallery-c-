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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType()
        {
            // Arrange
            const string uploadUrl = "api/fileInfo/UploadFile";
            //var client = _factory.CreateClient();
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
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
                var configurationRoot = configurationBuilder.Build();

                var webHostBuilder = new WebHostBuilder()
                    .UseConfiguration(configurationRoot)
                    .UseStartup<Startup>();

                // Configure the in-memory test server, and create an HttpClient for interacting with it
                var server = new TestServer(webHostBuilder);
                HttpClient client = server.CreateClient();

               // var webHost = Microsoft.AspNetCore.WebHost.CreateDefaultBuilder()
               ////.UseConfiguration(configurationRoot)
               //.UseStartup<Startup>().Build();
               // webHost.Start();

               // webHost.

            //    var hostBuilder = new HostBuilder().ConfigureWebHost(webHost =>
            //{
            //    // Add TestServer
            //    webHost.UseTestServer();
            //    webHost.Configure(app => app.Run(async ctx =>
            //        await ctx.Response.WriteAsync("Hello World!")));
            //});

            //    // Build and start the IHost
            //    var host = await hostBuilder.StartAsync();

            //    // Create an HttpClient to send requests to the TestServer
            //    var client = host.GetTestClient();

                // Act
                var response = await client.PostAsync(uploadUrl, multipartFormDataContent);

                // Assert
                response.EnsureSuccessStatusCode(); // Status Code 200-299
                Assert.Equal("text/html; charset=utf-8",
                    response.Content.Headers.ContentType.ToString());
            }
        }
    }
}
