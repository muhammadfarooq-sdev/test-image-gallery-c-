using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageGalleryApp.BusinessLogicLayer;
using ImageGalleryApp.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImageGalleryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileInfoController : Controller
    {
        private IFileInfoService fileInfoService;

        public FileInfoController(IFileInfoService fileInfoService)
        {
            this.fileInfoService = fileInfoService;
        }
        [HttpPost("[action]")]
        public IActionResult UploadFile(FileInfoViewModel fileInfoViewModel)
        {
            return Ok(fileInfoService.SaveFile(fileInfoViewModel));
        }

    }
}