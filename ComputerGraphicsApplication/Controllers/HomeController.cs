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
        private readonly ImageService _imageService;
        private readonly ApplicationSettings _applicationSettings;

        private readonly HashSet<string> _accessibleContentTypes = new HashSet<string>
        {
            "image/jpeg",
            "image/png"
        };

        private readonly Dictionary<string, Func<string, AbstractFilter>> _filterDictionary = new Dictionary<string, Func<string, AbstractFilter>>
        {
            {"grayscale", value => new GrayscaleFilter()},
            {"rotate", value => new RotateFilter(value)},
            {"sobel", value => new SobelFilter()}
        };

        private const string FileNameCookie = "fileName";

        public HomeController(FileService fileService,
                              GuidService guidService,
                              ImageService imageService,
                              ApplicationSettings applicationSettings)
        {
            _fileService = fileService;
            _guidService = guidService;
            _imageService = imageService;
            _applicationSettings = applicationSettings;
        }

        [Route("")]
        [Route("index")]
        public async Task<ActionResult> Index(IndexViewModel indexViewModel)
        {
            var filter = indexViewModel.Filter;
            var valueFilter = indexViewModel.ValueFilter;

            var abstractFilter = filter != null && _filterDictionary.ContainsKey(filter)
                                     ? _filterDictionary[filter](valueFilter)
                                     : null;

            var fileNameCookie = GetCookie(FileNameCookie);
            if (fileNameCookie != null && _fileService.IsExisted(FileService.ImagesStorage, fileNameCookie))
            {
                indexViewModel.ImageUrl = Path.Combine(_applicationSettings.GetStorageFolder(false), FileService.ImagesStorage, fileNameCookie);

                if (abstractFilter != null)
                {
                    indexViewModel.FilteredImageUrl = Path.Combine(_applicationSettings.GetStorageFolder(false), FileService.FilterImagesStorage, fileNameCookie);
                    var fileStream = _fileService.GetStream(FileService.ImagesStorage, fileNameCookie);
                    var bitmap = _imageService.GetBitmapFromStream(fileStream);
                    await abstractFilter.Apply(bitmap).SaveToStorage(_fileService, fileNameCookie, FileService.FilterImagesStorage);
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
            var image = _imageService.GetImageFromStream(uploadFileModel.Stream);
            await _imageService.NormalizeImage(image).SaveToStorage(_fileService, fileName, FileService.ImagesStorage);

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