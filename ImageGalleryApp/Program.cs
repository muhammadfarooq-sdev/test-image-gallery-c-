using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ImageGalleryApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            System.Diagnostics.Debugger.Launch();
            var webHost = CreateWebHostBuilder(args).Build();
            //using (var scope = webHost.Services.CreateScope())
            //{
            //    var services = scope.ServiceProvider;
            //    SeedData.Initialize(services);
            //    var applicationDbContext = services.GetRequiredService<ApplicationDbContext>();

            //    applicationDbContext.Database.Migrate();
            //}

            webHost.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseContentRoot(Directory.GetCurrentDirectory())
           .UseKestrel()
            .UseIISIntegration()
            .UseStartup<Startup>();
    }
    //CreateWebHostBuilder(args).Build().Run();
    //    }

    //    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
    //        WebHost.CreateDefaultBuilder(args)
    //            .UseStartup<Startup>();
    //}
}
