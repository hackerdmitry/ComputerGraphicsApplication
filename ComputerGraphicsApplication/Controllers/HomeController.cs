using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using ComputerGraphicsApplication.Extensions;
using ComputerGraphicsApplication.Models.Common;
using ComputerGraphicsApplication.Models.Filters;
using ComputerGraphicsApplication.Models.Home;
using ComputerGraphicsApplication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ComputerGraphicsApplication.Controllers
{
    [Route("")]
    [Route("home")]
    public class HomeController : Controller
    {
        private readonly FileService _fileService;
        private readonly GuidService _guidService;
        private readonly ApplicationSettings _applicationSettings;

        private readonly HashSet<string> _accessibleContentTypes = new HashSet<string>
        {
            "image/jpeg",
            "image/png"
        };

        private readonly Dictionary<string, Func<string, AbstractFilter>> _filterDictionary = new Dictionary<string, Func<string, AbstractFilter>>
        {
            {"grayscale", value => new GrayscaleFilter()},
            {"rotate", value => new RotateFilter(value)}
        };

        private const string FileNameCookie = "fileName";

        public HomeController(FileService fileService,
                              GuidService guidService,
                              ApplicationSettings applicationSettings)
        {
            _fileService = fileService;
            _guidService = guidService;
            _applicationSettings = applicationSettings;
        }

        [Route("")]
        [Route("index")]
        public async Task<ActionResult> Index(string filter = null, string valueFilter = null)
        {
            var abstractFilter = filter != null && _filterDictionary.ContainsKey(filter)
                                     ? _filterDictionary[filter](valueFilter)
                                     : null;

            var fileNameCookie = GetCookie(FileNameCookie);
            var indexViewModel = new IndexViewModel();
            if (fileNameCookie != null)
            {
                indexViewModel.ImageUrl = Path.Combine(_applicationSettings.GetStorageFolder(false), FileService.ImagesStorage, fileNameCookie);

                if (abstractFilter != null)
                {
                    indexViewModel.FilteredImageUrl = Path.Combine(_applicationSettings.GetStorageFolder(false), FileService.FilterImagesStorage, fileNameCookie);
                    var fileStream = _fileService.GetStream(FileService.ImagesStorage, fileNameCookie);
                    var image = Image.FromStream(fileStream);
                    var bitmap = new Bitmap(image);
                    await abstractFilter.Apply(bitmap).SaveToStorage(_fileService, fileNameCookie);
                }
            }

            return View("Index", indexViewModel);
        }

        [HttpPost, Route("send-files")]
        public async Task<string> SendFile(IFormFile file)
        {
            if (!_accessibleContentTypes.Contains(file.ContentType))
            {
                HttpContext.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                return "Недопустимый формат файла";
            }

            var fileName = GetCookie(FileNameCookie) ?? Path.ChangeExtension(_guidService.Create(), Path.GetExtension(file.FileName));
            var uploadFileModel = await UploadFileModel.CreateAsync(file, FileService.ImagesStorage);
            await _fileService.UploadFileAsync(uploadFileModel, fileName);

            return fileName;
        }

        [HttpDelete, Route("delete-file")]
        public async Task DeleteFile()
        {
            var fileNameCookie = GetCookie(FileNameCookie);
            if (fileNameCookie == null)
            {
                return;
            }

            await _fileService.DeleteAsync(Path.Combine(FileService.ImagesStorage, fileNameCookie));
            await _fileService.DeleteAsync(Path.Combine(FileService.FilterImagesStorage, fileNameCookie));
        }

        private string GetCookie(string name)
        {
            return HttpContext.Request.Cookies.ContainsKey(name) && !string.IsNullOrEmpty(HttpContext.Request.Cookies[name])
                       ? HttpContext.Request.Cookies[name]
                       : null;
        }
    }
}