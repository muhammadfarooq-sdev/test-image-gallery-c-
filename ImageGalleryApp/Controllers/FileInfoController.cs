using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private IFileInfoService _fileInfoService;
        private ContentResult _internalServerError;
        public FileInfoController(IFileInfoService fileInfoService)
        {
            this._fileInfoService = fileInfoService;
            this._internalServerError = new ContentResult()
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Content = "Internal Server Error"
            };
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UploadFile([FromForm]FileInfoViewModel fileInfoViewModel)
        {
            try
            {
                var fileInfoViewModelDB = await this._fileInfoService.SaveFile(fileInfoViewModel);
                var routeValues = new {
                    fileInfoViewModelDB.Id
                };
                return new CreatedAtActionResult(actionName: "", controllerName: "FileInfo", 
                    routeValues: routeValues, value:fileInfoViewModelDB);
            }
            catch (Exception)
            {
                return _internalServerError;
                throw;
            }
        }

    }
}