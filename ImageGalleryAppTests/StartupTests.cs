using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using ImageGalleryApp;
using Microsoft.Extensions.DependencyInjection;
using ImageGalleryApp.Data;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using Amazon.S3;
using ImageGalleryApp.BusinessLogicLayer;
using Shouldly;

namespace ImageGalleryAppTests
{
    public class StartupTests
    {

        [Fact]
        public void WhenProvidedConfigurationShouldConfigureServices()
        {
            // Arrange
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            var configurationRoot = configurationBuilder.Build();

            // Act
            var webHost = Microsoft.AspNetCore.WebHost.CreateDefaultBuilder()
                .UseConfiguration(configurationRoot)
                .UseStartup<Startup>().Build();
            webHost.Start();
            var serviceScope = webHost.Services.CreateScope();

            // Assert
            var applicationDbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
            var httpClientFactory = webHost.Services.GetService<IHttpClientFactory>();
            var amazonS3 = webHost.Services.GetService<IAmazonS3>();
            var fileInfoService = serviceScope.ServiceProvider.GetService<IFileInfoService>();

            //applicationDbContext.ShouldBeOfType<ApplicationDbContext>();
            httpClientFactory.ShouldBeAssignableTo<IHttpClientFactory>();
            amazonS3.ShouldBeAssignableTo<IAmazonS3>();
            fileInfoService.ShouldBeAssignableTo<IFileInfoService>();
        }
    }
}
